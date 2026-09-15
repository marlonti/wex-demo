import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import { createPinia, setActivePinia } from 'pinia'
import { createRouter, createWebHistory } from 'vue-router'

const createPurchaseMock = vi.fn<typeof import('@/api/purchases').createPurchase>()
vi.mock('@/api/purchases', async () => {
  const actual = await vi.importActual<typeof import('@/api/purchases')>('@/api/purchases')
  return {
    ...actual,
    createPurchase: (...args: Parameters<typeof actual.createPurchase>) =>
      createPurchaseMock(...args),
  }
})

import PurchaseListView from '../PurchaseListView.vue'
import { usePurchasesStore } from '@/stores/purchases'

const vuetify = createVuetify({ components, directives })

async function mountView() {
  const router = createRouter({
    history: createWebHistory(),
    routes: [{ path: '/', component: PurchaseListView }],
  })
  router.push('/')
  await router.isReady()

  return mount(PurchaseListView, {
    global: { plugins: [vuetify, router] },
    attachTo: document.body,
  })
}

describe('PurchaseListView', () => {
  beforeEach(() => {
    localStorage.clear()
    createPurchaseMock.mockReset()
    setActivePinia(createPinia())
  })

  it('renders purchases from the store', async () => {
    const store = usePurchasesStore()
    store.purchases.push({
      id: '1',
      description: 'Coffee',
      transactionDate: '2026-01-01',
      amount: 4.5,
    })

    const wrapper = await mountView()

    expect(wrapper.text()).toContain('Coffee')
    expect(wrapper.text()).toContain('2026-01-01')
  })

  it('blocks submit when the form is invalid and does not call the API', async () => {
    const wrapper = await mountView()

    const newPurchaseBtn = wrapper
      .findAll('button')
      .find((b) => b.text().includes('New Purchase'))
    await newPurchaseBtn?.trigger('click')
    await wrapper.vm.$nextTick()

    const saveBtn = wrapper.findAll('button').find((b) => b.text().includes('Save'))
    await saveBtn?.trigger('click')
    await wrapper.vm.$nextTick()
    await wrapper.vm.$nextTick()

    expect(createPurchaseMock).not.toHaveBeenCalled()
  })

  it('opens a details modal instead of navigating when "View details" is clicked', async () => {
    const store = usePurchasesStore()
    store.purchases.push({
      id: '1',
      description: 'Coffee',
      transactionDate: '2026-01-01',
      amount: 4.5,
    })

    const wrapper = await mountView()

    const viewDetailsLink = wrapper.findAll('a').find((a) => a.text().includes('View details'))
    await viewDetailsLink?.trigger('click')
    await wrapper.vm.$nextTick()
    await wrapper.vm.$nextTick()

    expect(document.body.textContent).toContain('Purchase Details')
    expect(document.body.textContent).toContain('Country-Currency')
    expect(wrapper.vm.$route.path).toBe('/')
  })
})
