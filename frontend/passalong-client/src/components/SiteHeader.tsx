import { Link, useNavigate } from 'react-router-dom'
import { clearSession, getToken } from '../auth/session'

export default function SiteHeader() {
  const navigate = useNavigate()
  const signedIn = Boolean(getToken())

  const signOut = () => {
    clearSession()
    navigate('/')
  }

  return (
    <header>
      <Link className="brand" to="/">
        PASS<span>ALONG</span>
      </Link>
      <nav>
        <Link to="/">Explore</Link>
        <Link to="/wanted-items">Wanted</Link>
        <Link to="/community">Community</Link>
        {signedIn ? (
          <>
            <Link to="/my-posts">My posts</Link>
            <Link to="/dashboard">Dashboard</Link>
            <Link to="/create">Create</Link>
            <button onClick={signOut}>Sign out</button>
          </>
        ) : (
          <>
            <Link to="/login">Sign in</Link>
            <Link className="nav-button" to="/register">
              Join
            </Link>
          </>
        )}
      </nav>
    </header>
  )
}
