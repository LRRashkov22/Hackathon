import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { usersApi } from '../api/endpoints'
import type { UserProfile } from '../api/types'
import SiteHeader from '../components/SiteHeader'
import { errorMessage, formatDate } from '../utils/format'
import '../styles/FeaturePages.css'
import '../styles/components.css'

export default function UserProfilePage() {
  const { id = '' } = useParams()
  const [loaded, setLoaded] = useState<UserProfile | null>(null)
  const [failure, setFailure] = useState<{ id: string; message: string } | null>(null)

  useEffect(() => {
    usersApi
      .get(id)
      .then(setLoaded)
      .catch((e) => setFailure({ id, message: errorMessage(e) }))
  }, [id])

  // Ignore results that belong to a previously viewed profile.
  const profile = loaded?.id === id ? loaded : null
  const error = failure?.id === id ? failure.message : ''

  return (
    <main className="feature">
      <SiteHeader />
      <section>
        {error && <p className="error-text">{error}</p>}
        {!profile && !error && <p>Loading…</p>}

        {profile && (
          <>
            <div className="profile-head">
              <div className="avatar">
                {profile.avatarUrl ? (
                  <img src={profile.avatarUrl} alt="" />
                ) : (
                  profile.displayName.charAt(0).toUpperCase()
                )}
              </div>
              <div>
                <p className="eyebrow">COMMUNITY MEMBER</p>
                <h1 style={{ margin: 0 }}>{profile.displayName}</h1>
                <span className="reputation">
                  {profile.reputationScore} reputation{profile.location && ` · ${profile.location}`}
                </span>
              </div>
            </div>
            {profile.bio && <p className="lead">{profile.bio}</p>}

            <div className="feature-grid">
              <div className="panel">
                <h2>Listings</h2>
                {profile.listings.length === 0 && <p className="muted">No listings yet.</p>}
                <ul className="links">
                  {profile.listings.map((listing) => (
                    <li key={listing.id}>
                      <Link to={`/listings/${listing.id}`}>{listing.title}</Link>
                      <span className={`badge status-${listing.status}`}>{listing.status}</span>
                    </li>
                  ))}
                </ul>
              </div>

              <div className="panel">
                <h2>Reviews</h2>
                {profile.reviews.length === 0 && <p className="muted">No reviews yet.</p>}
                {profile.reviews.map((review, index) => (
                  <div className="review" key={index}>
                    <span className="stars">
                      {'★'.repeat(review.rating)}
                      {'☆'.repeat(5 - review.rating)}
                    </span>
                    {review.comment && <p>{review.comment}</p>}
                    <small>{formatDate(review.createdAt)}</small>
                  </div>
                ))}
              </div>
            </div>
          </>
        )}
      </section>
    </main>
  )
}
