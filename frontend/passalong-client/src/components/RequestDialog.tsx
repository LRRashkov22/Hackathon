import { useEffect, useState } from 'react'
import { exchangeRequestsApi, listingsApi } from '../api/endpoints'
import type { ExchangeType, Listing } from '../api/types'
import { conditionLabel, errorMessage } from '../utils/format'
import '../styles/Modal.css'

interface RequestDialogProps {
  listing: { id: string; title: string; exchangeType: ExchangeType }
  onClose: () => void
  onSent: (message: string) => void
}

export default function RequestDialog({ listing, onClose, onSent }: RequestDialogProps) {
  const isSwap = listing.exchangeType === 'Swap'
  const [myListings, setMyListings] = useState<Listing[]>([])
  const [offeredId, setOfferedId] = useState('')
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')
  const [sending, setSending] = useState(false)

  useEffect(() => {
    if (!isSwap) {
      return
    }

    listingsApi
      .mine()
      .then((items) => setMyListings(items.filter((item) => item.status === 'Available')))
      .catch((e) => setError(errorMessage(e)))
  }, [isSwap])

  const send = async () => {
    if (isSwap && !offeredId) {
      setError('Choose an item to offer.')
      return
    }

    setSending(true)
    setError('')

    try {
      await exchangeRequestsApi.create({
        listingId: listing.id,
        offeredListingId: isSwap ? offeredId : null,
        message: message.trim() || null,
      })
      onSent('Request sent. The owner will see it on their dashboard.')
      onClose()
    } catch (e) {
      setError(errorMessage(e))
      setSending(false)
    }
  }

  return (
    <div className="modal-back">
      <div className="modal">
        <button className="close" onClick={onClose} aria-label="Close">
          ×
        </button>
        <p className="eyebrow">REQUEST ITEM</p>
        <h2>{listing.title}</h2>
        {isSwap && (
          <label>
            Offer one of your items
            <select value={offeredId} onChange={(e) => setOfferedId(e.target.value)}>
              <option value="">Choose an item to swap</option>
              {myListings.map((item) => (
                <option value={item.id} key={item.id}>
                  {item.title} · {conditionLabel(item.condition)}
                </option>
              ))}
            </select>
          </label>
        )}
        <label>
          Message
          <textarea
            value={message}
            onChange={(e) => setMessage(e.target.value)}
            placeholder="Tell the owner why you are interested…"
          />
        </label>
        {error && <p className="error-text">{error}</p>}
        <button onClick={send} disabled={sending}>
          {sending ? 'Sending…' : 'Send request'}
        </button>
      </div>
    </div>
  )
}
