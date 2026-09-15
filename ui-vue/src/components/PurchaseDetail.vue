<script setup lang="ts">
import { ref } from 'vue'
import { getPurchaseConverted, ApiError } from '@/api/purchases'
import type { PurchaseConversionResponse } from '@/types/purchase'

const props = defineProps<{ id: string }>()

const countryCurrencySuggestions = [
  'Canada-Dollar',
  'United Kingdom-Pound',
  'Euro Zone-Euro',
  'Japan-Yen',
  'Mexico-Peso',
  'Australia-Dollar',
]

const countryCurrency = ref('')
const loading = ref(false)
const error = ref('')
const result = ref<PurchaseConversionResponse | null>(null)

async function convert() {
  if (!countryCurrency.value.trim()) {
    error.value = 'Enter a country-currency, e.g. "Canada-Dollar".'
    return
  }

  loading.value = true
  error.value = ''
  result.value = null
  try {
    result.value = await getPurchaseConverted(props.id, countryCurrency.value.trim())
  } catch (err) {
    error.value = err instanceof ApiError ? err.message : 'Failed to retrieve conversion.'
  } finally {
    loading.value = false
  }
}

function formatUsd(value: number) {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value)
}

function formatDecimal(value: number) {
  return value.toFixed(2)
}
</script>

<template>
  <div>
    <v-card variant="flat">
      <v-card-text>
        <div class="d-flex justify-center">
          <v-tooltip interactive>
            <template v-slot:activator="{ props: activatorProps }">
              <v-text-field v-model="countryCurrency" v-bind="activatorProps" label="What Country-Currency would you like to see?"
                hint='Treasury API format, e.g. "Canada-Dollar"' persistent-hint variant="outlined">
              </v-text-field>
            </template>
            <div class="suggestion-tooltip">
                  <div class="suggestion-tooltip-title">Examples</div>
                  <div v-for="item in countryCurrencySuggestions" :key="item">{{ item }}</div>
                </div>
          </v-tooltip>
        </div>
        <v-alert v-if="error" type="error" density="compact" class="mt-2">
          {{ error }}
        </v-alert>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="primary" variant="tonal" :loading="loading" @click="convert">Convert</v-btn>
      </v-card-actions>
    </v-card>

    <v-card v-if="result" class="mt-4" variant="flat">
      <v-card-text>
        <dl class="detail-grid">
          <dt>Id</dt>
          <dd>{{ result.id }}</dd>
          <dt>Description</dt>
          <dd>{{ result.description }}</dd>
          <dt>Transaction Date</dt>
          <dd>{{ result.transactionDate }}</dd>
          <dt>Original Amount (USD)</dt>
          <dd>{{ formatUsd(result.originalAmountUsd) }}</dd>
          <dt>Exchange Rate</dt>
          <dd>{{ result.exchangeRate }}</dd>
          <dt>Converted Amount</dt>
          <dd>{{ formatDecimal(result.convertedAmount) }} {{ result.currency }}</dd>
          <dt>Country</dt>
          <dd>{{ result.country }}</dd>
          <dt>Rate Date</dt>
          <dd>{{ result.rateDate }}</dd>
        </dl>
      </v-card-text>
    </v-card>
  </div>
</template>

<style scoped>

.suggestion-tooltip-title {
  font-weight: 600;
  margin-bottom: 0.25rem;
}

.detail-grid {
  display: grid;
  grid-template-columns: auto 1fr;
  gap: 0.25rem 1rem;
}

.detail-grid dt {
  font-weight: 600;
}

.detail-grid dd {
  margin: 0;
}

</style>
