import axios from 'axios'

/*
 * One HTTP client for the whole app.
 *
 * Access tokens live only in this module's memory - never in localStorage or sessionStorage - so
 * a script injected into the page cannot read them from storage. The refresh token is an HttpOnly
 * cookie the browser sends only to the refresh endpoint; this code never sees it.
 */

const tokens = { student: null, admin: null }
const listeners = { student: new Set(), admin: new Set() }

export const realmOf = (url = '') => (url.toLowerCase().startsWith('/admin') ? 'admin' : 'student')
export const setToken = (realm, token) => { tokens[realm] = token }
export const getToken = (realm) => tokens[realm]

/** Called when a realm's session is definitely over (refresh failed). */
export const onSessionLost = (realm, fn) => {
  listeners[realm].add(fn)
  return () => listeners[realm].delete(fn)
}

const api = axios.create({
  baseURL: '/api',
  timeout: 20000,
  withCredentials: true,
  // Required by the refresh endpoints; a cross-site form cannot send it.
  headers: { 'X-Requested-With': 'XMLHttpRequest' },
})

api.interceptors.request.use((config) => {
  // `appealToken` overrides the session token: used for appeals by a restricted student and for
  // single-event evidence uploads.
  const token = config.appealToken || tokens[realmOf(config.url)]
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

const REFRESH_URL = { student: '/auth/refresh', admin: '/admin/auth/refresh' }
const inFlight = { student: null, admin: null }

/** Exchanges the refresh cookie for a new access token. Returns the response body or null. */
export function refreshSession(realm) {
  if (!inFlight[realm]) {
    const run = async () => {
      try {
        const { data } = await axios.post('/api' + REFRESH_URL[realm], null, {
          withCredentials: true,
          headers: { 'X-Requested-With': 'XMLHttpRequest' },
          timeout: 15000,
        })
        tokens[realm] = data.token
        return data
      } catch {
        tokens[realm] = null
        return null
      }
    }
    // The refresh cookie is single-use, so two tabs must not refresh at the same moment.
    const locked = navigator.locks
      ? navigator.locks.request(`hc-refresh-${realm}`, run)
      : run()
    inFlight[realm] = locked.finally(() => { inFlight[realm] = null })
  }
  return inFlight[realm]
}

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const { config, response } = error
    const url = config?.url || ''
    const isAuthCall = /\/auth\/(login|refresh|logout)$/.test(url)
    if (response?.status === 401 && config && !config._retried && !isAuthCall && !config.appealToken) {
      const realm = realmOf(url)
      config._retried = true
      const refreshed = await refreshSession(realm)
      if (refreshed) return api(config)
      listeners[realm].forEach((fn) => fn(response.data))
    }
    return Promise.reject(error)
  },
)

/** A message suitable for showing to the user. */
export function errorMessage(error, fallback = 'Something went wrong. Please try again.') {
  if (error?.response?.data?.message) return error.response.data.message
  if (error?.code === 'ECONNABORTED' || !error?.response) return 'Cannot reach the server. Check your internet connection.'
  return fallback
}

export const errorCode = (error) => error?.response?.data?.code

/** Downloads a file from an authenticated endpoint. */
export async function download(url, fallbackName = 'download') {
  const response = await api.get(url, { responseType: 'blob' })
  const match = /filename="?([^";]+)"?/.exec(response.headers['content-disposition'] || '')
  const link = document.createElement('a')
  link.href = URL.createObjectURL(response.data)
  link.download = match ? match[1] : fallbackName
  document.body.appendChild(link)
  link.click()
  link.remove()
  setTimeout(() => URL.revokeObjectURL(link.href), 1000)
}

export default api
