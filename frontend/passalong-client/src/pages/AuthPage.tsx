import { useState } from 'react'
import type { FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { authApi } from '../api/endpoints'
import { saveSession } from '../auth/session'
import { errorMessage } from '../utils/format'
import '../styles/AuthPage.css'
import '../styles/components.css'

export default function AuthPage({ mode }: { mode: 'login' | 'register' }) {
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [displayName, setDisplayName] = useState('')
  const [location, setLocation] = useState('')
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  const isLogin = mode === 'login'

  const submit = async (event: FormEvent) => {
    event.preventDefault()
    setError('')
    setBusy(true)

    try {
      const response = isLogin
        ? await authApi.login({ email, password })
        : await authApi.register({ email, password, displayName, location: location.trim() || null })

      saveSession(response)
      navigate('/')
    } catch (e) {
      setError(isLogin ? 'Incorrect email or password.' : errorMessage(e))
      setBusy(false)
    }
  }

  return (
    <main className="auth">
      <Link to="/" className="auth-brand">
        PASS<span>ALONG</span>
      </Link>
      <section>
        <div className="auth-copy">
          <p>COMMUNITY EXCHANGE PLATFORM</p>
          <h1>{isLogin ? 'Welcome back to the loop.' : 'Give useful things a second life.'}</h1>
          <span>No prices. No payments. Just people passing useful things along.</span>
        </div>
        <form onSubmit={submit}>
          <h2>{isLogin ? 'Sign in' : 'Create your account'}</h2>
          {!isLogin && (
            <>
              <input
                required
                value={displayName}
                onChange={(e) => setDisplayName(e.target.value)}
                placeholder="Display name"
              />
              <input value={location} onChange={(e) => setLocation(e.target.value)} placeholder="City" />
            </>
          )}
          <input
            required
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="Email address"
          />
          <input
            required
            minLength={8}
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Password"
          />
          <button disabled={busy}>{isLogin ? 'Sign in' : 'Create account'}</button>
          {error && <small className="error">{error}</small>}
          <p>
            {isLogin ? (
              <>
                New here? <Link to="/register">Create an account</Link>
              </>
            ) : (
              <>
                Already a member? <Link to="/login">Sign in</Link>
              </>
            )}
          </p>
          {isLogin && <small>Demo: maria@passalong.local / Passalong123!</small>}
        </form>
      </section>
    </main>
  )
}
