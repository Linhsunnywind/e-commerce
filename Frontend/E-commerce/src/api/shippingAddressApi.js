import { privateClient } from './axiosInstance.js'

const shippingAddressApi = {
  getAll:     ()         => privateClient.get('/shipping-addresses'),
  create:     (data)     => privateClient.post('/shipping-addresses', data),
  update:     (id, data) => privateClient.put(`/shipping-addresses/${id}`, data),
  delete:     (id)       => privateClient.delete(`/shipping-addresses/${id}`),
  setDefault: (id)       => privateClient.patch(`/shipping-addresses/${id}/default`)
}

export default shippingAddressApi
