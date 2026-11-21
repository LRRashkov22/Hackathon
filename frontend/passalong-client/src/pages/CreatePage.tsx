import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import { categoriesApi, listingsApi, wantedItemsApi } from '../api/endpoints'
import type { Category, ExchangeType, ListingCondition } from '../api/types'
import { LISTING_CONDITIONS } from '../api/types'
import SiteHeader from '../components/SiteHeader'
import { useRequireAuth } from '../hooks/useRequireAuth'
import { conditionLabel, errorMessage, PLACEHOLDER_IMAGE } from '../utils/format'
import '../styles/CreatePage.css'
import '../styles/components.css'

type Kind = 'listing' | 'wanted'

const NO_PREFERENCE = ''

export default function CreatePage() {
  const navigate = useNavigate()
  const signedIn = useRequireAuth()
  const [categories, setCategories] = useState<Category[]>([])
  const [kind, setKind] = useState<Kind>('listing')
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [categoryId, setCategoryId] = useState('')
  const [location, setLocation] = useState('')
  const [condition, setCondition] = useState<ListingCondition | typeof NO_PREFERENCE>('Good')
  const [exchangeType, setExchangeType] = useState<ExchangeType>('GiveAway')
  const [desiredSwap, setDesiredSwap] = useState('')
  const [imageUrl, setImageUrl] = useState('')
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    if (signedIn) {
      categoriesApi.list().then(setCategories).catch((e) => setError(errorMessage(e)))
    }
  }, [signedIn])

  const switchKind = (next: Kind) => {
    setKind(next)
    // A listing always needs a condition; a wanted item may have none.
    setCondition(next === 'listing' && condition === NO_PREFERENCE ? 'Good' : condition)
  }

  const submit = async (event: FormEvent) => {
    event.preventDefault()
    setError('')
    setBusy(true)

    try {
      if (kind === 'listing') {
        const created = await listingsApi.create({
          categoryId,
          title,
          description,
          condition: condition || 'Good',
          exchangeType,
          desiredSwap: exchangeType === 'Swap' ? desiredSwap : null,
          location,
          imageUrls: [imageUrl.trim() || PLACEHOLDER_IMAGE],
        })

        navigate(`/listings/${created.id}`)
      } else {
        await wantedItemsApi.create({
          categoryId,
          title,
          description,
          preferredCondition: condition || null,
          location,
        })

        navigate('/wanted-items')
      }
    } catch (e) {
      setError(errorMessage(e))
      setBusy(false)
    }
  }

  return (
    <main className="create">
      <SiteHeader />
      <section>
        <p className="eyebrow">CONTRIBUTE TO THE LOOP</p>
        <h1>What would you like to share?</h1>
        <div className="tabs">
          <button className={kind === 'listing' ? 'active' : ''} onClick={() => switchKind('listing')}>
            List an item
          </button>
          <button className={kind === 'wanted' ? 'active' : ''} onClick={() => switchKind('wanted')}>
            Post a wanted item
          </button>
        </div>

        <form onSubmit={submit}>
          <label>
            Title
            <input required value={title} onChange={(e) => setTitle(e.target.value)} />
          </label>
          <label>
            Description
            <textarea required value={description} onChange={(e) => setDescription(e.target.value)} />
          </label>
          <div className="two">
            <label>
              Category
              <select required value={categoryId} onChange={(e) => setCategoryId(e.target.value)}>
                <option value="">Choose a category</option>
                {categories.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.name}
                  </option>
                ))}
              </select>
            </label>
            <label>
              Location
              <input required value={location} onChange={(e) => setLocation(e.target.value)} />
            </label>
          </div>
          <div className="two">
            <label>
              {kind === 'listing' ? 'Condition' : 'Preferred condition'}
              <select
                value={condition}
                onChange={(e) => setCondition(e.target.value as ListingCondition | typeof NO_PREFERENCE)}
              >
                {kind === 'wanted' && <option value={NO_PREFERENCE}>No preference</option>}
                {LISTING_CONDITIONS.map((value) => (
                  <option key={value} value={value}>
                    {conditionLabel(value)}
                  </option>
                ))}
              </select>
            </label>
            {kind === 'listing' && (
              <label>
                Exchange type
                <select value={exchangeType} onChange={(e) => setExchangeType(e.target.value as ExchangeType)}>
                  <option value="GiveAway">Give away</option>
                  <option value="Swap">Swap</option>
                </select>
              </label>
            )}
          </div>
          {kind === 'listing' && exchangeType === 'Swap' && (
            <label>
              What do you want in return?
              <textarea
                required
                value={desiredSwap}
                onChange={(e) => setDesiredSwap(e.target.value)}
                placeholder="Example: a working wireless keyboard, desk lamp, or another monitor"
              />
            </label>
          )}
          {kind === 'listing' && (
            <label>
              Photo URL (optional)
              <input
                type="url"
                value={imageUrl}
                onChange={(e) => setImageUrl(e.target.value)}
                placeholder="https://…"
              />
            </label>
          )}
          <button className="submit" disabled={busy}>
            {kind === 'listing' ? 'Publish item' : 'Publish wanted item'}
          </button>
          {error && <p className="error-text">{error}</p>}
        </form>
      </section>
    </main>
  )
}
