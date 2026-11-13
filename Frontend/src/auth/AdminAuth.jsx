import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react'
import api, { onSessionLost, refreshSession, setToken } from '../api/client'

const AdminAuthContext = createContext(null)
export const useAdminAuth = () => useContext(AdminAuthContext)

export function AdminAuthProvider({ children }) {
  const [state, setState] = useState({ status: 'loading', profile: null })

  useEffect(() => {
    let cancelled = false
    refreshSession('admin').then((data) => {
      if (cancelled) return
      setState(data ? { status: 'authed', profile: data.profile } : { status: 'anon', profile: null })
    })
    const off = onSessionLost('admin', () => {
      setToken('admin', null)
      setState({ status: 'anon', profile: null })
    })
    return () => { cancelled = true; off() }
  }, [])

  const login = useCallback(async (username, password) => {
    const { data } = await api.post('/admin/auth/login', { username, password })
    setToken('admin', data.token)
    setState({ status: 'authed', profile: data.profile })
    return data.profile
  }, [])

  const logout = useCallback(async () => {
    try { await api.post('/admin/auth/logout') } catch { /* already gone */ }
    setToken('admin', null)
    setState({ status: 'anon', profile: null })
  }, [])

  const reload = useCallback(async () => {
    const { data } = await api.get('/admin/auth/me')
    setState({ status: 'authed', profile: data.profile })
  }, [])

  const value = useMemo(() => {
    const permissions = new Set(state.profile?.permissions || [])
    // Hides what the admin cannot use. The server enforces the same permissions on every request.
    return { ...state, login, logout, reload, can: (p) => permissions.has(p) }
  }, [state, login, logout, reload])

  return <AdminAuthContext.Provider value={value}>{children}</AdminAuthContext.Provider>
}
