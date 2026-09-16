import { ref } from 'vue'
import { defineStore } from 'pinia'
import { createPurchase } from '@/api/purchases'
import type { CreatePurchaseRequest, PurchaseResponse } from '@/types/purchase'

export const usePurchasesStore = defineStore('purchases', () => {
  const purchases = ref<PurchaseResponse[]>([])

  async function create(payload: CreatePurchaseRequest): Promise<PurchaseResponse> {
    const created = await createPurchase(payload)
    purchases.value.push(created)
    return created
  }

  return { purchases, create }
})
