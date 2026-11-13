import { useCallback, useEffect, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useProctoring } from './ProctoringProvider'
import { acquireOwnership, initializeTabManager, startHeartbeat, stopHeartbeat } from './ActiveTabManager'

/*
 * Wraps a game or quiz while it is being played. Watches the browser for the things the rules
 * forbid and reports each one; the server decides the consequence. Nothing here is trusted as a
 * security boundary - answers and scores are validated on the server regardless.
 */
export default function ExamGuard({ children, onTerminated }) {
  const { config, report, onTerminated: subscribe } = useProctoring()
  const navigate = useNavigate()
  const canFullscreen = !!document.documentElement.requestFullscreen && document.fullscreenEnabled !== false
  const needFullscreen = !!config?.fullscreenRequired && canFullscreen
  const [inFullscreen, setInFullscreen] = useState(!!document.fullscreenElement)
  const [terminated, setTerminated] = useState(null)
  const wasFullscreen = useRef(!!document.fullscreenElement)

  // One tab only.
  useEffect(() => {
    initializeTabManager()
    if (!acquireOwnership()) {
      report('DUPLICATE_TAB', 'Competition opened in a second tab')
      navigate('/duplicate-tab', { replace: true })
      return undefined
    }
    startHeartbeat()
    return stopHeartbeat
  }, [navigate, report])

  useEffect(() => subscribe((outcome) => {
    setTerminated(outcome)
    onTerminated?.(outcome)
  }), [subscribe, onTerminated])

  useEffect(() => {
    // A reload in the middle of an attempt.
    const nav = performance.getEntriesByType?.('navigation')?.[0]
    if (nav?.type === 'reload' && !sessionStorage.getItem('hc_reload_reported')) {
      sessionStorage.setItem('hc_reload_reported', String(Date.now()))
      report('PAGE_REFRESH', 'Page reloaded during an attempt')
    }

    const onVisibility = () => document.hidden && report('TAB_SWITCH', 'Switched to another tab or window')
    // Blur also fires on tab switches; only report it when the tab itself is still visible.
    const onBlur = () => setTimeout(() => !document.hidden && !document.hasFocus() && report('WINDOW_BLUR', 'Test window lost focus'), 300)
    const onFullscreen = () => {
      const now = !!document.fullscreenElement
      setInFullscreen(now)
      if (!now && wasFullscreen.current) report('FULLSCREEN_EXIT', 'Left fullscreen mode')
      wasFullscreen.current = now
    }
    const onKey = (e) => {
      const key = e.key?.toUpperCase()
      const mod = e.ctrlKey || e.metaKey
      if (key === 'F12' || (mod && e.shiftKey && ['I', 'J', 'C'].includes(key)) || (mod && key === 'U')) {
        e.preventDefault()
        report('DEVTOOLS', 'Developer tools shortcut pressed')
      } else if (mod && ['P', 'S'].includes(key)) {
        e.preventDefault()
        report('BLOCKED_KEY', `Blocked shortcut: ${key}`)
      }
    }
    const onClipboard = (e) => { e.preventDefault(); report('COPY_PASTE', `${e.type} attempt`) }
    const onContext = (e) => { e.preventDefault(); report('CONTEXT_MENU', 'Right-click attempt') }
    let offlineAt = null
    const onOffline = () => { offlineAt = Date.now() }
    const onOnline = () => {
      if (offlineAt) report('NETWORK_LOST', 'Connection lost', { seconds: Math.round((Date.now() - offlineAt) / 1000) })
      offlineAt = null
    }

    document.addEventListener('visibilitychange', onVisibility)
    window.addEventListener('blur', onBlur)
    document.addEventListener('fullscreenchange', onFullscreen)
    window.addEventListener('keydown', onKey)
    for (const type of ['copy', 'cut', 'paste']) document.addEventListener(type, onClipboard)
    document.addEventListener('contextmenu', onContext)
    window.addEventListener('offline', onOffline)
    window.addEventListener('online', onOnline)
    return () => {
      document.removeEventListener('visibilitychange', onVisibility)
      window.removeEventListener('blur', onBlur)
      document.removeEventListener('fullscreenchange', onFullscreen)
      window.removeEventListener('keydown', onKey)
      for (const type of ['copy', 'cut', 'paste']) document.removeEventListener(type, onClipboard)
      document.removeEventListener('contextmenu', onContext)
      window.removeEventListener('offline', onOffline)
      window.removeEventListener('online', onOnline)
    }
  }, [report])

  // Leave fullscreen when the attempt screen is closed.
  useEffect(() => () => {
    wasFullscreen.current = false
    if (document.fullscreenElement) document.exitFullscreen().catch(() => {})
  }, [])

  const enterFullscreen = useCallback(async () => {
    try { await document.documentElement.requestFullscreen() } catch { /* the overlay stays until it works */ }
  }, [])

  if (terminated) {
    return (
      <div className="proctor-overlay" role="alertdialog" aria-label="Attempt ended">
        <div style={{ maxWidth: 460 }}>
          <i className="bi bi-shield-exclamation display-4 d-block mb-3 text-warning" aria-hidden="true" />
          <h2 className="h4 fw-bold">This attempt has been ended</h2>
          <p className="text-white-50">{terminated.message}</p>
          <p className="small text-white-50">The attempt has been sent for review. You can continue with the other activities.</p>
          <button className="btn btn-success btn-lg rounded-pill px-4" onClick={() => navigate('/dashboard', { replace: true })}>Back to dashboard</button>
        </div>
      </div>
    )
  }

  return (
    <>
      {children}
      {needFullscreen && !inFullscreen && (
        <div className="proctor-overlay" role="alertdialog" aria-label="Fullscreen required">
          <div style={{ maxWidth: 440 }}>
            <i className="bi bi-arrows-fullscreen display-4 d-block mb-3" aria-hidden="true" />
            <h2 className="h4 fw-bold">Fullscreen is required</h2>
            <p className="text-white-50">The competition runs in fullscreen. Leaving fullscreen is recorded.</p>
            <button className="btn btn-success btn-lg rounded-pill px-4" onClick={enterFullscreen} autoFocus>Enter fullscreen</button>
            <div className="mt-3"><button className="btn btn-link text-white-50" onClick={() => navigate('/dashboard')}>Back to dashboard</button></div>
          </div>
        </div>
      )}
    </>
  )
}
