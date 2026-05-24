import { privateClient } from './axiosInstance'

const staffApi = {
  getAll:  ()         => privateClient.get('/staffs'),
  getById: (id)       => privateClient.get(`/staffs/${id}`),
  create:  (data)     => privateClient.post('/staffs', data),
  update:  (id, data) => privateClient.put(`/staffs/${id}`, data),
  delete:  (id)       => privateClient.delete(`/staffs/${id}`)
}
export default staffApi
