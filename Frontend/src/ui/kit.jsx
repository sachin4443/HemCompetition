import { useCallback, useEffect, useRef, useState } from 'react'
import Swal from 'sweetalert2'
import api, { errorMessage } from '../api/client'

// ----------------------------------------------------------------------------- formatting

export const fmtDateTime = (v) =>
  v ? new Date(v).toLocaleString(undefined, { day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit' }) : '—'

export const fmtDuration = (seconds) => {
  if (seconds == null) return '—'
  const s = Math.max(0, Math.round(seconds))
  const h = Math.floor(s / 3600)
  const m = Math.floor((s % 3600) / 60)
  const pad = (n) => String(n).padStart(2, '0')
  return h > 0 ? `${h}:${pad(m)}:${pad(s % 60)}` : `${pad(m)}:${pad(s % 60)}`
}

export const fmtBytes = (n) => (n > 1048576 ? `${(n / 1048576).toFixed(1)} MB` : `${Math.max(1, Math.round(n / 1024))} KB`)

/** Value for an <input type="datetime-local"> in the viewer's time zone. */
export const toLocalInput = (v) => {
  if (!v) return ''
  const d = new Date(v)
  return new Date(d.getTime() - d.getTimezoneOffset() * 60000).toISOString().slice(0, 16)
}
/** Default for a date-time field: this many hours from now. */
export const localInputInHours = (hours) => toLocalInput(new Date(new Date().getTime() + hours * 3600000))
export const fromLocalInput = (v) => (v ? new Date(v).toISOString() : null)

// ----------------------------------------------------------------------------- dialogs

export const toast = (title, icon = 'success') =>
  Swal.fire({ toast: true, position: 'top-end', timer: 3200, showConfirmButton: false, icon, title, timerProgressBar: true })

export const alertError = (error, fallback) => Swal.fire({ icon: 'error', title: 'Could not complete that', text: errorMessage(error, fallback) })

export const confirm = async (title, text, confirmButtonText = 'Yes', danger = false) =>
  (await Swal.fire({
    title, text, icon: danger ? 'warning' : 'question', showCancelButton: true, confirmButtonText,
    confirmButtonColor: danger ? '#dc3545' : '#198754', reverseButtons: true, focusCancel: danger,
  })).isConfirmed

/** Asks for a short written reason. Resolves to the text, or null when cancelled. */
export const askReason = async (title, label = 'Reason', min = 5) => {
  const result = await Swal.fire({
    title, input: 'textarea', inputLabel: label, showCancelButton: true, confirmButtonText: 'Confirm', reverseButtons: true,
    inputValidator: (v) => (!v || v.trim().length < min ? `Please write at least ${min} characters.` : undefined),
  })
  return result.isConfirmed ? result.value.trim() : null
}

// ----------------------------------------------------------------------------- small components

export function Spinner({ label = 'Loading…', full = false }) {
  return (
    <div className={`d-flex flex-column align-items-center justify-content-center text-secondary ${full ? 'min-vh-100' : 'py-5'}`} role="status">
      <div className="spinner-border text-success mb-2" aria-hidden="true" />
      <span className="small">{label}</span>
    </div>
  )
}

export function ErrorBox({ error, onRetry }) {
  return (
    <div className="alert alert-danger d-flex flex-wrap align-items-center gap-2" role="alert">
      <i className="bi bi-exclamation-triangle-fill" aria-hidden="true" />
      <span className="flex-grow-1">{typeof error === 'string' ? error : errorMessage(error)}</span>
      {onRetry && <button className="btn btn-sm btn-outline-danger" onClick={onRetry}>Try again</button>}
    </div>
  )
}

export function Empty({ icon = 'bi-inbox', children }) {
  return (
    <div className="text-center text-secondary py-5">
      <i className={`bi ${icon} display-6 d-block mb-2`} aria-hidden="true" />
      {children}
    </div>
  )
}

const BADGES = {
  success: ['Completed', 'Correct', 'Active', 'Approved', 'Live', 'Published', 'Confirmed', 'Yes', 'Online'],
  warning: ['InProgress', 'Pending', 'Flagged', 'Warning', 'Suspension', 'Upcoming', 'Open', 'Serious', 'Draft'],
  danger: ['Terminated', 'Suspended', 'Ban', 'Rejected', 'Critical', 'Banned', 'Restricted'],
  secondary: ['Quit', 'Expired', 'Skipped', 'Ended', 'Dismissed', 'Minor', 'Inactive', 'Recorded', 'No', 'Lifted'],
}
const LABELS = { InProgress: 'In progress', Quit: 'Left early', Suspension: 'Suspended', Ban: 'Banned' }

export function Badge({ value, label }) {
  const tone = Object.keys(BADGES).find((k) => BADGES[k].includes(value)) || 'secondary'
  return <span className={`badge rounded-pill text-bg-${tone} fw-semibold`}>{label || LABELS[value] || value}</span>
}

/** Modal dialog. Closes on Escape and backdrop click; returns focus management to the browser. */
export function Modal({ title, onClose, children, footer, size = '', scrollable = true }) {
  const ref = useRef(null)
  useEffect(() => {
    const onKey = (e) => e.key === 'Escape' && onClose?.()
    document.addEventListener('keydown', onKey)
    document.body.classList.add('modal-open')
    ref.current?.focus()
    return () => {
      document.removeEventListener('keydown', onKey)
      document.body.classList.remove('modal-open')
    }
  }, [onClose])
  return (
    <>
      <div className="modal-backdrop fade show" />
      <div className="modal fade show d-block" role="dialog" aria-modal="true" aria-label={title} tabIndex={-1} ref={ref}
        onMouseDown={(e) => e.target === e.currentTarget && onClose?.()}>
        <div className={`modal-dialog modal-dialog-centered ${scrollable ? 'modal-dialog-scrollable' : ''} ${size}`}>
          <div className="modal-content border-0 shadow-lg rounded-4">
            <div className="modal-header">
              <h2 className="modal-title fs-5 fw-bold">{title}</h2>
              {onClose && <button type="button" className="btn-close" aria-label="Close" onClick={onClose} />}
            </div>
            <div className="modal-body">{children}</div>
            {footer && <div className="modal-footer">{footer}</div>}
          </div>
        </div>
      </div>
    </>
  )
}

export function Field({ label, hint, children, className = 'mb-3' }) {
  return (
    <label className={`d-block ${className}`}>
      <span className="form-label fw-semibold small mb-1 d-block">{label}</span>
      {children}
      {hint && <span className="form-text d-block">{hint}</span>}
    </label>
  )
}

export function Check({ label, checked, onChange, disabled, hint, switchStyle = true }) {
  return (
    <label className={`form-check ${switchStyle ? 'form-switch' : ''} mb-2`}>
      <input type="checkbox" className="form-check-input" role={switchStyle ? 'switch' : undefined} checked={!!checked} disabled={disabled}
        onChange={(e) => onChange(e.target.checked)} />
      <span className="form-check-label">{label}</span>
      {hint && <span className="form-text d-block">{hint}</span>}
    </label>
  )
}

export function SubmitButton({ busy, children, className = 'btn btn-success', ...rest }) {
  return (
    <button type="submit" className={className} disabled={busy || rest.disabled} {...rest}>
      {busy && <span className="spinner-border spinner-border-sm me-2" aria-hidden="true" />}
      {children}
    </button>
  )
}

// ----------------------------------------------------------------------------- data loading

/** Loads a resource; reloads when `deps` change or `reload()` is called. */
export function useFetch(url, deps = []) {
  const [state, setState] = useState({ data: null, loading: true, error: null })
  const [tick, setTick] = useState(0)
  useEffect(() => {
    if (!url) return undefined
    let cancelled = false
    setState((s) => ({ ...s, loading: true, error: null }))
    api.get(url)
      .then((r) => !cancelled && setState({ data: r.data, loading: false, error: null }))
      .catch((e) => !cancelled && setState((s) => ({ data: s.data, loading: false, error: e })))
    return () => { cancelled = true }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [url, tick, ...deps])
  const reload = useCallback(() => setTick((t) => t + 1), [])
  return { ...state, reload }
}

/** Server-paged list with search and extra filters. */
export function usePaged(url, filters = {}, pageSize = 25) {
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [debounced, setDebounced] = useState('')
  useEffect(() => {
    const t = setTimeout(() => { setDebounced(search); setPage(1) }, 350)
    return () => clearTimeout(t)
  }, [search])
  const filterKey = JSON.stringify(filters)
  useEffect(() => setPage(1), [filterKey])
  const params = new URLSearchParams({ page, pageSize })
  if (debounced) params.set('search', debounced)
  Object.entries(filters).forEach(([k, v]) => v !== '' && v != null && params.set(k, v))
  const result = useFetch(`${url}?${params}`)
  return { ...result, items: result.data?.items || [], total: result.data?.total || 0, page, setPage, pageSize, search, setSearch }
}

export function Pager({ page, pageSize, total, onPage }) {
  const pages = Math.max(1, Math.ceil(total / pageSize))
  if (total === 0) return null
  return (
    <div className="d-flex flex-wrap align-items-center justify-content-between gap-2 mt-3 small text-secondary">
      <span>{(page - 1) * pageSize + 1}–{Math.min(page * pageSize, total)} of {total}</span>
      <div className="btn-group">
        <button className="btn btn-sm btn-outline-secondary" disabled={page <= 1} onClick={() => onPage(page - 1)} aria-label="Previous page">
          <i className="bi bi-chevron-left" />
        </button>
        <span className="btn btn-sm btn-outline-secondary disabled">Page {page} / {pages}</span>
        <button className="btn btn-sm btn-outline-secondary" disabled={page >= pages} onClick={() => onPage(page + 1)} aria-label="Next page">
          <i className="bi bi-chevron-right" />
        </button>
      </div>
    </div>
  )
}

/**
 * Responsive table. `columns`: [{ key, label, render?, className? }]. On phones each row becomes a
 * card with the column label shown beside the value (see .table-stack in index.css).
 */
export function DataTable({ columns, rows, loading, error, onRetry, empty = 'Nothing to show yet.', rowKey = 'id', onRowClick }) {
  if (error && !rows?.length) return <ErrorBox error={error} onRetry={onRetry} />
  if (loading && !rows?.length) return <Spinner />
  if (!rows?.length) return <Empty>{empty}</Empty>
  return (
    <div className={`table-responsive ${loading ? 'opacity-50' : ''}`}>
      <table className="table table-hover align-middle table-stack mb-0">
        <thead className="table-light">
          <tr>{columns.map((c) => <th key={c.key} scope="col" className={c.className}>{c.label}</th>)}</tr>
        </thead>
        <tbody>
          {rows.map((row, i) => (
            <tr key={row[rowKey] ?? i} onClick={onRowClick ? () => onRowClick(row) : undefined} style={onRowClick ? { cursor: 'pointer' } : undefined}>
              {columns.map((c) => (
                <td key={c.key} data-label={c.label} className={c.className}>{c.render ? c.render(row) : row[c.key] ?? '—'}</td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

export function SearchBox({ value, onChange, placeholder = 'Search…' }) {
  return (
    <div className="input-group input-group-sm" style={{ maxWidth: 320 }}>
      <span className="input-group-text bg-white"><i className="bi bi-search" aria-hidden="true" /></span>
      <input type="search" className="form-control" placeholder={placeholder} aria-label={placeholder} value={value} onChange={(e) => onChange(e.target.value)} />
    </div>
  )
}

/** Countdown to a server deadline, corrected for the difference between server and device clocks. */
export function useCountdown(deadlineUtc, serverTimeUtc) {
  const offset = useRef(0)
  const [remaining, setRemaining] = useState(null)
  useEffect(() => {
    if (!deadlineUtc) { setRemaining(null); return undefined }
    if (serverTimeUtc) offset.current = new Date(serverTimeUtc).getTime() - Date.now()
    const tick = () => setRemaining(Math.max(0, Math.round((new Date(deadlineUtc).getTime() - (Date.now() + offset.current)) / 1000)))
    tick()
    const timer = setInterval(tick, 1000)
    return () => clearInterval(timer)
  }, [deadlineUtc, serverTimeUtc])
  return remaining
}
