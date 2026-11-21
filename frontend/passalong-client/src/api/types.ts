export type ListingCondition = 'New' | 'LikeNew' | 'Good' | 'Fair' | 'NeedsRepair'

export type ExchangeType = 'GiveAway' | 'Swap'

export type ListingStatus = 'Available' | 'Reserved' | 'Transferred' | 'Archived' | 'Removed'

export type ExchangeRequestStatus = 'Pending' | 'Accepted' | 'Rejected' | 'Cancelled' | 'Completed'

export const LISTING_CONDITIONS: ListingCondition[] = ['New', 'LikeNew', 'Good', 'Fair', 'NeedsRepair']

export const EXCHANGE_TYPES: ExchangeType[] = ['GiveAway', 'Swap']

// Auth

export interface AuthUser {
  id: string
  displayName: string
  email: string
}

export interface AuthResponse {
  token: string
  user: AuthUser
}

export interface RegisterPayload {
  email: string
  password: string
  displayName: string
  location: string | null
}

export interface LoginPayload {
  email: string
  password: string
}

// Categories

export interface Category {
  id: string
  name: string
  description: string | null
  icon: string | null
}

// Listings

export interface ListingSummary {
  id: string
  title: string
  description: string
  condition: ListingCondition
  exchangeType: ExchangeType
  location: string
  createdAt: string
  category: string
  owner: { id: string; displayName: string }
  images: string[]
}

export interface ListingPage {
  total: number
  page: number
  pageSize: number
  items: ListingSummary[]
}

export interface BrowseParams {
  search?: string
  categoryId?: string
  exchangeType?: ExchangeType
  condition?: ListingCondition
  page?: number
  pageSize?: number
}

export interface ListingImage {
  id: string
  imageUrl: string
  displayOrder: number
}

/** Full listing entity returned by GET /listings/{id} and GET /listings/mine. */
export interface Listing {
  id: string
  ownerId: string
  categoryId: string
  title: string
  description: string
  condition: ListingCondition
  exchangeType: ExchangeType
  desiredSwap: string | null
  status: ListingStatus
  location: string
  createdAt: string
  updatedAt: string
  category: Category
  owner?: { id: string; displayName: string; location: string | null }
  images: ListingImage[]
}

export interface CreateListingPayload {
  categoryId: string
  title: string
  description: string
  condition: ListingCondition
  exchangeType: ExchangeType
  desiredSwap: string | null
  location: string
  imageUrls: string[]
}

export interface CreatedListing {
  id: string
  title: string
}

// Exchange requests

export interface ExchangeRequest {
  id: string
  listingId: string
  requesterId: string
  ownerId: string
  offeredListingId: string | null
  message: string | null
  status: ExchangeRequestStatus
  requesterConfirmed: boolean
  ownerConfirmed: boolean
  createdAt: string
  updatedAt: string
  listing: Listing
  requester?: { id: string; displayName: string }
  owner?: { id: string; displayName: string }
}

export interface CreateExchangePayload {
  listingId: string
  offeredListingId: string | null
  message: string | null
}

export interface HandoverConfirmation {
  id: string
  status: ExchangeRequestStatus
  requesterConfirmed: boolean
  ownerConfirmed: boolean
}

// Reviews

export interface CreateReviewPayload {
  transferId: string
  rating: number
  comment: string | null
}

// Wanted items

export interface WantedItem {
  id: string
  title: string
  description: string
  location: string
  preferredCondition: ListingCondition | null
  status: string
  category: Category
}

export interface CreateWantedPayload {
  categoryId: string
  title: string
  description: string
  preferredCondition: ListingCondition | null
  location: string
}

export interface WantedMatch {
  score: number
  listing: {
    id: string
    title: string
    description: string
    location: string
    condition: ListingCondition
    exchangeType: ExchangeType
    category: string
    owner: string
  }
}

// Community

export interface ImpactSummary {
  completedTransfers: number
  itemsReused: number
  activeListings: number
  members: number
}

export interface LeaderboardEntry {
  id: string
  displayName: string
  reputationScore: number
  location: string | null
}

export interface TransferHistoryEntry {
  id: string
  completedAt: string
  previousOwner: string
  newOwner: string
}

// Users

export interface UserProfile {
  id: string
  displayName: string
  bio: string | null
  location: string | null
  avatarUrl: string | null
  reputationScore: number
  listings: { id: string; title: string; status: ListingStatus }[]
  reviews: { rating: number; comment: string | null; createdAt: string }[]
}
