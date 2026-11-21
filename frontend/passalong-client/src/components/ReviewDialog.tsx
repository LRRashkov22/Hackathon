import { useState } from 'react'
import { communityApi, reviewsApi } from '../api/endpoints'
import { errorMessage } from '../utils/format'
import '../styles/Modal.css'

interface ReviewDialogProps {
  /** The listing that changed hands; its latest transfer is the one being reviewed. */
  listingId: string
  listingTitle: string
  otherPersonName: string
  onClose: () => void
  onDone: (message: string) => void
}

export default function ReviewDialog({
  listingId,
  listingTitle,
  otherPersonName,
  onClose,
  onDone,
}: ReviewDialogProps) {
  const [rating, setRating] = useState(5)
  const [comment, setComment] = useState('')
  const [error, setError] = useState('')
  const [sending, setSending] = useState(false)

  const submit = async () => {
    setSending(true)
    setError('')

    try {
      const history = await communityApi.itemHistory(listingId)
      const transfer = history[history.length - 1]

      if (!transfer) {
        setError('No completed transfer was found for this item.')
        setSending(false)
        return
      }

      await reviewsApi.create({
        transferId: transfer.id,
        rating,
        comment: comment.trim() || null,
      })
      onDone(`Thanks! Your review of ${otherPersonName} was saved.`)
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
        <p className="eyebrow">LEAVE A REVIEW</p>
        <h2>{listingTitle}</h2>
        <label>
          How was the exchange with {otherPersonName}?
          <select value={rating} onChange={(e) => setRating(Number(e.target.value))}>
            {[5, 4, 3, 2, 1].map((value) => (
              <option key={value} value={value}>
                {'★'.repeat(value)} ({value})
              </option>
            ))}
          </select>
        </label>
        <label>
          Comment (optional)
          <textarea value={comment} onChange={(e) => setComment(e.target.value)} />
        </label>
        {error && <p className="error-text">{error}</p>}
        <button onClick={submit} disabled={sending}>
          {sending ? 'Saving…' : 'Submit review'}
        </button>
      </div>
    </div>
  )
}
