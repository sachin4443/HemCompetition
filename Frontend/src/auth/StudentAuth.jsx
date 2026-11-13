import { createContext, useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react'
import api, { onSessionLost, refreshSession, setToken } from '../api/client'

const StudentAuthContext = createContext(null)
export const useStudentAuth = () => useContext(StudentAuthContext)

const HEARTBEAT_MS = 30000

export function StudentAuthProvider({ children }) {
  // loading: checking for an existing session; anon: signed out; authed: signed in
  const [state, setState] = useState({ status: 'loading', profile: null })
  // Shown on the login page after an involuntary sign-out.
  const [notice, setNotice] = useState(null)
  const statusRef = useRef('loading')
  useEffect(() => { statusRef.current = state.status }, [state.status])

  const signOutLocally = useCallback((message) => {
    setToken('student', null)
    if (statusRef.current === 'authed' && message) setNotice(message)
    setState({ status: 'anon', profile: null })
  }, [])

  useEffect(() => {
    let cancelled = false
    refreshSession('student').then((data) => {
      if (cancelled) return
      setState(data ? { status: 'authed', profile: data.profile } : { status: 'anon', profile: null })
    })
    const off = onSessionLost('student', (body) =>
      signOutLocally(body?.message || 'Your session has ended. Please sign in again.'))
    return () => { cancelled = true; off() }
  }, [signOutLocally])

  // Keeps the server-side session alive and notices quickly when it has been ended elsewhere.
  useEffect(() => {
    if (state.status !== 'authed') return undefined
    const beat = () => api.post('/auth/heartbeat').catch(() => {})
    const timer = setInterval(beat, HEARTBEAT_MS)
    return () => clearInterval(timer)
  }, [state.status])

  const login = useCallback(async (usernameOrEmail, password, terminateExisting = false) => {
    const { data } = await api.post('/auth/login', { usernameOrEmail, password, terminateExisting })
    setToken('student', data.token)
    setNotice(null)
    setState({ status: 'authed', profile: data.profile })
    return data
  }, [])

  const logout = useCallback(async () => {
    try { await api.post('/proctoring/end') } catch { /* best effort */ }
    try { await api.post('/auth/logout') } catch { /* already gone */ }
    setToken('student', null)
    setState({ status: 'anon', profile: null })
  }, [])

  const value = useMemo(
    () => ({ ...state, login, logout, signOutLocally, notice, clearNotice: () => setNotice(null) }),
    [state, login, logout, signOutLocally, notice],
  )
  return <StudentAuthContext.Provider value={value}>{children}</StudentAuthContext.Provider>
}
