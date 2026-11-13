import { useEffect, useState } from 'react'
import { Navigate, useNavigate } from 'react-router-dom'
import Swal from 'sweetalert2'
import api, { errorCode, errorMessage } from '../api/client'
import { useStudentAuth } from '../auth/StudentAuth'
import { fmtDateTime, Spinner, useCountdown } from '../ui/kit'

function CompetitionBanner({ info }) {
  const target = info?.status === 'NOT_STARTED' ? info.startDateTimeUtc : info?.status === 'LIVE' ? info.endDateTimeUtc : null
  const left = useCountdown(target, info?.serverTimeUtc)
  if (!info || info.status === 'NONE') return <div className="alert alert-secondary small py-2">No competition is scheduled at the moment.</div>
  const clock = left == null ? '' : `${Math.floor(left / 86400) > 0 ? `${Math.floor(left / 86400)}d ` : ''}${String(Math.floor((left % 86400) / 3600)).padStart(2, '0')}:${String(Math.floor((left % 3600) / 60)).padStart(2, '0')}:${String(left % 60).padStart(2, '0')}`
  return (
    <div className={`alert small py-2 ${info.status === 'LIVE' ? 'alert-success' : info.status === 'NOT_STARTED' ? 'alert-warning' : 'alert-secondary'}`} role="status">
      <div className="fw-bold">{info.competitionName}</div>
      {info.status === 'LIVE' && <div>Live now · ends in <span className="play-timer fw-semibold">{clock}</span></div>}
      {info.status === 'NOT_STARTED' && <div>Starts {fmtDateTime(info.startDateTimeUtc)} · in <span className="play-timer fw-semibold">{clock}</span></div>}
      {info.status === 'ENDED' && <div>Ended {fmtDateTime(info.endDateTimeUtc)}. You can still sign in to see your attempts.</div>}
    </div>
  )
}

export default function LoginPage() {
  const auth = useStudentAuth()
  const navigate = useNavigate()
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [showPassword, setShowPassword] = useState(false)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')
  const [info, setInfo] = useState(null)

  useEffect(() => { api.get('/competition/active').then((r) => setInfo(r.data)).catch(() => {}) }, [])

  if (auth.status === 'loading') return <Spinner full />
  if (auth.status === 'authed') return <Navigate to="/precheck" replace />

  const submit = async (e, terminateExisting = false) => {
    e?.preventDefault()
    if (!username.trim() || !password) { setError('Enter your username and password.'); return }
    setBusy(true)
    setError('')
    try {
      await auth.login(username.trim(), password, terminateExisting)
      navigate('/precheck', { replace: true })
    } catch (err) {
      const code = errorCode(err)
      if (code === 'SESSION_ACTIVE') {
        const result = await Swal.fire({
          icon: 'warning', title: 'Already signed in',
          text: 'You are signed in on another device or tab. Continue here and end that session? This is recorded.',
          showCancelButton: true, confirmButtonText: 'Continue here', reverseButtons: true,
        })
        if (result.isConfirmed) { setBusy(false); return submit(null, true) }
      } else if (code === 'BANNED') {
        // Access is restricted: go to the page that explains why and offers a review request.
        navigate('/appeal', { replace: true, state: err.response.data })
        return
      } else {
        setError(errorMessage(err))
      }
    }
    setBusy(false)
  }

  return (
    <div className="login-wrap d-flex align-items-center py-4">
      <div className="container" style={{ maxWidth: 440 }}>
        <div className="card border-0 shadow rounded-4 fade-in">
          <div className="card-body p-4 p-sm-5">
            <div className="text-center mb-3">
              <img src="/logo.png" alt="HEM Foundation" style={{ maxHeight: 84, maxWidth: '70%' }} />
              <h1 className="h4 fw-bold mt-3 mb-1">Student Competition</h1>
              <p className="text-secondary small mb-0">Sign in with the details you were given.</p>
            </div>
            <CompetitionBanner info={info} />
            {auth.notice && <div className="alert alert-warning small py-2" role="alert">{auth.notice}</div>}
            {error && <div className="alert alert-danger small py-2" role="alert">{error}</div>}
            <form onSubmit={submit} noValidate>
              <label className="form-label fw-semibold small" htmlFor="login-user">Username or email</label>
              <input id="login-user" className="form-control form-control-lg mb-3" autoComplete="username" autoCapitalize="none" spellCheck={false}
                value={username} onChange={(e) => setUsername(e.target.value)} maxLength={256} required />
              <label className="form-label fw-semibold small" htmlFor="login-pass">Password</label>
              <div className="input-group input-group-lg mb-4">
                <input id="login-pass" className="form-control" type={showPassword ? 'text' : 'password'} autoComplete="current-password"
                  value={password} onChange={(e) => setPassword(e.target.value)} maxLength={200} required />
                <button type="button" className="btn btn-outline-secondary" onClick={() => setShowPassword((s) => !s)}
                  aria-label={showPassword ? 'Hide password' : 'Show password'}>
                  <i className={`bi ${showPassword ? 'bi-eye-slash' : 'bi-eye'}`} aria-hidden="true" />
                </button>
              </div>
              <button type="submit" className="btn btn-success btn-lg w-100 rounded-pill" disabled={busy}>
                {busy && <span className="spinner-border spinner-border-sm me-2" aria-hidden="true" />}Sign in
              </button>
            </form>
            <p className="text-center text-secondary small mt-4 mb-0">
              Forgotten your password? Ask your school coordinator or the competition administrator to reset it.
            </p>
          </div>
        </div>
      </div>
    </div>
  )
}
