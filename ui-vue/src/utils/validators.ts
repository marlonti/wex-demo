export type ValidationRule = (value: unknown) => true | string

export const required: ValidationRule = (value) => {
  if (value === null || value === undefined || String(value).trim() === '') {
    return 'Required'
  }
  return true
}

export const maxLength =
  (max: number): ValidationRule =>
  (value) =>
    String(value ?? '').length <= max ? true : `Must be ${max} characters or fewer`

export const validDate: ValidationRule = (value) => {
  if (!value) return true
  return Number.isNaN(new Date(String(value)).getTime()) ? 'Must be a valid date' : true
}

export const positiveAmount: ValidationRule = (value) => {
  if (value === null || value === undefined || value === '') return true
  const amount = Number(value)
  if (Number.isNaN(amount)) return 'Must be a number'
  return amount > 0 ? true : 'Must be a positive amount'
}
