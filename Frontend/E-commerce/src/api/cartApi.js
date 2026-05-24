import { privateClient } from './axiosInstance.js'

const cartApi = {
  getCart:    ()        => privateClient.get('/cart'),
  addItem:    (data)    => privateClient.post('/cart/items', data),
  updateItem: (id, data)=> privateClient.put(`/cart/items/${id}`, data),
  deleteItem: (id)      => privateClient.delete(`/cart/items/${id}`),
  clearCart:  ()        => privateClient.delete('/cart')
}
export default cartApi;
