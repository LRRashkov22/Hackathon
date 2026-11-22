import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { communityApi, listingsApi } from '../api/endpoints'
import type { Listing, TransferHistoryEntry } from '../api/types'
import RequestDialog from '../components/RequestDialog'
import SiteHeader from '../components/SiteHeader'
import { getToken, getUser } from '../auth/session'
import {
  conditionLabel,
  errorMessage,
  exchangeTypeLabel,
  formatDate,
  imageSrc,
  PLACEHOLDER_IMAGE,
} from '../utils/format'
import '../styles/FeaturePages.css'
import '../styles/components.css'

export default function ListingDetailPage() {
  const { id = '' } = useParams()
  const user = getUser()
  const [loaded, setLoaded] = useState<{ listing: Listing; history: TransferHistoryEntry[] } | null>(null)
  const [failure, setFailure] = useState<{ id: string; message: string } | null>(null)
  const [imageIndex, setImageIndex] = useState(0)
  const [requesting, setRequesting] = useState(false)
  const [notice, setNotice] = useState('')

  useEffect(() => {
    Promise.all([listingsApi.get(id), communityApi.itemHistory(id)])
      .then(([listing, history]) => {
        setLoaded({ listing, history })
        setImageIndex(0)
      })
      .catch((e) => setFailure({ id, message: errorMessage(e) }))
  }, [id])

  // Ignore results that belong to a previously viewed listing.
  const current = loaded?.listing.id === id ? loaded : null
  const listing = current?.listing ?? null
  const history = current?.history ?? []
  const error = failure?.id === id ? failure.message : ''

  const isOwner = Boolean(listing && user && listing.ownerId === user.id)
  const canRequest = Boolean(listing && listing.status === 'Available' && !isOwner)
  const images = listing?.images.length ? listing.images.map((image) => imageSrc(image.imageUrl)) : [PLACEHOLDER_IMAGE]

  const startRequest = () => {
    if (!getToken()) {
      window.location.assign('/login')
      return
    }

    setRequesting(true)
  }

  return (
    <main className="feature">
      <SiteHeader />
      <section>
        <Link to="/">← Back to explore</Link>
        {error && <p className="error-text">{error}</p>}
        {!listing && !error && <p>Loading…</p>}

        {listing && (
          <div className="detail">
            <div className="gallery">
              <img src={images[imageIndex]} alt={listing.title} />
              {images.length > 1 && (
                <div className="thumbs">
                  {images.map((src, index) => (
                    <img
                      key={src}
                      src={src}
                      alt=""
                      className={index === imageIndex ? 'active' : ''}
                      onClick={() => setImageIndex(index)}
                    />
                  ))}
                </div>
              )}
            </div>

            <div>
              <p className="eyebrow">{exchangeTypeLabel(listing.exchangeType).toUpperCase()}</p>
              <h1>{listing.title}</h1>
              <div className="badges">
                <span className={`badge status-${listing.status}`}>{listing.status}</span>
                <span className="badge">{listing.category.name}</span>
                <span className="badge">{conditionLabel(listing.condition)}</span>
              </div>
              <p>{listing.description}</p>

              <dl>
                <dt>Location</dt>
                <dd>{listing.location}</dd>
                <dt>Listed</dt>
                <dd>{formatDate(listing.createdAt)}</dd>
                {listing.owner && (
                  <>
                    <dt>Owner</dt>
                    <dd>
                      <Link to={`/users/${listing.owner.id}`}>{listing.owner.displayName}</Link>
                    </dd>
                  </>
                )}
                {listing.desiredSwap && (
                  <>
                    <dt>Wants in return</dt>
                    <dd>{listing.desiredSwap}</dd>
                  </>
                )}
              </dl>

              {notice && <p className="notice">{notice}</p>}
              {isOwner && <p className="muted">This is your listing.</p>}
              {canRequest && <button onClick={startRequest}>Request item</button>}
              {!canRequest && !isOwner && <p className="muted">This item is no longer available.</p>}

              <div className="panel" style={{ marginTop: 28 }}>
                <h2>Ownership history</h2>
                {history.length === 0 ? (
                  <p className="muted">This item has not changed hands yet.</p>
                ) : (
                  <ul className="history">
                    {history.map((entry) => (
                      <li key={entry.id}>
                        <b>
                          {entry.previousOwner} → {entry.newOwner}
                        </b>
                        <span>{formatDate(entry.completedAt)}</span>
                      </li>
                    ))}
                  </ul>
                )}
              </div>
            </div>
          </div>
        )}
      </section>

      {requesting && listing && (
        <RequestDialog
          listing={listing}
          onClose={() => setRequesting(false)}
          onSent={setNotice}
        />
      )}
    </main>
  )
}
