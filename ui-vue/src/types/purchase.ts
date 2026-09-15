export interface CreatePurchaseRequest {
  description: string
  transactionDate: string
  amount: number
}

export interface PurchaseResponse {
  id: string
  description: string
  transactionDate: string
  amount: number
}

export interface PurchaseConversionResponse {
  id: string
  description: string
  transactionDate: string
  originalAmountUsd: number
  exchangeRate: number
  convertedAmount: number
  country: string
  currency: string
  rateDate: string
}
