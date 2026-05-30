import { publicClient, privateClient } from './axiosInstance.js'

const voucherApi = {
  getAll:   ()        => publicClient.get('/vouchers'),
  getById:  (id)      => publicClient.get(`/vouchers/${id}`),
  validate: (data)    => publicClient.post('/vouchers/validate', data),
  create:   (data)    => privateClient.post('/vouchers', data),
  update:   (id, data)=> privateClient.patch(`/vouchers/${id}`, data),
  delete:   (id)      => privateClient.delete(`/vouchers/${id}`)
}
export default voucherApi
