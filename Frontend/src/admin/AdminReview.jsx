import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import api from '../api/client'
import { useAdminAuth } from '../auth/AdminAuth'
import {
  askReason, Badge, Check, confirm, DataTable, Empty, ErrorBox, Field, fmtBytes, fmtDateTime, fmtDuration, fromLocalInput, Modal, Pager,
  SearchBox, Spinner, SubmitButton, localInputInHours, useFetch, usePaged,
} from '../ui/kit'
import { BanForm } from './AdminContent'
import { Card, DownloadButton, errorMessage, PageHeader, run, Select, studentName, Toolbar } from './shared'

const STATUSES = [['InProgress', 'In progress'], ['Completed', 'Completed'], ['Quit', 'Left early'], ['Expired', 'Expired'], ['Terminated', 'Terminated']]

function CompetitionPicker({ value, onChange }) {
  const { data } = useFetch('/admin/competitions')
  return <Select label="Competition" all="Active" value={value} onChange={onChange} options={(data || []).map((c) => [c.id, c.competitionName])} />
}

// ============================================================================= Attempts

export function AttemptsPage() {
  const [kind, setKind] = useState('game')
  const [status, setStatus] = useState('')
  const [flagged, setFlagged] = useState('')
  const [competitionId, setCompetitionId] = useState('')
  const list = usePaged(`/admin/${kind}-attempts`, { status, flagged, competitionId })
  return (
    <>
      <PageHeader title="Attempts" subtitle="Every game and quiz attempt, with the generated puzzle or question paper and what the student submitted." />
      <ul className="nav nav-tabs mb-3">
        {[['game', 'Number & word games'], ['quiz', 'Mega Quiz']].map(([k, label]) => (
          <li className="nav-item" key={k}><button className={`nav-link ${kind === k ? 'active fw-semibold' : ''}`} onClick={() => setKind(k)}>{label}</button></li>
        ))}
      </ul>
      <Card>
        <Toolbar>
          <SearchBox value={list.search} onChange={list.setSearch} placeholder="Student name, username or ID…" />
          <CompetitionPicker value={competitionId} onChange={setCompetitionId} />
          <Select label="Status" value={status} onChange={setStatus} options={STATUSES} />
          <Select label="Flag" value={flagged} onChange={setFlagged} options={[['true', 'Flagged only']]} />
        </Toolbar>
        <DataTable loading={list.loading} error={list.error} onRetry={list.reload} rows={list.items} empty="No attempts match."
          columns={[
            { key: 'student', label: 'Student', render: (r) => <Link to={`/admin/students/${r.studentId}`} className="text-decoration-none">{studentName(r)}</Link> },
            { key: 'name', label: kind === 'game' ? 'Game' : 'Quiz', render: (r) => <Link to={`/admin/attempts/${kind}/${r.id}`} className="fw-semibold text-decoration-none">{r.game || r.quiz}</Link> },
            { key: 'status', label: 'Status', render: (r) => <><Badge value={r.status} />{r.isFlagged && <i className="bi bi-flag-fill text-danger ms-1" title="Flagged for review" />}</> },
            { key: 'score', label: 'Score', render: (r) => `${r.score} / ${r.maxScore}` },
            { key: 'correct', label: 'Correct', render: (r) => (kind === 'game' ? `${r.correct} / ${r.total}` : `${r.correct} ✓ ${r.incorrect} ✗ ${r.unanswered} –`) },
            { key: 'timeTakenSeconds', label: 'Time', render: (r) => fmtDuration(r.timeTakenSeconds) },
            { key: 'startedAtUtc', label: 'Started', render: (r) => fmtDateTime(r.startedAtUtc) },
          ]} />
        <Pager {...list} onPage={list.setPage} />
      </Card>
    </>
  )
}

function ViolationList({ rows }) {
  const [open, setOpen] = useState(null)
  return (
    <>
      <DataTable rows={rows} empty="No violations during this attempt."
        columns={[
          { key: 'occurredAtUtc', label: 'When', render: (r) => fmtDateTime(r.occurredAtUtc) },
          { key: 'ruleCode', label: 'Rule' },
          { key: 'severity', label: 'Severity', render: (r) => <Badge value={r.severity} /> },
          { key: 'action', label: 'Action', render: (r) => <Badge value={r.action} /> },
          { key: 'reviewStatus', label: 'Review', render: (r) => <Badge value={r.reviewStatus} /> },
          { key: 'evidenceCount', label: 'Evidence', render: (r) => r.evidenceCount ?? r.evidence?.length ?? 0 },
          { key: 'open', label: '', render: (r) => <button className="btn btn-sm btn-outline-secondary" onClick={() => setOpen(r.id)}>Open</button> },
        ]} />
      {open && <ViolationModal id={open} onClose={() => setOpen(null)} />}
    </>
  )
}

export function AttemptDetailPage() {
  const { kind, id } = useParams()
  const { can } = useAdminAuth()
  const { data, loading, error, reload } = useFetch(`/admin/${kind}-attempts/${id}`)
  if (loading && !data) return <Spinner />
  if (error && !data) return <ErrorBox error={error} onRetry={reload} />
  const a = data

  const reset = async () => {
    const reason = await askReason('Reset this attempt?', 'The attempt and its result are deleted so the student can take it again. Give the reason (recorded in the audit log).')
    if (reason) run(() => api.post(`/admin/${kind}-attempts/${id}/reset`, { reason }), 'Attempt reset', () => window.history.back())
  }

  return (
    <>
      <PageHeader title={`${a.game || a.quiz}`} subtitle={<>Attempt by <Link to={`/admin/students/${a.studentId}`}>{studentName(a)}</Link> · started {fmtDateTime(a.startedAtUtc)}</>}>
        <Link to="/admin/attempts" className="btn btn-sm btn-outline-secondary"><i className="bi bi-arrow-left me-1" />Attempts</Link>
        {can('attempts.manage') && <button className="btn btn-sm btn-outline-secondary" onClick={() => run(() => api.post(`/admin/${kind}-attempts/${id}/flag?value=${!a.isFlagged}`), a.isFlagged ? 'Flag removed' : 'Flagged', reload)}>{a.isFlagged ? 'Remove flag' : 'Flag for review'}</button>}
        {can('attempts.manage') && <button className="btn btn-sm btn-outline-danger" onClick={reset}>Reset attempt</button>}
      </PageHeader>
      <div className="row g-3 mb-3">
        {[['Status', <Badge key="s" value={a.status} />], ['Score', `${a.score} / ${a.maxScore}`], ['Correct', kind === 'game' ? `${a.correct} / ${a.total}` : `${a.correct} right · ${a.incorrect} wrong · ${a.unanswered} blank`],
          ['Time', fmtDuration(a.timeTakenSeconds)], ['Finished', fmtDateTime(a.completedAtUtc || a.submittedAtUtc)], ['Flagged', a.isFlagged ? 'Yes' : 'No']].map(([k, v]) => (
          <div className="col-6 col-md-4 col-xl-2" key={k}><Card><div className="small text-secondary">{k}</div><div className="fw-bold">{v}</div></Card></div>
        ))}
      </div>

      {kind === 'game' ? (
        <Card className="mb-3">
          <h2 className="h6 fw-bold">Rounds <span className="fw-normal text-secondary small">· {a.engine} · {a.difficulty}</span></h2>
          {a.rounds.map((r) => (
            <details key={r.roundNo} className="border rounded-3 p-2 mb-2">
              <summary className="d-flex flex-wrap gap-2 align-items-center">
                <strong>Round {r.roundNo}</strong><Badge value={r.status} />
                <span className="small text-secondary">{r.score} pts · {fmtDuration(r.timeTakenSeconds)} · {r.wrongSubmissions} wrong tries</span>
              </summary>
              <div className="row g-2 mt-1 small">
                {[['Generated puzzle', r.puzzle], ['Correct answer / validation data', r.solution], ['Last submission', r.submission]].map(([label, value]) => (
                  <div className="col-12 col-lg-4" key={label}><div className="fw-semibold">{label}</div>
                    <pre className="bg-light rounded p-2 mb-0" style={{ maxHeight: 220, overflow: 'auto', whiteSpace: 'pre-wrap' }}>{value ? JSON.stringify(value, null, 1) : '—'}</pre></div>
                ))}
              </div>
            </details>
          ))}
        </Card>
      ) : (
        <Card className="mb-3">
          <h2 className="h6 fw-bold">Question paper <span className="fw-normal text-secondary small">· quiz version {a.quizVersion} · as presented to this student</span></h2>
          {a.questions.map((q) => (
            <div key={q.orderNo} className="border rounded-3 p-2 mb-2 small">
              <div className="d-flex justify-content-between gap-2">
                <strong style={{ whiteSpace: 'pre-wrap' }}>{q.orderNo}. {q.text}</strong>
                <span className={`badge align-self-start ${q.isCorrect ? 'text-bg-success' : q.isCorrect === false ? 'text-bg-danger' : 'text-bg-secondary'}`}>{q.awardedMarks > 0 ? '+' : ''}{q.awardedMarks} / {q.marks}</span>
              </div>
              <ol type="A" className="mb-0">
                {q.options.map((o) => (
                  <li key={o.id} className={o.isCorrect ? 'text-success fw-semibold' : o.id === q.selectedOptionId ? 'text-danger' : ''}>
                    {o.text}{o.isCorrect && ' ✓'}{o.id === q.selectedOptionId && ' ← chosen'}
                  </li>
                ))}
              </ol>
            </div>
          ))}
        </Card>
      )}
      <Card><h2 className="h6 fw-bold">Violations during this attempt</h2><ViolationList rows={a.violations} /></Card>
    </>
  )
}

// ============================================================================= Results & reports

const REPORTS = [
  ['leaderboard', 'Leaderboard', 'Rank, scores per section, total time'],
  ['game-attempts', 'Game attempts', 'One row per number or word game attempt'],
  ['quiz-attempts', 'Quiz attempts', 'One row per Mega Quiz attempt'],
  ['participation', 'Participation', 'Every student and whether they took part'],
  ['violations', 'Violations', 'All recorded security events'],
]

export function ResultsPage() {
  const { can } = useAdminAuth()
  const [competitionId, setCompetitionId] = useState('')
  const board = usePaged('/admin/results/leaderboard', { competitionId }, 50)
  const summary = useFetch(`/admin/results/summary${competitionId ? `?competitionId=${competitionId}` : ''}`)
  const query = competitionId ? `?competitionId=${competitionId}` : ''
  return (
    <>
      <PageHeader title="Results & reports" subtitle="Results are calculated from finished attempts. Ties are broken by the shorter total time."><CompetitionPicker value={competitionId} onChange={setCompetitionId} /></PageHeader>
      <Card className="mb-3">
        <h2 className="h6 fw-bold">Participation and averages</h2>
        <DataTable loading={summary.loading} error={summary.error} rows={summary.data?.items} rowKey="name" empty="No attempts yet."
          columns={[
            { key: 'section', label: 'Section' }, { key: 'name', label: 'Activity', render: (r) => <span className="fw-semibold">{r.name}</span> },
            { key: 'started', label: 'Started' }, { key: 'finished', label: 'Finished' }, { key: 'inProgress', label: 'In progress' },
            { key: 'averageScore', label: 'Avg score', render: (r) => Number(r.averageScore).toFixed(2) },
            { key: 'averageSeconds', label: 'Avg time', render: (r) => fmtDuration(r.averageSeconds) },
            { key: 'flagged', label: 'Flagged' },
          ]} />
      </Card>
      <Card className="mb-3">
        <h2 className="h6 fw-bold">Leaderboard</h2>
        <Toolbar><SearchBox value={board.search} onChange={board.setSearch} placeholder="Student name or ID…" /></Toolbar>
        <DataTable loading={board.loading} error={board.error} onRetry={board.reload} rows={board.items} rowKey="studentId" empty="No finished attempts yet."
          columns={[
            { key: 'rank', label: 'Rank', render: (r) => <strong>#{r.rank}</strong> },
            { key: 'name', label: 'Student', render: (r) => <><Link to={`/admin/students/${r.studentId}`} className="text-decoration-none">{r.name}</Link>{r.flagged && <i className="bi bi-flag-fill text-danger ms-1" title="Has a flagged attempt" />}</> },
            { key: 'city', label: 'City' }, { key: 'quizScore', label: 'Quiz' }, { key: 'numberScore', label: 'Number' }, { key: 'wordScore', label: 'Word' },
            { key: 'totalScore', label: 'Total', render: (r) => <strong>{r.totalScore}</strong> },
            { key: 'timeTakenSeconds', label: 'Time', render: (r) => fmtDuration(r.timeTakenSeconds) },
          ]} />
        <Pager {...board} onPage={board.setPage} />
      </Card>
      {can('reports.export') && (
        <Card>
          <h2 className="h6 fw-bold">Download reports (CSV)</h2>
          <div className="row g-2">
            {REPORTS.map(([key, label, text]) => (
              <div className="col-12 col-md-6 col-xl-4" key={key}>
                <div className="border rounded-3 p-3 h-100 d-flex justify-content-between align-items-center gap-2">
                  <div><div className="fw-semibold">{label}</div><div className="small text-secondary">{text}</div></div>
                  <DownloadButton url={`/admin/reports/${key}${query}`} name={`${key}.csv`}>CSV</DownloadButton>
                </div>
              </div>
            ))}
          </div>
        </Card>
      )}
    </>
  )
}

// ============================================================================= Proctoring

export function LiveMonitorPage() {
  const { data, loading, error, reload } = useFetch('/admin/proctoring/live')
  useEffect(() => { const t = setInterval(reload, 10000); return () => clearInterval(t) }, [reload])
  return (
    <>
      <PageHeader title="Live monitoring" subtitle="Students with an open monitoring session in the last five minutes. Refreshes every 10 seconds.">
        <button className="btn btn-sm btn-outline-secondary" onClick={reload}><i className="bi bi-arrow-clockwise me-1" />Refresh</button>
      </PageHeader>
      <Card>
        <DataTable loading={loading} error={error} onRetry={reload} rows={data} rowKey="sessionId" empty="Nobody is being monitored right now."
          columns={[
            { key: 'student', label: 'Student', render: (r) => <Link to={`/admin/students/${r.studentId}`} className="text-decoration-none fw-semibold">{studentName(r)}</Link> },
            { key: 'activity', label: 'Doing', render: (r) => r.activity || <span className="text-secondary">Browsing</span> },
            {
              key: 'cameraStatus', label: 'Camera', render: (r) => (!r.cameraStatus ? <span className="text-secondary">No data</span>
                : r.cameraStatus === 'OK' ? <Badge value="Active" label="1 person" />
                  : <Badge value={r.cameraStatus === 'NO_FACE' ? 'Flagged' : 'Critical'} label={r.cameraStatus === 'NO_FACE' ? 'Nobody visible' : `${r.personCount} people`} />),
            },
            { key: 'violations', label: 'Violations', render: (r) => (r.violations ? <Link to={`/admin/violations?studentId=${r.studentId}`}>{r.violations}</Link> : 0) },
            { key: 'consent', label: 'Evidence consent', render: (r) => (r.evidenceConsent ? (r.videoConsent ? 'Photo + video' : 'Photo') : 'None') },
            { key: 'lastSeenAtUtc', label: 'Last seen', render: (r) => fmtDateTime(r.lastSeenAtUtc) },
          ]} />
      </Card>
    </>
  )
}

/** Loads an evidence file with the admin's token and shows it. Each view is audit-logged by the API. */
export function EvidenceView({ file }) {
  const [src, setSrc] = useState(null)
  const [error, setError] = useState('')
  const [show, setShow] = useState(false)
  useEffect(() => {
    if (!show) return undefined
    let url
    api.get(`/admin/proctoring/evidence/${file.id}/content`, { responseType: 'blob', timeout: 60000 })
      .then((r) => { url = URL.createObjectURL(r.data); setSrc(url) })
      .catch((e) => setError(e?.response?.status === 403 ? 'You do not have permission to view evidence.' : 'The file is not available.'))
    return () => url && URL.revokeObjectURL(url)
  }, [show, file.id])
  if (file.deletedAtUtc) return <div className="border rounded-3 p-3 small text-secondary">Deleted {fmtDateTime(file.deletedAtUtc)}{file.deletedBy && ` (${file.deletedBy})`}</div>
  return (
    <div className="border rounded-3 p-2">
      {!show ? (
        <button className="btn btn-outline-secondary w-100" onClick={() => setShow(true)}>
          <i className={`bi ${file.kind === 'Video' ? 'bi-play-btn' : 'bi-image'} me-1`} aria-hidden="true" />View {file.kind.toLowerCase()} · {fmtDateTime(file.capturedAtUtc)}
        </button>
      ) : error ? <div className="small text-danger">{error}</div>
        : !src ? <Spinner label="Loading evidence…" />
          : file.kind === 'Video'
            ? <video src={src} controls className="evidence-thumb" aria-label="Evidence clip" />
            : <img src={src} alt={`Evidence captured ${fmtDateTime(file.capturedAtUtc)}`} className="evidence-thumb" />}
      {file.sizeBytes && <div className="small text-secondary mt-1">{fmtBytes(file.sizeBytes)} · kept until {fmtDateTime(file.retainUntilUtc)}</div>}
    </div>
  )
}

function ViolationModal({ id, onClose, onChanged }) {
  const { can } = useAdminAuth()
  const { data, loading, error, reload } = useFetch(`/admin/proctoring/violations/${id}`)
  const [note, setNote] = useState('')
  const review = (status) => run(() => api.post(`/admin/proctoring/violations/${id}/review`, { status, note }), 'Review saved', () => { reload(); onChanged?.() })
  return (
    <Modal title="Violation" onClose={onClose} size="modal-lg">
      {loading && !data ? <Spinner /> : error && !data ? <ErrorBox error={error} /> : (
        <>
          <dl className="row small mb-2">
            <dt className="col-4 col-md-3 text-secondary">Student</dt><dd className="col-8 col-md-9"><Link to={`/admin/students/${data.studentId}`}>{studentName(data)}</Link></dd>
            <dt className="col-4 col-md-3 text-secondary">Rule</dt><dd className="col-8 col-md-9">{data.ruleCode} <Badge value={data.severity} /></dd>
            <dt className="col-4 col-md-3 text-secondary">When</dt><dd className="col-8 col-md-9">{fmtDateTime(data.occurredAtUtc)} · occurrence {data.occurrenceNo}</dd>
            <dt className="col-4 col-md-3 text-secondary">What happened</dt><dd className="col-8 col-md-9">{data.message}</dd>
            <dt className="col-4 col-md-3 text-secondary">System action</dt><dd className="col-8 col-md-9"><Badge value={data.action} /></dd>
            <dt className="col-4 col-md-3 text-secondary">Attempt</dt>
            <dd className="col-8 col-md-9">{data.gameAttemptId ? <Link to={`/admin/attempts/game/${data.gameAttemptId}`}>Game attempt</Link> : data.quizAttemptId ? <Link to={`/admin/attempts/quiz/${data.quizAttemptId}`}>Quiz attempt</Link> : 'None in progress'}</dd>
            <dt className="col-4 col-md-3 text-secondary">Review</dt><dd className="col-8 col-md-9"><Badge value={data.reviewStatus} />{data.reviewNote && <span className="ms-2">{data.reviewNote}</span>}</dd>
          </dl>
          <h3 className="h6 fw-bold">Evidence</h3>
          {!data.evidence.length && <p className="small text-secondary">No evidence was captured (not required by the rule, or the student did not consent).</p>}
          {can('evidence.view')
            ? <div className="row g-2 mb-3">{data.evidence.map((e) => <div className="col-12 col-md-6" key={e.id}><EvidenceView file={e} /></div>)}</div>
            : data.evidence.length > 0 && <p className="small text-secondary">{data.evidence.length} file(s). Your role cannot view evidence.</p>}
          {can('violations.review') && (
            <>
              <Field label="Review note"><textarea className="form-control" rows={2} value={note} onChange={(e) => setNote(e.target.value)} maxLength={2000} /></Field>
              <div className="d-flex flex-wrap gap-2">
                <button className="btn btn-outline-danger" onClick={() => review('Confirmed')}>Confirm violation</button>
                <button className="btn btn-outline-success" onClick={() => review('Dismissed')}>Dismiss (not a violation)</button>
                <button className="btn btn-outline-secondary" onClick={() => review('Open')}>Reopen</button>
              </div>
              <p className="form-text">Dismissed events no longer count towards automatic flags, terminations or suspensions.</p>
            </>
          )}
        </>
      )}
    </Modal>
  )
}

export function ViolationsPage() {
  const initialStudent = new URLSearchParams(window.location.search).get('studentId') || ''
  const [review, setReview] = useState(initialStudent ? '' : 'Open')
  const [severity, setSeverity] = useState('')
  const [ruleCode, setRuleCode] = useState('')
  const [studentId] = useState(initialStudent)
  const rules = useFetch('/admin/proctoring/rules')
  const list = usePaged('/admin/proctoring/violations', { review, severity, ruleCode, studentId })
  const [open, setOpen] = useState(null)
  return (
    <>
      <PageHeader title="Violations" subtitle="Security events reported during the competition. A detection is a prompt for review, not proof of cheating." />
      <Card>
        <Toolbar>
          <SearchBox value={list.search} onChange={list.setSearch} placeholder="Student name or username…" />
          <Select label="Review" value={review} onChange={setReview} options={['Open', 'Confirmed', 'Dismissed']} />
          <Select label="Severity" value={severity} onChange={setSeverity} options={['Minor', 'Serious', 'Critical']} />
          <Select label="Rule" value={ruleCode} onChange={setRuleCode} options={(rules.data || []).map((r) => [r.code, r.name])} />
          {studentId && <Link to="/admin/violations" reloadDocument className="btn btn-sm btn-outline-secondary">Student {studentId} ✕</Link>}
        </Toolbar>
        <DataTable loading={list.loading} error={list.error} onRetry={list.reload} rows={list.items} empty="Nothing to review." onRowClick={(r) => setOpen(r.id)}
          columns={[
            { key: 'occurredAtUtc', label: 'When', render: (r) => fmtDateTime(r.occurredAtUtc) },
            { key: 'student', label: 'Student', render: studentName },
            { key: 'ruleCode', label: 'Rule', render: (r) => <><div>{r.ruleCode}</div><div className="small text-secondary">occurrence {r.occurrenceNo}</div></> },
            { key: 'severity', label: 'Severity', render: (r) => <Badge value={r.severity} /> },
            { key: 'action', label: 'Action', render: (r) => <Badge value={r.action} /> },
            { key: 'evidenceCount', label: 'Evidence', render: (r) => (r.evidenceCount ? <><i className="bi bi-paperclip" /> {r.evidenceCount}</> : '—') },
            { key: 'reviewStatus', label: 'Review', render: (r) => <Badge value={r.reviewStatus} /> },
          ]} />
        <Pager {...list} onPage={list.setPage} />
      </Card>
      {open && <ViolationModal id={open} onClose={() => setOpen(null)} onChanged={list.reload} />}
    </>
  )
}

export function EvidencePage() {
  const { can } = useAdminAuth()
  const [kind, setKind] = useState('')
  const [includeDeleted, setIncludeDeleted] = useState(false)
  const list = usePaged('/admin/proctoring/evidence', { kind, includeDeleted: includeDeleted ? 'true' : '' }, 12)
  const remove = async (id) => (await confirm('Delete this evidence file?', 'The file is removed permanently. A record that it existed is kept.', 'Delete', true))
    && run(() => api.delete(`/admin/proctoring/evidence/${id}`), 'Deleted', list.reload)
  return (
    <>
      <PageHeader title="Evidence" subtitle="Photos and short clips captured, with consent, when a violation was detected. Viewing a file is recorded in the audit log." />
      <Card>
        <Toolbar>
          <SearchBox value={list.search} onChange={list.setSearch} placeholder="Student name or username…" />
          <Select label="Type" value={kind} onChange={setKind} options={['Image', 'Video']} />
          <Check label="Show deleted" checked={includeDeleted} onChange={setIncludeDeleted} />
        </Toolbar>
        {list.loading && !list.items.length ? <Spinner /> : list.error ? <ErrorBox error={list.error} onRetry={list.reload} /> : !list.items.length ? <Empty icon="bi-images">No evidence files.</Empty> : (
          <div className="row g-3">
            {list.items.map((e) => (
              <div className="col-12 col-md-6 col-xl-4" key={e.id}>
                <div className="border rounded-3 p-2 h-100">
                  <div className="d-flex justify-content-between small mb-2">
                    <Link to={`/admin/students/${e.studentId}`} className="fw-semibold text-decoration-none">{studentName(e)}</Link>
                    <span className="badge text-bg-light border">{e.ruleCode}</span>
                  </div>
                  <EvidenceView file={e} />
                  {can('evidence.delete') && !e.deletedAtUtc && <button className="btn btn-sm btn-link text-danger px-0" onClick={() => remove(e.id)}>Delete file</button>}
                </div>
              </div>
            ))}
          </div>
        )}
        <Pager {...list} onPage={list.setPage} />
      </Card>
    </>
  )
}

function RuleForm({ rule, onClose, onSaved }) {
  const [v, setV] = useState({ ...rule, description: rule.description || '', suspendMinutes: rule.suspendMinutes ?? '' })
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')
  const set = (k) => (e) => setV({ ...v, [k]: e?.target ? e.target.value : e })
  const save = async (e) => {
    e.preventDefault()
    setBusy(true); setError('')
    try {
      await api.put(`/admin/proctoring/rules/${rule.id}`, {
        ...v, flagAfter: Number(v.flagAfter), terminateAfter: Number(v.terminateAfter), suspendAfter: Number(v.suspendAfter),
        suspendMinutes: v.suspendMinutes === '' ? null : Number(v.suspendMinutes),
      })
      onSaved()
    } catch (err) { setError(errorMessage(err)) }
    setBusy(false)
  }
  const num = (key, label, hint) => <div className="col-6 col-md-3"><Field label={label} hint={hint}><input type="number" min={0} className="form-control" value={v[key]} onChange={set(key)} /></Field></div>
  return (
    <Modal title={`Rule: ${rule.code}`} onClose={onClose} size="modal-lg">
      <form onSubmit={save}>
        {error && <div className="alert alert-danger small py-2">{error}</div>}
        <div className="row g-2">
          <div className="col-12 col-md-8"><Field label="Name shown to students and reviewers"><input className="form-control" value={v.name} onChange={set('name')} required /></Field></div>
          <div className="col-12 col-md-4"><Field label="Severity"><select className="form-select" value={v.severity} onChange={set('severity')}>{['Minor', 'Serious', 'Critical'].map((s) => <option key={s}>{s}</option>)}</select></Field></div>
        </div>
        <p className="small text-secondary mb-2">Every occurrence is recorded and the student is warned. The numbers below say after how many occurrences (per student, per competition) each stronger step is taken. 0 = never.</p>
        <div className="row g-2">
          {num('flagAfter', 'Flag attempt after', 'Marks the attempt for review')}
          {num('terminateAfter', 'End attempt after', 'Closes the running game or quiz')}
          {num('suspendAfter', 'Suspend after', 'Signs the student out pending review')}
          <div className="col-6 col-md-3"><Field label="Suspension (minutes)" hint="Empty = until reviewed"><input type="number" min={1} className="form-control" value={v.suspendMinutes} onChange={set('suspendMinutes')} /></Field></div>
        </div>
        <Check label="Rule enabled" checked={v.isEnabled} onChange={set('isEnabled')} />
        <Check label="Capture a photo when this happens" checked={v.captureImage} onChange={set('captureImage')} hint="Only if the student agreed to evidence capture and the camera is on." />
        <Check label="Capture a short video clip as well" checked={v.captureVideo} onChange={set('captureVideo')} hint="Only if the student also agreed to video." />
        <div className="alert alert-light border small">Automatic action never goes beyond a suspension. A permanent ban can only be applied by an administrator.</div>
        <div className="text-end"><SubmitButton busy={busy}>Save</SubmitButton></div>
      </form>
    </Modal>
  )
}

export function RulesPage() {
  const { can } = useAdminAuth()
  const { data, loading, error, reload } = useFetch('/admin/proctoring/rules')
  const [editing, setEditing] = useState(null)
  const n = (v) => (v > 0 ? v : '—')
  return (
    <>
      <PageHeader title="Violation rules" subtitle="What counts as a violation and how the system responds as it repeats." />
      <Card>
        <DataTable loading={loading} error={error} onRetry={reload} rows={data}
          columns={[
            { key: 'name', label: 'Rule', render: (r) => <><div className="fw-semibold">{r.name}</div><div className="small text-secondary font-monospace">{r.code}</div></> },
            { key: 'severity', label: 'Severity', render: (r) => <Badge value={r.severity} /> },
            { key: 'flagAfter', label: 'Flag after', render: (r) => n(r.flagAfter) },
            { key: 'terminateAfter', label: 'End attempt after', render: (r) => n(r.terminateAfter) },
            { key: 'suspendAfter', label: 'Suspend after', render: (r) => (r.suspendAfter > 0 ? `${r.suspendAfter} (${r.suspendMinutes ? `${r.suspendMinutes} min` : 'until reviewed'})` : '—') },
            { key: 'capture', label: 'Evidence', render: (r) => [r.captureImage && 'Photo', r.captureVideo && 'Video'].filter(Boolean).join(' + ') || '—' },
            { key: 'isEnabled', label: 'State', render: (r) => <Badge value={r.isEnabled ? 'Active' : 'Inactive'} label={r.isEnabled ? 'On' : 'Off'} /> },
            { key: 'actions', label: '', render: (r) => can('violations.review') && <button className="btn btn-sm btn-outline-secondary" onClick={() => setEditing(r)}>Edit</button> },
          ]} />
      </Card>
      {editing && <RuleForm rule={editing} onClose={() => setEditing(null)} onSaved={() => { setEditing(null); reload() }} />}
    </>
  )
}

// ============================================================================= Bans & appeals

export function BansPage() {
  const { can } = useAdminAuth()
  const [state, setState] = useState('active')
  const list = usePaged('/admin/bans', { state })
  const [adding, setAdding] = useState(false)
  const lift = async (ban) => {
    const reason = await askReason(`Restore access for ${studentName(ban)}?`, 'Reason (recorded in the audit log)')
    if (reason) run(() => api.post(`/admin/bans/${ban.id}/lift`, { reason }), 'Access restored', list.reload)
  }
  return (
    <>
      <PageHeader title="Banned students" subtitle="Suspensions end on their date; bans stay until an administrator lifts them. Students cannot remove a restriction themselves.">
        {can('bans.manage') && <button className="btn btn-sm btn-danger" onClick={() => setAdding(true)}><i className="bi bi-slash-circle me-1" />Restrict a student</button>}
      </PageHeader>
      <Card>
        <Toolbar>
          <SearchBox value={list.search} onChange={list.setSearch} placeholder="Name, username, email or ID…" />
          <Select label="Show" all={null} value={state} onChange={setState} options={[['active', 'Active restrictions'], ['lifted', 'Lifted'], ['expired', 'Expired'], ['all', 'All']]} />
        </Toolbar>
        <DataTable loading={list.loading} error={list.error} onRetry={list.reload} rows={list.items} empty="No restrictions."
          columns={[
            { key: 'student', label: 'Student', render: (r) => <Link to={`/admin/students/${r.studentId}`} className="text-decoration-none fw-semibold">{studentName(r)}</Link> },
            { key: 'type', label: 'Type', render: (r) => <><Badge value={r.type} /> <span className="small text-secondary">{r.source === 'Automatic' ? 'automatic' : 'by admin'}</span></> },
            { key: 'reason', label: 'Reason' },
            { key: 'startsAtUtc', label: 'Since', render: (r) => fmtDateTime(r.startsAtUtc) },
            { key: 'endsAtUtc', label: 'Until', render: (r) => (r.liftedAtUtc ? <span title={r.liftReason}>Lifted {fmtDateTime(r.liftedAtUtc)}</span> : r.endsAtUtc ? fmtDateTime(r.endsAtUtc) : 'Until lifted') },
            { key: 'pendingAppeal', label: 'Appeal', render: (r) => (r.pendingAppeal ? <Link to="/admin/appeals"><Badge value="Pending" /></Link> : '—') },
            { key: 'actions', label: '', render: (r) => can('bans.manage') && r.isActive && <button className="btn btn-sm btn-outline-success" onClick={() => lift(r)}>Restore access</button> },
          ]} />
        <Pager {...list} onPage={list.setPage} />
      </Card>
      {adding && <BanForm onClose={() => setAdding(false)} onSaved={() => { setAdding(false); list.reload() }} />}
    </>
  )
}

export function AppealsPage() {
  const [status, setStatus] = useState('Pending')
  const list = usePaged('/admin/appeals', { status })
  return (
    <>
      <PageHeader title="Appeals" subtitle="Review requests from restricted students." />
      <Card>
        <Toolbar>
          <SearchBox value={list.search} onChange={list.setSearch} placeholder="Name, email, username or student ID…" />
          <Select label="Status" value={status} onChange={setStatus} options={['Pending', 'Approved', 'Rejected']} />
        </Toolbar>
        <DataTable loading={list.loading} error={list.error} onRetry={list.reload} rows={list.items} empty="No appeals."
          columns={[
            { key: 'student', label: 'Student', render: (r) => <><Link to={`/admin/appeals/${r.id}`} className="fw-semibold text-decoration-none">{studentName(r)}</Link><div className="small text-secondary">ID {r.studentId}{r.email && ` · ${r.email}`}</div></> },
            { key: 'banType', label: 'Restriction', render: (r) => <Badge value={r.banType} /> },
            { key: 'banReason', label: 'Reason for restriction' },
            { key: 'submittedAtUtc', label: 'Submitted', render: (r) => fmtDateTime(r.submittedAtUtc) },
            { key: 'status', label: 'Status', render: (r) => <Badge value={r.status} /> },
            { key: 'open', label: '', render: (r) => <Link to={`/admin/appeals/${r.id}`} className="btn btn-sm btn-outline-secondary">Review</Link> },
          ]} />
        <Pager {...list} onPage={list.setPage} />
      </Card>
    </>
  )
}

export function AppealDetailPage() {
  const { id } = useParams()
  const { can } = useAdminAuth()
  const { data, loading, error, reload } = useFetch(`/admin/appeals/${id}`)
  const [note, setNote] = useState('')
  const [decision, setDecision] = useState(null) // 'approve' | 'reject' | 'suspend'
  const [reason, setReason] = useState('')
  const [until, setUntil] = useState(localInputInHours(24))
  const [busy, setBusy] = useState(false)
  const [formError, setFormError] = useState('')
  if (loading && !data) return <Spinner />
  if (error && !data) return <ErrorBox error={error} onRetry={reload} />
  const a = data
  const review = can('appeals.review')

  const decide = async (e) => {
    e.preventDefault()
    setBusy(true); setFormError('')
    try {
      await api.post(`/admin/appeals/${id}/decision`, { decision, reason, suspendUntilUtc: decision === 'suspend' ? fromLocalInput(until) : null })
      setDecision(null); setReason('')
      reload()
    } catch (err) { setFormError(errorMessage(err)) }
    setBusy(false)
  }
  const LABEL = { approve: 'Approve – restore access', reject: 'Reject – keep the restriction', suspend: 'Change to a temporary suspension' }

  return (
    <>
      <PageHeader title={`Appeal from ${a.student.name || a.student.username}`} subtitle={`Student ID ${a.student.userId} · submitted ${fmtDateTime(a.submittedAtUtc)}`}>
        <Link to="/admin/appeals" className="btn btn-sm btn-outline-secondary"><i className="bi bi-arrow-left me-1" />Appeals</Link>
        <Link to={`/admin/students/${a.student.userId}`} className="btn btn-sm btn-outline-secondary">Student record</Link>
      </PageHeader>
      <div className="row g-3">
        <div className="col-12 col-xl-5">
          <Card className="mb-3">
            <div className="d-flex justify-content-between"><h2 className="h6 fw-bold">Student's request</h2><Badge value={a.status} /></div>
            <p style={{ whiteSpace: 'pre-wrap' }}>{a.message}</p>
            {a.decisionReason && <div className="alert alert-light border small mb-0"><strong>Decision by {a.decidedBy} on {fmtDateTime(a.decidedAtUtc)}:</strong> {a.decisionReason}</div>}
          </Card>
          <Card className="mb-3">
            <h2 className="h6 fw-bold">Restriction under appeal</h2>
            <dl className="small mb-0">
              {[['Type', <Badge key="t" value={a.ban.type} />], ['Applied', `${fmtDateTime(a.ban.startsAtUtc)} (${a.ban.source === 'Automatic' ? 'automatically' : 'by an administrator'})`],
                ['Reason', a.ban.reason], ['Rule', a.ban.ruleCode], ['Ends', a.ban.liftedAtUtc ? `Lifted ${fmtDateTime(a.ban.liftedAtUtc)}: ${a.ban.liftReason}` : a.ban.endsAtUtc ? fmtDateTime(a.ban.endsAtUtc) : 'When lifted']]
                .filter(([, v]) => v).map(([k, v]) => <div key={k} className="d-flex justify-content-between gap-3 py-1 border-bottom"><dt className="fw-normal text-secondary">{k}</dt><dd className="mb-0 text-end">{v}</dd></div>)}
            </dl>
          </Card>
          <Card className="mb-3">
            <h2 className="h6 fw-bold">Internal review notes</h2>
            <p className="small text-secondary">Visible to reviewers only, never to the student.</p>
            {!a.notes.length && <p className="small text-secondary">No notes yet.</p>}
            {a.notes.map((n) => <div key={n.id} className="border-start border-3 ps-2 mb-2 small"><div className="text-secondary">{n.admin} · {fmtDateTime(n.createdAtUtc)}</div><div style={{ whiteSpace: 'pre-wrap' }}>{n.note}</div></div>)}
            {review && (
              <form className="mt-2" onSubmit={(e) => { e.preventDefault(); run(() => api.post(`/admin/appeals/${id}/notes`, { note }), 'Note added', () => { setNote(''); reload() }) }}>
                <textarea className="form-control mb-2" rows={2} value={note} onChange={(e) => setNote(e.target.value)} placeholder="Add a note for other reviewers…" aria-label="Internal note" required minLength={2} />
                <button className="btn btn-sm btn-outline-secondary">Add note</button>
              </form>
            )}
          </Card>
          {review && a.status === 'Pending' && (
            <Card>
              <h2 className="h6 fw-bold">Decision</h2>
              {!decision ? (
                <div className="d-grid gap-2">
                  <button className="btn btn-success" onClick={() => setDecision('approve')}>{LABEL.approve}</button>
                  <button className="btn btn-outline-warning" onClick={() => setDecision('suspend')}>{LABEL.suspend}</button>
                  <button className="btn btn-outline-danger" onClick={() => setDecision('reject')}>{LABEL.reject}</button>
                </div>
              ) : (
                <form onSubmit={decide}>
                  <div className="fw-semibold mb-2">{LABEL[decision]}</div>
                  {formError && <div className="alert alert-danger small py-2">{formError}</div>}
                  {decision === 'suspend' && <Field label="Suspended until"><input type="datetime-local" className="form-control" value={until} onChange={(e) => setUntil(e.target.value)} required /></Field>}
                  <Field label="Reason" hint="Recorded and shown to the student."><textarea className="form-control" rows={3} value={reason} onChange={(e) => setReason(e.target.value)} required minLength={5} /></Field>
                  <div className="d-flex gap-2"><SubmitButton busy={busy}>Confirm decision</SubmitButton><button type="button" className="btn btn-outline-secondary" onClick={() => setDecision(null)}>Back</button></div>
                </form>
              )}
            </Card>
          )}
        </div>
        <div className="col-12 col-xl-7">
          <Card className="mb-3">
            <h2 className="h6 fw-bold">Violation history and evidence</h2>
            <ViolationList rows={a.violations} />
          </Card>
          <Card className="mb-3">
            <h2 className="h6 fw-bold">Test and game attempts</h2>
            <DataTable rows={[...a.quizAttempts.map((x) => ({ ...x, kind: 'quiz' })), ...a.gameAttempts.map((x) => ({ ...x, kind: 'game' }))]} empty="No attempts."
              columns={[
                { key: 'name', label: 'Activity', render: (r) => <Link to={`/admin/attempts/${r.kind}/${r.id}`} className="text-decoration-none">{r.name}</Link> },
                { key: 'status', label: 'Status', render: (r) => <><Badge value={r.status} />{r.isFlagged && <i className="bi bi-flag-fill text-danger ms-1" />}</> },
                { key: 'score', label: 'Score', render: (r) => `${r.score} / ${r.maxScore}` },
                { key: 'startedAtUtc', label: 'Started', render: (r) => fmtDateTime(r.startedAtUtc) },
              ]} />
          </Card>
          {a.previousAppeals.length > 0 && (
            <Card>
              <h2 className="h6 fw-bold">Earlier appeals by this student</h2>
              <DataTable rows={a.previousAppeals}
                columns={[
                  { key: 'submittedAtUtc', label: 'Submitted', render: (r) => <Link to={`/admin/appeals/${r.id}`}>{fmtDateTime(r.submittedAtUtc)}</Link> },
                  { key: 'status', label: 'Status', render: (r) => <Badge value={r.status} /> },
                  { key: 'decisionReason', label: 'Decision reason' },
                ]} />
            </Card>
          )}
        </div>
      </div>
    </>
  )
}
