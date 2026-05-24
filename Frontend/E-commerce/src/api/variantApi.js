import { publicClient, privateClient } from './axiosInstance.js'

const variantApi = {
  getByProduct: (productId)       => publicClient.get(`/products/${productId}/variants`),
  create:       (productId, data) => privateClient.post(`/products/${productId}/variants`, data),
  update:       (id, data)        => privateClient.put(`/variants/${id}`, data),
  delete:       (id)              => privateClient.delete(`/variants/${id}`)
}
export default variantApi;