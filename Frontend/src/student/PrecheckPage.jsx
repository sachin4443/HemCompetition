import { useCallback, useEffect, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { FaceDetector, FilesetResolver } from '@mediapipe/tasks-vision'
import api, { errorMessage } from '../api/client'
import { useStudentAuth } from '../auth/StudentAuth'
import { openCamera } from '../proctoring/ProctoringProvider'
import { Check, ErrorBox, Spinner } from '../ui/kit'

const RULES = [
  'Work alone. Only you may be visible to the camera while you play.',
  'Stay on the competition page. Do not switch tabs, windows or apps.',
  'Keep the page in fullscreen while a game or quiz is open.',
  'Do not copy, paste, or use developer tools.',
  'Use one device and one browser tab. Signing in elsewhere ends this session.',
  '"Next" skips a question for good. "Skip" leaves the whole game and you cannot return to it.',
]

function CheckRow({ state, label, detail }) {
  const icon = state === 'ok' ? 'bi-check-circle-fill text-success' : state === 'fail' ? 'bi-x-circle-fill text-danger' : state === 'warn' ? 'bi-exclamation-circle-fill text-warning' : 'bi-hourglass-split text-secondary'
  return (
    <li className="d-flex gap-2 align-items-start py-2 border-bottom">
      <i className={`bi ${icon} fs-5`} aria-hidden="true" />
      <div><div className="fw-semibold">{label}</div>{detail && <div className="small text-secondary">{detail}</div>}</div>
    </li>
  )
}

/**
 * Shown after sign-in and before the dashboard: explains monitoring, asks for explicit consent,
 * and checks that the device can actually run the competition. The camera is only switched on
 * after the student presses the button that says so.
 */
export default function PrecheckPage() {
  const navigate = useNavigate()
  const { logout } = useStudentAuth()
  const [status, setStatus] = useState(null)
  const [error, setError] = useState(null)
  const [terms, setTerms] = useState(false)
  const [evidence, setEvidence] = useState(false)
  const [video, setVideo] = useState(false)
  const [cam, setCam] = useState({ state: 'idle', detail: 'Not checked yet' })
  const [faces, setFaces] = useState(null)
  const [busy, setBusy] = useState(false)
  const videoRef = useRef(null)
  const streamRef = useRef(null)

  useEffect(() => {
    api.get('/proctoring/status').then(({ data }) => {
      setStatus(data)
      if (!data.proctoringRequired) {
        // Nothing to consent to or check: record acceptance of the rules page only when asked.
        setCam({ state: 'ok', detail: 'Not required for this competition' })
      }
    }).catch(setError)
    return () => streamRef.current?.getTracks().forEach((t) => t.stop())
  }, [])

  const testCamera = useCallback(async () => {
    setCam({ state: 'pending', detail: 'Waiting for permission…' })
    try {
      streamRef.current?.getTracks().forEach((t) => t.stop())
      const stream = await openCamera()
      streamRef.current = stream
      videoRef.current.srcObject = stream
      await videoRef.current.play()
      setCam({ state: 'pending', detail: 'Loading face detection…' })
      const vision = await FilesetResolver.forVisionTasks('/mediapipe-wasm')
      const detector = await FaceDetector.createFromOptions(vision, {
        baseOptions: { modelAssetPath: '/models/blaze_face_short_range.tflite' }, runningMode: 'VIDEO',
      })
      let seen = 0
      for (let i = 0; i < 12; i++) {
        await new Promise((r) => setTimeout(r, 300))
        const count = detector.detectForVideo(videoRef.current, performance.now()).detections.length
        setFaces(count)
        seen = count === 1 ? seen + 1 : 0
        if (seen >= 3) break
      }
      detector.close()
      setCam(seen >= 3
        ? { state: 'ok', detail: 'Camera works and you are clearly visible.' }
        : { state: 'fail', detail: 'We could not see exactly one face. Sit facing the camera in good light, alone, and test again.' })
    } catch (e) {
      setCam({
        state: 'fail',
        detail: e?.name === 'NotAllowedError' ? 'Camera permission was denied. Allow it in the browser address bar, then test again.'
          : e?.name === 'NotFoundError' ? 'No camera was found on this device.'
            : 'The camera could not be started on this device.',
      })
    }
  }, [])

  if (error) return <div className="container py-5"><ErrorBox error={error} onRetry={() => window.location.reload()} /></div>
  if (!status) return <Spinner full label="Preparing your security check…" />

  const cameraNeeded = status.proctoringRequired && status.cameraRequired
  const fullscreenOk = !status.fullscreenRequired || (!!document.documentElement.requestFullscreen && document.fullscreenEnabled !== false)
  const online = navigator.onLine
  const canContinue = terms && online && (!cameraNeeded || cam.state === 'ok') && (!status.evidenceCaptureEnabled || !cameraNeeded || evidence)

  const proceed = async () => {
    setBusy(true)
    try {
      await api.post('/proctoring/consent', { acceptTerms: true, acceptEvidenceCapture: evidence, acceptVideoCapture: video })
      streamRef.current?.getTracks().forEach((t) => t.stop())
      // The monitoring provider mounts with the dashboard and reads the consent just saved.
      navigate('/dashboard', { replace: true })
    } catch (e) {
      setError(errorMessage(e))
      setBusy(false)
    }
  }

  return (
    <div className="container py-4" style={{ maxWidth: 920 }}>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h1 className="h4 fw-bold m-0"><i className="bi bi-shield-check text-success me-2" aria-hidden="true" />Before you start</h1>
        <button className="btn btn-sm btn-outline-secondary" onClick={() => logout().then(() => navigate('/'))}>Sign out</button>
      </div>

      <div className="row g-3">
        <div className="col-12 col-lg-6">
          <div className="card border-0 shadow-sm rounded-4 h-100">
            <div className="card-body">
              <h2 className="h6 fw-bold">Fair competition rules</h2>
              <ol className="small ps-3 mb-3">{RULES.map((r) => <li key={r} className="mb-1">{r}</li>)}</ol>
              <p className="small"><a href="/Competition.pdf" target="_blank" rel="noopener noreferrer"><i className="bi bi-file-earmark-pdf me-1" aria-hidden="true" />Open the illustrated user guide (PDF)</a></p>

              {status.proctoringRequired && (
                <>
                  <h2 className="h6 fw-bold">How monitoring works</h2>
                  <ul className="small ps-3 mb-0">
                    {cameraNeeded && <li>Your camera stays on while you are signed in. Face detection runs on your own device; video is <strong>not</strong> streamed or recorded continuously.</li>}
                    {cameraNeeded && <li>The system only checks whether one person is visible. It does <strong>not</strong> identify you by your face.</li>}
                    <li>Rule violations (for example leaving the page) are recorded with the time they happened.</li>
                    {status.evidenceCaptureEnabled && cameraNeeded && (
                      <li>When a violation is detected, one photo{status.videoEvidenceEnabled ? ` and, if you agree, a ${status.videoClipSeconds}-second clip` : ''} may be saved as evidence.</li>
                    )}
                    {status.evidenceCaptureEnabled && <li>Evidence can be seen only by authorised reviewers and is deleted after {status.evidenceRetentionDays} days.</li>}
                    <li>A detection is not treated as proof of cheating. Repeated or serious violations are reviewed by a person, and you can ask for a review of any restriction.</li>
                  </ul>
                </>
              )}
            </div>
          </div>
        </div>

        <div className="col-12 col-lg-6">
          <div className="card border-0 shadow-sm rounded-4 mb-3">
            <div className="card-body">
              <h2 className="h6 fw-bold">Device check</h2>
              <ul className="list-unstyled mb-0">
                <CheckRow state={online ? 'ok' : 'fail'} label="Internet connection" detail={online ? 'Connected' : 'You appear to be offline.'} />
                <CheckRow state={fullscreenOk ? 'ok' : 'warn'} label="Fullscreen mode"
                  detail={fullscreenOk ? 'Supported' : 'This browser cannot go fullscreen; the competition will run without it.'} />
                {cameraNeeded && <CheckRow state={cam.state === 'idle' ? 'pending' : cam.state} label="Camera" detail={cam.detail} />}
              </ul>
              {cameraNeeded && (
                <div className="mt-3">
                  <div className="ratio ratio-4x3 bg-dark rounded-3 overflow-hidden mb-2" style={{ maxWidth: 320 }}>
                    <video ref={videoRef} muted playsInline style={{ transform: 'scaleX(-1)', objectFit: 'cover' }} aria-label="Camera preview" />
                  </div>
                  {faces != null && <div className="small text-secondary mb-2">People detected: {faces}</div>}
                  <button className="btn btn-outline-success" onClick={testCamera} disabled={cam.state === 'pending'}>
                    <i className="bi bi-camera-video me-2" aria-hidden="true" />{cam.state === 'idle' ? 'Turn on camera and test' : 'Test again'}
                  </button>
                </div>
              )}
            </div>
          </div>

          <div className="card border-0 shadow-sm rounded-4">
            <div className="card-body">
              <h2 className="h6 fw-bold">Your consent</h2>
              <Check switchStyle={false} checked={terms} onChange={setTerms}
                label={status.proctoringRequired ? 'I have read the rules and agree to be monitored as described.' : 'I have read and accept the competition rules.'} />
              {status.evidenceCaptureEnabled && cameraNeeded && (
                <Check switchStyle={false} checked={evidence} onChange={(v) => { setEvidence(v); if (!v) setVideo(false) }}
                  label="I agree that a photo may be captured when a violation is detected." />
              )}
              {status.videoEvidenceEnabled && cameraNeeded && (
                <Check switchStyle={false} checked={video} onChange={setVideo} disabled={!evidence}
                  label={`Optional: I also agree to a short video clip (${status.videoClipSeconds} seconds) in that case.`} />
              )}
              <button className="btn btn-success btn-lg w-100 rounded-pill mt-2" disabled={!canContinue || busy} onClick={proceed}>
                {busy && <span className="spinner-border spinner-border-sm me-2" aria-hidden="true" />}Accept and continue
              </button>
              {!canContinue && <div className="form-text">Complete the checks and tick the required boxes to continue.</div>}
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
