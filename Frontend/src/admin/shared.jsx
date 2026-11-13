import { useRef, useState } from 'react'
import Swal from 'sweetalert2'
import api, { download, errorMessage } from '../api/client'
import { alertError, toast } from '../ui/kit'

export function PageHeader({ title, subtitle, children }) {
  return (
    <div className="d-flex flex-wrap justify-content-between align-items-start gap-2 mb-3">
      <div><h1 className="h4 fw-bold m-0">{title}</h1>{subtitle && <p className="text-secondary small mb-0">{subtitle}</p>}</div>
      <div className="d-flex flex-wrap gap-2">{children}</div>
    </div>
  )
}

export function Card({ children, className = '' }) {
  return <div className={`card border-0 shadow-sm rounded-4 ${className}`}><div className="card-body">{children}</div></div>
}

export function Toolbar({ children }) {
  return <div className="d-flex flex-wrap align-items-center gap-2 mb-3">{children}</div>
}

export function Select({ value, onChange, options, label, all = 'All' }) {
  return (
    <select className="form-select form-select-sm w-auto" aria-label={label} value={value} onChange={(e) => onChange(e.target.value)}>
      {all != null && <option value="">{label}: {all}</option>}
      {options.map((o) => (Array.isArray(o) ? <option key={o[0]} value={o[0]}>{o[1]}</option> : <option key={o} value={o}>{o}</option>))}
    </select>
  )
}

/** Uploads a CSV/Excel file to an import endpoint and reports row-level problems. */
export function ImportButton({ url, onDone, label = 'Import CSV / Excel', help }) {
  const input = useRef(null)
  const [busy, setBusy] = useState(false)
  const pick = async (e) => {
    const file = e.target.files[0]
    e.target.value = ''
    if (!file) return
    setBusy(true)
    try {
      const form = new FormData()
      form.append('file', file)
      const { data } = await api.post(url, form, { timeout: 120000 })
      const parts = [`${data.imported} imported`]
      if (data.updated) parts.push(`${data.updated} updated`)
      if (data.skipped) parts.push(`${data.skipped} already present`)
      toast(parts.join(' · '))
      onDone?.()
    } catch (err) {
      const errors = err?.response?.data?.errors
      if (Array.isArray(errors) && errors.length) {
        Swal.fire({
          icon: 'error', title: 'Nothing was imported', width: 640,
          html: `<p>${err.response.data.message}</p><ul style="text-align:left;max-height:260px;overflow:auto;font-size:.875rem">${errors.map((x) => `<li>${String(x).replace(/[<>&]/g, (c) => ({ '<': '&lt;', '>': '&gt;', '&': '&amp;' }[c]))}</li>`).join('')}</ul>`,
        })
      } else alertError(err)
    }
    setBusy(false)
  }
  return (
    <>
      <input ref={input} type="file" accept=".csv,.xlsx" hidden onChange={pick} />
      <button className="btn btn-sm btn-outline-success" disabled={busy} onClick={() => input.current.click()} title={help}>
        {busy ? <span className="spinner-border spinner-border-sm me-1" aria-hidden="true" /> : <i className="bi bi-upload me-1" aria-hidden="true" />}{label}
      </button>
    </>
  )
}

export function DownloadButton({ url, name, children, className = 'btn btn-sm btn-outline-secondary' }) {
  const [busy, setBusy] = useState(false)
  return (
    <button className={className} disabled={busy} onClick={async () => {
      setBusy(true)
      try { await download(url, name) } catch (e) { alertError(e, 'The file could not be downloaded.') }
      setBusy(false)
    }}>
      {busy ? <span className="spinner-border spinner-border-sm me-1" aria-hidden="true" /> : <i className="bi bi-download me-1" aria-hidden="true" />}{children}
    </button>
  )
}

/** Runs an API call for a button press: success toast, error dialog, then refresh. */
export async function run(action, success, after) {
  try {
    const result = await action()
    if (success) toast(success)
    after?.(result)
    return result
  } catch (e) {
    alertError(e)
    return null
  }
}

export const studentName = (r) => (r.student && r.student.trim()) || r.username

export { errorMessage }
