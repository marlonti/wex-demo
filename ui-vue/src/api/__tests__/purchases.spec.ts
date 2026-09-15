import { describe, it, expect, vi, afterEach } from 'vitest'
import { createPurchase, getPurchaseConverted, ApiError } from '../purchases'

function jsonResponse(body: unknown, status = 200) {
  return new Response(JSON.stringify(body), {
    status,
    headers: { 'Content-Type': 'application/json' },
  })
}

describe('purchases api client', () => {
  afterEach(() => {
    vi.unstubAllGlobals()
  })

  it('createPurchase posts the payload and returns the created purchase', async () => {
    const created = {
      id: '11111111-1111-1111-1111-111111111111',
      description: 'Office chair',
      transactionDate: '2026-06-15',
      amount: 123.46,
    }
    const fetchMock = vi.fn<typeof fetch>().mockResolvedValue(jsonResponse(created, 201))
    vi.stubGlobal('fetch', fetchMock)

    const result = await createPurchase({
      description: 'Office chair',
      transactionDate: '2026-06-15',
      amount: 123.456,
    })

    expect(result).toEqual(created)
    expect(fetchMock).toHaveBeenCalledWith(
      expect.stringContaining('/purchases/'),
      expect.objectContaining({ method: 'POST' }),
    )
  })

  it('createPurchase throws an ApiError with the server detail on failure', async () => {
    const fetchMock = vi
      .fn<typeof fetch>()
      .mockResolvedValue(
        jsonResponse({ title: 'Validation failed', detail: 'Description is required' }, 400),
      )
    vi.stubGlobal('fetch', fetchMock)

    await expect(
      createPurchase({ description: '', transactionDate: '2026-06-15', amount: 1 }),
    ).rejects.toMatchObject(new ApiError(400, 'Description is required'))
  })

  it('getPurchaseConverted requests the id with the countryCurrency query param', async () => {
    const converted = {
      id: '11111111-1111-1111-1111-111111111111',
      description: 'Office chair',
      transactionDate: '2026-06-15',
      originalAmountUsd: 123.46,
      exchangeRate: 1.35,
      convertedAmount: 166.67,
      country: 'Canada',
      currency: 'Dollar',
      rateDate: '2026-06-01',
    }
    const fetchMock = vi.fn<typeof fetch>().mockResolvedValue(jsonResponse(converted))
    vi.stubGlobal('fetch', fetchMock)

    const result = await getPurchaseConverted(converted.id, 'Canada-Dollar')

    expect(result).toEqual(converted)
    const url = fetchMock.mock.calls[0]?.[0]
    expect(String(url)).toContain(`/purchases/${converted.id}`)
    expect(String(url)).toContain('countryCurrency=Canada-Dollar')
  })

  it('getPurchaseConverted throws an ApiError when conversion is unavailable', async () => {
    const fetchMock = vi
      .fn<typeof fetch>()
      .mockResolvedValue(jsonResponse({ title: 'Conversion unavailable', detail: 'No rate found' }, 422))
    vi.stubGlobal('fetch', fetchMock)

    await expect(getPurchaseConverted('some-id', 'Nowhere-Coin')).rejects.toMatchObject(
      new ApiError(422, 'No rate found'),
    )
  })
})
