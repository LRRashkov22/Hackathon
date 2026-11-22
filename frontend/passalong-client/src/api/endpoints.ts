import { request } from './client'
import type {
  AuthResponse,
  BrowseParams,
  Category,
  CreateExchangePayload,
  CreateListingPayload,
  CreatedListing,
  CreateReviewPayload,
  CreateWantedPayload,
  ExchangeRequest,
  HandoverConfirmation,
  ImpactSummary,
  LeaderboardEntry,
  Listing,
  ListingPage,
  LoginPayload,
  RegisterPayload,
  TransferHistoryEntry,
  UserProfile,
  WantedItem,
  WantedMatch,
} from './types'

// POST /api/auth/register, POST /api/auth/login
export const authApi = {
  register: (payload: RegisterPayload) =>
    request<AuthResponse>('/auth/register', { method: 'POST', body: payload, redirectOnUnauthorized: false }),

  login: (payload: LoginPayload) =>
    request<AuthResponse>('/auth/login', { method: 'POST', body: payload, redirectOnUnauthorized: false }),
}

// GET /api/categories
export const categoriesApi = {
  list: () => request<Category[]>('/categories'),
}

// GET /api/listings, GET /api/listings/{id}, GET /api/listings/mine, POST /api/listings
export const listingsApi = {
  browse: (params: BrowseParams) => {
    const query = new URLSearchParams()

    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== '') {
        query.set(key, String(value))
      }
    })

    return request<ListingPage>(`/listings?${query.toString()}`)
  },

  get: (id: string) => request<Listing>(`/listings/${id}`),

  mine: () => request<Listing[]>('/listings/mine'),

  create: (payload: CreateListingPayload) =>
    request<CreatedListing>('/listings', { method: 'POST', body: payload }),
}

// /api/exchange-requests/*
export const exchangeRequestsApi = {
  create: (payload: CreateExchangePayload) =>
    request<ExchangeRequest>('/exchange-requests', { method: 'POST', body: payload }),

  incoming: () => request<ExchangeRequest[]>('/exchange-requests/incoming'),

  outgoing: () => request<ExchangeRequest[]>('/exchange-requests/outgoing'),

  accept: (id: string) => request<ExchangeRequest>(`/exchange-requests/${id}/accept`, { method: 'POST' }),

  reject: (id: string) => request<ExchangeRequest>(`/exchange-requests/${id}/reject`, { method: 'POST' }),

  cancel: (id: string) => request<ExchangeRequest>(`/exchange-requests/${id}/cancel`, { method: 'POST' }),

  confirmHandover: (id: string) =>
    request<HandoverConfirmation>(`/exchange-requests/${id}/confirm-handover`, { method: 'POST' }),
}

// POST /api/reviews
export const reviewsApi = {
  create: (payload: CreateReviewPayload) => request<void>('/reviews', { method: 'POST', body: payload }),
}

// GET /api/users/{id}
export const usersApi = {
  get: (id: string) => request<UserProfile>(`/users/${id}`),
}

// POST /api/wanted-items, GET /api/wanted-items/mine, GET /api/wanted-items/{id}/matches
export const wantedItemsApi = {
  create: (payload: CreateWantedPayload) => request<WantedItem>('/wanted-items', { method: 'POST', body: payload }),

  mine: () => request<WantedItem[]>('/wanted-items/mine'),

  matches: (id: string) => request<WantedMatch[]>(`/wanted-items/${id}/matches`),
}

// GET /api/impact/summary, GET /api/leaderboard, GET /api/items/{id}/history
export const communityApi = {
  impact: () => request<ImpactSummary>('/impact/summary'),

  leaderboard: () => request<LeaderboardEntry[]>('/leaderboard'),

  itemHistory: (listingId: string) => request<TransferHistoryEntry[]>(`/items/${listingId}/history`),
}

// POST /api/images (multipart, field "files") -> { urls }
export const imagesApi = {
  upload: (files: File[]) => {
    const form = new FormData()
    files.forEach((file) => form.append('files', file))

    return request<{ urls: string[] }>('/images', { method: 'POST', body: form })
  },
}
