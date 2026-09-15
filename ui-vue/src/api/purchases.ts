import type { CreatePurchaseRequest, PurchaseConversionResponse, PurchaseResponse } from '@/types/purchase'

const baseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5274'

export class ApiError extends Error {
  status: number

  constructor(status: number, message: string) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

interface ProblemDetails {
  title?: string
  detail?: string
}

async function toApiError(response: Response): Promise<ApiError> {
  let message = response.statusText
  try {
    const problem = (await response.json()) as ProblemDetails
    message = problem.detail ?? problem.title ?? message
  } catch {
    // Response body wasn't JSON problem details; fall back to statusText.
  }
  return new ApiError(response.status, message)
}

export async function createPurchase(payload: CreatePurchaseRequest): Promise<PurchaseResponse> {
  const response = await fetch(`${baseUrl}/purchases/`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  })

  if (!response.ok) {
    throw await toApiError(response)
  }

  return (await response.json()) as PurchaseResponse
}

export async function getPurchaseConverted(
  id: string,
  countryCurrency: string,
): Promise<PurchaseConversionResponse> {
  const url = new URL(`${baseUrl}/purchases/${id}`)
  url.searchParams.set('countryCurrency', countryCurrency)

  const response = await fetch(url, { headers: { Accept: 'application/json' } })

  if (!response.ok) {
    throw await toApiError(response)
  }

  return (await response.json()) as PurchaseConversionResponse
}
