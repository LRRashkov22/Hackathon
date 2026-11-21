import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { listingsApi } from '../api/endpoints'
import type { Listing } from '../api/types'
import SiteHeader from '../components/SiteHeader'
import { useRequireAuth } from '../hooks/useRequireAuth'
import { conditionLabel, errorMessage, exchangeTypeLabel, PLACEHOLDER_IMAGE } from '../utils/format'
import '../styles/FeaturePages.css'
import '../styles/App.css'
import '../styles/components.css'

export default function MyPostsPage() {
  const signedIn = useRequireAuth()
  const [items, setItems] = useState<Listing[] | null>(null)
  const [error, setError] = useState('')

  useEffect(() => {
    if (signedIn) {
      listingsApi.mine().then(setItems).catch((e) => setError(errorMessage(e)))
    }
  }, [signedIn])

  return (
    <main className="feature">
      <SiteHeader />
      <section>
        <p className="eyebrow">YOUR CONTRIBUTIONS</p>
        <h1>My posts</h1>
        <p className="lead">
          Your listings are kept separate from Explore, where you discover other people’s items.
        </p>
        {error && <p className="error-text">{error}</p>}

        <div className="grid">
          {items?.map((item) => (
            <article key={item.id}>
              <Link to={`/listings/${item.id}`}>
                <img src={item.images[0]?.imageUrl ?? PLACEHOLDER_IMAGE} alt="" />
              </Link>
              <small>
                {exchangeTypeLabel(item.exchangeType)} · {item.status}
              </small>
              <h3>
                <Link className="card-link" to={`/listings/${item.id}`}>
                  {item.title}
                </Link>
              </h3>
              <p>{item.description}</p>
              <footer>
                {item.location}
                <b>{conditionLabel(item.condition)}</b>
              </footer>
            </article>
          ))}
        </div>

        {items && items.length === 0 && (
          <div className="panel">
            <p>
              You have not published anything yet. <Link to="/create">Create your first listing</Link>.
            </p>
          </div>
        )}
      </section>
    </main>
  )
}
