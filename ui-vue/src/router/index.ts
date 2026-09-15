import { createRouter, createWebHistory } from 'vue-router'
import PurchaseListView from '../views/PurchaseListView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'purchases',
      component: PurchaseListView,
    },
  ],
})

export default router
