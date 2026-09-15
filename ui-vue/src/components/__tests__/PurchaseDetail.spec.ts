import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'

const getPurchaseConvertedMock = vi.fn<typeof import('@/api/purchases').getPurchaseConverted>()
vi.mock('@/api/purchases', async () => {
  const actual = await vi.importActual<typeof import('@/api/purchases')>('@/api/purchases')
  return {
    ...actual,
    getPurchaseConverted: (...args: Parameters<typeof actual.getPurchaseConverted>) =>
      getPurchaseConvertedMock(...args),
  }
})

import PurchaseDetailView from '@/components/PurchaseDetail.vue'
import { ApiError } from '@/api/purchases'

const vuetify = createVuetify({ components, directives })

function mountView(id = 'abc-123') {
  return mount(PurchaseDetailView, {
    props: { id },
    global: { plugins: [vuetify] },
    attachTo: document.body,
  })
}

async function clickConvert(wrapper: ReturnType<typeof mountView>) {
  const btn = wrapper.findAll('button').find((b) => b.text().includes('Convert'))
  await btn?.trigger('click')
  await wrapper.vm.$nextTick()
  await wrapper.vm.$nextTick()
}

describe('PurchaseDetailView', () => {
  beforeEach(() => {
    getPurchaseConvertedMock.mockReset()
  })

  it('renders the conversion result on success', async () => {
    getPurchaseConvertedMock.mockResolvedValue({
      id: 'abc-123',
      description: 'Office chair',
      transactionDate: '2026-06-15',
      originalAmountUsd: 123.46,
      exchangeRate: 1.35,
      convertedAmount: 166.67,
      country: 'Canada',
      currency: 'Dollar',
      rateDate: '2026-06-01',
    })

    const wrapper = mountView()
    await wrapper.find('input').setValue('Canada-Dollar')
    await clickConvert(wrapper)

    expect(getPurchaseConvertedMock).toHaveBeenCalledWith('abc-123', 'Canada-Dollar')
    expect(wrapper.text()).toContain('Office chair')
    expect(wrapper.text()).toContain('166.67')
    expect(wrapper.text()).toContain('Canada')
  })

  it('renders the server error message when conversion is unavailable', async () => {
    getPurchaseConvertedMock.mockRejectedValue(
      new ApiError(422, 'no exchange rate available within 6 months'),
    )

    const wrapper = mountView()
    await wrapper.find('input').setValue('Nowhere-Coin')
    await clickConvert(wrapper)

    expect(wrapper.text()).toContain('no exchange rate available within 6 months')
  })
})
