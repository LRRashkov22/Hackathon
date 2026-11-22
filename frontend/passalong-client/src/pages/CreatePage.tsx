import { useEffect, useMemo, useState } from 'react'
import type { FormEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import { categoriesApi, imagesApi, listingsApi, wantedItemsApi } from '../api/endpoints'
import type { Category, ExchangeType, ListingCondition } from '../api/types'
import { LISTING_CONDITIONS } from '../api/types'
import SiteHeader from '../components/SiteHeader'
import { useRequireAuth } from '../hooks/useRequireAuth'
import { conditionLabel, errorMessage, PLACEHOLDER_IMAGE } from '../utils/format'
import '../styles/CreatePage.css'
import '../styles/components.css'

type Kind = 'listing' | 'wanted'

const NO_PREFERENCE = ''
const MAX_PHOTOS = 6
const MAX_PHOTO_BYTES = 5 * 1024 * 1024

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
  const [photos, setPhotos] = useState<File[]>([])
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    if (signedIn) {
      categoriesApi.list().then(setCategories).catch((e) => setError(errorMessage(e)))
    }
  }, [signedIn])

  const previews = useMemo(() => photos.map((file) => URL.createObjectURL(file)), [photos])

  useEffect(() => () => previews.forEach((url) => URL.revokeObjectURL(url)), [previews])

  const addPhotos = (selected: FileList | null) => {
    const files = Array.from(selected ?? [])
    const tooBig = files.find((file) => file.size > MAX_PHOTO_BYTES)

    if (tooBig) {
      setError(`"${tooBig.name}" is larger than 5 MB.`)
      return
    }

    setError('')
    setPhotos((current) => [...current, ...files].slice(0, MAX_PHOTOS))
  }

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
        const uploaded = photos.length > 0 ? (await imagesApi.upload(photos)).urls : []
        const created = await listingsApi.create({
          categoryId,
          title,
          description,
          condition: condition || 'Good',
          exchangeType,
          desiredSwap: exchangeType === 'Swap' ? desiredSwap : null,
          location,
          imageUrls: uploaded.length > 0 ? uploaded : [PLACEHOLDER_IMAGE],
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
            <div className="photos">
              <label>
                Photos (optional, up to {MAX_PHOTOS}, 5 MB each)
                <input
                  type="file"
                  accept="image/jpeg,image/png,image/webp,image/gif"
                  multiple
                  disabled={photos.length >= MAX_PHOTOS}
                  onChange={(e) => {
                    addPhotos(e.target.files)
                    e.target.value = ''
                  }}
                />
              </label>
              {previews.length > 0 && (
                <ul className="photo-previews">
                  {previews.map((src, index) => (
                    <li key={src}>
                      <img src={src} alt={`Photo ${index + 1}`} />
                      <button
                        type="button"
                        aria-label={`Remove photo ${index + 1}`}
                        onClick={() => setPhotos((current) => current.filter((_, i) => i !== index))}
                      >
                        ×
                      </button>
                    </li>
                  ))}
                </ul>
              )}
            </div>
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
