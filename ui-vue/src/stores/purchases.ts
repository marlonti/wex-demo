import { ref } from 'vue'
import { defineStore } from 'pinia'
import { createPurchase } from '@/api/purchases'
import type { CreatePurchaseRequest, PurchaseResponse } from '@/types/purchase'

const STORAGE_KEY = 'wex-purchases'

function loadFromStorage(): PurchaseResponse[] {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    return raw ? (JSON.parse(raw) as PurchaseResponse[]) : []
  } catch {
    return []
  }
}

function saveToStorage(purchases: PurchaseResponse[]) {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(purchases))
  } catch {
    // Ignore storage failures (e.g. private browsing quota).
  }
}

export const usePurchasesStore = defineStore('purchases', () => {
  const purchases = ref<PurchaseResponse[]>(loadFromStorage())

  async function create(payload: CreatePurchaseRequest): Promise<PurchaseResponse> {
    const created = await createPurchase(payload)
    purchases.value.push(created)
    saveToStorage(purchases.value)
    return created
  }

  return { purchases, create }
})
