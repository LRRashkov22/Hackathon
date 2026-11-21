import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { categoriesApi, listingsApi } from '../api/endpoints'
import type { BrowseParams, Category, ExchangeType, ListingCondition, ListingPage } from '../api/types'
import { EXCHANGE_TYPES, LISTING_CONDITIONS } from '../api/types'
import RequestDialog from '../components/RequestDialog'
import SiteHeader from '../components/SiteHeader'
import { getToken } from '../auth/session'
import { conditionLabel, errorMessage, exchangeTypeLabel, PLACEHOLDER_IMAGE } from '../utils/format'
import '../styles/App.css'
import '../styles/components.css'

const PAGE_SIZE = 9

type SelectedListing = ListingPage['items'][number]

export default function ExplorePage() {
  const navigate = useNavigate()
  const [categories, setCategories] = useState<Category[]>([])
  const [result, setResult] = useState<ListingPage | null>(null)
  const [search, setSearch] = useState('')
  const [appliedSearch, setAppliedSearch] = useState('')
  const [categoryId, setCategoryId] = useState('')
  const [exchangeType, setExchangeType] = useState<ExchangeType | ''>('')
  const [condition, setCondition] = useState<ListingCondition | ''>('')
  const [page, setPage] = useState(1)
  const [selected, setSelected] = useState<SelectedListing | null>(null)
  const [notice, setNotice] = useState('')
  const [error, setError] = useState('')

  useEffect(() => {
    categoriesApi.list().then(setCategories).catch((e) => setError(errorMessage(e)))
  }, [])

  useEffect(() => {
    const params: BrowseParams = {
      search: appliedSearch,
      categoryId,
      exchangeType: exchangeType || undefined,
      condition: condition || undefined,
      page,
      pageSize: PAGE_SIZE,
    }

    listingsApi
      .browse(params)
      .then((data) => {
        setResult(data)
        setError('')
      })
      .catch((e) => setError(errorMessage(e)))
  }, [appliedSearch, categoryId, exchangeType, condition, page])

  const submitSearch = (event: FormEvent) => {
    event.preventDefault()
    setAppliedSearch(search.trim())
    setPage(1)
  }

  const changeFilter = (apply: () => void) => {
    apply()
    setPage(1)
  }

  const requestItem = (item: SelectedListing) => {
    if (!getToken()) {
      navigate('/login')
      return
    }

    setSelected(item)
  }

  const totalPages = result ? Math.max(1, Math.ceil(result.total / result.pageSize)) : 1

  return (
    <>
      <SiteHeader />
      <main>
        <section className="hero">
          <div>
            <label>COMMUNITY EXCHANGE PLATFORM</label>
            <h1>
              Your unused things could <i>change someone’s day.</i>
            </h1>
            <p>
              Give useful items a second life. No prices, no payments — just neighbours helping neighbours.
            </p>
            <a className="primary" href="#explore">
              Explore items →
            </a>
          </div>
          <div className="loop">
            ↻
            <small>
              Every transfer
              <br />
              starts a story
            </small>
          </div>
        </section>

        <section id="explore">
          <label>FIND YOUR NEXT THING</label>
          <h2>Ready for a new home</h2>

          <form className="filters" onSubmit={submitSearch}>
            <input
              type="search"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Search monitors, books, bikes…"
            />
            <select value={categoryId} onChange={(e) => changeFilter(() => setCategoryId(e.target.value))}>
              <option value="">All categories</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>
            <select
              value={exchangeType}
              onChange={(e) => changeFilter(() => setExchangeType(e.target.value as ExchangeType | ''))}
            >
              <option value="">Give away & swap</option>
              {EXCHANGE_TYPES.map((type) => (
                <option key={type} value={type}>
                  {exchangeTypeLabel(type)}
                </option>
              ))}
            </select>
            <select
              value={condition}
              onChange={(e) => changeFilter(() => setCondition(e.target.value as ListingCondition | ''))}
            >
              <option value="">Any condition</option>
              {LISTING_CONDITIONS.map((value) => (
                <option key={value} value={value}>
                  {conditionLabel(value)}
                </option>
              ))}
            </select>
            <button>Search</button>
          </form>

          {notice && <p className="notice">{notice}</p>}
          {error && <p className="error-text">{error}</p>}

          <div className="grid">
            {result?.items.map((item) => (
              <article key={item.id}>
                <Link to={`/listings/${item.id}`}>
                  <img src={item.images[0] ?? PLACEHOLDER_IMAGE} alt="" />
                </Link>
                <small>
                  {item.exchangeType === 'GiveAway' ? 'GIVE AWAY' : 'SWAP'} · {item.category}
                </small>
                <h3>
                  <Link className="card-link" to={`/listings/${item.id}`}>
                    {item.title}
                  </Link>
                </h3>
                <p>{item.description}</p>
                <button onClick={() => requestItem(item)}>Request item</button>
                <footer>
                  {item.location}
                  <b>
                    <Link to={`/users/${item.owner.id}`}>{item.owner.displayName}</Link>
                  </b>
                </footer>
              </article>
            ))}
          </div>

          {result && result.items.length === 0 && !error && (
            <p className="muted">No items match your search yet. Try different filters.</p>
          )}

          {result && result.total > result.pageSize && (
            <div className="pagination">
              <button disabled={page <= 1} onClick={() => setPage(page - 1)}>
                ← Previous
              </button>
              <span>
                Page {page} of {totalPages} · {result.total} items
              </span>
              <button disabled={page >= totalPages} onClick={() => setPage(page + 1)}>
                Next →
              </button>
            </div>
          )}
        </section>
      </main>

      {selected && (
        <RequestDialog
          listing={selected}
          onClose={() => setSelected(null)}
          onSent={(message) => {
            setNotice(message)
            window.scrollTo({ top: 0, behavior: 'smooth' })
          }}
        />
      )}
    </>
  )
}
