import { useCallback, useEffect, useState } from 'react'
import { Link, useLocation } from 'react-router-dom'
import api, { errorMessage } from '../api/client'
import { Badge, fmtDateTime, Spinner } from '../ui/kit'

/**
 * Where a restricted student lands after signing in. Uses the short-lived appeal token from the
 * login response (kept only in memory), which can file and track a request and nothing else.
 */
export default function AppealPage() {
  const { state } = useLocation()
  const appealToken = state?.appealToken
  const [data, setData] = useState(null)
  const [message, setMessage] = useState('')
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')
  const [expired, setExpired] = useState(false)

  const load = useCallback(async () => {
    try {
      setData((await api.get('/appeals/mine', { appealToken })).data)
    } catch (e) {
      if (e?.response?.status === 401) setExpired(true)
      else setError(errorMessage(e))
    }
  }, [appealToken])

  useEffect(() => { if (appealToken) load() }, [appealToken, load])

  if (!appealToken || expired) {
    return (
      <div className="login-wrap d-flex align-items-center">
        <div className="container text-center" style={{ maxWidth: 440 }}>
          <div className="card border-0 shadow rounded-4"><div className="card-body p-4">
            <h1 className="h5 fw-bold">Please sign in again</h1>
            <p className="text-secondary small">For your security this page is only available for a short time after signing in.</p>
            <Link to="/" className="btn btn-success rounded-pill px-4">Go to sign in</Link>
          </div></div>
        </div>
      </div>
    )
  }
  if (!data) return error ? <div className="container py-5"><div className="alert alert-danger">{error}</div></div> : <Spinner full />

  const ban = data.ban || state.ban
  const submit = async (e) => {
    e.preventDefault()
    setBusy(true)
    setError('')
    try {
      await api.post('/appeals', { message }, { appealToken })
      setMessage('')
      await load()
    } catch (err) {
      setError(errorMessage(err))
    }
    setBusy(false)
  }

  return (
    <div className="login-wrap py-4">
      <div className="container" style={{ maxWidth: 640 }}>
        <div className="card border-0 shadow rounded-4 fade-in">
          <div className="card-body p-4">
            <div className="d-flex align-items-center gap-3 mb-3">
              <i className="bi bi-shield-lock-fill text-danger display-6" aria-hidden="true" />
              <div>
                <h1 className="h4 fw-bold mb-0">{data.restricted ? 'Your access is restricted' : 'Your access has been restored'}</h1>
                <div className="text-secondary small">{data.restricted ? state.message : 'You can sign in and continue.'}</div>
              </div>
            </div>

            {data.restricted && ban && (
              <dl className="row small mb-3">
                <dt className="col-4 text-secondary">Status</dt><dd className="col-8"><Badge value={ban.type} /></dd>
                <dt className="col-4 text-secondary">Reason</dt><dd className="col-8">{ban.reason}</dd>
                <dt className="col-4 text-secondary">Since</dt><dd className="col-8">{fmtDateTime(ban.startsAtUtc)}</dd>
                <dt className="col-4 text-secondary">Until</dt>
                <dd className="col-8">{ban.endsAtUtc ? fmtDateTime(ban.endsAtUtc) : 'Until an administrator reviews it'}</dd>
              </dl>
            )}

            {data.appeals.length > 0 && (
              <>
                <h2 className="h6 fw-bold">Your review requests</h2>
                {data.appeals.map((a) => (
                  <div key={a.id} className="border rounded-3 p-3 mb-2 small">
                    <div className="d-flex justify-content-between align-items-center mb-1">
                      <span className="text-secondary">Sent {fmtDateTime(a.submittedAtUtc)}</span>
                      <Badge value={a.status} label={a.status === 'Pending' ? 'Under review' : a.status === 'Approved' ? 'Accepted' : 'Not accepted'} />
                    </div>
                    <div style={{ whiteSpace: 'pre-wrap' }}>{a.message}</div>
                    {a.decisionReason && <div className="mt-2 p-2 bg-light rounded"><strong>Decision ({fmtDateTime(a.decidedAtUtc)}):</strong> {a.decisionReason}</div>}
                  </div>
                ))}
              </>
            )}

            {data.restricted && data.canAppeal && (
              <form onSubmit={submit} className="mt-3">
                <h2 className="h6 fw-bold">Request reconsideration</h2>
                <p className="small text-secondary">
                  If you believe this restriction is a mistake, explain what happened. A reviewer will look at your request together with the recorded events.
                  Sending a request does not restore access by itself.
                </p>
                <textarea className="form-control" rows={5} maxLength={4000} value={message} onChange={(e) => setMessage(e.target.value)}
                  placeholder="Describe what happened in your own words (at least 20 characters)." aria-label="Your explanation" required />
                <div className="d-flex justify-content-between align-items-center mt-2">
                  <span className="form-text">{message.trim().length}/4000 · {data.appealsRemaining} request(s) left</span>
                  <button className="btn btn-success rounded-pill px-4" disabled={busy || message.trim().length < 20}>
                    {busy && <span className="spinner-border spinner-border-sm me-2" aria-hidden="true" />}Send request
                  </button>
                </div>
              </form>
            )}
            {data.restricted && !data.canAppeal && data.appeals.some((a) => a.status === 'Pending') && (
              <div className="alert alert-info small mt-3 mb-0">Your request is being reviewed. Sign in again later to see the decision.</div>
            )}
            {data.restricted && !data.canAppeal && !data.appeals.some((a) => a.status === 'Pending') && (
              <div className="alert alert-secondary small mt-3 mb-0">
                {data.appealsEnabled ? 'No further requests can be made for this restriction.' : 'Review requests are not being accepted at the moment.'}
                {data.supportContact && <> Contact: {data.supportContact}</>}
              </div>
            )}
            {error && <div className="alert alert-danger small mt-3 mb-0" role="alert">{error}</div>}
            <div className="text-center mt-4"><Link to="/" className="btn btn-link">Back to sign in</Link></div>
          </div>
        </div>
      </div>
    </div>
  )
}
