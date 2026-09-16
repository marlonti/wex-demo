<script setup lang="ts">
import { ref } from 'vue'
import { usePurchasesStore } from '@/stores/purchases'
import PurchaseDetail from '../components/PurchaseDetail.vue'
import NewPurchaseDialog from '../components/NewPurchaseDialog.vue'

const store = usePurchasesStore()

const dialogOpen = ref(false)

const detailsDialogOpen = ref(false)
const selectedPurchaseId = ref('')

function openDetails(id: string) {
  selectedPurchaseId.value = id
  detailsDialogOpen.value = true
}

function closeDetails() {
  detailsDialogOpen.value = false
}

function formatAmount(value: number) {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value)
}
</script>

<template>
  <v-container>
    <div class="d-flex align-center justify-space-between mb-4">
      <h1>Purchase Transactions</h1>
      <v-btn color="primary" @click="dialogOpen = true">New Purchase</v-btn>
    </div>

    <v-table density="compact">
      <thead>
        <tr>
          <th class="text-left">Description</th>
          <th class="text-left">Transaction Date</th>
          <th class="text-left">Amount</th>
          <th class="text-left"></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="item in store.purchases" :key="item.id">
          <td>{{ item.description }}</td>
          <td>{{ item.transactionDate }}</td>
          <td>{{ formatAmount(item.amount) }}</td>
          <td>
            <a href="#" @click.prevent="openDetails(item.id)">View Converted Currency</a>
          </td>
        </tr>
        <tr v-if="store.purchases.length === 0">
          <td colspan="4" class="text-center text-medium-emphasis">No purchases yet.</td>
        </tr>
      </tbody>
    </v-table>

    <NewPurchaseDialog v-model="dialogOpen" />

    <v-dialog v-model="detailsDialogOpen" max-width="560">
      <v-card title="Purchase Details">
        <template #append>
          <v-btn icon="mdi-close" variant="text" @click="closeDetails"></v-btn>
        </template>
        <v-card-text>
          <PurchaseDetail v-if="detailsDialogOpen" :id="selectedPurchaseId" />
        </v-card-text>
      </v-card>
    </v-dialog>
  </v-container>
</template>
