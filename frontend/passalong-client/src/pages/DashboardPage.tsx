import { useEffect, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { exchangeRequestsApi } from '../api/endpoints'
import type { ExchangeRequest } from '../api/types'
import ReviewDialog from '../components/ReviewDialog'
import { clearSession, getUser } from '../auth/session'
import { useRequireAuth } from '../hooks/useRequireAuth'
import { errorMessage, formatDate } from '../utils/format'
import '../styles/Dashboard.css'
import '../styles/components.css'

type Direction = 'incoming' | 'outgoing'

interface ReviewTarget {
  listingId: string
  listingTitle: string
  otherPersonName: string
}

export default function DashboardPage() {
  const navigate = useNavigate()
  const signedIn = useRequireAuth()
  const user = getUser()
  const [incoming, setIncoming] = useState<ExchangeRequest[]>([])
  const [outgoing, setOutgoing] = useState<ExchangeRequest[]>([])
  const [notice, setNotice] = useState('')
  const [error, setError] = useState('')
  const [reviewing, setReviewing] = useState<ReviewTarget | null>(null)

  const [version, setVersion] = useState(0)

  useEffect(() => {
    if (!signedIn) {
      return
    }

    Promise.all([exchangeRequestsApi.incoming(), exchangeRequestsApi.outgoing()])
      .then(([inbox, outbox]) => {
        setIncoming(inbox)
        setOutgoing(outbox)
      })
      .catch((e) => setError(errorMessage(e)))
  }, [signedIn, version])

  const run = async (action: () => Promise<unknown>, success: string) => {
    setError('')
    setNotice('')

    try {
      await action()
      setNotice(success)
    } catch (e) {
      setError(errorMessage(e))
    }

    setVersion((value) => value + 1)
  }

  const signOut = () => {
    clearSession()
    navigate('/')
  }

  const renderRequest = (request: ExchangeRequest, direction: Direction) => {
    const isIncoming = direction === 'incoming'
    const other = isIncoming ? request.requester : request.owner
    const myConfirmed = isIncoming ? request.ownerConfirmed : request.requesterConfirmed

    return (
      <div className="request" key={request.id}>
        <div className="who">
          <b>
            <Link to={`/listings/${request.listingId}`}>{request.listing.title}</Link>
          </b>
          <small>
            {isIncoming ? 'From' : 'To'}{' '}
            {other ? <Link to={`/users/${other.id}`}>{other.displayName}</Link> : 'a member'} ·{' '}
            {formatDate(request.createdAt)}
          </small>
          {request.message && <small>“{request.message}”</small>}
          {request.offeredListingId && (
            <small>
              Swap offer: <Link to={`/listings/${request.offeredListingId}`}>view offered item</Link>
            </small>
          )}
        </div>
        <span>{request.status}</span>

        <div className="actions">
          {request.status === 'Pending' && isIncoming && (
            <>
              <button onClick={() => run(() => exchangeRequestsApi.accept(request.id), 'Request accepted.')}>
                Accept
              </button>
              <button
                className="secondary"
                onClick={() => run(() => exchangeRequestsApi.reject(request.id), 'Request rejected.')}
              >
                Reject
              </button>
            </>
          )}

          {request.status === 'Pending' && !isIncoming && (
            <button
              className="secondary"
              onClick={() => run(() => exchangeRequestsApi.cancel(request.id), 'Request cancelled.')}
            >
              Cancel request
            </button>
          )}

          {request.status === 'Accepted' &&
            (myConfirmed ? (
              <span className="waiting">You confirmed the handover. Waiting for the other person.</span>
            ) : (
              <button
                onClick={() =>
                  run(() => exchangeRequestsApi.confirmHandover(request.id), 'Handover confirmed.')
                }
              >
                Confirm handover
              </button>
            ))}

          {request.status === 'Completed' && (
            <button
              className="secondary"
              onClick={() =>
                setReviewing({
                  listingId: request.listingId,
                  listingTitle: request.listing.title,
                  otherPersonName: other?.displayName ?? 'this member',
                })
              }
            >
              Leave a review
            </button>
          )}
        </div>
      </div>
    )
  }

  const renderPanel = (title: string, requests: ExchangeRequest[], direction: Direction) => (
    <div className="dash-card">
      <h2>{title}</h2>
      {requests.length ? requests.map((request) => renderRequest(request, direction)) : <p>Nothing here yet.</p>}
    </div>
  )

  return (
    <main className="dash">
      <aside>
        <Link className="brand" to="/">
          PASS<span>ALONG</span>
        </Link>
        <p>MEMBER SPACE</p>
        <Link to="/dashboard">Dashboard</Link>
        <Link to="/my-posts">My posts</Link>
        <Link to="/create">Create a listing</Link>
        <Link to="/wanted-items">Wanted items</Link>
        <Link to="/community">Community</Link>
        {user && <Link to={`/users/${user.id}`}>My profile</Link>}
        <Link to="/">Explore items</Link>
        <button onClick={signOut}>Sign out</button>
      </aside>
      <section>
        <p className="eyebrow">YOUR COMMUNITY ACTIVITY</p>
        <h1>Dashboard</h1>
        <p className="sub">Manage your exchanges and keep useful things moving.</p>
        {notice && <p className="notice">{notice}</p>}
        {error && <p className="error-text">{error}</p>}
        <div className="dash-grid">
          {renderPanel('Incoming requests', incoming, 'incoming')}
          {renderPanel('Outgoing requests', outgoing, 'outgoing')}
        </div>
      </section>

      {reviewing && (
        <ReviewDialog
          {...reviewing}
          onClose={() => setReviewing(null)}
          onDone={(message) => {
            setNotice(message)
            setError('')
          }}
        />
      )}
    </main>
  )
}
