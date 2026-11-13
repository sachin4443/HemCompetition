import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import api from '../api/client'
import { Badge, DataTable, Empty, ErrorBox, fmtDateTime, fmtDuration, Modal, Spinner, useCountdown, useFetch } from '../ui/kit'
import { BUTTON_RULES, GAME_INFO, QUIZ_IMAGES, SECTION_ART, sectionStatus, statusButton } from './content'

/** Loads the three sections (generated games + quizzes) in the shape the UI needs. */
function useSections() {
  const games = useFetch('/games')
  const quizzes = useFetch('/quiz')
  const sections = (games.data || []).map((c) => ({
    key: c.key, name: c.name, description: c.description,
    items: c.key === 'quiz'
      ? (quizzes.data || []).map((q, i) => ({
        kind: 'quiz', id: q.id, name: q.title, description: q.description, status: q.status, attemptId: q.attemptId,
        image: QUIZ_IMAGES[i % QUIZ_IMAGES.length],
        meta: `${q.questionCount} questions · ${q.durationMinutes} min${q.negativeMarks > 0 ? ` · −${q.negativeMarks} for a wrong answer` : ''}`,
        to: `/quiz/${q.id}`,
      }))
      : c.games.map((g) => ({
        kind: 'game', id: g.key, name: g.name, description: g.description, status: g.status, attemptId: g.attemptId,
        image: GAME_INFO[g.key]?.image, rules: GAME_INFO[g.key]?.rules || [],
        meta: `${g.rounds} rounds${g.timeLimitSeconds ? ` · ${Math.round(g.timeLimitSeconds / 60)} min` : ''}`,
        progress: g.progress, rounds: g.rounds, to: `/play/${g.key}`,
      })),
  }))
  return {
    sections, loading: games.loading || quizzes.loading, error: games.error || quizzes.error,
    reload: () => { games.reload(); quizzes.reload() },
  }
}

function CompetitionStrip({ competition, serverTimeUtc }) {
  const phase = competition?.phase
  const left = useCountdown(phase === 'Live' ? competition.endDateTimeUtc : phase === 'NotStarted' ? competition.startDateTimeUtc : null, serverTimeUtc)
  if (!competition) return <div className="alert alert-secondary">No competition is active right now.</div>
  const tone = phase === 'Live' ? 'success' : phase === 'NotStarted' ? 'warning' : 'secondary'
  return (
    <div className={`alert alert-${tone} d-flex flex-wrap justify-content-between align-items-center gap-2`} role="status">
      <div>
        <div className="fw-bold">{competition.name}</div>
        <div className="small">{fmtDateTime(competition.startDateTimeUtc)} – {fmtDateTime(competition.endDateTimeUtc)}</div>
      </div>
      <div className="text-end">
        <Badge value={phase === 'Live' ? 'Live' : phase === 'NotStarted' ? 'Upcoming' : 'Ended'} />
        {left != null && <div className="small play-timer mt-1">{phase === 'Live' ? 'Time left' : 'Starts in'} {fmtDuration(left)}</div>}
      </div>
    </div>
  )
}

function History({ showResults }) {
  const { data, loading, error, reload } = useFetch('/student/history')
  return (
    <DataTable loading={loading} error={error} onRetry={reload} rows={data} empty="You have not started anything yet."
      columns={[
        { key: 'name', label: 'Activity', render: (r) => <><div className="fw-semibold">{r.name}</div><div className="small text-secondary">{r.section}</div></> },
        { key: 'status', label: 'Status', render: (r) => <Badge value={r.status} /> },
        { key: 'startedAtUtc', label: 'Started', render: (r) => fmtDateTime(r.startedAtUtc) },
        { key: 'timeTakenSeconds', label: 'Time', render: (r) => (r.status === 'InProgress' ? '—' : fmtDuration(r.timeTakenSeconds)) },
        { key: 'score', label: 'Score', render: (r) => (r.score == null ? <span className="text-secondary small">{showResults ? '—' : 'Not published'}</span> : `${r.score} / ${r.maxScore}`) },
      ]} />
  )
}

function Leaderboard() {
  const { data, loading, error, reload } = useFetch('/student/leaderboard')
  if (loading) return <Spinner />
  if (error) return <ErrorBox error={error} onRetry={reload} />
  if (!data.visible) return <Empty icon="bi-trophy">The leaderboard will appear here when the organisers publish it.</Empty>
  const rows = data.me && !data.top.some((r) => r.isMe) ? [...data.top, data.me] : data.top
  return (
    <>
      <p className="small text-secondary">{data.participants} participants · ranked by total score, then by shortest time.</p>
      <DataTable rows={rows} rowKey="rank" empty="No results yet."
        columns={[
          { key: 'rank', label: 'Rank', render: (r) => <span className="fw-bold">{r.rank <= 3 ? ['🥇', '🥈', '🥉'][r.rank - 1] : `#${r.rank}`}</span> },
          { key: 'name', label: 'Student', render: (r) => <span className={r.isMe ? 'fw-bold text-success' : ''}>{r.name}{r.isMe && ' (you)'}</span> },
          { key: 'city', label: 'City' },
          { key: 'totalScore', label: 'Score' },
          { key: 'timeTakenSeconds', label: 'Time', render: (r) => fmtDuration(r.timeTakenSeconds) },
        ]} />
    </>
  )
}

function Notifications({ onRead }) {
  const { data, loading, error, reload } = useFetch('/student/notifications')
  useEffect(() => {
    if (data?.some((n) => !n.read)) api.post('/student/notifications/read').then(onRead).catch(() => {})
  }, [data, onRead])
  if (loading) return <Spinner />
  if (error) return <ErrorBox error={error} onRetry={reload} />
  if (!data.length) return <Empty icon="bi-bell">No notifications.</Empty>
  return data.map((n) => (
    <div key={n.id} className={`border rounded-3 p-3 mb-2 ${n.read ? 'bg-white' : 'bg-success-subtle'}`}>
      <div className="d-flex justify-content-between gap-2"><strong>{n.title}</strong><span className="small text-secondary text-nowrap">{fmtDateTime(n.createdAtUtc)}</span></div>
      <div className="small" style={{ whiteSpace: 'pre-wrap' }}>{n.body}</div>
    </div>
  ))
}

export default function DashboardPage() {
  const dash = useFetch('/student/dashboard')
  const { sections, loading, error, reload } = useSections()
  const [tab, setTab] = useState('play')
  const [unread, setUnread] = useState(0)
  useEffect(() => { if (dash.data) setUnread(dash.data.unreadNotifications) }, [dash.data])

  if (dash.loading && !dash.data) return <Spinner full />
  if (dash.error && !dash.data) return <div className="container py-4"><ErrorBox error={dash.error} onRetry={dash.reload} /></div>
  const { profile, competition, serverTimeUtc } = dash.data
  const live = competition?.phase === 'Live'

  const tabs = [
    ['play', 'bi-controller', 'Play'], ['history', 'bi-clock-history', 'My attempts'],
    ['leaderboard', 'bi-trophy', 'Leaderboard'], ['notifications', 'bi-bell', `Notifications${unread ? ` (${unread})` : ''}`],
    ['profile', 'bi-person', 'Profile'],
  ]

  return (
    <div className="container py-3 py-md-4 fade-in">
      <div className="d-flex flex-wrap justify-content-between align-items-end gap-2 mb-3">
        <div>
          <h1 className="h4 fw-bold mb-0">Hello, {profile.firstName || profile.username}</h1>
          <div className="text-secondary small">Account status: <Badge value="Active" /></div>
        </div>
      </div>
      <CompetitionStrip competition={competition} serverTimeUtc={serverTimeUtc} />

      <ul className="nav nav-pills flex-nowrap overflow-auto mb-3 pb-1" role="tablist">
        {tabs.map(([key, icon, label]) => (
          <li className="nav-item" key={key} role="presentation">
            <button className={`nav-link text-nowrap ${tab === key ? 'active bg-success' : 'text-success'}`} role="tab" aria-selected={tab === key} onClick={() => setTab(key)}>
              <i className={`bi ${icon} me-1`} aria-hidden="true" />{label}
            </button>
          </li>
        ))}
      </ul>

      {tab === 'play' && (
        loading && !sections.length ? <Spinner /> : error ? <ErrorBox error={error} onRetry={reload} /> : (
          <div className="row g-3 g-md-4">
            {sections.map((s) => {
              const status = sectionStatus(s.items)
              const btn = statusButton(status)
              const done = s.items.filter((i) => i.status === 'finished').length
              return (
                <div className="col-12 col-md-6 col-xl-4" key={s.key}>
                  <div className="card border-0 shadow-sm rounded-4 h-100 text-center card-lift">
                    <div className="p-3 pb-0"><img src={SECTION_ART[s.key]?.image} alt="" className="img-fluid" style={{ height: 170, objectFit: 'contain' }} /></div>
                    <div className="card-body d-flex flex-column">
                      <h2 className="h5 fw-bold mb-1">{s.name}</h2>
                      <p className="text-secondary small mb-2">{SECTION_ART[s.key]?.blurb}</p>
                      <div className="small text-secondary mb-3">{status === 'empty' ? 'Nothing available yet' : `${done} of ${s.items.length} completed`}</div>
                      <Link to={`/section/${s.key}`} className={`btn ${status === 'finished' ? 'btn-outline-secondary' : btn.className} btn-lg rounded-pill mt-auto ${status === 'empty' ? 'disabled' : ''}`}>
                        <i className={`bi ${btn.icon} me-2`} aria-hidden="true" />{status === 'finished' ? 'Completed · view' : status === 'in_progress' ? 'Continue' : 'Start Challenge'}
                      </Link>
                    </div>
                  </div>
                </div>
              )
            })}
            {!live && <div className="col-12"><div className="alert alert-info small mb-0">Games can be played only while the competition is live.</div></div>}
          </div>
        )
      )}
      {tab === 'history' && <div className="card border-0 shadow-sm rounded-4"><div className="card-body"><History showResults={competition?.showResults} /></div></div>}
      {tab === 'leaderboard' && <div className="card border-0 shadow-sm rounded-4"><div className="card-body"><Leaderboard /></div></div>}
      {tab === 'notifications' && <Notifications onRead={() => setUnread(0)} />}
      {tab === 'profile' && (
        <div className="card border-0 shadow-sm rounded-4"><div className="card-body">
          <dl className="row mb-0">
            {[['Student ID', profile.userId], ['Username', profile.username], ['Name', [profile.firstName, profile.lastName].filter(Boolean).join(' ')],
              ['Email', profile.email], ['Mobile', profile.mobileNo], ['City', profile.city], ['Last sign-in', fmtDateTime(profile.lastLoginAtUtc)]].map(([k, v]) => (
              <div className="col-12 col-md-6 d-flex py-1" key={k}><dt className="text-secondary fw-normal" style={{ width: 120 }}>{k}</dt><dd className="mb-0 fw-semibold">{v || '—'}</dd></div>
            ))}
          </dl>
          {dash.data.supportContact && <p className="small text-secondary mt-3 mb-0">Need help? {dash.data.supportContact}</p>}
        </div></div>
      )}
    </div>
  )
}

/** The games (or quizzes) of one section, with rules and a Start / Continue / Completed button each. */
export function SectionPage() {
  const { key } = useParams()
  const { sections, loading, error, reload } = useSections()
  const [rulesFor, setRulesFor] = useState(null)
  const section = sections.find((s) => s.key === key)

  if (loading && !section) return <Spinner full />
  if (error) return <div className="container py-4"><ErrorBox error={error} onRetry={reload} /></div>
  if (!section) return <div className="container py-4"><Empty>This section is not available.</Empty></div>

  return (
    <div className="container py-3 py-md-4 fade-in">
      <nav aria-label="breadcrumb"><ol className="breadcrumb small"><li className="breadcrumb-item"><Link to="/dashboard">Dashboard</Link></li><li className="breadcrumb-item active">{section.name}</li></ol></nav>
      <h1 className="h4 fw-bold mb-3">{section.name}</h1>
      {!section.items.length && <Empty>Nothing has been published in this section yet.</Empty>}
      <div className="row g-3 g-md-4">
        {section.items.map((item) => {
          const btn = statusButton(item.status)
          return (
            <div className="col-12 col-sm-6 col-xl-4" key={item.id}>
              <div className="card border-0 shadow-sm rounded-4 h-100 text-center card-lift">
                {item.image && <div className="p-3 pb-0"><img src={item.image} alt="" className="img-fluid" style={{ height: 140, objectFit: 'contain' }} /></div>}
                <div className="card-body d-flex flex-column">
                  <h2 className="h5 fw-bold mb-1">{item.name}</h2>
                  <p className="text-secondary small mb-1">{item.description}</p>
                  <div className="small text-secondary mb-3">{item.meta}{item.status === 'in_progress' && item.kind === 'game' && ` · ${item.progress}/${item.rounds} done`}</div>
                  <div className="d-flex gap-2 mt-auto">
                    <button className="btn btn-outline-secondary rounded-pill" onClick={() => setRulesFor(item)} aria-label={`Rules for ${item.name}`}><i className="bi bi-info-circle" aria-hidden="true" /> Rules</button>
                    {btn.disabled
                      ? <span className="btn btn-outline-secondary rounded-pill flex-grow-1 disabled"><i className={`bi ${btn.icon} me-1`} aria-hidden="true" />{btn.label}</span>
                      : <Link to={item.to} className={`btn ${btn.className} rounded-pill flex-grow-1 fw-semibold`}><i className={`bi ${btn.icon} me-1`} aria-hidden="true" />{btn.label}</Link>}
                  </div>
                </div>
              </div>
            </div>
          )
        })}
      </div>

      {rulesFor && (
        <Modal title={`${rulesFor.name} – how to play`} onClose={() => setRulesFor(null)}
          footer={<button className="btn btn-success rounded-pill px-4" onClick={() => setRulesFor(null)}>Got it</button>}>
          {rulesFor.kind === 'quiz' ? (
            <ul className="mb-3">
              <li>Choose one option for each question. You can change an answer until you submit.</li>
              <li>The timer keeps running if you leave the page. When time is up the quiz is submitted automatically.</li>
              <li>Press <strong>Submit</strong> to finish. After that your answers cannot be changed.</li>
            </ul>
          ) : (
            <ol className="mb-3">{rulesFor.rules.map((r) => <li key={r} className="mb-1">{r}</li>)}</ol>
          )}
          {rulesFor.kind === 'game' && (
            <table className="table table-sm small mb-0"><tbody>
              {BUTTON_RULES.map(([name, text]) => <tr key={name}><th scope="row" className="text-nowrap">{name}</th><td>{text}</td></tr>)}
            </tbody></table>
          )}
        </Modal>
      )}
    </div>
  )
}
