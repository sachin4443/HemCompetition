import { useEffect, useState } from 'react'
import { Link, Navigate, NavLink, Route, Routes, useLocation, useNavigate } from 'react-router-dom'
import { errorMessage } from '../api/client'
import api from '../api/client'
import { AdminAuthProvider, useAdminAuth } from '../auth/AdminAuth'
import { Badge, ErrorBox, fmtDateTime, Spinner, SubmitButton, toast, useFetch } from '../ui/kit'
import { CompetitionsPage, GamesPage, QuestionsPage, QuizzesPage, StudentDetailPage, StudentsPage, WordBankPage } from './AdminContent'
import { AppealDetailPage, AppealsPage, AttemptDetailPage, AttemptsPage, BansPage, EvidencePage, LiveMonitorPage, ResultsPage, RulesPage, ViolationsPage } from './AdminReview'
import { AdminUsersPage, AuditPage, DiagnosticsPage, NotificationsPage, RolesPage, SessionsPage, SettingsPage } from './AdminSystem'

// Each entry is shown only when the admin holds the permission. The API enforces the same rule.
const NAV = [
  ['Overview', [['', 'bi-speedometer2', 'Dashboard', 'dashboard.view']]],
  ['People', [
    ['students', 'bi-people', 'Students', 'students.view'],
    ['notifications', 'bi-megaphone', 'Notifications', 'notifications.send'],
    ['sessions', 'bi-pc-display', 'Login sessions', 'sessions.view'],
  ]],
  ['Content', [
    ['competitions', 'bi-calendar-event', 'Competitions', 'competitions.view'],
    ['games', 'bi-controller', 'Games', 'games.view'],
    ['words', 'bi-fonts', 'Word bank', 'games.view'],
    ['quizzes', 'bi-ui-checks', 'Mega Quiz', 'quiz.view'],
    ['questions', 'bi-question-circle', 'Question bank', 'quiz.view'],
  ]],
  ['Results', [
    ['attempts', 'bi-list-check', 'Attempts', 'attempts.view'],
    ['results', 'bi-trophy', 'Results & reports', 'results.view'],
  ]],
  ['Integrity', [
    ['monitor', 'bi-camera-video', 'Live monitoring', 'proctoring.view'],
    ['violations', 'bi-exclamation-triangle', 'Violations', 'proctoring.view'],
    ['evidence', 'bi-images', 'Evidence', 'evidence.view'],
    ['rules', 'bi-sliders', 'Violation rules', 'proctoring.view'],
    ['bans', 'bi-slash-circle', 'Banned students', 'bans.view'],
    ['appeals', 'bi-chat-left-text', 'Appeals', 'appeals.view'],
  ]],
  ['Administration', [
    ['admins', 'bi-person-badge', 'Admin users', 'admins.manage'],
    ['roles', 'bi-key', 'Roles & permissions', 'roles.manage'],
    ['settings', 'bi-gear', 'Settings', 'settings.manage'],
    ['audit', 'bi-journal-text', 'Audit log', 'audit.view'],
    ['diagnostics', 'bi-activity', 'Diagnostics', 'diagnostics.view'],
  ]],
]

function AdminLogin() {
  const auth = useAdminAuth()
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')
  if (auth.status === 'authed') return <Navigate to="/admin" replace />
  const submit = async (e) => {
    e.preventDefault()
    setBusy(true); setError('')
    try { await auth.login(username.trim(), password) } catch (err) { setError(errorMessage(err)) }
    setBusy(false)
  }
  return (
    <div className="login-wrap d-flex align-items-center py-4">
      <div className="container" style={{ maxWidth: 400 }}>
        <div className="card border-0 shadow rounded-4"><div className="card-body p-4">
          <div className="text-center mb-3">
            <img src="/logo.png" alt="HEM Foundation" style={{ maxHeight: 64 }} />
            <h1 className="h5 fw-bold mt-3 mb-0">Administration</h1>
          </div>
          {error && <div className="alert alert-danger small py-2" role="alert">{error}</div>}
          <form onSubmit={submit}>
            <label className="form-label small fw-semibold" htmlFor="a-user">Username</label>
            <input id="a-user" className="form-control mb-3" autoComplete="username" value={username} onChange={(e) => setUsername(e.target.value)} required />
            <label className="form-label small fw-semibold" htmlFor="a-pass">Password</label>
            <input id="a-pass" type="password" className="form-control mb-4" autoComplete="current-password" value={password} onChange={(e) => setPassword(e.target.value)} required />
            <SubmitButton busy={busy} className="btn btn-success w-100 rounded-pill">Sign in</SubmitButton>
          </form>
        </div></div>
        <p className="text-center small mt-3"><Link to="/">Student sign-in</Link></p>
      </div>
    </div>
  )
}

export function ChangePassword({ forced }) {
  const auth = useAdminAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState({ currentPassword: '', newPassword: '', confirm: '' })
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')
  const submit = async (e) => {
    e.preventDefault()
    if (form.newPassword !== form.confirm) { setError('The two new passwords do not match.'); return }
    setBusy(true); setError('')
    try {
      await api.post('/admin/auth/change-password', { currentPassword: form.currentPassword, newPassword: form.newPassword })
      await auth.reload()
      toast('Password changed')
      navigate('/admin', { replace: true })
    } catch (err) { setError(errorMessage(err)) }
    setBusy(false)
  }
  const field = (key, label, autoComplete) => (
    <>
      <label className="form-label small fw-semibold" htmlFor={key}>{label}</label>
      <input id={key} type="password" className="form-control mb-3" autoComplete={autoComplete} value={form[key]} onChange={(e) => setForm({ ...form, [key]: e.target.value })} required />
    </>
  )
  return (
    <div className="mx-auto" style={{ maxWidth: 440 }}>
      <div className="card border-0 shadow-sm rounded-4"><div className="card-body p-4">
        <h1 className="h5 fw-bold">Change password</h1>
        {forced && <div className="alert alert-warning small">You must choose a new password before you can continue.</div>}
        {error && <div className="alert alert-danger small py-2" role="alert">{error}</div>}
        <form onSubmit={submit}>
          {field('currentPassword', 'Current password', 'current-password')}
          {field('newPassword', 'New password (at least 10 characters, letters and numbers)', 'new-password')}
          {field('confirm', 'Repeat new password', 'new-password')}
          <SubmitButton busy={busy} className="btn btn-success rounded-pill px-4">Save</SubmitButton>
        </form>
      </div></div>
    </div>
  )
}

function Stat({ icon, label, value, tone = 'success', to }) {
  const body = (
    <div className="card border-0 shadow-sm rounded-4 h-100 stat-card card-lift"><div className="card-body d-flex align-items-center gap-3">
      <div className={`rounded-3 bg-${tone}-subtle text-${tone} d-flex align-items-center justify-content-center`} style={{ width: 48, height: 48 }}>
        <i className={`bi ${icon} fs-4`} aria-hidden="true" />
      </div>
      <div><div className="stat-value">{value}</div><div className="small text-secondary">{label}</div></div>
    </div></div>
  )
  return <div className="col-6 col-lg-3">{to ? <Link to={to} className="text-decoration-none text-reset">{body}</Link> : body}</div>
}

function Dashboard() {
  const { data, loading, error, reload } = useFetch('/admin/dashboard')
  useEffect(() => { const t = setInterval(reload, 30000); return () => clearInterval(t) }, [reload])
  if (loading && !data) return <Spinner />
  if (error && !data) return <ErrorBox error={error} onRetry={reload} />
  const { competition: c, students: s, attempts: a, proctoring: p, content, metrics } = data
  return (
    <>
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-3">
        <h1 className="h4 fw-bold m-0">Dashboard</h1>
        <span className="small text-secondary">Updated {fmtDateTime(data.serverTimeUtc)} · refreshes every 30 s</span>
      </div>
      <div className={`alert ${c?.phase === 'Live' ? 'alert-success' : 'alert-secondary'} d-flex flex-wrap justify-content-between gap-2`}>
        {c ? <><span><strong>{c.name}</strong> · {fmtDateTime(c.startDateTimeUtc)} – {fmtDateTime(c.endDateTimeUtc)}</span><Badge value={c.phase === 'NotStarted' ? 'Upcoming' : c.phase} /></>
          : <span>No competition is active. <Link to="/admin/competitions">Create or activate one</Link>.</span>}
      </div>
      <div className="row g-3 mb-3">
        <Stat icon="bi-people" label="Students" value={s.total} to="/admin/students" />
        <Stat icon="bi-wifi" label="Online now" value={s.online} tone="primary" to="/admin/sessions" />
        <Stat icon="bi-person-check" label="Took part" value={s.participated} to="/admin/results" />
        <Stat icon="bi-slash-circle" label="Restricted" value={s.restricted} tone="danger" to="/admin/bans" />
        <Stat icon="bi-hourglass-split" label="Attempts in progress" value={a.gamesInProgress + a.quizzesInProgress} tone="warning" to="/admin/attempts" />
        <Stat icon="bi-check2-all" label="Attempts finished" value={a.gamesFinished + a.quizzesFinished} to="/admin/attempts" />
        <Stat icon="bi-flag" label="Flagged attempts" value={a.flagged} tone="danger" to="/admin/attempts" />
        <Stat icon="bi-camera-video" label="Monitored now" value={p.monitoredNow} tone="primary" to="/admin/monitor" />
        <Stat icon="bi-exclamation-triangle" label="Violations to review" value={p.openViolations} tone="warning" to="/admin/violations" />
        <Stat icon="bi-exclamation-octagon" label="Serious, unreviewed" value={p.openSerious} tone="danger" to="/admin/violations" />
        <Stat icon="bi-chat-left-text" label="Appeals waiting" value={p.pendingAppeals} tone="warning" to="/admin/appeals" />
        <Stat icon="bi-ui-checks" label="Published quizzes" value={content.publishedQuizzes} to="/admin/quizzes" />
      </div>
      <div className="row g-3">
        <div className="col-12 col-lg-6"><div className="card border-0 shadow-sm rounded-4 h-100"><div className="card-body">
          <h2 className="h6 fw-bold">Content</h2>
          <ul className="list-unstyled mb-0 small">
            <li className="d-flex justify-content-between py-1 border-bottom"><span>Questions in the bank</span><strong>{content.questions}</strong></li>
            <li className="d-flex justify-content-between py-1 border-bottom"><span>Active words in the word bank</span><strong>{content.words}</strong></li>
            <li className="d-flex justify-content-between py-1"><span>Violations in the last hour</span><strong>{p.violationsLastHour}</strong></li>
          </ul>
        </div></div></div>
        {metrics && (
          <div className="col-12 col-lg-6"><div className="card border-0 shadow-sm rounded-4 h-100"><div className="card-body">
            <h2 className="h6 fw-bold">API traffic</h2>
            <ul className="list-unstyled mb-0 small">
              <li className="d-flex justify-content-between py-1 border-bottom"><span>Requests</span><strong>{metrics.totalHits}</strong></li>
              <li className="d-flex justify-content-between py-1 border-bottom"><span>Failed</span><strong>{metrics.failedHits}</strong></li>
              <li className="d-flex justify-content-between py-1"><span>Average response</span><strong>{Math.round(metrics.averageLatencyMs)} ms</strong></li>
            </ul>
            <Link to="/admin/diagnostics" className="small">Open diagnostics</Link>
          </div></div></div>
        )}
      </div>
    </>
  )
}

function Layout() {
  const auth = useAdminAuth()
  const [open, setOpen] = useState(false)
  const { pathname } = useLocation()
  useEffect(() => setOpen(false), [pathname])
  const mustChange = auth.profile.mustChangePassword
  const allowed = (permission) => auth.can(permission)
  const guard = (permission, element) => (allowed(permission) ? element : <NoAccess />)

  return (
    <div className="admin-shell">
      <aside className={`admin-sidebar p-2 ${open ? 'open' : ''}`} aria-label="Admin navigation">
        <div className="d-flex align-items-center gap-2 px-2 py-3">
          <img src="/logo.png" alt="" height="32" className="bg-white rounded-2 p-1" />
          <span className="fw-bold text-white">HEM Admin</span>
        </div>
        {!mustChange && NAV.map(([group, items]) => {
          const visible = items.filter(([, , , permission]) => allowed(permission))
          if (!visible.length) return null
          return (
            <div key={group}>
              <div className="nav-group">{group}</div>
              <nav className="nav flex-column">
                {visible.map(([path, icon, label]) => (
                  <NavLink key={path} to={`/admin/${path}`} end={path === ''} className="nav-link"><i className={`bi ${icon}`} aria-hidden="true" />{label}</NavLink>
                ))}
              </nav>
            </div>
          )
        })}
      </aside>
      {open && <div className="admin-scrim" onClick={() => setOpen(false)} />}

      <div className="admin-main d-flex flex-column min-vh-100">
        <header className="bg-white border-bottom sticky-top">
          <div className="d-flex align-items-center justify-content-between px-3 py-2 gap-2">
            <button className="btn btn-outline-secondary d-lg-none" onClick={() => setOpen(true)} aria-label="Open menu"><i className="bi bi-list" /></button>
            <div className="ms-auto d-flex align-items-center gap-2">
              <span className="small text-secondary d-none d-sm-inline">{auth.profile.displayName || auth.profile.username} · <strong>{auth.profile.role}</strong></span>
              <Link to="/admin/password" className="btn btn-sm btn-outline-secondary" title="Change password"><i className="bi bi-key" aria-hidden="true" /><span className="visually-hidden">Change password</span></Link>
              <button className="btn btn-sm btn-outline-danger" onClick={auth.logout}><i className="bi bi-box-arrow-right me-1" aria-hidden="true" />Sign out</button>
            </div>
          </div>
        </header>
        <main className="flex-grow-1 p-3 p-md-4">
          {mustChange ? <ChangePassword forced /> : (
            <Routes>
              <Route index element={guard('dashboard.view', <Dashboard />)} />
              <Route path="password" element={<ChangePassword />} />
              <Route path="students" element={guard('students.view', <StudentsPage />)} />
              <Route path="students/:userId" element={guard('students.view', <StudentDetailPage />)} />
              <Route path="notifications" element={guard('notifications.send', <NotificationsPage />)} />
              <Route path="sessions" element={guard('sessions.view', <SessionsPage />)} />
              <Route path="competitions" element={guard('competitions.view', <CompetitionsPage />)} />
              <Route path="games" element={guard('games.view', <GamesPage />)} />
              <Route path="words" element={guard('games.view', <WordBankPage />)} />
              <Route path="quizzes" element={guard('quiz.view', <QuizzesPage />)} />
              <Route path="questions" element={guard('quiz.view', <QuestionsPage />)} />
              <Route path="attempts" element={guard('attempts.view', <AttemptsPage />)} />
              <Route path="attempts/:kind/:id" element={guard('attempts.view', <AttemptDetailPage />)} />
              <Route path="results" element={guard('results.view', <ResultsPage />)} />
              <Route path="monitor" element={guard('proctoring.view', <LiveMonitorPage />)} />
              <Route path="violations" element={guard('proctoring.view', <ViolationsPage />)} />
              <Route path="evidence" element={guard('evidence.view', <EvidencePage />)} />
              <Route path="rules" element={guard('proctoring.view', <RulesPage />)} />
              <Route path="bans" element={guard('bans.view', <BansPage />)} />
              <Route path="appeals" element={guard('appeals.view', <AppealsPage />)} />
              <Route path="appeals/:id" element={guard('appeals.view', <AppealDetailPage />)} />
              <Route path="admins" element={guard('admins.manage', <AdminUsersPage />)} />
              <Route path="roles" element={guard('roles.manage', <RolesPage />)} />
              <Route path="settings" element={guard('settings.manage', <SettingsPage />)} />
              <Route path="audit" element={guard('audit.view', <AuditPage />)} />
              <Route path="diagnostics" element={guard('diagnostics.view', <DiagnosticsPage />)} />
              <Route path="*" element={<Navigate to="/admin" replace />} />
            </Routes>
          )}
        </main>
      </div>
    </div>
  )
}

function NoAccess() {
  return <div className="alert alert-secondary">Your role does not include this section. Use the menu to open one you can access.</div>
}

function Gate() {
  const auth = useAdminAuth()
  if (auth.status === 'loading') return <Spinner full />
  return auth.status === 'authed' ? <Layout /> : <AdminLogin />
}

export default function AdminApp() {
  return <AdminAuthProvider><Gate /></AdminAuthProvider>
}
