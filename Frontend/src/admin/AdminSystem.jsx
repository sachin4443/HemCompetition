import { useEffect, useState } from 'react'
import api from '../api/client'
import { useAdminAuth } from '../auth/AdminAuth'
import { Badge, Check, confirm, DataTable, ErrorBox, Field, fmtDateTime, fmtDuration, Modal, Pager, SearchBox, Spinner, SubmitButton, toast, useFetch, usePaged } from '../ui/kit'
import { Card, DownloadButton, errorMessage, PageHeader, run, Select, Toolbar } from './shared'

// ============================================================================= Admin users

function AdminForm({ admin, roles, onClose, onSaved }) {
  const [v, setV] = useState(admin.id
    ? { username: admin.username, displayName: admin.displayName || '', email: admin.email || '', roleId: admin.roleId, isActive: admin.isActive, password: '' }
    : { username: '', displayName: '', email: '', roleId: roles.find((r) => r.name === 'Admin')?.id || roles[0]?.id, isActive: true, password: '' })
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')
  const set = (k) => (e) => setV({ ...v, [k]: e?.target ? e.target.value : e })
  const save = async (e) => {
    e.preventDefault()
    setBusy(true); setError('')
    try {
      const body = { ...v, roleId: Number(v.roleId), email: v.email || null, password: v.password || null }
      await (admin.id ? api.put(`/admin/admins/${admin.id}`, body) : api.post('/admin/admins', body))
      onSaved()
    } catch (err) { setError(errorMessage(err)) }
    setBusy(false)
  }
  return (
    <Modal title={admin.id ? 'Edit admin user' : 'Add admin user'} onClose={onClose}>
      <form onSubmit={save}>
        {error && <div className="alert alert-danger small py-2">{error}</div>}
        <Field label="Username"><input className="form-control" value={v.username} onChange={set('username')} required minLength={3} autoComplete="off" /></Field>
        <div className="row g-2">
          <div className="col-6"><Field label="Display name"><input className="form-control" value={v.displayName} onChange={set('displayName')} /></Field></div>
          <div className="col-6"><Field label="Email"><input type="email" className="form-control" value={v.email} onChange={set('email')} /></Field></div>
        </div>
        <Field label="Role"><select className="form-select" value={v.roleId} onChange={set('roleId')} required>{roles.map((r) => <option key={r.id} value={r.id}>{r.name}</option>)}</select></Field>
        <Field label={admin.id ? 'New password (leave empty to keep the current one)' : 'Temporary password'} hint="At least 10 characters with letters and numbers. The user must change it at first sign-in.">
          <input className="form-control" value={v.password} onChange={set('password')} required={!admin.id} minLength={10} autoComplete="new-password" />
        </Field>
        <Check label="Account active" checked={v.isActive} onChange={set('isActive')} />
        <div className="text-end"><SubmitButton busy={busy}>Save</SubmitButton></div>
      </form>
    </Modal>
  )
}

export function AdminUsersPage() {
  const { data, loading, error, reload } = useFetch('/admin/admins')
  const roles = useFetch('/admin/roles')
  const [editing, setEditing] = useState(null)
  return (
    <>
      <PageHeader title="Admin users" subtitle="People who can sign in to this panel, and the role that decides what they can do.">
        <button className="btn btn-sm btn-success" onClick={() => setEditing({})}><i className="bi bi-plus-lg me-1" />Add admin</button>
      </PageHeader>
      <Card>
        <DataTable loading={loading} error={error} onRetry={reload} rows={data}
          columns={[
            { key: 'username', label: 'Username', render: (r) => <span className="fw-semibold">{r.username}</span> },
            { key: 'displayName', label: 'Name' }, { key: 'role', label: 'Role' },
            { key: 'isActive', label: 'Status', render: (r) => <><Badge value={r.isActive ? 'Active' : 'Inactive'} />{r.mustChangePassword && <span className="badge text-bg-warning ms-1">Must change password</span>}</> },
            { key: 'lastLoginAtUtc', label: 'Last sign-in', render: (r) => fmtDateTime(r.lastLoginAtUtc) },
            { key: 'actions', label: '', render: (r) => <button className="btn btn-sm btn-outline-secondary" onClick={() => setEditing(r)}>Edit</button> },
          ]} />
      </Card>
      {editing && roles.data && <AdminForm admin={editing} roles={roles.data} onClose={() => setEditing(null)} onSaved={() => { setEditing(null); reload() }} />}
    </>
  )
}

// ============================================================================= Roles

function RoleForm({ role, catalog, onClose, onSaved }) {
  const [name, setName] = useState(role.name || '')
  const [description, setDescription] = useState(role.description || '')
  const [selected, setSelected] = useState(new Set(role.permissions || []))
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')
  const groups = [...new Set(catalog.map((p) => p.group))]
  const toggle = (key, on) => setSelected((s) => { const n = new Set(s); if (on) n.add(key); else n.delete(key); return n })
  const save = async (e) => {
    e.preventDefault()
    setBusy(true); setError('')
    try {
      const body = { name, description, permissions: [...selected] }
      await (role.id ? api.put(`/admin/roles/${role.id}`, body) : api.post('/admin/roles', body))
      onSaved()
    } catch (err) { setError(errorMessage(err)) }
    setBusy(false)
  }
  return (
    <Modal title={role.id ? `Role: ${role.name}` : 'New role'} onClose={onClose} size="modal-lg">
      <form onSubmit={save}>
        {error && <div className="alert alert-danger small py-2">{error}</div>}
        <div className="row g-2">
          <div className="col-12 col-md-5"><Field label="Name"><input className="form-control" value={name} onChange={(e) => setName(e.target.value)} required disabled={role.isSystem} /></Field></div>
          <div className="col-12 col-md-7"><Field label="Description"><input className="form-control" value={description} onChange={(e) => setDescription(e.target.value)} /></Field></div>
        </div>
        <div className="row g-3">
          {groups.map((g) => (
            <div className="col-12 col-md-6" key={g}>
              <div className="fw-semibold small text-uppercase text-secondary mb-1">{g}</div>
              {catalog.filter((p) => p.group === g).map((p) => <Check key={p.key} switchStyle={false} label={p.label} checked={selected.has(p.key)} onChange={(on) => toggle(p.key, on)} />)}
            </div>
          ))}
        </div>
        <div className="text-end"><SubmitButton busy={busy}>Save</SubmitButton></div>
      </form>
    </Modal>
  )
}

export function RolesPage() {
  const { data, loading, error, reload } = useFetch('/admin/roles')
  const catalog = useFetch('/admin/permissions')
  const [editing, setEditing] = useState(null)
  return (
    <>
      <PageHeader title="Roles & permissions" subtitle="Permissions are checked by the server on every request; this panel only hides what a role cannot use.">
        <button className="btn btn-sm btn-success" onClick={() => setEditing({})}><i className="bi bi-plus-lg me-1" />New role</button>
      </PageHeader>
      <Card>
        <DataTable loading={loading} error={error} onRetry={reload} rows={data}
          columns={[
            { key: 'name', label: 'Role', render: (r) => <><span className="fw-semibold">{r.name}</span>{r.isSystem && <span className="badge text-bg-light border ms-1">Built-in</span>}</> },
            { key: 'description', label: 'Description' },
            { key: 'permissions', label: 'Permissions', render: (r) => (r.isSuperAdmin ? 'All' : r.permissions.length) },
            { key: 'admins', label: 'Admins' },
            {
              key: 'actions', label: '', render: (r) => !r.isSuperAdmin && (
                <div className="btn-group btn-group-sm">
                  <button className="btn btn-outline-secondary" onClick={() => setEditing(r)}>Edit</button>
                  {!r.isSystem && <button className="btn btn-outline-danger" onClick={async () => (await confirm(`Delete role ${r.name}?`, '', 'Delete', true)) && run(() => api.delete(`/admin/roles/${r.id}`), 'Deleted', reload)}>Delete</button>}
                </div>
              ),
            },
          ]} />
      </Card>
      {editing && catalog.data && <RoleForm role={editing} catalog={catalog.data} onClose={() => setEditing(null)} onSaved={() => { setEditing(null); reload() }} />}
    </>
  )
}

// ============================================================================= Settings

export function SettingsPage() {
  const { data, loading, error, reload } = useFetch('/admin/settings')
  const [v, setV] = useState(null)
  const [busy, setBusy] = useState(false)
  useEffect(() => { if (data) setV(data) }, [data])
  if (loading && !v) return <Spinner />
  if (error && !v) return <ErrorBox error={error} onRetry={reload} />
  if (!v) return null
  const set = (k) => (e) => setV({ ...v, [k]: e?.target ? (e.target.type === 'number' ? Number(e.target.value) : e.target.value) : e })
  const num = (key, label, hint, props = {}) => <div className="col-6 col-md-4"><Field label={label} hint={hint}><input type="number" className="form-control" value={v[key]} onChange={set(key)} {...props} /></Field></div>
  const save = async (e) => {
    e.preventDefault()
    setBusy(true)
    await run(() => api.put('/admin/settings', v), 'Settings saved', (r) => setV(r.data))
    setBusy(false)
  }
  return (
    <form onSubmit={save}>
      <PageHeader title="Settings" subtitle="Security, monitoring and general settings. Changes take effect within about half a minute.">
        <SubmitButton busy={busy} className="btn btn-sm btn-success">Save settings</SubmitButton>
      </PageHeader>
      <Card className="mb-3">
        <h2 className="h6 fw-bold">Sign-in security</h2>
        <Check label="Students must type a verification code (captcha) when signing in" checked={v.loginCaptchaEnabled} onChange={set('loginCaptchaEnabled')} />
        <div className="row g-2">
          {num('maxFailedLogins', 'Failed sign-ins before lockout', '3–20', { min: 3, max: 20 })}
          {num('lockoutMinutes', 'Lockout length (minutes)', null, { min: 1, max: 1440 })}
        </div>
      </Card>
      <Card className="mb-3">
        <h2 className="h6 fw-bold">Monitoring</h2>
        <Check label="Camera required when a competition is monitored" checked={v.cameraRequired} onChange={set('cameraRequired')} />
        <Check label="Fullscreen required during games and quizzes" checked={v.fullscreenRequired} onChange={set('fullscreenRequired')} />
        <div className="row g-2 mt-1">
          {num('noFaceSeconds', 'Nobody visible for (seconds)', 'Before it counts as a violation', { min: 3, max: 120 })}
          {num('multipleFacesSeconds', 'Several people for (seconds)', null, { min: 3, max: 120 })}
          <div className="col-12 col-md-4"><Field label="Consent version" hint="Change it to make every student accept the terms again.">
            <input className="form-control" value={v.consentVersion} onChange={set('consentVersion')} required maxLength={20} /></Field></div>
        </div>
      </Card>
      <Card className="mb-3">
        <h2 className="h6 fw-bold">Evidence</h2>
        <Check label="Capture evidence when a violation is detected" checked={v.evidenceCaptureEnabled} onChange={set('evidenceCaptureEnabled')} hint="Only for students who consented, and only for rules that ask for it." />
        <Check label="Allow short video clips" checked={v.videoEvidenceEnabled} onChange={set('videoEvidenceEnabled')} disabled={!v.evidenceCaptureEnabled} />
        <div className="row g-2 mt-1">
          {num('videoClipSeconds', 'Clip length (seconds)', '2–15', { min: 2, max: 15 })}
          {num('evidenceRetentionDays', 'Keep evidence for (days)', 'Files are deleted automatically afterwards', { min: 1, max: 730 })}
          {num('maxEvidenceImageKb', 'Max photo size (KB)', null, { min: 100, max: 10240 })}
          {num('maxEvidenceVideoKb', 'Max clip size (KB)', null, { min: 500, max: 51200 })}
        </div>
      </Card>
      <Card>
        <h2 className="h6 fw-bold">Appeals and general</h2>
        <Check label="Restricted students may request a review" checked={v.appealsEnabled} onChange={set('appealsEnabled')} />
        <div className="row g-2 mt-1">
          {num('maxAppealsPerBan', 'Requests allowed per restriction', null, { min: 1, max: 10 })}
          {num('leaderboardSize', 'Leaderboard size shown to students', null, { min: 5, max: 500 })}
          <div className="col-12 col-md-4"><Field label="Support contact shown to students"><input className="form-control" value={v.supportContact} onChange={set('supportContact')} maxLength={200} /></Field></div>
        </div>
      </Card>
    </form>
  )
}

// ============================================================================= Audit log

export function AuditPage() {
  const [entityType, setEntityType] = useState('')
  const list = usePaged('/admin/audit-logs', { entityType }, 50)
  const [open, setOpen] = useState(null)
  return (
    <>
      <PageHeader title="Audit log" subtitle="Every change made through this panel, who made it and when. Entries cannot be edited or deleted here." />
      <Card>
        <Toolbar>
          <SearchBox value={list.search} onChange={list.setSearch} placeholder="Admin, action or record ID…" />
          <Select label="Record type" value={entityType} onChange={setEntityType}
            options={['student', 'admin', 'role', 'quiz', 'question', 'quiz_category', 'game', 'word', 'competition', 'violation', 'violation_rule', 'evidence', 'appeal', 'settings', 'report']} />
        </Toolbar>
        <DataTable loading={list.loading} error={list.error} onRetry={list.reload} rows={list.items} empty="No entries."
          columns={[
            { key: 'createdAtUtc', label: 'When', render: (r) => fmtDateTime(r.createdAtUtc) },
            { key: 'actorName', label: 'Who' },
            { key: 'action', label: 'Action', render: (r) => <code>{r.action}</code> },
            { key: 'entity', label: 'Record', render: (r) => (r.entityType ? `${r.entityType}${r.entityId ? ` ${r.entityId}` : ''}` : '—') },
            { key: 'ipAddress', label: 'IP' },
            { key: 'details', label: '', render: (r) => r.details && <button className="btn btn-sm btn-outline-secondary" onClick={() => setOpen(r)}>Details</button> },
          ]} />
        <Pager {...list} onPage={list.setPage} />
      </Card>
      {open && (
        <Modal title={open.action} onClose={() => setOpen(null)}>
          <pre className="bg-light rounded p-2 small mb-0" style={{ whiteSpace: 'pre-wrap' }}>{(() => { try { return JSON.stringify(JSON.parse(open.details), null, 2) } catch { return open.details } })()}</pre>
        </Modal>
      )}
    </>
  )
}

// ============================================================================= Notifications

export function NotificationsPage() {
  const { data, loading, error, reload } = useFetch('/admin/students/notifications')
  const [v, setV] = useState({ studentId: '', title: '', body: '' })
  const [busy, setBusy] = useState(false)
  const send = async (e) => {
    e.preventDefault()
    if (!v.studentId && !(await confirm('Send to every student?', 'This notification will appear on every student dashboard.', 'Send to all'))) return
    setBusy(true)
    await run(() => api.post('/admin/students/notifications', { studentId: v.studentId ? Number(v.studentId) : null, title: v.title, body: v.body }), 'Notification sent',
      () => { setV({ studentId: '', title: '', body: '' }); reload() })
    setBusy(false)
  }
  return (
    <>
      <PageHeader title="Notifications" subtitle="Messages shown on the student dashboard." />
      <div className="row g-3">
        <div className="col-12 col-lg-5"><Card>
          <h2 className="h6 fw-bold">Send a notification</h2>
          <form onSubmit={send}>
            <Field label="Student ID" hint="Leave empty to send to all students."><input type="number" className="form-control" value={v.studentId} onChange={(e) => setV({ ...v, studentId: e.target.value })} /></Field>
            <Field label="Title"><input className="form-control" value={v.title} onChange={(e) => setV({ ...v, title: e.target.value })} required maxLength={200} /></Field>
            <Field label="Message"><textarea className="form-control" rows={4} value={v.body} onChange={(e) => setV({ ...v, body: e.target.value })} required maxLength={4000} /></Field>
            <SubmitButton busy={busy}>Send</SubmitButton>
          </form>
        </Card></div>
        <div className="col-12 col-lg-7"><Card>
          <h2 className="h6 fw-bold">Recently sent</h2>
          <DataTable loading={loading} error={error} onRetry={reload} rows={data} empty="Nothing sent yet."
            columns={[
              { key: 'createdAtUtc', label: 'Sent', render: (r) => fmtDateTime(r.createdAtUtc) },
              { key: 'studentId', label: 'To', render: (r) => (r.studentId ? `Student ${r.studentId}` : 'Everyone') },
              { key: 'title', label: 'Message', render: (r) => <><div className="fw-semibold">{r.title}</div><div className="small text-secondary">{r.body}</div></> },
            ]} />
        </Card></div>
      </div>
    </>
  )
}

// ============================================================================= Login sessions

export function SessionsPage() {
  const { can } = useAdminAuth()
  const [status, setStatus] = useState('')
  const list = usePaged('/admin-sessions', { status }, 50)
  const stats = useFetch('/admin-sessions/stats')
  // This endpoint predates the paged format and returns { data, total }.
  const rows = list.data?.data || []
  const total = list.data?.total || 0
  const end = async (r) => (await confirm(`End the session of ${r.fullName || r.username}?`, 'The student is signed out immediately.', 'End session', true))
    && run(() => api.post(`/admin/students/${r.userId}/terminate-session`), 'Session ended', () => { list.reload(); stats.reload() })
  return (
    <>
      <PageHeader title="Login sessions" subtitle="Student sign-ins. A student can have only one live session at a time.">
        <DownloadButton url="/admin-sessions/export" name="sessions.csv">Export CSV</DownloadButton>
      </PageHeader>
      {stats.data && (
        <div className="row g-3 mb-3">
          {[['Total', stats.data.totalSessions], ['Active now', stats.data.activeSessions], ['Today', stats.data.todaySessions], ['Average length', fmtDuration(stats.data.averageDurationSeconds)]].map(([k, val]) => (
            <div className="col-6 col-lg-3" key={k}><Card><div className="small text-secondary">{k}</div><div className="fs-4 fw-bold">{val}</div></Card></div>
          ))}
        </div>
      )}
      <Card>
        <Toolbar>
          <SearchBox value={list.search} onChange={list.setSearch} placeholder="Name, username or email…" />
          <Select label="Status" value={status} onChange={setStatus} options={[['active', 'Active'], ['loggedout', 'Ended']]} />
        </Toolbar>
        <DataTable loading={list.loading} error={list.error} onRetry={list.reload} rows={rows} rowKey="sessionRecordId" empty="No sessions."
          columns={[
            { key: 'fullName', label: 'Student', render: (r) => <><div className="fw-semibold">{r.fullName || r.username}</div><div className="small text-secondary">{r.username}</div></> },
            { key: 'loginAtUtc', label: 'Signed in', render: (r) => fmtDateTime(r.loginAtUtc) },
            { key: 'lastActivityUtc', label: 'Last activity', render: (r) => fmtDateTime(r.lastActivityUtc) },
            { key: 'duration', label: 'Length' },
            { key: 'ipAddress', label: 'IP' },
            { key: 'isActive', label: 'Status', render: (r) => <Badge value={r.isActive ? 'Active' : 'Ended'} /> },
            { key: 'actions', label: '', render: (r) => r.isActive && can('sessions.manage') && <button className="btn btn-sm btn-outline-danger" onClick={() => end(r)}>End</button> },
          ]} />
        <Pager page={list.page} pageSize={list.pageSize} total={total} onPage={list.setPage} />
      </Card>
    </>
  )
}

// ============================================================================= Diagnostics

export function DiagnosticsPage() {
  const { data, loading, error, reload } = useFetch('/admin-dashboard/diagnostics')
  useEffect(() => { const t = setInterval(reload, 5000); return () => clearInterval(t) }, [reload])
  if (loading && !data) return <Spinner />
  if (error && !data) return <ErrorBox error={error} onRetry={reload} />
  const d = data.data
  const ms = (v) => `${Math.round(v)} ms`
  return (
    <>
      <PageHeader title="Diagnostics" subtitle="Live API and database timings from this server process. Refreshes every 5 seconds.">
        <button type="button" className="btn btn-sm btn-outline-secondary" onClick={() => { reload(); toast('Refreshed') }}><i className="bi bi-arrow-clockwise me-1" />Refresh</button>
      </PageHeader>
      <div className="row g-3 mb-3">
        {[['Requests', d.totalRequests], ['Per second', Number(d.requestsPerSecond).toFixed(1)], ['Failed', d.failedRequests], ['Error rate', `${Number(d.errorRate).toFixed(1)}%`],
          ['Average', ms(d.averageLatencyMs)], ['P95', ms(d.p95LatencyMs)], ['P99', ms(d.p99LatencyMs)], ['Slowest', ms(d.maxLatencyMs)],
          ['CPU', `${Number(d.system?.processCpuPercent ?? 0).toFixed(1)}%`], ['Memory', `${Math.round(d.system?.workingSetMb ?? 0)} MB`], ['Threads', d.system?.threadCount ?? '—']].map(([k, v]) => (
          <div className="col-6 col-md-3 col-xl-2" key={k}><Card><div className="small text-secondary">{k}</div><div className="fs-5 fw-bold">{v}</div></Card></div>
        ))}
      </div>
      {d.bottlenecks?.length > 0 && <div className="alert alert-warning small"><strong>Possible bottlenecks:</strong><ul className="mb-0">{d.bottlenecks.map((b, i) => <li key={i}>{typeof b === 'string' ? b : b.message || JSON.stringify(b)}</li>)}</ul></div>}
      <Card className="mb-3">
        <h2 className="h6 fw-bold">Endpoints</h2>
        <DataTable rows={d.endpoints} rowKey="path" empty="No requests recorded yet."
          columns={[
            { key: 'method', label: 'Method' }, { key: 'path', label: 'Path', render: (r) => <code>{r.path}</code> },
            { key: 'requests', label: 'Requests' }, { key: 'failed', label: 'Failed' },
            { key: 'averageLatencyMs', label: 'Average', render: (r) => ms(r.averageLatencyMs) },
            { key: 'p50LatencyMs', label: 'Median', render: (r) => ms(r.p50LatencyMs) },
          ]} />
      </Card>
      <div className="row g-3">
        <div className="col-12 col-xl-6"><Card>
          <h2 className="h6 fw-bold">Slow requests</h2>
          <DataTable rows={d.slowRequests} empty="None."
            columns={[
              { key: 'timestampUtc', label: 'When', render: (r) => fmtDateTime(r.timestampUtc) },
              { key: 'path', label: 'Request', render: (r) => <code>{r.method} {r.path}</code> },
              { key: 'statusCode', label: 'Status' }, { key: 'durationMs', label: 'Time', render: (r) => ms(r.durationMs) },
            ]} />
        </Card></div>
        <div className="col-12 col-xl-6"><Card>
          <h2 className="h6 fw-bold">Slow database queries</h2>
          <DataTable rows={d.slowSqlQueries} rowKey="timestampUtc" empty="None."
            columns={[
              { key: 'timestampUtc', label: 'When', render: (r) => fmtDateTime(r.timestampUtc) },
              { key: 'durationMs', label: 'Time', render: (r) => ms(r.durationMs) },
              { key: 'sql', label: 'Query', render: (r) => <code className="small" style={{ whiteSpace: 'pre-wrap', wordBreak: 'break-all' }}>{String(r.sql).slice(0, 220)}</code> },
            ]} />
        </Card></div>
      </div>
    </>
  )
}
