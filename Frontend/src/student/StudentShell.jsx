import { useEffect, useState } from 'react'
import { Link, Navigate, Outlet, useLocation, useNavigate } from 'react-router-dom'
import { useStudentAuth } from '../auth/StudentAuth'
import { ProctoringProvider, useProctoring } from '../proctoring/ProctoringProvider'
import { confirm, Spinner } from '../ui/kit'

/** Requires sign-in, then consent; after that everything inside runs under monitoring. */
export function RequireStudent() {
  const auth = useStudentAuth()
  if (auth.status === 'loading') return <Spinner full />
  if (auth.status !== 'authed') return <Navigate to="/" replace />
  return <Outlet />
}

function ConsentGate() {
  const { config } = useProctoring()
  if (!config) return <Spinner full label="Checking your security status…" />
  // Terms must be accepted for the current consent version, and monitoring opened for this login.
  if (!config.termsAccepted || (config.proctoringRequired && !config.sessionActive)) return <Navigate to="/precheck" replace />
  return <Outlet />
}

export function MonitoredArea() {
  return <ProctoringProvider><ConsentGate /></ProctoringProvider>
}

export function StudentLayout() {
  const { profile, logout } = useStudentAuth()
  const navigate = useNavigate()
  const { pathname } = useLocation()
  const [online, setOnline] = useState(navigator.onLine)
  useEffect(() => {
    const on = () => setOnline(true), off = () => setOnline(false)
    window.addEventListener('online', on); window.addEventListener('offline', off)
    return () => { window.removeEventListener('online', on); window.removeEventListener('offline', off) }
  }, [])
  const name = [profile?.firstName, profile?.lastName].filter(Boolean).join(' ') || profile?.username

  const signOut = async () => {
    if (await confirm('Sign out?', 'You can sign in again later to continue.', 'Sign out')) {
      await logout()
      navigate('/', { replace: true })
    }
  }

  return (
    <div className="min-vh-100 d-flex flex-column">
      <header className="brand-bar shadow-sm">
        <div className="container d-flex align-items-center justify-content-between py-2 gap-2">
          <Link to="/dashboard" className="d-flex align-items-center gap-2 text-white text-decoration-none">
            <img src="/logo.png" alt="" height="36" className="bg-white rounded-2 p-1" />
            <span className="fw-bold d-none d-sm-inline">HEM Competition</span>
          </Link>
          <div className="d-flex align-items-center gap-2">
            {pathname !== '/dashboard' && <Link to="/dashboard" className="btn btn-sm btn-outline-light"><i className="bi bi-grid-fill me-1" aria-hidden="true" />Dashboard</Link>}
            <span className="small text-truncate d-none d-md-inline" style={{ maxWidth: 200 }}><i className="bi bi-person-circle me-1" aria-hidden="true" />{name}</span>
            <button className="btn btn-sm btn-light" onClick={signOut}><i className="bi bi-box-arrow-right me-1" aria-hidden="true" /><span className="d-none d-sm-inline">Sign out</span></button>
          </div>
        </div>
      </header>
      {!online && <div className="bg-warning text-dark text-center small py-1" role="status">You are offline. Your work will continue when the connection returns.</div>}
      <main className="flex-grow-1"><Outlet /></main>
    </div>
  )
}

export function DuplicateTabPage() {
  return (
    <div className="login-wrap d-flex align-items-center">
      <div className="container text-center" style={{ maxWidth: 460 }}>
        <div className="card border-0 shadow rounded-4"><div className="card-body p-4">
          <i className="bi bi-window-stack display-4 text-warning" aria-hidden="true" />
          <h1 className="h4 fw-bold mt-2">Already open in another tab</h1>
          <p className="text-secondary">The competition can be open in only one tab. Close this tab and continue in the first one.</p>
          <button className="btn btn-outline-secondary rounded-pill px-4" onClick={() => window.location.assign('/dashboard')}>
            I have closed the other tab
          </button>
        </div></div>
      </div>
    </div>
  )
}
