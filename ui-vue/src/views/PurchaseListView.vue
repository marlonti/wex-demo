<script setup lang="ts">
import { ref } from 'vue'
import { usePurchasesStore } from '@/stores/purchases'
import { ApiError } from '@/api/purchases'
import { required, maxLength, validDate, positiveAmount } from '@/utils/validators'
import PurchaseDetailView from '../components/PurchaseDetail.vue'

const store = usePurchasesStore()

const dialogOpen = ref(false)
const formRef = ref()
const submitting = ref(false)
const submitError = ref('')

const detailsDialogOpen = ref(false)
const selectedPurchaseId = ref('')

const description = ref('')
const transactionDate = ref('')
const amount = ref<number | null>(null)

const descriptionRules = [required, maxLength(50)]
const dateRules = [required, validDate]
const amountRules = [required, positiveAmount]

function openDialog() {
  description.value = ''
  transactionDate.value = ''
  amount.value = null
  submitError.value = ''
  dialogOpen.value = true
}

function closeDialog() {
  dialogOpen.value = false
}

function openDetails(id: string) {
  selectedPurchaseId.value = id
  detailsDialogOpen.value = true
}

function closeDetails() {
  detailsDialogOpen.value = false
}

async function save() {
  const { valid } = await formRef.value.validate()
  if (!valid) return

  submitting.value = true
  submitError.value = ''
  try {
    await store.create({
      description: description.value,
      transactionDate: transactionDate.value,
      amount: Number(amount.value),
    })
    dialogOpen.value = false
  } catch (error) {
    submitError.value = error instanceof ApiError ? error.message : 'Failed to save purchase.'
  } finally {
    submitting.value = false
  }
}

function formatAmount(value: number) {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value)
}
</script>

<template>
  <v-container>
    <div class="d-flex align-center justify-space-between mb-4">
      <h1>Purchase Transactions</h1>
      <v-btn color="primary" @click="openDialog">New Purchase</v-btn>
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
            <a href="#" @click.prevent="openDetails(item.id)">View details</a>
          </td>
        </tr>
        <tr v-if="store.purchases.length === 0">
          <td colspan="4" class="text-center text-medium-emphasis">No purchases yet.</td>
        </tr>
      </tbody>
    </v-table>

    <v-dialog v-model="dialogOpen" max-width="480">
      <v-card title="New Purchase">
        <v-card-text>
          <v-form ref="formRef" @submit.prevent="save">
            <v-text-field
              v-model="description"
              label="Description"
              :rules="descriptionRules"
              counter="50"
            ></v-text-field>
            <v-text-field
              v-model="transactionDate"
              type="date"
              label="Transaction Date"
              :rules="dateRules"
            ></v-text-field>
            <v-text-field
              v-model.number="amount"
              type="number"
              step="0.01"
              label="Purchase Amount ($)"
              :rules="amountRules"
            ></v-text-field>
            <v-alert v-if="submitError" type="error" density="compact" class="mb-2">
              {{ submitError }}
            </v-alert>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn text="Close" variant="plain" @click="closeDialog"></v-btn>
          <v-btn
            color="primary"
            text="Save"
            variant="tonal"
            :loading="submitting"
            @click="save"
          ></v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="detailsDialogOpen" max-width="560">
      <v-card title="Purchase Details">
        <template #append>
          <v-btn icon="mdi-close" variant="text" @click="closeDetails"></v-btn>
        </template>
        <v-card-text>
          <PurchaseDetailView v-if="detailsDialogOpen" :id="selectedPurchaseId" />
        </v-card-text>
      </v-card>
    </v-dialog>
  </v-container>
</template>
