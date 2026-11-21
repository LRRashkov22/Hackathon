import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { wantedItemsApi } from '../api/endpoints'
import type { WantedItem, WantedMatch } from '../api/types'
import SiteHeader from '../components/SiteHeader'
import { useRequireAuth } from '../hooks/useRequireAuth'
import { conditionLabel, errorMessage, exchangeTypeLabel } from '../utils/format'
import '../styles/FeaturePages.css'
import '../styles/components.css'

export default function WantedPage() {
  const signedIn = useRequireAuth()
  const [items, setItems] = useState<WantedItem[] | null>(null)
  const [matches, setMatches] = useState<WantedMatch[] | null>(null)
  const [selectedId, setSelectedId] = useState('')
  const [error, setError] = useState('')

  useEffect(() => {
    if (signedIn) {
      wantedItemsApi.mine().then(setItems).catch((e) => setError(errorMessage(e)))
    }
  }, [signedIn])

  const showMatches = async (id: string) => {
    setSelectedId(id)
    setMatches(null)
    setError('')

    try {
      setMatches(await wantedItemsApi.matches(id))
    } catch (e) {
      setError(errorMessage(e))
    }
  }

  return (
    <main className="feature">
      <SiteHeader />
      <section>
        <p className="eyebrow">SMART NEED MATCHING</p>
        <h1>Wanted items</h1>
        <p className="lead">
          These are the things you are looking for. Select one to see ranked local matches.
        </p>
        {error && <p className="error-text">{error}</p>}

        <div className="feature-grid">
          <div className="panel">
            <h2>My wanted items</h2>
            {items && items.length === 0 && (
              <p>
                No wanted items yet. <Link to="/create">Create one</Link>.
              </p>
            )}
            {items?.map((item) => (
              <button
                className={selectedId === item.id ? 'chosen' : ''}
                key={item.id}
                onClick={() => showMatches(item.id)}
              >
                <b>{item.title}</b>
                <span>
                  {item.category.name} · {item.location}
                  {item.preferredCondition && ` · ${conditionLabel(item.preferredCondition)}`}
                </span>
              </button>
            ))}
          </div>

          <div className="panel">
            <h2>Suggested matches</h2>
            {!selectedId && <p>Select a wanted item to discover matches.</p>}
            {selectedId && matches === null && !error && <p>Looking for matches…</p>}
            {matches && matches.length === 0 && <p>No matches yet. Check back when new items are listed.</p>}
            {matches?.map((match) => (
              <div className="match" key={match.listing.id}>
                <span className="score">{match.score}% match</span>
                <b>
                  <Link to={`/listings/${match.listing.id}`}>{match.listing.title}</Link>
                </b>
                <p>{match.listing.description}</p>
                <small>
                  {match.listing.location} · {exchangeTypeLabel(match.listing.exchangeType)} ·{' '}
                  {conditionLabel(match.listing.condition)} · {match.listing.owner}
                </small>
              </div>
            ))}
          </div>
        </div>
      </section>
    </main>
  )
}
