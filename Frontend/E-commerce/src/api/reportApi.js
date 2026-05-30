import { privateClient } from './axiosInstance'

const reportApi = {
  getRevenue: (config) => privateClient.get('/reports/revenue', config),
  getOrders:    () => privateClient.get('/reports/orders'),
  getCustomers: () => privateClient.get('/reports/customers')
}
export default reportApi
