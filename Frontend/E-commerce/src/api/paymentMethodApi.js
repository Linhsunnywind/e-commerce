import { publicClient, privateClient } from './axiosInstance'

const paymentMethodApi = {
  getAll: ()         => publicClient.get('/payment-methods'),
  create: (data)     => privateClient.post('/payment-methods', data),
  update: (id, data) => privateClient.put(`/payment-methods/${id}`, data),
  delete: (id)       => privateClient.delete(`/payment-methods/${id}`)
}
export default paymentMethodApi
