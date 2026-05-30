import { privateClient } from './axiosInstance'

const imageApi = {
  upload: (productId, formData) => privateClient.post(`/products/${productId}/images`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }),
  delete: (imageId) => privateClient.delete(`/images/${imageId}`)
}
export default imageApi
