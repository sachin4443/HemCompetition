import { useCallback, useEffect, useRef, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import api, { errorCode, errorMessage } from '../api/client'
import ExamGuard from '../proctoring/ExamGuard'
import { confirm, ErrorBox, fmtDuration, Spinner, toast, useCountdown } from '../ui/kit'
import { BOARDS } from './boards'

const NUMBER_GAMES = ['game24', 'kenken', 'futoshiki']

function Elapsed({ since, serverTimeUtc }) {
  const [seconds, setSeconds] = useState(0)
  useEffect(() => {
    const offset = serverTimeUtc ? new Date(serverTimeUtc).getTime() - Date.now() : 0
    const tick = () => setSeconds(Math.max(0, Math.round((Date.now() + offset - new Date(since).getTime()) / 1000)))
    tick()
    const t = setInterval(tick, 1000)
    return () => clearInterval(t)
  }, [since, serverTimeUtc])
  return <span className="play-timer">{fmtDuration(seconds)}</span>
}

/**
 * The one screen every generated game is played on: timer, progress, the game's board, and the
 * Submit / Next / Skip / Back buttons. Refreshing or reconnecting resumes the same round because
 * the attempt lives on the server.
 */
export default function GamePlayPage() {
  const { gameKey } = useParams()
  const navigate = useNavigate()
  const [attempt, setAttempt] = useState(null)
  const [error, setError] = useState(null)
  const [message, setMessage] = useState(null) // { tone, text }
  const [busy, setBusy] = useState(false)
  const answerRef = useRef(null)
  const busyRef = useRef(false)
  const sectionKey = NUMBER_GAMES.includes(gameKey) ? 'number' : 'word'

  const start = useCallback(async () => {
    setError(null)
    try {
      setAttempt((await api.post(`/games/${gameKey}/start`)).data)
    } catch (e) {
      setError(e)
    }
  }, [gameKey])
  useEffect(() => { start() }, [start])

  const remaining = useCountdown(attempt?.status === 'InProgress' ? attempt.deadlineUtc : null, attempt?.serverTimeUtc)
  // When the clock runs out, ask the server for the final state (it closes the attempt).
  useEffect(() => {
    if (remaining === 0 && attempt?.status === 'InProgress') api.get(`/games/attempts/${attempt.id}`).then((r) => setAttempt(r.data)).catch(() => {})
  }, [remaining, attempt?.id, attempt?.status])

  const setAnswer = useCallback((a) => {
    const changed = JSON.stringify(a) !== JSON.stringify(answerRef.current)
    answerRef.current = a
    // The "not correct" hint belongs to the previous answer; drop it once the student changes something.
    if (changed) setMessage((m) => (m?.tone === 'warning' ? null : m))
  }, [])

  const act = useCallback(async (path, body, onDone) => {
    if (busyRef.current || !attempt) return // ignores double clicks
    busyRef.current = true
    setBusy(true)
    setMessage(null)
    try {
      const { data } = await api.post(`/games/attempts/${attempt.id}/${path}`, body)
      onDone(data)
    } catch (e) {
      if (['ATTEMPT_EXPIRED', 'ATTEMPT_FINISHED', 'ROUND_CLOSED'].includes(errorCode(e))) {
        // Our copy is out of date (time ran out, or another request already moved on): resync.
        try { setAttempt((await api.get(`/games/attempts/${attempt.id}`)).data) } catch { /* shown below */ }
      }
      setMessage({ tone: 'danger', text: errorMessage(e) })
    }
    busyRef.current = false
    setBusy(false)
  }, [attempt])

  const submit = useCallback((answer) => act('submit', { roundNo: attempt.currentRound, answer: answer || answerRef.current || {} }, (data) => {
    setAttempt(data.attempt)
    if (data.correct) toast('Correct! Answer saved.')
    else setMessage({ tone: 'warning', text: data.message || 'That is not correct. Try again.' })
  }), [act, attempt])

  const next = async () => {
    if (await confirm('Go to the next question?', 'You will not be able to come back to this one.', 'Next', true)) {
      act('next', { roundNo: attempt.currentRound }, setAttempt)
    }
  }
  const quit = async () => {
    if (await confirm('Skip this game?', 'This ends the game. You will not be able to rejoin or play it again.', 'Skip game', true)) {
      act('quit', {}, setAttempt)
    }
  }

  if (error) {
    return (
      <div className="container py-5" style={{ maxWidth: 560 }}>
        <ErrorBox error={error} onRetry={start} />
        <Link to="/dashboard" className="btn btn-outline-secondary rounded-pill">Back to dashboard</Link>
      </div>
    )
  }
  if (!attempt) return <Spinner full label="Preparing your game…" />

  if (attempt.status !== 'InProgress') {
    const reason = { Completed: 'You have completed this game.', Quit: 'You left this game.', Expired: 'Time is up for this game.', Terminated: 'This game was ended and sent for review.' }[attempt.status]
    return (
      <div className="play-shell d-flex align-items-center justify-content-center p-3">
        <div className="play-card shadow p-4 p-md-5 text-center fade-in" style={{ maxWidth: 460, width: '100%' }}>
          <i className={`bi ${attempt.status === 'Completed' ? 'bi-patch-check-fill text-success' : 'bi-flag-fill text-secondary'} display-3`} aria-hidden="true" />
          <h1 className="h4 fw-bold mt-3">{attempt.gameName}</h1>
          <p className="text-secondary">{reason}</p>
          <p className="fs-5 fw-semibold">{attempt.correctCount} of {attempt.totalRounds} solved</p>
          <button className="btn btn-success btn-lg rounded-pill px-4" onClick={() => navigate(`/section/${sectionKey}`, { replace: true })}>Continue</button>
        </div>
      </div>
    )
  }

  const Board = BOARDS[attempt.engine]
  const round = attempt.round
  return (
    <ExamGuard>
      <div className="play-shell py-3 px-2">
        <div className="container" style={{ maxWidth: 760 }}>
          <div className="d-flex justify-content-between align-items-center mb-3 gap-2">
            <Link to={`/section/${sectionKey}`} className="btn btn-sm btn-outline-light rounded-pill"><i className="bi bi-arrow-left me-1" aria-hidden="true" />Back</Link>
            <div className="text-center">
              <div className="fw-bold">{attempt.gameName}</div>
              <div className="progress-dots mt-1" aria-label={`Question ${attempt.currentRound} of ${attempt.totalRounds}`}>
                {Array.from({ length: attempt.totalRounds }, (_, i) => (
                  <span key={i} className={i + 1 < attempt.currentRound ? 'done' : i + 1 === attempt.currentRound ? 'current' : ''} />
                ))}
              </div>
            </div>
            <div className="text-end small">
              <div><i className="bi bi-stopwatch me-1" aria-hidden="true" /><Elapsed since={round.presentedAtUtc || attempt.startedAtUtc} serverTimeUtc={attempt.serverTimeUtc} /></div>
              {remaining != null && remaining < 3600 && <div className={remaining < 60 ? 'text-warning fw-bold' : 'text-white-50'}>ends in <span className="play-timer">{fmtDuration(remaining)}</span></div>}
            </div>
          </div>

          <div className="play-card shadow p-3 p-md-4 fade-in" key={round.roundNo}>
            <div className="text-secondary small text-center mb-2">Question {round.roundNo} of {attempt.totalRounds}</div>
            {Board
              ? <Board puzzle={round.puzzle} onChange={setAnswer} onComplete={submit} disabled={busy} />
              : <div className="alert alert-warning">This game cannot be displayed in this version of the app.</div>}

            {message && <div className={`alert alert-${message.tone} py-2 small mt-3 mb-0`} role="alert">{message.text}</div>}

            <div className="d-flex flex-wrap gap-2 justify-content-center mt-4">
              <button className="btn btn-success btn-lg rounded-pill px-4 flex-grow-1 flex-sm-grow-0" disabled={busy} onClick={() => submit()}>
                {busy ? <span className="spinner-border spinner-border-sm me-2" aria-hidden="true" /> : <i className="bi bi-check-lg me-1" aria-hidden="true" />}Submit
              </button>
              <button className="btn btn-outline-primary btn-lg rounded-pill px-4" disabled={busy} onClick={next}>Next<i className="bi bi-chevron-right ms-1" aria-hidden="true" /></button>
              <button className="btn btn-outline-danger btn-lg rounded-pill px-4" disabled={busy} onClick={quit}><i className="bi bi-skip-forward-fill me-1" aria-hidden="true" />Skip</button>
            </div>
          </div>
        </div>
      </div>
    </ExamGuard>
  )
}
