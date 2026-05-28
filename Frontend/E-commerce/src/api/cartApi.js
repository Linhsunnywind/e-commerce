import { privateClient } from './axiosInstance.js'

const cartApi = {
  getCart:    ()         => privateClient.get('/cart'),
  addItem:    (data)     => privateClient.post('/cart/add', data),
  updateItem: (data)      => privateClient.put('/cart/update', data),
  deleteItem: (variantId)=> privateClient.delete(`/cart/remove?productVariantId=${variantId}`),
  clearCart:  ()         => privateClient.delete('/cart')
}
export default cartApi;
