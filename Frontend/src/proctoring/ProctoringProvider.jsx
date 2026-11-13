import { createContext, useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react'
import { FaceDetector, FilesetResolver } from '@mediapipe/tasks-vision'
import api from '../api/client'
import { useStudentAuth } from '../auth/StudentAuth'
import { toast } from '../ui/kit'

/*
 * Browser side of proctoring.
 *
 * This component only observes and reports. It never decides a penalty: every observation is sent
 * to the server, which applies the configured policy and answers with what happens next
 * (warning, flag, attempt ended, suspension). Snapshots and clips are captured only when the
 * server asks for them, which it does only if the student consented and the rule requires it.
 * All face detection runs on the device; no video is streamed anywhere.
 */

const ProctoringContext = createContext(null)
export const useProctoring = () => useContext(ProctoringContext)

const DETECT_EVERY_MS = 500
const STATUS_EVERY_MS = 20000

let detectorPromise = null
function loadDetector() {
  if (!detectorPromise) {
    detectorPromise = FilesetResolver.forVisionTasks('/mediapipe-wasm')
      .then((vision) => FaceDetector.createFromOptions(vision, {
        baseOptions: { modelAssetPath: '/models/blaze_face_short_range.tflite' },
        runningMode: 'VIDEO',
        minDetectionConfidence: 0.5,
      }))
      .catch((e) => { detectorPromise = null; throw e })
  }
  return detectorPromise
}

export async function openCamera() {
  if (!navigator.mediaDevices?.getUserMedia) throw new Error('This browser does not support camera access.')
  return navigator.mediaDevices.getUserMedia({
    video: { width: { ideal: 640 }, height: { ideal: 480 }, facingMode: 'user' },
    audio: false,
  })
}

function snapshot(video) {
  return new Promise((resolve) => {
    if (!video || !video.videoWidth) return resolve(null)
    const canvas = document.createElement('canvas')
    const scale = Math.min(1, 640 / video.videoWidth)
    canvas.width = Math.round(video.videoWidth * scale)
    canvas.height = Math.round(video.videoHeight * scale)
    canvas.getContext('2d').drawImage(video, 0, 0, canvas.width, canvas.height)
    canvas.toBlob((blob) => resolve(blob), 'image/jpeg', 0.7)
  })
}

function recordClip(stream, seconds) {
  return new Promise((resolve) => {
    if (typeof MediaRecorder === 'undefined' || !stream?.active) return resolve(null)
    const mimeType = ['video/webm;codecs=vp8', 'video/webm', 'video/mp4'].find((t) => MediaRecorder.isTypeSupported(t))
    if (!mimeType) return resolve(null)
    try {
      const chunks = []
      const recorder = new MediaRecorder(stream, { mimeType, videoBitsPerSecond: 400000 })
      recorder.ondataavailable = (e) => e.data.size && chunks.push(e.data)
      recorder.onstop = () => resolve(chunks.length ? new Blob(chunks, { type: mimeType.split(';')[0] }) : null)
      recorder.onerror = () => resolve(null)
      recorder.start()
      setTimeout(() => recorder.state !== 'inactive' && recorder.stop(), seconds * 1000)
    } catch {
      resolve(null)
    }
  })
}

export function ProctoringProvider({ children }) {
  const auth = useStudentAuth()
  const [config, setConfig] = useState(null)
  // ok | starting | blocked (no camera) | off (not required)
  const [camera, setCamera] = useState('starting')
  const [cameraError, setCameraError] = useState('')
  const [faces, setFaces] = useState(1)
  const [countdown, setCountdown] = useState(null)
  const [lastOutcome, setLastOutcome] = useState(null)
  const videoRef = useRef(null)
  const streamRef = useRef(null)
  const terminatedListeners = useRef(new Set())
  const configRef = useRef(null)
  useEffect(() => { configRef.current = config }, [config])

  const loadStatus = useCallback(async () => {
    const { data } = await api.get('/proctoring/status')
    setConfig(data)
    return data
  }, [])

  useEffect(() => { loadStatus().catch(() => setConfig({ proctoringRequired: false })) }, [loadStatus])

  const uploadEvidence = useCallback(async (eventId, uploadToken, blob, name) => {
    if (!blob) return
    const form = new FormData()
    form.append('file', blob, name)
    try { await api.post(`/proctoring/events/${eventId}/evidence`, form, { timeout: 60000, appealToken: uploadToken }) } catch { /* evidence is best effort */ }
  }, [])

  /** Reports one observed violation and acts on the server's decision. */
  const report = useCallback(async (ruleCode, message, details) => {
    let outcome
    try {
      outcome = (await api.post('/proctoring/events', { ruleCode, message, details })).data
    } catch {
      return null
    }
    if (!outcome?.recorded) return outcome
    setLastOutcome({ ...outcome, at: Date.now() })

    if (outcome.captureImage) snapshot(videoRef.current).then((blob) => uploadEvidence(outcome.eventId, outcome.uploadToken, blob, 'snapshot.jpg'))
    if (outcome.captureVideo) {
      recordClip(streamRef.current, configRef.current?.videoClipSeconds || 6)
        .then((blob) => uploadEvidence(outcome.eventId, outcome.uploadToken, blob, blob?.type === 'video/mp4' ? 'clip.mp4' : 'clip.webm'))
    }

    if (outcome.action === 'Suspended') {
      auth.signOutLocally(outcome.message)
    } else if (outcome.action === 'Terminated') {
      terminatedListeners.current.forEach((fn) => fn(outcome))
    } else {
      toast(outcome.message, 'warning')
    }
    return outcome
  }, [auth, uploadEvidence])

  const onTerminated = useCallback((fn) => {
    terminatedListeners.current.add(fn)
    return () => terminatedListeners.current.delete(fn)
  }, [])

  const stopCamera = useCallback(() => {
    streamRef.current?.getTracks().forEach((t) => t.stop())
    streamRef.current = null
  }, [])

  const startCamera = useCallback(async () => {
    setCamera('starting')
    setCameraError('')
    try {
      stopCamera()
      const stream = await openCamera()
      streamRef.current = stream
      // The student unplugging the camera or revoking permission mid-session.
      stream.getVideoTracks()[0].addEventListener('ended', () => {
        setCamera('blocked')
        setCameraError('The camera stopped. Reconnect it and allow access to continue.')
        report('CAMERA_OFF', 'Camera stream ended')
      })
      if (videoRef.current) {
        videoRef.current.srcObject = stream
        await videoRef.current.play().catch(() => {})
      }
      await loadDetector()
      setCamera('ok')
    } catch (e) {
      setCamera('blocked')
      setCameraError(e?.name === 'NotAllowedError'
        ? 'Camera permission was denied. Allow camera access in your browser settings, then try again.'
        : e?.name === 'NotFoundError'
          ? 'No camera was found on this device.'
          : 'The camera or the face-detection component could not be started.')
    }
  }, [report, stopCamera])

  const needsCamera = !!(config?.proctoringRequired && config?.cameraRequired && config?.sessionActive)

  useEffect(() => {
    if (!config) return undefined
    if (!needsCamera) { setCamera('off'); return undefined }
    startCamera()
    return stopCamera
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [needsCamera, !!config])

  // Face monitoring loop.
  useEffect(() => {
    if (camera !== 'ok') return undefined
    let stopped = false
    let badSince = null
    let badKind = null
    let lastCount = -1
    let lastStatusAt = 0

    const tick = async () => {
      if (stopped) return
      const video = videoRef.current
      const cfg = configRef.current
      if (video && video.readyState >= 2 && !document.hidden) {
        let count
        try {
          const detector = await loadDetector()
          count = detector.detectForVideo(video, performance.now()).detections.length
        } catch {
          // A detector hiccup is not the student's fault: skip this frame.
          count = lastCount < 0 ? 1 : lastCount
        }
        if (count !== lastCount) setFaces(count)
        const kind = count === 0 ? 'NO_FACE' : count > 1 ? 'MULTIPLE_FACES' : null
        const now = Date.now()
        if (kind !== badKind) { badKind = kind; badSince = kind ? now : null }
        if (kind) {
          const limit = (kind === 'NO_FACE' ? cfg.noFaceSeconds : cfg.multipleFacesSeconds) * 1000
          const left = Math.ceil((limit - (now - badSince)) / 1000)
          setCountdown({ kind, left: Math.max(0, left) })
          if (now - badSince >= limit) {
            badSince = now // start counting again; a continuing problem is reported once per period
            report(kind, kind === 'NO_FACE' ? 'No person visible on camera' : `${count} people visible on camera`, { personCount: count })
          }
        } else {
          setCountdown(null)
        }
        if (count !== lastCount || now - lastStatusAt > STATUS_EVERY_MS) {
          lastStatusAt = now
          api.post('/proctoring/camera-status', { cameraStatus: kind || 'OK', personCount: Math.min(count, 50) }).catch(() => {})
        }
        lastCount = count
      }
      if (!stopped) setTimeout(tick, DETECT_EVERY_MS)
    }
    tick()
    return () => { stopped = true }
  }, [camera, report])

  const value = useMemo(() => ({
    config, reloadConfig: loadStatus, camera, cameraError, faces, report, onTerminated, lastOutcome, retryCamera: startCamera,
  }), [config, loadStatus, camera, cameraError, faces, report, onTerminated, lastOutcome, startCamera])

  return (
    <ProctoringContext.Provider value={value}>
      {children}

      {needsCamera && (
        <div className={`cam-preview ${countdown ? 'alert-state' : ''}`} aria-hidden="true">
          <video ref={videoRef} muted playsInline />
          <div className="cam-label">{camera === 'ok' ? (countdown ? 'Check camera' : 'Monitoring on') : 'Camera…'}</div>
        </div>
      )}

      {needsCamera && countdown && camera === 'ok' && (
        <div className="position-fixed top-0 start-50 translate-middle-x mt-2 px-2" style={{ zIndex: 1900, maxWidth: 560, width: '100%' }} role="alert">
          <div className="alert alert-danger shadow mb-0 py-2 d-flex align-items-center gap-2">
            <i className="bi bi-camera-video-off-fill fs-5" aria-hidden="true" />
            <span className="small fw-semibold">
              {countdown.kind === 'NO_FACE'
                ? 'We cannot see you. Please stay in front of the camera.'
                : 'More than one person is visible. Only you may be in view.'}
              {countdown.left > 0 && ` This will be recorded in ${countdown.left}s.`}
            </span>
          </div>
        </div>
      )}

      {needsCamera && camera === 'blocked' && (
        <div className="proctor-overlay" role="alertdialog" aria-label="Camera required">
          <div style={{ maxWidth: 440 }}>
            <i className="bi bi-camera-video-off display-4 d-block mb-3" aria-hidden="true" />
            <h2 className="h4 fw-bold">Camera required</h2>
            <p className="text-white-50">{cameraError}</p>
            <button className="btn btn-success btn-lg rounded-pill px-4" onClick={startCamera}>Try again</button>
            <div className="mt-3"><button className="btn btn-link text-white-50" onClick={auth.logout}>Sign out</button></div>
          </div>
        </div>
      )}
    </ProctoringContext.Provider>
  )
}
