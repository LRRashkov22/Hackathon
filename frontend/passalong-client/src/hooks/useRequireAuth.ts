import { useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { getToken } from '../auth/session'

/** Redirects to the login page when nobody is signed in. Returns whether a session exists. */
export function useRequireAuth(): boolean {
  const navigate = useNavigate()
  const signedIn = Boolean(getToken())

  useEffect(() => {
    if (!signedIn) {
      navigate('/login', { replace: true })
    }
  }, [signedIn, navigate])

  return signedIn
}
