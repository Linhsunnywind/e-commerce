import { privateClient } from './axiosInstance.js'

const supportApi = {
  getMyTickets: () => privateClient.get('/supports'),
  getAll:       () => privateClient.get('/supports/all'),
  create:       (data) => privateClient.post('/supports', data),
  updateStatus: (id, status) => privateClient.patch(`/supports/${id}/status`, { status })
}
export default supportApi
