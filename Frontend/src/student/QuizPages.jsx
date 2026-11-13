import { useCallback, useEffect, useRef, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import api, { errorCode, errorMessage } from '../api/client'
import ExamGuard from '../proctoring/ExamGuard'
import { confirm, ErrorBox, fmtDuration, Spinner, useCountdown } from '../ui/kit'

/** Question image, fetched with the student's token (images are never public URLs). */
function QuestionImage({ attemptId, orderNo }) {
  const [src, setSrc] = useState(null)
  useEffect(() => {
    let url
    api.get(`/quiz/attempts/${attemptId}/questions/${orderNo}/image`, { responseType: 'blob' })
      .then((r) => { url = URL.createObjectURL(r.data); setSrc(url) }).catch(() => {})
    return () => url && URL.revokeObjectURL(url)
  }, [attemptId, orderNo])
  return src ? <img src={src} alt="Illustration for this question" className="img-fluid rounded-3 mb-3" style={{ maxHeight: 280 }} /> : null
}

export function QuizPlayPage() {
  const { quizId } = useParams()
  const navigate = useNavigate()
  const [paper, setPaper] = useState(null)
  const [error, setError] = useState(null)
  const [index, setIndex] = useState(0)
  const [answers, setAnswers] = useState({})     // orderNo -> optionId
  const [saving, setSaving] = useState({})       // orderNo -> 'saving' | 'error'
  const [submitting, setSubmitting] = useState(false)
  const [notice, setNotice] = useState('')
  const submitted = useRef(false)

  const load = useCallback(async () => {
    setError(null)
    try {
      const { data } = await api.post(`/quiz/${quizId}/start`)
      if (data.status !== 'InProgress') { navigate(`/quiz/result/${data.id}`, { replace: true }); return }
      setPaper(data)
      setAnswers(Object.fromEntries(data.questions.filter((q) => q.selectedOptionId != null).map((q) => [q.orderNo, q.selectedOptionId])))
    } catch (e) {
      setError(e)
    }
  }, [quizId, navigate])
  useEffect(() => { load() }, [load])

  const remaining = useCountdown(paper?.expiresAtUtc, paper?.serverTimeUtc)

  const finish = useCallback(async () => {
    if (submitted.current || !paper) return
    submitted.current = true
    setSubmitting(true)
    try {
      await api.post(`/quiz/attempts/${paper.id}/submit`)
      navigate(`/quiz/result/${paper.id}`, { replace: true })
    } catch (e) {
      submitted.current = false
      setSubmitting(false)
      setNotice(errorMessage(e))
    }
  }, [paper, navigate])

  // Time up: submit automatically. The server enforces the deadline as well.
  useEffect(() => { if (remaining === 0 && paper) finish() }, [remaining, paper, finish])

  const choose = async (orderNo, optionId) => {
    const previous = answers[orderNo]
    const value = previous === optionId ? null : optionId // tapping the chosen option again clears it
    setAnswers((a) => ({ ...a, [orderNo]: value }))
    setSaving((s) => ({ ...s, [orderNo]: 'saving' }))
    try {
      await api.put(`/quiz/attempts/${paper.id}/answer`, { orderNo, optionId: value })
      setSaving((s) => ({ ...s, [orderNo]: undefined }))
    } catch (e) {
      if (['ATTEMPT_EXPIRED', 'ATTEMPT_FINISHED'].includes(errorCode(e))) { navigate(`/quiz/result/${paper.id}`, { replace: true }); return }
      // Roll back so the screen always shows what the server actually has.
      setAnswers((a) => ({ ...a, [orderNo]: previous }))
      setSaving((s) => ({ ...s, [orderNo]: 'error' }))
      setNotice(`Your answer to question ${orderNo} was not saved. ${errorMessage(e)}`)
    }
  }

  const askSubmit = async () => {
    const unanswered = paper.questions.filter((q) => answers[q.orderNo] == null).length
    const text = unanswered ? `${unanswered} question(s) are not answered. After submitting you cannot change anything.` : 'After submitting you cannot change your answers.'
    if (await confirm('Submit the quiz?', text, 'Submit', unanswered > 0)) finish()
  }

  if (error) {
    return (
      <div className="container py-5" style={{ maxWidth: 560 }}>
        <ErrorBox error={error} onRetry={load} />
        <Link to="/section/quiz" className="btn btn-outline-secondary rounded-pill">Back</Link>
      </div>
    )
  }
  if (!paper) return <Spinner full label="Preparing your quiz…" />

  const q = paper.questions[index]
  const answered = paper.questions.filter((x) => answers[x.orderNo] != null).length
  const low = remaining != null && remaining <= 60

  return (
    <ExamGuard>
      <div className="play-shell py-3 px-2">
        <div className="container" style={{ maxWidth: 860 }}>
          <div className="d-flex justify-content-between align-items-center mb-3 gap-2">
            <div>
              <div className="fw-bold">{paper.title}</div>
              <div className="small text-white-50">{answered} of {paper.questions.length} answered{paper.negativeMarks > 0 && ` · −${paper.negativeMarks} for a wrong answer`}</div>
            </div>
            <div className={`badge fs-6 play-timer ${low ? 'text-bg-danger' : 'text-bg-light'}`} role="timer" aria-live={low ? 'assertive' : 'off'}>
              <i className="bi bi-clock me-1" aria-hidden="true" />{fmtDuration(remaining)}
            </div>
          </div>

          <div className="play-card shadow p-3 p-md-4">
            <div className="d-flex justify-content-between small text-secondary mb-2">
              <span>Question {index + 1} of {paper.questions.length}</span>
              <span>{q.marks} mark{q.marks === 1 ? '' : 's'}{saving[q.orderNo] === 'saving' && ' · saving…'}</span>
            </div>
            <h1 className="h5 fw-semibold mb-3" style={{ whiteSpace: 'pre-wrap' }}>{q.text}</h1>
            {q.hasImage && <QuestionImage attemptId={paper.id} orderNo={q.orderNo} />}
            <div className="d-grid gap-2" role="radiogroup" aria-label="Answer options">
              {q.options.map((o, i) => {
                const selected = answers[q.orderNo] === o.id
                return (
                  <button key={o.id} type="button" role="radio" aria-checked={selected} className={`quiz-option ${selected ? 'selected' : ''}`}
                    onClick={() => choose(q.orderNo, o.id)} disabled={submitting}>
                    <span className={`badge rounded-circle ${selected ? 'text-bg-success' : 'text-bg-light border'}`} style={{ width: 28, height: 28, lineHeight: '20px' }}>{String.fromCharCode(65 + i)}</span>
                    <span>{o.text}</span>
                  </button>
                )
              })}
            </div>

            {notice && <div className="alert alert-warning small py-2 mt-3 mb-0" role="alert">{notice}</div>}

            <div className="d-flex justify-content-between gap-2 mt-4">
              <button className="btn btn-outline-secondary rounded-pill px-3" disabled={index === 0} onClick={() => setIndex(index - 1)}><i className="bi bi-chevron-left" aria-hidden="true" /> Previous</button>
              {index < paper.questions.length - 1
                ? <button className="btn btn-primary rounded-pill px-4" onClick={() => setIndex(index + 1)}>Next <i className="bi bi-chevron-right" aria-hidden="true" /></button>
                : <button className="btn btn-success rounded-pill px-4" onClick={askSubmit} disabled={submitting}>Submit quiz</button>}
            </div>
          </div>

          <div className="play-card shadow p-3 mt-3">
            <div className="d-flex justify-content-between align-items-center mb-2">
              <span className="small fw-semibold text-secondary">Jump to a question</span>
              <button className="btn btn-sm btn-success rounded-pill px-3" onClick={askSubmit} disabled={submitting}>
                {submitting && <span className="spinner-border spinner-border-sm me-1" aria-hidden="true" />}Submit quiz
              </button>
            </div>
            <div className="quiz-nav d-flex flex-wrap gap-2">
              {paper.questions.map((x, i) => (
                <button key={x.orderNo} className={`${answers[x.orderNo] != null ? 'answered' : ''} ${i === index ? 'current' : ''}`} onClick={() => setIndex(i)}
                  aria-label={`Question ${i + 1}${answers[x.orderNo] != null ? ', answered' : ', not answered'}`}>{i + 1}</button>
              ))}
            </div>
          </div>
        </div>
      </div>
    </ExamGuard>
  )
}

export function QuizResultPage() {
  const { attemptId } = useParams()
  const [result, setResult] = useState(null)
  const [error, setError] = useState(null)
  useEffect(() => { api.get(`/quiz/attempts/${attemptId}/result`).then((r) => setResult(r.data)).catch(setError) }, [attemptId])

  if (error) return <div className="container py-5" style={{ maxWidth: 560 }}><ErrorBox error={error} /><Link to="/dashboard" className="btn btn-outline-secondary rounded-pill">Dashboard</Link></div>
  if (!result) return <Spinner full />

  return (
    <div className="container py-4 fade-in" style={{ maxWidth: 760 }}>
      <div className="card border-0 shadow-sm rounded-4 text-center mb-3">
        <div className="card-body p-4">
          <i className="bi bi-patch-check-fill text-success display-4" aria-hidden="true" />
          <h1 className="h4 fw-bold mt-2">{result.title}</h1>
          <p className="text-secondary mb-3">
            {result.status === 'Expired' ? 'Time ran out and your quiz was submitted automatically.'
              : result.status === 'Terminated' ? 'This quiz was ended and has been sent for review.' : 'Your quiz has been submitted.'}
          </p>
          {result.resultsVisible ? (
            <div className="row g-2 justify-content-center">
              {[['Score', `${result.score} / ${result.maxScore}`, 'success'], ['Correct', result.correct, 'success'], ['Wrong', result.incorrect, 'danger'],
                ['Not answered', result.unanswered, 'secondary'], ['Time', fmtDuration(result.timeTakenSeconds), 'primary']].map(([label, value, tone]) => (
                <div className="col-6 col-md" key={label}>
                  <div className={`border border-${tone}-subtle rounded-3 py-2`}><div className="small text-secondary">{label}</div><div className="fs-5 fw-bold">{value}</div></div>
                </div>
              ))}
            </div>
          ) : (
            <div className="alert alert-info small mb-0">
              You answered {result.answered} of {result.totalQuestions} questions. Scores will be shown when the organisers publish the results.
            </div>
          )}
          <Link to="/section/quiz" className="btn btn-success rounded-pill px-4 mt-4">Continue</Link>
        </div>
      </div>

      {result.review && (
        <>
          <h2 className="h6 fw-bold">Answer review</h2>
          {result.review.map((q) => (
            <div key={q.orderNo} className="card border-0 shadow-sm rounded-4 mb-2"><div className="card-body">
              <div className="d-flex justify-content-between gap-2">
                <div className="fw-semibold" style={{ whiteSpace: 'pre-wrap' }}>{q.orderNo}. {q.text}</div>
                <span className={`badge align-self-start ${q.isCorrect ? 'text-bg-success' : q.isCorrect === false ? 'text-bg-danger' : 'text-bg-secondary'}`}>{q.awardedMarks > 0 ? '+' : ''}{q.awardedMarks}</span>
              </div>
              <ul className="list-unstyled small mt-2 mb-0">
                {q.options.map((o) => (
                  <li key={o.id} className={o.id === q.correctOptionId ? 'text-success fw-semibold' : o.id === q.selectedOptionId ? 'text-danger' : ''}>
                    <i className={`bi me-1 ${o.id === q.correctOptionId ? 'bi-check-circle-fill' : o.id === q.selectedOptionId ? 'bi-x-circle-fill' : 'bi-circle'}`} aria-hidden="true" />
                    {o.text}{o.id === q.selectedOptionId && ' (your answer)'}
                  </li>
                ))}
              </ul>
              {q.explanation && <div className="small bg-light rounded p-2 mt-2">{q.explanation}</div>}
            </div></div>
          ))}
        </>
      )}
    </div>
  )
}
