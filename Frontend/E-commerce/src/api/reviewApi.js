import { publicClient, privateClient } from './axiosInstance.js'

const reviewApi = {
  getByProduct: (productId)            => publicClient.get(`/products/${productId}/reviews`),
  create:       (productId, data)      => privateClient.post(`/products/${productId}/reviews`, data),
  update:       (productId, reviewId, data) => privateClient.put(`/products/${productId}/reviews/${reviewId}`, data),
  delete:       (productId, reviewId)  => privateClient.delete(`/products/${productId}/reviews/${reviewId}`)
}
export default reviewApi
