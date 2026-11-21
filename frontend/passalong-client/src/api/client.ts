import { clearSession, getToken } from '../auth/session'

/** Base URL of the API. In dev it is proxied by Vite (see vite.config.ts); override with VITE_API_URL. */
const API_URL: string = import.meta.env.VITE_API_URL ?? '/api'

export class ApiError extends Error {
  readonly status: number

  constructor(status: number, message: string) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

interface RequestOptions {
  method?: 'GET' | 'POST'
  body?: unknown
  /** Redirect to the login page when the API answers 401. Disable for the login/register calls. */
  redirectOnUnauthorized?: boolean
}

/** Turns an error response body into a readable message. */
function readErrorMessage(status: number, text: string): string {
  if (!text) {
    return `Request failed (${status}).`
  }

  try {
    const data: unknown = JSON.parse(text)

    // ASP.NET Identity errors: [{ code, description }]
    if (Array.isArray(data)) {
      const messages = data
        .map((item) => (typeof item === 'object' && item !== null ? (item as { description?: string }).description : null))
        .filter((message): message is string => Boolean(message))

      if (messages.length > 0) {
        return messages.join(' ')
      }
    }

    // ProblemDetails / validation problems.
    if (typeof data === 'object' && data !== null) {
      const problem = data as { title?: string; errors?: Record<string, string[]> }
      const validation = problem.errors ? Object.values(problem.errors).flat().join(' ') : ''

      if (validation) {
        return validation
      }

      if (problem.title) {
        return problem.title
      }
    }

    if (typeof data === 'string') {
      return data
    }
  } catch {
    // Not JSON: plain text such as BadRequest("message").
  }

  return text
}

export async function request<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const { method = 'GET', body, redirectOnUnauthorized = true } = options
  const headers: Record<string, string> = {}
  const token = getToken()

  if (token) {
    headers.Authorization = `Bearer ${token}`
  }

  if (body !== undefined) {
    headers['Content-Type'] = 'application/json'
  }

  let response: Response

  try {
    response = await fetch(`${API_URL}${path}`, {
      method,
      headers,
      body: body === undefined ? undefined : JSON.stringify(body),
    })
  } catch {
    throw new ApiError(0, 'Cannot reach the server. Check that the API is running.')
  }

  if (response.status === 401 && redirectOnUnauthorized) {
    clearSession()
    window.location.assign('/login')
    throw new ApiError(401, 'Your session has expired. Please sign in again.')
  }

  const text = await response.text()

  if (!response.ok) {
    const fallback = response.status === 403 ? 'You are not allowed to do that.' : ''
    throw new ApiError(response.status, readErrorMessage(response.status, text) || fallback)
  }

  return (text ? JSON.parse(text) : undefined) as T
}
