import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import api from '../api/client'
import { useAdminAuth } from '../auth/AdminAuth'
import {
  askReason, Badge, Check, confirm, DataTable, Empty, ErrorBox, Field, fmtDateTime, fmtDuration, fromLocalInput, Modal, Pager, SearchBox,
  Spinner, SubmitButton, toLocalInput, localInputInHours, useFetch, usePaged,
} from '../ui/kit'
import { Card, DownloadButton, errorMessage, ImportButton, PageHeader, run, Select, Toolbar } from './shared'

const DIFFICULTIES = ['Easy', 'Medium', 'Hard']

/** Small helper for modal forms: holds values, a busy flag and the last error. */
function useForm(initial) {
  const [values, setValues] = useState(initial)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')
  const set = (key) => (e) => setValues((v) => ({ ...v, [key]: e?.target ? e.target.value : e }))
  const submit = (action, onDone) => async (e) => {
    e.preventDefault()
    setBusy(true); setError('')
    try { onDone?.(await action(values)) } catch (err) { setError(errorMessage(err)) }
    setBusy(false)
  }
  return { values, setValues, set, busy, error, submit }
}

const FormError = ({ error }) => (error ? <div className="alert alert-danger small py-2" role="alert">{error}</div> : null)

// ============================================================================= Students

function StudentForm({ student, onClose, onSaved }) {
  const f = useForm(student
    ? { username: student.username, firstName: student.firstName || '', lastName: student.lastName || '', email: student.email || '', phoneNumber: student.phoneNumber || '', city: student.city || '', isActive: student.isActive }
    : { username: '', password: '', firstName: '', lastName: '', email: '', phoneNumber: '', city: '', isActive: true })
  const save = (v) => {
    // Optional fields are sent as null, not as empty text.
    const body = Object.fromEntries(Object.entries(v).map(([k, val]) => [k, val === '' ? null : val]))
    return student ? api.put(`/admin/students/${student.userId}`, body) : api.post('/admin/students', body)
  }
  return (
    <Modal title={student ? 'Edit student' : 'Add student'} onClose={onClose}>
      <form onSubmit={f.submit(save, onSaved)}>
        <FormError error={f.error} />
        <Field label="Username (usually the mobile number)"><input className="form-control" value={f.values.username} onChange={f.set('username')} required minLength={3} /></Field>
        {!student && <Field label="Password" hint="At least 8 characters."><input className="form-control" value={f.values.password} onChange={f.set('password')} required minLength={8} autoComplete="new-password" /></Field>}
        <div className="row g-2">
          <div className="col-6"><Field label="First name"><input className="form-control" value={f.values.firstName} onChange={f.set('firstName')} /></Field></div>
          <div className="col-6"><Field label="Last name"><input className="form-control" value={f.values.lastName} onChange={f.set('lastName')} /></Field></div>
          <div className="col-12 col-sm-6"><Field label="Email"><input type="email" className="form-control" value={f.values.email} onChange={f.set('email')} /></Field></div>
          <div className="col-6 col-sm-3"><Field label="Phone"><input className="form-control" value={f.values.phoneNumber} onChange={f.set('phoneNumber')} /></Field></div>
          <div className="col-6 col-sm-3"><Field label="City"><input className="form-control" value={f.values.city} onChange={f.set('city')} /></Field></div>
        </div>
        <Check label="Account active" checked={f.values.isActive} onChange={f.set('isActive')} hint="Inactive students cannot sign in." />
        <div className="text-end"><SubmitButton busy={f.busy}>Save</SubmitButton></div>
      </form>
    </Modal>
  )
}

export function StudentsPage() {
  const { can } = useAdminAuth()
  const [status, setStatus] = useState('')
  const list = usePaged('/admin/students', { status })
  const [editing, setEditing] = useState(null)
  return (
    <>
      <PageHeader title="Students" subtitle="Accounts that may sign in to the competition.">
        {can('students.manage') && <ImportButton url="/admin/students/import" onDone={list.reload} help="Columns: username, password, first_name, last_name, email, phone, city" />}
        {can('students.manage') && <button className="btn btn-sm btn-success" onClick={() => setEditing({})}><i className="bi bi-plus-lg me-1" />Add student</button>}
      </PageHeader>
      <Card>
        <Toolbar>
          <SearchBox value={list.search} onChange={list.setSearch} placeholder="Name, username, email, ID…" />
          <Select label="Status" value={status} onChange={setStatus} options={[['active', 'Active'], ['inactive', 'Inactive'], ['restricted', 'Restricted'], ['online', 'Online now']]} />
        </Toolbar>
        <DataTable loading={list.loading} error={list.error} onRetry={list.reload} rows={list.items} rowKey="userId" empty="No students match."
          columns={[
            { key: 'userId', label: 'ID' },
            { key: 'name', label: 'Student', render: (r) => <Link to={`/admin/students/${r.userId}`} className="fw-semibold text-decoration-none">{[r.firstName, r.lastName].filter(Boolean).join(' ') || r.username}</Link> },
            { key: 'username', label: 'Username' },
            { key: 'city', label: 'City' },
            { key: 'state', label: 'Status', render: (r) => <>{r.restricted ? <Badge value="Restricted" /> : <Badge value={r.isActive ? 'Active' : 'Inactive'} />}{r.online && <span className="ms-1"><Badge value="Online" /></span>}</> },
            { key: 'lastLoginAtUtc', label: 'Last sign-in', render: (r) => fmtDateTime(r.lastLoginAtUtc) },
          ]} />
        <Pager {...list} onPage={list.setPage} />
      </Card>
      {editing && <StudentForm student={editing.userId ? editing : null} onClose={() => setEditing(null)} onSaved={() => { setEditing(null); list.reload() }} />}
    </>
  )
}

export function StudentDetailPage() {
  const { userId } = useParams()
  const { can } = useAdminAuth()
  const { data, loading, error, reload } = useFetch(`/admin/students/${userId}`)
  const [editing, setEditing] = useState(false)
  const [banning, setBanning] = useState(false)
  if (loading && !data) return <Spinner />
  if (error && !data) return <ErrorBox error={error} onRetry={reload} />
  const p = data.profile

  const resetPassword = async () => {
    const password = await askReason(`New password for ${p.username}`, 'Password (at least 8 characters). The student will be signed out.', 8)
    if (password) run(() => api.post(`/admin/students/${userId}/reset-password`, { newPassword: password }), 'Password reset', reload)
  }

  return (
    <>
      <PageHeader title={[p.firstName, p.lastName].filter(Boolean).join(' ') || p.username} subtitle={`Student ID ${p.userId} · ${p.username}`}>
        <Link to="/admin/students" className="btn btn-sm btn-outline-secondary"><i className="bi bi-arrow-left me-1" />All students</Link>
        {can('students.manage') && <button className="btn btn-sm btn-outline-secondary" onClick={() => setEditing(true)}>Edit</button>}
        {can('students.manage') && <button className="btn btn-sm btn-outline-secondary" onClick={resetPassword}>Reset password</button>}
        {can('sessions.manage') && <button className="btn btn-sm btn-outline-secondary" onClick={() => run(() => api.post(`/admin/students/${userId}/terminate-session`), 'Session ended')}>End session</button>}
        {can('bans.manage') && !data.activeBan && <button className="btn btn-sm btn-outline-danger" onClick={() => setBanning(true)}>Ban / suspend</button>}
      </PageHeader>

      {data.activeBan && (
        <div className="alert alert-danger d-flex flex-wrap justify-content-between gap-2">
          <span><Badge value={data.activeBan.type} /> {data.activeBan.reason} {data.activeBan.endsAtUtc && `· until ${fmtDateTime(data.activeBan.endsAtUtc)}`}</span>
          <Link to="/admin/bans" className="alert-link">Manage</Link>
        </div>
      )}

      <div className="row g-3">
        <div className="col-12 col-lg-4"><Card>
          <h2 className="h6 fw-bold">Profile</h2>
          <dl className="small mb-0">
            {[['Email', p.email], ['Phone', p.phoneNumber], ['City', p.city], ['Status', p.isActive ? 'Active' : 'Inactive'], ['Created', fmtDateTime(p.createdAtUtc)],
              ['Last sign-in', fmtDateTime(p.lastLoginAtUtc)], ['Locked until', p.lockedUntilUtc ? fmtDateTime(p.lockedUntilUtc) : null]].filter(([, v]) => v)
              .map(([k, v]) => <div key={k} className="d-flex justify-content-between py-1 border-bottom"><dt className="fw-normal text-secondary">{k}</dt><dd className="mb-0 text-end">{v}</dd></div>)}
          </dl>
        </Card></div>
        <div className="col-12 col-lg-8">
          <Card className="mb-3">
            <h2 className="h6 fw-bold">Attempts</h2>
            <DataTable rows={[...data.quizAttempts.map((a) => ({ ...a, kind: 'quiz', name: a.quiz })), ...data.gameAttempts.map((a) => ({ ...a, kind: 'game', name: a.game }))]}
              empty="No attempts yet."
              columns={[
                { key: 'name', label: 'Activity', render: (r) => <Link to={`/admin/attempts/${r.kind}/${r.id}`} className="text-decoration-none">{r.name}</Link> },
                { key: 'status', label: 'Status', render: (r) => <><Badge value={r.status} />{r.isFlagged && <i className="bi bi-flag-fill text-danger ms-1" title="Flagged" />}</> },
                { key: 'score', label: 'Score', render: (r) => `${r.score} / ${r.maxScore}` },
                { key: 'timeTakenSeconds', label: 'Time', render: (r) => fmtDuration(r.timeTakenSeconds) },
                { key: 'startedAtUtc', label: 'Started', render: (r) => fmtDateTime(r.startedAtUtc) },
              ]} />
          </Card>
          <Card className="mb-3">
            <h2 className="h6 fw-bold">Violations</h2>
            <DataTable rows={data.violations} empty="No violations recorded."
              columns={[
                { key: 'occurredAtUtc', label: 'When', render: (r) => fmtDateTime(r.occurredAtUtc) },
                { key: 'ruleCode', label: 'Rule' },
                { key: 'severity', label: 'Severity', render: (r) => <Badge value={r.severity} /> },
                { key: 'action', label: 'Action', render: (r) => <Badge value={r.action} /> },
                { key: 'reviewStatus', label: 'Review', render: (r) => <Badge value={r.reviewStatus} /> },
                { key: 'evidenceCount', label: 'Evidence' },
              ]} />
          </Card>
          <Card>
            <h2 className="h6 fw-bold">Restriction history</h2>
            <DataTable rows={data.bans} empty="Never restricted."
              columns={[
                { key: 'type', label: 'Type', render: (r) => <Badge value={r.type} /> },
                { key: 'reason', label: 'Reason' },
                { key: 'startsAtUtc', label: 'From', render: (r) => fmtDateTime(r.startsAtUtc) },
                { key: 'endsAtUtc', label: 'Until', render: (r) => (r.liftedAtUtc ? `Lifted ${fmtDateTime(r.liftedAtUtc)}` : r.endsAtUtc ? fmtDateTime(r.endsAtUtc) : 'Open') },
              ]} />
          </Card>
        </div>
      </div>
      {editing && <StudentForm student={p} onClose={() => setEditing(false)} onSaved={() => { setEditing(false); reload() }} />}
      {banning && <BanForm studentId={p.userId} onClose={() => setBanning(false)} onSaved={() => { setBanning(false); reload() }} />}
    </>
  )
}

export function BanForm({ studentId, onClose, onSaved }) {
  const f = useForm({ studentId: studentId || '', type: 'Suspension', reason: '', endsAt: localInputInHours(24) })
  const save = (v) => api.post('/admin/bans', { studentId: Number(v.studentId), type: v.type, reason: v.reason, endsAtUtc: v.type === 'Suspension' ? fromLocalInput(v.endsAt) : null })
  return (
    <Modal title="Restrict a student" onClose={onClose}>
      <form onSubmit={f.submit(save, onSaved)}>
        <FormError error={f.error} />
        {!studentId && <Field label="Student ID"><input type="number" className="form-control" value={f.values.studentId} onChange={f.set('studentId')} required /></Field>}
        <Field label="Restriction">
          <select className="form-select" value={f.values.type} onChange={f.set('type')}>
            <option value="Suspension">Temporary suspension</option><option value="Ban">Ban (until lifted by an administrator)</option>
          </select>
        </Field>
        {f.values.type === 'Suspension' && <Field label="Suspended until"><input type="datetime-local" className="form-control" value={f.values.endsAt} onChange={f.set('endsAt')} required /></Field>}
        <Field label="Reason" hint="Shown to the student."><textarea className="form-control" rows={3} value={f.values.reason} onChange={f.set('reason')} required minLength={5} /></Field>
        <p className="small text-secondary">The student is signed out immediately and can request a review.</p>
        <div className="text-end"><SubmitButton busy={f.busy} className="btn btn-danger">Apply restriction</SubmitButton></div>
      </form>
    </Modal>
  )
}

// ============================================================================= Competitions

function CompetitionForm({ item, onClose, onSaved }) {
  const f = useForm(item.id
    ? { ...item, start: toLocalInput(item.startDateTimeUtc), end: toLocalInput(item.endDateTimeUtc), description: item.description || '' }
    : { competitionName: '', description: '', start: localInputInHours(1), end: localInputInHours(3), showResultsToStudents: false, showLeaderboard: false, proctoringRequired: true })
  const save = (v) => {
    const body = { ...v, startDateTimeUtc: fromLocalInput(v.start), endDateTimeUtc: fromLocalInput(v.end) }
    return item.id ? api.put(`/admin/competitions/${item.id}`, body) : api.post('/admin/competitions', body)
  }
  return (
    <Modal title={item.id ? 'Edit competition' : 'New competition'} onClose={onClose}>
      <form onSubmit={f.submit(save, onSaved)}>
        <FormError error={f.error} />
        <Field label="Name"><input className="form-control" value={f.values.competitionName} onChange={f.set('competitionName')} required maxLength={200} /></Field>
        <Field label="Description"><textarea className="form-control" rows={2} value={f.values.description} onChange={f.set('description')} /></Field>
        <div className="row g-2">
          <div className="col-12 col-sm-6"><Field label="Starts (your local time)"><input type="datetime-local" className="form-control" value={f.values.start} onChange={f.set('start')} required /></Field></div>
          <div className="col-12 col-sm-6"><Field label="Ends"><input type="datetime-local" className="form-control" value={f.values.end} onChange={f.set('end')} required /></Field></div>
        </div>
        <Check label="Camera and browser monitoring required" checked={f.values.proctoringRequired} onChange={f.set('proctoringRequired')} />
        <Check label="Students can see their own scores" checked={f.values.showResultsToStudents} onChange={f.set('showResultsToStudents')} hint="Answer keys are released only after the competition ends." />
        <Check label="Students can see the leaderboard" checked={f.values.showLeaderboard} onChange={f.set('showLeaderboard')} />
        <div className="text-end"><SubmitButton busy={f.busy}>Save</SubmitButton></div>
      </form>
    </Modal>
  )
}

export function CompetitionsPage() {
  const { can } = useAdminAuth()
  const { data, loading, error, reload } = useFetch('/admin/competitions')
  const [editing, setEditing] = useState(null)
  const manage = can('competitions.manage')
  return (
    <>
      <PageHeader title="Competitions" subtitle="Only the active competition is shown to students. Games and quizzes can be played between its start and end.">
        {manage && <button className="btn btn-sm btn-success" onClick={() => setEditing({})}><i className="bi bi-plus-lg me-1" />New competition</button>}
      </PageHeader>
      <Card>
        <DataTable loading={loading} error={error} onRetry={reload} rows={data} empty="No competitions yet."
          columns={[
            { key: 'competitionName', label: 'Competition', render: (r) => <><div className="fw-semibold">{r.competitionName}</div>{r.isActive && <span className="badge text-bg-success">Active</span>}</> },
            { key: 'window', label: 'Window', render: (r) => <>{fmtDateTime(r.startDateTimeUtc)}<br />{fmtDateTime(r.endDateTimeUtc)}</> },
            { key: 'phase', label: 'Phase', render: (r) => <Badge value={r.phase} /> },
            { key: 'participants', label: 'Participants' },
            { key: 'options', label: 'Options', render: (r) => <span className="small">{r.proctoringRequired ? 'Monitored' : 'Unmonitored'} · scores {r.showResultsToStudents ? 'shown' : 'hidden'} · leaderboard {r.showLeaderboard ? 'shown' : 'hidden'}</span> },
            {
              key: 'actions', label: '', render: (r) => manage && (
                <div className="btn-group btn-group-sm">
                  <button className="btn btn-outline-secondary" onClick={() => setEditing(r)}>Edit</button>
                  {r.isActive
                    ? <button className="btn btn-outline-secondary" onClick={() => run(() => api.post(`/admin/competitions/${r.id}/deactivate`), 'Deactivated', reload)}>Deactivate</button>
                    : <button className="btn btn-outline-success" onClick={async () => (await confirm('Make this the active competition?', 'Any other active competition is deactivated.', 'Activate')) && run(() => api.post(`/admin/competitions/${r.id}/activate`), 'Activated', reload)}>Activate</button>}
                  {!r.participants && <button className="btn btn-outline-danger" onClick={async () => (await confirm('Delete this competition?', '', 'Delete', true)) && run(() => api.delete(`/admin/competitions/${r.id}`), 'Deleted', reload)}>Delete</button>}
                </div>
              ),
            },
          ]} />
      </Card>
      {editing && <CompetitionForm item={editing} onClose={() => setEditing(null)} onSaved={() => { setEditing(null); reload() }} />}
    </>
  )
}

// ============================================================================= Games

function GameForm({ game, onClose, onSaved }) {
  const f = useForm({ ...game, description: game.description || '', timeLimitMinutes: game.timeLimitSeconds ? Math.round(game.timeLimitSeconds / 60) : '', configText: JSON.stringify(game.config || {}, null, 2) })
  const [sample, setSample] = useState(null)
  const save = (v) => {
    let config
    try { config = JSON.parse(v.configText || '{}') } catch { throw Object.assign(new Error(), { response: { data: { message: 'Advanced options must be valid JSON.' } } }) }
    return api.put(`/admin/games/${game.id}`, {
      name: v.name, description: v.description, difficulty: v.difficulty, rounds: Number(v.rounds), pointsPerRound: Number(v.pointsPerRound),
      timeLimitSeconds: v.timeLimitMinutes ? Number(v.timeLimitMinutes) * 60 : null, isActive: v.isActive, config,
    })
  }
  const HINTS = {
    game24: '{"maxNumber": 10}', kenken: '{"size": 3} (3 to 5)', futoshiki: '{"size": 4} (4 or 5)',
    scramble: '{"rows": 4} (3 to 7 words per puzzle)', puzzle: '{"gridSize": 10, "words": 5}', word: 'No options',
  }
  return (
    <Modal title={`Configure ${game.name}`} onClose={onClose} size="modal-lg">
      <form onSubmit={f.submit(save, onSaved)}>
        <FormError error={f.error} />
        <div className="row g-2">
          <div className="col-12 col-sm-6"><Field label="Name"><input className="form-control" value={f.values.name} onChange={f.set('name')} required /></Field></div>
          <div className="col-6 col-sm-3"><Field label="Difficulty"><select className="form-select" value={f.values.difficulty} onChange={f.set('difficulty')}>{DIFFICULTIES.map((d) => <option key={d}>{d}</option>)}</select></Field></div>
          <div className="col-6 col-sm-3"><Field label="Rounds"><input type="number" min={1} max={50} className="form-control" value={f.values.rounds} onChange={f.set('rounds')} required /></Field></div>
          <div className="col-6 col-sm-3"><Field label="Points per round"><input type="number" min={0} step="0.5" className="form-control" value={f.values.pointsPerRound} onChange={f.set('pointsPerRound')} required /></Field></div>
          <div className="col-6 col-sm-3"><Field label="Time limit (min)" hint="Empty = no limit"><input type="number" min={1} className="form-control" value={f.values.timeLimitMinutes} onChange={f.set('timeLimitMinutes')} /></Field></div>
          <div className="col-12 col-sm-6"><Field label="Description"><input className="form-control" value={f.values.description} onChange={f.set('description')} /></Field></div>
        </div>
        <Field label="Advanced options (JSON)" hint={`For this game: ${HINTS[game.engineKey] || '{}'}`}>
          <textarea className="form-control font-monospace" rows={3} value={f.values.configText} onChange={f.set('configText')} spellCheck={false} />
        </Field>
        <Check label="Available to students" checked={f.values.isActive} onChange={f.set('isActive')} />
        <p className="small text-secondary">Changes apply to attempts started from now on. Attempts already started keep the settings they began with.</p>
        {sample && <pre className="bg-light rounded p-2 small" style={{ maxHeight: 220, overflow: 'auto' }}>{JSON.stringify(sample, null, 2)}</pre>}
        <div className="d-flex justify-content-between">
          <button type="button" className="btn btn-outline-secondary" onClick={() => run(() => api.post(`/admin/games/${game.id}/sample`), null, (r) => setSample(r.data))}>Generate a sample (saved settings)</button>
          <SubmitButton busy={f.busy}>Save</SubmitButton>
        </div>
      </form>
    </Modal>
  )
}

export function GamesPage() {
  const { can } = useAdminAuth()
  const { data, loading, error, reload } = useFetch('/admin/games')
  const [editing, setEditing] = useState(null)
  if (loading && !data) return <Spinner />
  if (error && !data) return <ErrorBox error={error} onRetry={reload} />
  return (
    <>
      <PageHeader title="Games" subtitle="Number and word games are generated for every attempt. Configure how many rounds, how hard, and how they score." />
      {data.map((c) => (
        <Card key={c.id} className="mb-3">
          <div className="d-flex justify-content-between align-items-center mb-2">
            <h2 className="h6 fw-bold m-0">{c.name}</h2>
            {can('games.manage') && <Check label="Section visible" checked={c.isActive} onChange={(v) => run(() => api.put(`/admin/games/categories/${c.id}/active?value=${v}`), 'Saved', reload)} />}
          </div>
          {c.key === 'quiz'
            ? <p className="small text-secondary mb-0">Quizzes are managed under <Link to="/admin/quizzes">Mega Quiz</Link>.</p>
            : (
              <DataTable rows={c.games}
                columns={[
                  { key: 'name', label: 'Game', render: (r) => <span className="fw-semibold">{r.name}</span> },
                  { key: 'difficulty', label: 'Difficulty' },
                  { key: 'rounds', label: 'Rounds' },
                  { key: 'pointsPerRound', label: 'Points / round' },
                  { key: 'timeLimitSeconds', label: 'Time limit', render: (r) => (r.timeLimitSeconds ? `${Math.round(r.timeLimitSeconds / 60)} min` : 'None') },
                  { key: 'attempts', label: 'Attempts' },
                  { key: 'isActive', label: 'Status', render: (r) => <Badge value={r.isActive ? 'Active' : 'Inactive'} /> },
                  { key: 'actions', label: '', render: (r) => can('games.manage') && <button className="btn btn-sm btn-outline-secondary" onClick={() => setEditing(r)}>Configure</button> },
                ]} />
            )}
        </Card>
      ))}
      {editing && <GameForm game={editing} onClose={() => setEditing(null)} onSaved={() => { setEditing(null); reload() }} />}
    </>
  )
}

// ============================================================================= Word bank

function WordForm({ word, onClose, onSaved }) {
  const f = useForm({ word: word.word || '', clue: word.clue || '', topic: word.topic || '', difficulty: word.difficulty || 'Medium', isActive: word.isActive ?? true })
  const save = (v) => (word.id ? api.put(`/admin/games/words/${word.id}`, v) : api.post('/admin/games/words', v))
  return (
    <Modal title={word.id ? 'Edit word' : 'Add word'} onClose={onClose}>
      <form onSubmit={f.submit(save, onSaved)}>
        <FormError error={f.error} />
        <Field label="Word" hint="3–20 letters, no spaces."><input className="form-control text-uppercase" value={f.values.word} onChange={f.set('word')} required pattern="[A-Za-z]{3,20}" /></Field>
        <Field label="Clue" hint="Needed for Word Challenge. Words without a clue are still used by Scramble and Puzzle."><input className="form-control" value={f.values.clue} onChange={f.set('clue')} maxLength={500} /></Field>
        <div className="row g-2">
          <div className="col-6"><Field label="Topic"><input className="form-control" value={f.values.topic} onChange={f.set('topic')} /></Field></div>
          <div className="col-6"><Field label="Difficulty"><select className="form-select" value={f.values.difficulty} onChange={f.set('difficulty')}>{DIFFICULTIES.map((d) => <option key={d}>{d}</option>)}</select></Field></div>
        </div>
        <Check label="Active" checked={f.values.isActive} onChange={f.set('isActive')} />
        <div className="text-end"><SubmitButton busy={f.busy}>Save</SubmitButton></div>
      </form>
    </Modal>
  )
}

export function WordBankPage() {
  const { can } = useAdminAuth()
  const list = usePaged('/admin/games/words', {}, 50)
  const [editing, setEditing] = useState(null)
  const manage = can('games.manage')
  return (
    <>
      <PageHeader title="Word bank" subtitle="The vocabulary Scramble, Puzzle and Word Challenge are generated from.">
        <DownloadButton url="/admin/games/words/export" name="word-bank.csv">Export</DownloadButton>
        {manage && <ImportButton url="/admin/games/words/import" onDone={list.reload} help="Columns: word, clue, topic, difficulty" />}
        {manage && <button className="btn btn-sm btn-success" onClick={() => setEditing({})}><i className="bi bi-plus-lg me-1" />Add word</button>}
      </PageHeader>
      <Card>
        <Toolbar><SearchBox value={list.search} onChange={list.setSearch} placeholder="Word, clue or topic…" /></Toolbar>
        <DataTable loading={list.loading} error={list.error} onRetry={list.reload} rows={list.items} empty="No words."
          columns={[
            { key: 'word', label: 'Word', render: (r) => <span className="fw-semibold font-monospace">{r.word}</span> },
            { key: 'clue', label: 'Clue' }, { key: 'topic', label: 'Topic' }, { key: 'difficulty', label: 'Difficulty' },
            { key: 'isActive', label: 'Status', render: (r) => <Badge value={r.isActive ? 'Active' : 'Inactive'} /> },
            {
              key: 'actions', label: '', render: (r) => manage && (
                <div className="btn-group btn-group-sm">
                  <button className="btn btn-outline-secondary" onClick={() => setEditing(r)}>Edit</button>
                  <button className="btn btn-outline-danger" onClick={async () => (await confirm(`Delete ${r.word}?`, '', 'Delete', true)) && run(() => api.delete(`/admin/games/words/${r.id}`), 'Deleted', list.reload)}>Delete</button>
                </div>
              ),
            },
          ]} />
        <Pager {...list} onPage={list.setPage} />
      </Card>
      {editing && <WordForm word={editing} onClose={() => setEditing(null)} onSaved={() => { setEditing(null); list.reload() }} />}
    </>
  )
}

// ============================================================================= Question bank

function AuthImage({ url, alt, className }) {
  const [src, setSrc] = useState(null)
  useEffect(() => {
    let objectUrl
    api.get(url, { responseType: 'blob' }).then((r) => { objectUrl = URL.createObjectURL(r.data); setSrc(objectUrl) }).catch(() => {})
    return () => objectUrl && URL.revokeObjectURL(objectUrl)
  }, [url])
  return src ? <img src={src} alt={alt} className={className} /> : null
}

function QuestionForm({ question, categories, onClose, onSaved }) {
  const blank = [{ text: '', isCorrect: true }, { text: '', isCorrect: false }, { text: '', isCorrect: false }, { text: '', isCorrect: false }]
  const f = useForm(question.id
    ? { categoryId: question.categoryId, text: question.text, explanation: question.explanation || '', difficulty: question.difficulty, marks: question.marks, isActive: question.isActive, options: question.options.map((o) => ({ text: o.text, isCorrect: o.isCorrect })) }
    : { categoryId: question.categoryId || categories[0]?.id || '', text: '', explanation: '', difficulty: 'Medium', marks: 1, isActive: true, options: blank })
  const [image, setImage] = useState(null)
  const [hasImage, setHasImage] = useState(!!question.hasImage)
  const setOption = (i, patch) => f.setValues((v) => ({ ...v, options: v.options.map((o, j) => (j === i ? { ...o, ...patch } : patch.isCorrect ? { ...o, isCorrect: false } : o)) }))
  const save = async (v) => {
    const body = { ...v, categoryId: Number(v.categoryId), marks: Number(v.marks), options: v.options.filter((o) => o.text.trim()) }
    const { data } = question.id ? await api.put(`/admin/quiz/questions/${question.id}`, body) : await api.post('/admin/quiz/questions', body)
    if (image) {
      const form = new FormData()
      form.append('file', image)
      await api.post(`/admin/quiz/questions/${data.id}/image`, form)
    }
  }
  return (
    <Modal title={question.id ? 'Edit question' : 'Add question'} onClose={onClose} size="modal-lg">
      <form onSubmit={f.submit(save, onSaved)}>
        <FormError error={f.error} />
        <div className="row g-2">
          <div className="col-12 col-sm-6"><Field label="Question bank"><select className="form-select" value={f.values.categoryId} onChange={f.set('categoryId')} required>{categories.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}</select></Field></div>
          <div className="col-6 col-sm-3"><Field label="Difficulty"><select className="form-select" value={f.values.difficulty} onChange={f.set('difficulty')}>{DIFFICULTIES.map((d) => <option key={d}>{d}</option>)}</select></Field></div>
          <div className="col-6 col-sm-3"><Field label="Marks"><input type="number" min={0.25} step="0.25" className="form-control" value={f.values.marks} onChange={f.set('marks')} required /></Field></div>
        </div>
        <Field label="Question"><textarea className="form-control" rows={3} value={f.values.text} onChange={f.set('text')} required maxLength={4000} /></Field>
        <fieldset className="mb-3">
          <legend className="form-label fw-semibold small">Options — select the correct answer</legend>
          {f.values.options.map((o, i) => (
            <div className="input-group mb-2" key={i}>
              <div className="input-group-text">
                <input type="radio" name="correct" className="form-check-input mt-0" checked={o.isCorrect} onChange={() => setOption(i, { isCorrect: true })} aria-label={`Option ${String.fromCharCode(65 + i)} is correct`} />
              </div>
              <span className="input-group-text">{String.fromCharCode(65 + i)}</span>
              <input className="form-control" value={o.text} onChange={(e) => setOption(i, { text: e.target.value })} maxLength={1000} aria-label={`Option ${String.fromCharCode(65 + i)}`} />
              {f.values.options.length > 2 && <button type="button" className="btn btn-outline-danger" aria-label="Remove option" onClick={() => f.setValues((v) => ({ ...v, options: v.options.filter((_, j) => j !== i) }))}><i className="bi bi-x-lg" /></button>}
            </div>
          ))}
          {f.values.options.length < 8 && <button type="button" className="btn btn-sm btn-outline-secondary" onClick={() => f.setValues((v) => ({ ...v, options: [...v.options, { text: '', isCorrect: false }] }))}>Add option</button>}
        </fieldset>
        <Field label="Explanation (shown to students only after the competition ends)"><textarea className="form-control" rows={2} value={f.values.explanation} onChange={f.set('explanation')} /></Field>
        <Field label="Image (optional, JPG / PNG / WebP up to 2 MB)">
          {hasImage && question.id && !image && (
            <div className="mb-2">
              <AuthImage url={`/admin/quiz/questions/${question.id}/image`} alt="Current question image" className="img-fluid rounded border" />
              <button type="button" className="btn btn-sm btn-link text-danger" onClick={() => run(() => api.delete(`/admin/quiz/questions/${question.id}/image`), 'Image removed', () => setHasImage(false))}>Remove image</button>
            </div>
          )}
          <input type="file" accept="image/jpeg,image/png,image/webp" className="form-control" onChange={(e) => setImage(e.target.files[0] || null)} />
        </Field>
        <Check label="Enabled (can be drawn into quizzes)" checked={f.values.isActive} onChange={f.set('isActive')} />
        <div className="text-end"><SubmitButton busy={f.busy}>Save</SubmitButton></div>
      </form>
    </Modal>
  )
}

function CategoriesModal({ onClose, onChanged }) {
  const { data, reload } = useFetch('/admin/quiz/categories')
  const [name, setName] = useState('')
  const changed = () => { reload(); onChanged() }
  return (
    <Modal title="Question banks" onClose={onClose}>
      <form className="input-group mb-3" onSubmit={(e) => { e.preventDefault(); run(() => api.post('/admin/quiz/categories', { name }), 'Created', () => { setName(''); changed() }) }}>
        <input className="form-control" placeholder="New question bank name" value={name} onChange={(e) => setName(e.target.value)} required maxLength={150} />
        <button className="btn btn-success">Add</button>
      </form>
      <ul className="list-group">
        {(data || []).map((c) => (
          <li key={c.id} className="list-group-item d-flex justify-content-between align-items-center gap-2">
            <span>{c.name} <span className="text-secondary small">· {c.questions} questions</span></span>
            <span className="btn-group btn-group-sm">
              <button className="btn btn-outline-secondary" onClick={async () => { const n = await askReason('Rename question bank', 'New name', 2); if (n) run(() => api.put(`/admin/quiz/categories/${c.id}`, { name: n, isActive: c.isActive }), 'Renamed', changed) }}>Rename</button>
              <button className="btn btn-outline-danger" disabled={c.questions > 0} title={c.questions > 0 ? 'Remove or move its questions first' : ''} onClick={() => run(() => api.delete(`/admin/quiz/categories/${c.id}`), 'Deleted', changed)}>Delete</button>
            </span>
          </li>
        ))}
      </ul>
    </Modal>
  )
}

export function QuestionsPage() {
  const { can } = useAdminAuth()
  const categories = useFetch('/admin/quiz/categories')
  const [categoryId, setCategoryId] = useState('')
  const [difficulty, setDifficulty] = useState('')
  const [active, setActive] = useState('')
  const list = usePaged('/admin/quiz/questions', { categoryId, difficulty, active })
  const [editing, setEditing] = useState(null)
  const [banks, setBanks] = useState(false)
  const manage = can('quiz.manage')
  const cats = categories.data || []
  return (
    <>
      <PageHeader title="Question bank" subtitle="Questions quizzes draw from. Answer keys are visible only here and never sent to students during a quiz.">
        <DownloadButton url="/admin/quiz/questions/template" name="question-import-template.csv">Template</DownloadButton>
        <DownloadButton url={`/admin/quiz/questions/export${categoryId ? `?categoryId=${categoryId}` : ''}`} name="questions.csv">Export</DownloadButton>
        {manage && <ImportButton url={`/admin/quiz/questions/import${categoryId ? `?categoryId=${categoryId}` : ''}`} onDone={() => { list.reload(); categories.reload() }}
          help="Columns: category, question, option_a…option_f, correct (letter, number or text), explanation, difficulty, marks" />}
        {manage && <button className="btn btn-sm btn-outline-secondary" onClick={() => setBanks(true)}>Question banks</button>}
        {manage && <button className="btn btn-sm btn-success" disabled={!cats.length} onClick={() => setEditing({ categoryId })}><i className="bi bi-plus-lg me-1" />Add question</button>}
      </PageHeader>
      <Card>
        <Toolbar>
          <SearchBox value={list.search} onChange={list.setSearch} placeholder="Search question text…" />
          <Select label="Bank" value={categoryId} onChange={setCategoryId} options={cats.map((c) => [c.id, `${c.name} (${c.questions})`])} />
          <Select label="Difficulty" value={difficulty} onChange={setDifficulty} options={DIFFICULTIES} />
          <Select label="State" value={active} onChange={setActive} options={[['true', 'Enabled'], ['false', 'Disabled']]} />
        </Toolbar>
        <DataTable loading={list.loading} error={list.error} onRetry={list.reload} rows={list.items} empty="No questions yet. Add one or import a file."
          columns={[
            {
              key: 'text', label: 'Question', render: (r) => (
                <div style={{ minWidth: 220 }}>
                  <div className="fw-semibold" style={{ whiteSpace: 'pre-wrap' }}>{r.text}{r.hasImage && <i className="bi bi-image ms-1 text-secondary" title="Has image" />}</div>
                  <ol type="A" className="small mb-0 ps-3 text-start">{r.options.map((o) => <li key={o.id} className={o.isCorrect ? 'text-success fw-semibold' : ''}>{o.text}{o.isCorrect && ' ✓'}</li>)}</ol>
                </div>
              ),
            },
            { key: 'category', label: 'Bank' }, { key: 'difficulty', label: 'Difficulty' }, { key: 'marks', label: 'Marks' },
            { key: 'isActive', label: 'State', render: (r) => <Badge value={r.isActive ? 'Active' : 'Inactive'} label={r.isActive ? 'Enabled' : 'Disabled'} /> },
            {
              key: 'actions', label: '', render: (r) => manage && (
                <div className="btn-group btn-group-sm">
                  <button className="btn btn-outline-secondary" onClick={() => setEditing(r)}>Edit</button>
                  <button className="btn btn-outline-secondary" onClick={() => run(() => api.put(`/admin/quiz/questions/${r.id}/active?value=${!r.isActive}`), null, list.reload)}>{r.isActive ? 'Disable' : 'Enable'}</button>
                  <button className="btn btn-outline-danger" onClick={async () => (await confirm('Delete this question?', 'Results of quizzes already taken are not affected.', 'Delete', true)) && run(() => api.delete(`/admin/quiz/questions/${r.id}`), 'Deleted', list.reload)}>Delete</button>
                </div>
              ),
            },
          ]} />
        <Pager {...list} onPage={list.setPage} />
      </Card>
      {editing && <QuestionForm question={editing} categories={cats} onClose={() => setEditing(null)} onSaved={() => { setEditing(null); list.reload(); categories.reload() }} />}
      {banks && <CategoriesModal onClose={() => setBanks(false)} onChanged={categories.reload} />}
    </>
  )
}

// ============================================================================= Quizzes

function QuizForm({ quiz, categories, onClose, onSaved }) {
  const f = useForm(quiz.id
    ? { ...quiz, description: quiz.description || '', categoryId: quiz.categoryId || '', difficulty: quiz.difficulty || '', marksPerQuestion: quiz.marksPerQuestion ?? '' }
    : { title: '', description: '', categoryId: categories[0]?.id || '', selectionMode: 'Random', difficulty: '', questionCount: 10, durationMinutes: 10, marksPerQuestion: '', negativeMarks: 0, shuffleQuestions: true, shuffleOptions: true, sortOrder: 0 })
  const save = (v) => {
    const body = {
      title: v.title, description: v.description, categoryId: v.categoryId ? Number(v.categoryId) : null, selectionMode: v.selectionMode,
      difficulty: v.difficulty || null, questionCount: Number(v.questionCount), durationMinutes: Number(v.durationMinutes),
      marksPerQuestion: v.marksPerQuestion === '' ? null : Number(v.marksPerQuestion), negativeMarks: Number(v.negativeMarks),
      shuffleQuestions: v.shuffleQuestions, shuffleOptions: v.shuffleOptions, sortOrder: Number(v.sortOrder),
    }
    return quiz.id ? api.put(`/admin/quiz/quizzes/${quiz.id}`, body) : api.post('/admin/quiz/quizzes', body)
  }
  const random = f.values.selectionMode === 'Random'
  return (
    <Modal title={quiz.id ? 'Edit quiz' : 'New quiz'} onClose={onClose} size="modal-lg">
      <form onSubmit={f.submit(save, onSaved)}>
        <FormError error={f.error} />
        <div className="row g-2">
          <div className="col-12 col-sm-8"><Field label="Title"><input className="form-control" value={f.values.title} onChange={f.set('title')} required maxLength={200} /></Field></div>
          <div className="col-12 col-sm-4"><Field label="Order on the student page"><input type="number" className="form-control" value={f.values.sortOrder} onChange={f.set('sortOrder')} /></Field></div>
        </div>
        <Field label="Description"><input className="form-control" value={f.values.description} onChange={f.set('description')} /></Field>
        <div className="row g-2">
          <div className="col-12 col-sm-6"><Field label="Question bank" hint="Used unless specific questions are attached to the quiz.">
            <select className="form-select" value={f.values.categoryId} onChange={f.set('categoryId')}><option value="">None</option>{categories.map((c) => <option key={c.id} value={c.id}>{c.name} ({c.activeQuestions})</option>)}</select>
          </Field></div>
          <div className="col-12 col-sm-6"><Field label="Question selection">
            <select className="form-select" value={f.values.selectionMode} onChange={f.set('selectionMode')}>
              <option value="Random">Random – each student gets a different set</option><option value="Fixed">Fixed – every student gets all the questions</option>
            </select>
          </Field></div>
          {random && <div className="col-6 col-sm-3"><Field label="Questions per student"><input type="number" min={1} max={500} className="form-control" value={f.values.questionCount} onChange={f.set('questionCount')} required /></Field></div>}
          {random && <div className="col-6 col-sm-3"><Field label="Only difficulty"><select className="form-select" value={f.values.difficulty} onChange={f.set('difficulty')}><option value="">Any</option>{DIFFICULTIES.map((d) => <option key={d}>{d}</option>)}</select></Field></div>}
          <div className="col-6 col-sm-3"><Field label="Duration (min)"><input type="number" min={1} max={600} className="form-control" value={f.values.durationMinutes} onChange={f.set('durationMinutes')} required /></Field></div>
          <div className="col-6 col-sm-3"><Field label="Marks per question" hint="Empty = each question's own marks"><input type="number" min={0.25} step="0.25" className="form-control" value={f.values.marksPerQuestion} onChange={f.set('marksPerQuestion')} /></Field></div>
          <div className="col-6 col-sm-3"><Field label="Negative marks" hint="Deducted per wrong answer"><input type="number" min={0} step="0.25" className="form-control" value={f.values.negativeMarks} onChange={f.set('negativeMarks')} /></Field></div>
        </div>
        <Check label="Shuffle the order of questions" checked={f.values.shuffleQuestions} onChange={f.set('shuffleQuestions')} />
        <Check label="Shuffle the order of options" checked={f.values.shuffleOptions} onChange={f.set('shuffleOptions')} />
        <div className="text-end"><SubmitButton busy={f.busy}>Save</SubmitButton></div>
      </form>
    </Modal>
  )
}

function QuizQuestionsModal({ quiz, onClose, onSaved }) {
  const detail = useFetch(`/admin/quiz/quizzes/${quiz.id}`)
  const [selected, setSelected] = useState(null) // [{id, text}]
  const [categoryId, setCategoryId] = useState(quiz.categoryId || '')
  const categories = useFetch('/admin/quiz/categories')
  const pool = usePaged('/admin/quiz/questions', { categoryId, active: 'true' }, 20)
  useEffect(() => { if (detail.data && !selected) setSelected(detail.data.questions.map((q) => ({ id: q.id, text: q.text }))) }, [detail.data, selected])
  if (!selected) return <Modal title="Quiz questions" onClose={onClose}><Spinner /></Modal>
  const ids = new Set(selected.map((s) => s.id))
  return (
    <Modal title={`Questions for ${quiz.title}`} onClose={onClose} size="modal-xl"
      footer={<><span className="me-auto small text-secondary">{selected.length ? `${selected.length} attached` : 'None attached – the whole question bank is used'}</span>
        <button className="btn btn-success" onClick={() => run(() => api.put(`/admin/quiz/quizzes/${quiz.id}/questions`, selected.map((s) => s.id)), 'Saved', onSaved)}>Save</button></>}>
      <div className="row g-3">
        <div className="col-12 col-lg-6">
          <h3 className="h6 fw-bold">Available</h3>
          <Toolbar>
            <SearchBox value={pool.search} onChange={pool.setSearch} />
            <Select label="Bank" value={categoryId} onChange={setCategoryId} options={(categories.data || []).map((c) => [c.id, c.name])} />
          </Toolbar>
          <ul className="list-group small">
            {pool.items.map((q) => (
              <li key={q.id} className="list-group-item d-flex justify-content-between gap-2 align-items-start">
                <span>{q.text} <span className="text-secondary">({q.difficulty})</span></span>
                <button className="btn btn-sm btn-outline-success" disabled={ids.has(q.id)} onClick={() => setSelected([...selected, { id: q.id, text: q.text }])}>{ids.has(q.id) ? 'Added' : 'Add'}</button>
              </li>
            ))}
          </ul>
          <Pager {...pool} onPage={pool.setPage} />
        </div>
        <div className="col-12 col-lg-6">
          <h3 className="h6 fw-bold">Attached to this quiz (in order)</h3>
          {!selected.length && <Empty>Nothing attached.</Empty>}
          <ol className="list-group list-group-numbered small">
            {selected.map((q, i) => (
              <li key={q.id} className="list-group-item d-flex justify-content-between gap-2 align-items-start">
                <span className="flex-grow-1">{q.text}</span>
                <span className="btn-group btn-group-sm">
                  <button className="btn btn-outline-secondary" disabled={i === 0} aria-label="Move up" onClick={() => setSelected(selected.map((x, j) => (j === i - 1 ? q : j === i ? selected[i - 1] : x)))}><i className="bi bi-arrow-up" /></button>
                  <button className="btn btn-outline-danger" aria-label="Remove" onClick={() => setSelected(selected.filter((x) => x.id !== q.id))}><i className="bi bi-x-lg" /></button>
                </span>
              </li>
            ))}
          </ol>
        </div>
      </div>
    </Modal>
  )
}

function QuizPreviewModal({ quiz, onClose }) {
  const { data, loading, error, reload } = useFetch(`/admin/quiz/quizzes/${quiz.id}/preview`)
  return (
    <Modal title={`Preview – ${quiz.title}`} onClose={onClose} size="modal-lg"
      footer={<button className="btn btn-outline-secondary" onClick={reload}><i className="bi bi-shuffle me-1" />Draw another sample</button>}>
      {loading ? <Spinner /> : error ? <ErrorBox error={error} /> : (
        <>
          <p className="small text-secondary">A sample paper drawn with the quiz's current rules · {data.questions.length} questions · {data.durationMinutes} min · {data.maxScore} marks{data.negativeMarks > 0 && ` · −${data.negativeMarks} per wrong answer`}. Correct answers are marked for your review only.</p>
          {!data.questions.length && <Empty>No usable questions. Add questions to the bank or attach some to the quiz.</Empty>}
          {data.questions.map((q) => (
            <div key={q.orderNo} className="border rounded-3 p-3 mb-2">
              <div className="fw-semibold" style={{ whiteSpace: 'pre-wrap' }}>{q.orderNo}. {q.text} <span className="text-secondary small fw-normal">({q.marks} marks · {q.difficulty})</span></div>
              {q.hasImage && <AuthImage url={`/admin/quiz/questions/${q.questionId}/image`} alt="" className="img-fluid rounded my-2" />}
              <ol type="A" className="small mb-0">{q.options.map((o) => <li key={o.id} className={o.isCorrect ? 'text-success fw-semibold' : ''}>{o.text}{o.isCorrect && ' ✓'}</li>)}</ol>
              {q.explanation && <div className="small bg-light rounded p-2 mt-2">{q.explanation}</div>}
            </div>
          ))}
        </>
      )}
    </Modal>
  )
}

export function QuizzesPage() {
  const { can } = useAdminAuth()
  const { data, loading, error, reload } = useFetch('/admin/quiz/quizzes')
  const categories = useFetch('/admin/quiz/categories')
  const [modal, setModal] = useState(null) // { type, quiz }
  const manage = can('quiz.manage')
  const publish = can('quiz.publish')
  const close = () => setModal(null)
  const done = () => { close(); reload() }
  return (
    <>
      <PageHeader title="Mega Quiz" subtitle="Students only see published quizzes. Editing a quiz or its questions never changes results that are already recorded.">
        <Link to="/admin/questions" className="btn btn-sm btn-outline-secondary">Question bank</Link>
        {manage && <button className="btn btn-sm btn-success" onClick={() => setModal({ type: 'form', quiz: {} })}><i className="bi bi-plus-lg me-1" />New quiz</button>}
      </PageHeader>
      <Card>
        <DataTable loading={loading} error={error} onRetry={reload} rows={data} empty="No quizzes yet."
          columns={[
            { key: 'title', label: 'Quiz', render: (r) => <><div className="fw-semibold">{r.title}</div><div className="small text-secondary">{r.category || 'No question bank'} · v{r.version}</div></> },
            { key: 'rules', label: 'Rules', render: (r) => <span className="small">{r.selectionMode === 'Random' ? `${r.questionCount} random` : 'All (fixed)'} · {r.durationMinutes} min{r.negativeMarks > 0 && ` · −${r.negativeMarks}`}</span> },
            { key: 'usableQuestions', label: 'Usable questions', render: (r) => <span className={r.selectionMode === 'Random' && r.usableQuestions < r.questionCount ? 'text-danger fw-semibold' : ''}>{r.usableQuestions}{r.attachedQuestions > 0 && ' (attached)'}</span> },
            { key: 'attempts', label: 'Attempts' },
            { key: 'isPublished', label: 'Status', render: (r) => <Badge value={r.isPublished ? 'Published' : 'Draft'} /> },
            {
              key: 'actions', label: '', render: (r) => (
                <div className="btn-group btn-group-sm flex-wrap">
                  <button className="btn btn-outline-secondary" onClick={() => setModal({ type: 'preview', quiz: r })}>Preview</button>
                  {manage && <button className="btn btn-outline-secondary" onClick={() => setModal({ type: 'form', quiz: r })}>Edit</button>}
                  {manage && <button className="btn btn-outline-secondary" onClick={() => setModal({ type: 'questions', quiz: r })}>Questions</button>}
                  {publish && (r.isPublished
                    ? <button className="btn btn-outline-warning" onClick={async () => (await confirm('Unpublish this quiz?', 'Students will no longer be able to start it. Attempts in progress continue.', 'Unpublish')) && run(() => api.post(`/admin/quiz/quizzes/${r.id}/unpublish`), 'Unpublished', reload)}>Unpublish</button>
                    : <button className="btn btn-outline-success" onClick={() => run(() => api.post(`/admin/quiz/quizzes/${r.id}/publish`), 'Published', reload)}>Publish</button>)}
                  {manage && <button className="btn btn-outline-danger" onClick={async () => (await confirm(`Delete ${r.title}?`, 'Recorded attempts and results are kept.', 'Delete', true)) && run(() => api.delete(`/admin/quiz/quizzes/${r.id}`), 'Deleted', reload)}>Delete</button>}
                </div>
              ),
            },
          ]} />
      </Card>
      {modal?.type === 'form' && <QuizForm quiz={modal.quiz} categories={categories.data || []} onClose={close} onSaved={done} />}
      {modal?.type === 'questions' && <QuizQuestionsModal quiz={modal.quiz} onClose={close} onSaved={done} />}
      {modal?.type === 'preview' && <QuizPreviewModal quiz={modal.quiz} onClose={close} />}
    </>
  )
}
