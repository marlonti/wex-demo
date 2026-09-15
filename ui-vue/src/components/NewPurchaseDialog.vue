<script setup lang="ts">
import { ref, watch } from 'vue'
import { usePurchasesStore } from '@/stores/purchases'
import { ApiError } from '@/api/purchases'
import { required, maxLength, validDate, positiveAmount } from '@/utils/validators'

const open = defineModel<boolean>({ required: true })

const store = usePurchasesStore()

const formRef = ref()
const submitting = ref(false)
const submitError = ref('')

const description = ref('')
const transactionDate = ref<Date>()
const amount = ref<number | null>(null)

const descriptionRules = [required, maxLength(50)]
const dateRules = [required, validDate]
const amountRules = [required, positiveAmount]

watch(open, (isOpen) => {
  if (isOpen) {
    description.value = ''
    transactionDate.value = undefined
    amount.value = null
    submitError.value = ''
  }
})

function close() {
  open.value = false
}

async function save() {
  const { valid } = await formRef.value.validate()
  if (!valid) return

  submitting.value = true
  submitError.value = ''
  try {
    await store.create({
      description: description.value,
      transactionDate: transactionDate.value!.toISOString().split('T')[0]!,
      amount: Number(amount.value),
    })
    open.value = false
  } catch (error) {
    submitError.value = error instanceof ApiError ? error.message : 'Failed to save purchase.'
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <v-dialog v-model="open" max-width="480">
    <v-card title="New Purchase">
      <v-card-text>
        <v-form ref="formRef" @submit.prevent="save">
          <v-text-field v-model="description" label="Description" :rules="descriptionRules" variant="outlined" counter="50"></v-text-field>
          <v-date-input v-model="transactionDate" :rules="dateRules" variant="outlined" label="Date input"></v-date-input>
          <v-text-field v-model.number="amount" type="number" step="0.01" label="Purchase Amount ($)"
            :rules="amountRules" variant="outlined"></v-text-field>
          <v-alert v-if="submitError" type="error" density="compact" class="mb-2">
            {{ submitError }}
          </v-alert>
        </v-form>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn text="Close" variant="plain" @click="close"></v-btn>
        <v-btn color="primary" text="Save" variant="tonal" :loading="submitting" @click="save"></v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
