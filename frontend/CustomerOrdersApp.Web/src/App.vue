<script setup lang="ts">
import { onMounted, ref } from 'vue'

type UploadResult = {
  success: boolean
  totalRows: number
  failedRows: number
  errors: string[]
}

type OrderItemDto = {
  id: number
  itemId: number
  listPrice: number
  finalPrice: number
  discount: number
}

type OrderDto = {
  id: number
  status: string
  orderDate: string
  requiredDate: string
  shippedDate: string | null
  totalOrder: number
  items: OrderItemDto[]
}

type CustomerDto = {
  id: number
  firstName: string
  lastName: string
  email: string
  street: string
  city: string
  state: string
  zipCode: string
  phoneNumber: string | null
  totalSpend: number
  orders: OrderDto[]
}
type GetCustomerResult = {
  success: boolean
  customerDtos: CustomerDto[]
  errors: string[]
}

type DataResult = {
  success: boolean
  errors: string[]
}

type ApplyDiscountResult = {
  success: boolean
  errors: string[]
  discountsApplied: number
}

const uploadApiBaseUrl = 'http://localhost:5294/api/UploadFile'
const customersApiUrl = 'http://localhost:5294/api/Customer'

const activeTab = ref<'customers' | 'upload'>('customers')

const selectedCustomersFile = ref<File | null>(null)
const selectedOrdersFile = ref<File | null>(null)
const selectedOrderItemsFile = ref<File | null>(null)

const loading = ref(false)
const customersLoading = ref(false)

const result = ref<UploadResult | null>(null)
const errorMessage = ref<string | null>(null)

const customers = ref<CustomerDto[]>([])
const expandedCustomers = ref<Set<number>>(new Set())
const expandedOrders = ref<Set<number>>(new Set())

const actionMessage = ref<string | null>(null)

onMounted(async () => {
  await getCustomers()
})

function onFileSelected(event: Event, target: 'customers' | 'orders' | 'orderItems') {
  const input = event.target as HTMLInputElement
  const file = input.files?.[0] ?? null

  if (target === 'customers') selectedCustomersFile.value = file
  if (target === 'orders') selectedOrdersFile.value = file
  if (target === 'orderItems') selectedOrderItemsFile.value = file
}

async function clearAllData() {
  loading.value = true
  errorMessage.value = null
  actionMessage.value = null
  result.value = null

  try {
    const response = await fetch(uploadApiBaseUrl, {
      method: 'DELETE'
    })

    if (!response.ok) {
      throw new Error(`Clear all data failed with status ${response.status}`)
    }

    const data: ClearAllDataResult = await response.json()

    if (!data.success) {
      throw new Error(data.errors.join(', ') || 'Clear all data failed.')
    }

    actionMessage.value = 'All data cleared.'
    await getCustomers()
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : 'Clear all data failed.'
  } finally {
    loading.value = false
  }
}

async function applyDiscounts() {
  loading.value = true
  errorMessage.value = null
  actionMessage.value = null

  try {
    const response = await fetch(`${customersApiUrl}/ApplyDiscounts`, {
      method: 'POST'
    })

    if (!response.ok) {
      throw new Error(`Apply discounts failed with status ${response.status}`)
    }

    const data: ApplyDiscountResult = await response.json()

    if (!data.success) {
      throw new Error(data.errors.join(', ') || 'Apply discounts failed.')
    }

    actionMessage.value = `${data.discountsApplied} discount(s) applied.`

    if (data.discountsApplied !== 0) {
      await getCustomers()
    }
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : 'Apply discounts failed.'
  } finally {
    loading.value = false
  }
}

async function getCustomers() {
  customersLoading.value = true
  errorMessage.value = null

  try {
    const response = await fetch(customersApiUrl)

    if (!response.ok) {
      throw new Error(`Failed to load customers with status ${response.status}`)
    }

    const data: GetCustomerResult = await response.json()

    if (!data.success) {
      throw new Error(data.errors.join(', ') || 'Failed to load customers.')
    }

    customers.value = data.customerDtos ?? []
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : 'Failed to load customers.'
  } finally {
    customersLoading.value = false
  }
}

async function uploadFile(endpoint: string, file: File | null) {
  if (!file) {
    errorMessage.value = 'Please choose a file first.'
    return
  }

  loading.value = true
  result.value = null
  errorMessage.value = null

  const formData = new FormData()
  formData.append('file', file)

  try {
    const response = await fetch(`${uploadApiBaseUrl}/${endpoint}`, {
      method: 'POST',
      body: formData
    })

    if (!response.ok) {
      throw new Error(`Upload failed with status ${response.status}`)
    }

    result.value = await response.json()

    await getCustomers()
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : 'Upload failed.'
  } finally {
    loading.value = false
  }
}

function toggleCustomer(customerId: number) {
  const updated = new Set(expandedCustomers.value)

  if (updated.has(customerId)) {
    updated.delete(customerId)
  } else {
    updated.add(customerId)
  }

  expandedCustomers.value = updated
}

function toggleOrder(orderId: number) {
  const updated = new Set(expandedOrders.value)

  if (updated.has(orderId)) {
    updated.delete(orderId)
  } else {
    updated.add(orderId)
  }

  expandedOrders.value = updated
}

function formatCurrency(value: number) {
  return new Intl.NumberFormat('en-GB', {
    style: 'currency',
    currency: 'GBP'
  }).format(value)
}

function formatDate(value: string | null) {
  if (!value) return '-'

  return new Intl.DateTimeFormat('en-GB').format(new Date(value))
}
</script>

<template>
  <main class="container">
    <h1>Customer Orders Import</h1>

    <nav class="tabs">
      <button
        :class="{ active: activeTab === 'customers' }"
        @click="activeTab = 'customers'"
      >
        Customers
      </button>

      <button
        :class="{ active: activeTab === 'upload' }"
        @click="activeTab = 'upload'"
      >
        Upload Files
      </button>
    </nav>

    <p v-if="errorMessage" class="error">
      {{ errorMessage }}
    </p>
    <p v-if="actionMessage" class="success">
      {{ actionMessage }}
    </p>

    <section v-if="activeTab === 'customers'">
      <h2>Customers</h2>
      <button @click="applyDiscounts" :disabled="loading || customersLoading">
        Apply Discounts
      </button>
      <p v-if="customersLoading">Loading customers...</p>

      <table v-else class="data-table">
        <thead>
          <tr>
            <th></th>
            <th>Name</th>
            <th>Email</th>
            <th>Location</th>
            <th>Orders</th>
            <th>Total Spend</th>
          </tr>
        </thead>

        <tbody>
          <template v-for="customer in customers" :key="customer.id">
            <tr>
              <td>
                <button class="expand-button" @click="toggleCustomer(customer.id)">
                  {{ expandedCustomers.has(customer.id) ? '-' : '+' }}
                </button>
              </td>
              <td>{{ customer.firstName }} {{ customer.lastName }}</td>
              <td>{{ customer.email }}</td>
              <td>{{ customer.city }}, {{ customer.state }}</td>
              <td>{{ customer.orders.length }}</td>
              <td>{{ formatCurrency(customer.totalSpend) }}</td>
            </tr>

            <tr v-if="expandedCustomers.has(customer.id)">
              <td colspan="6">
                <table class="nested-table">
                  <thead>
                    <tr>
                      <th></th>
                      <th>Order Date</th>
                      <th>Required Date</th>
                      <th>Shipped Date</th>
                      <th>Status</th>
                      <th>Total Order</th>
                    </tr>
                  </thead>

                  <tbody>
                    <template v-for="order in customer.orders" :key="order.id">
                      <tr>
                        <td>
                          <button class="expand-button" @click="toggleOrder(order.id)">
                            {{ expandedOrders.has(order.id) ? '-' : '+' }}
                          </button>
                        </td>
                        <td>{{ formatDate(order.orderDate) }}</td>
                        <td>{{ formatDate(order.requiredDate) }}</td>
                        <td>{{ formatDate(order.shippedDate) }}</td>
                        <td>{{ order.status }}</td>
                        <td>{{ formatCurrency(order.totalOrder) }}</td>
                      </tr>

                      <tr v-if="expandedOrders.has(order.id)">
                        <td colspan="6">
                          <table class="nested-table items-table">
                            <thead>
                              <tr>
                                <th>Item Id</th>
                                <th>List Price</th>
                                <th>Discount</th>
                                <th>Final Price</th>
                              </tr>
                            </thead>

                            <tbody>
                              <tr v-for="item in order.items" :key="item.id">
                                <td>{{ item.itemId }}</td>
                                <td>{{ formatCurrency(item.listPrice) }}</td>
                                <td>{{ item.discount }}</td>
                                <td>{{ formatCurrency(item.finalPrice) }}</td>
                              </tr>

                              <tr v-if="!order.items.length">
                                <td colspan="4">No items found for this order.</td>
                              </tr>
                            </tbody>
                          </table>
                        </td>
                      </tr>
                    </template>

                    <tr v-if="!customer.orders.length">
                      <td colspan="6">No orders found for this customer.</td>
                    </tr>
                  </tbody>
                </table>
              </td>
            </tr>
          </template>

          <tr v-if="!customers.length">
            <td colspan="6">No customers found.</td>
          </tr>
        </tbody>
      </table>
    </section>

    <section v-if="activeTab === 'upload'" class="upload-layout">
  <div class="upload-left">
    <section class="upload-card">
      <label>Customers file</label>
      <input type="file" @change="event => onFileSelected(event, 'customers')" />

      <button
        @click="uploadFile('customers', selectedCustomersFile)"
        :disabled="loading"
      >
        Upload Customers
      </button>
    </section>

    <section class="upload-card">
      <label>Orders file</label>
      <input type="file" @change="event => onFileSelected(event, 'orders')" />

      <button
        @click="uploadFile('orders', selectedOrdersFile)"
        :disabled="loading"
      >
        Upload Orders
      </button>
    </section>

    <section class="upload-card">
      <label>Order Items file</label>
      <input type="file" @change="event => onFileSelected(event, 'orderItems')" />

      <button
        @click="uploadFile('order-items', selectedOrderItemsFile)"
        :disabled="loading"
      >
        Upload OrderItems
      </button>
    </section>
        <section class="danger-card">
      <h2>Clear Data</h2>
      <p>This will remove all imported customers, orders and order items.</p>

      <button class="danger-button" @click="clearAllData" :disabled="loading">
        Clear All Data
      </button>
    </section>
  </div>

  <aside class="upload-right">
        <p v-if="loading">Uploading...</p>
    <section v-if="result" class="result sticky-result">
      <h2>Upload Result</h2>

      <p><strong>Success:</strong> {{ result.success }}</p>
      <p><strong>Total rows:</strong> {{ result.totalRows }}</p>
      <p><strong>Failed rows:</strong> {{ result.failedRows }}</p>

      <ul v-if="result.errors.length">
        <li v-for="err in result.errors" :key="err">
          {{ err }}
        </li>
      </ul>
    </section>
  </aside>
</section>
  </main>
</template>

<style scoped>
.container {
  padding: 2rem;
  font-family: Arial, sans-serif;
  max-width: 1100px;
}

.tabs {
  display: flex;
  gap: 0.75rem;
  margin-bottom: 1.5rem;
}

.tabs button {
  max-width: none;
  border: 1px solid #ccc;
  background: #f7f7f7;
}

.tabs button.active {
  background: #222;
  color: white;
}

.upload-card {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  padding: 1rem;
  margin-bottom: 1rem;
  border: 1px solid #ddd;
  border-radius: 8px;
}

button {
  padding: 0.6rem 1rem;
  font-size: 1rem;
  cursor: pointer;
  max-width: 220px;
}

button:disabled {
  cursor: not-allowed;
  opacity: 0.6;
}

.expand-button {
  padding: 0.25rem 0.5rem;
  min-width: 32px;
  max-width: 32px;
}

.error {
  color: darkred;
}

.result {
  margin-top: 1.5rem;
  padding: 1rem;
  border: 1px solid #ddd;
  border-radius: 8px;
}

.data-table,
.nested-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1rem;
}

.data-table th,
.data-table td,
.nested-table th,
.nested-table td {
  border: 1px solid #ddd;
  padding: 0.65rem;
  text-align: left;
  vertical-align: top;
}

.data-table th,
.nested-table th {
  background: #f3f3f3;
}

.nested-table {
  margin: 0.5rem 0;
  background: #fafafa;
}

.items-table {
  background: white;
}

.success {
  color: darkgreen;
}

.danger-card {
  padding: 1rem;
  margin-bottom: 1rem;
  border: 1px solid #d88;
  border-radius: 8px;
  background: #fff7f7;
}

.danger-button {
  background: darkred;
  color: white;
  border: 1px solid darkred;
}

.upload-layout {
  display: grid;
  grid-template-columns: 1fr 350px;
  gap: 1.5rem;
  align-items: start;
}

.upload-left {
  min-width: 0;
}

.upload-right {
  position: relative;
}

.sticky-result {
  position: sticky;
  top: 1rem;
}

@media (max-width: 900px) {
  .upload-layout {
    grid-template-columns: 1fr;
  }

  .sticky-result {
    position: static;
  }
}
</style>