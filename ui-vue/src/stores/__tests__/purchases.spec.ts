import { describe, it, expect, vi, beforeEach } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import type { createPurchase } from '@/api/purchases'

const createPurchaseMock = vi.fn<typeof createPurchase>()
vi.mock('@/api/purchases', () => ({
  createPurchase: (...args: Parameters<typeof createPurchase>) => createPurchaseMock(...args),
}))

import { usePurchasesStore } from '../purchases'

const STORAGE_KEY = 'wex-purchases'

describe('purchases store', () => {
  beforeEach(() => {
    localStorage.clear()
    createPurchaseMock.mockReset()
    setActivePinia(createPinia())
  })

  it('starts empty when localStorage has nothing stored', () => {
    const store = usePurchasesStore()
    expect(store.purchases).toEqual([])
  })

  it('hydrates from localStorage on creation', () => {
    const existing = [
      { id: '1', description: 'Coffee', transactionDate: '2026-01-01', amount: 4.5 },
    ]
    localStorage.setItem(STORAGE_KEY, JSON.stringify(existing))

    const store = usePurchasesStore()

    expect(store.purchases).toEqual(existing)
  })

  it('create appends the created purchase and persists it to localStorage', async () => {
    const created = {
      id: '2',
      description: 'Office chair',
      transactionDate: '2026-06-15',
      amount: 123.46,
    }
    createPurchaseMock.mockResolvedValue(created)

    const store = usePurchasesStore()
    const result = await store.create({
      description: 'Office chair',
      transactionDate: '2026-06-15',
      amount: 123.456,
    })

    expect(result).toEqual(created)
    expect(store.purchases).toEqual([created])
    expect(JSON.parse(localStorage.getItem(STORAGE_KEY)!)).toEqual([created])
  })
})
