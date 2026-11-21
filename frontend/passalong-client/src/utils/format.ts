import type { ExchangeType, ListingCondition } from '../api/types'

export const PLACEHOLDER_IMAGE =
  'https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?auto=format&fit=crop&w=900&q=80'

const CONDITION_LABELS: Record<ListingCondition, string> = {
  New: 'New',
  LikeNew: 'Like new',
  Good: 'Good',
  Fair: 'Fair',
  NeedsRepair: 'Needs repair',
}

export function conditionLabel(condition: ListingCondition): string {
  return CONDITION_LABELS[condition]
}

export function exchangeTypeLabel(type: ExchangeType): string {
  return type === 'GiveAway' ? 'Give away' : 'Swap'
}

export function formatDate(value: string): string {
  // The API returns UTC timestamps; some come without a trailing "Z".
  const iso = /([zZ]|[+-]\d\d:\d\d)$/.test(value) ? value : `${value}Z`

  return new Date(iso).toLocaleDateString(undefined, { year: 'numeric', month: 'short', day: 'numeric' })
}

export function errorMessage(error: unknown): string {
  return error instanceof Error ? error.message : 'Something went wrong.'
}
