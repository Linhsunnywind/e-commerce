import { privateClient } from './axiosInstance'

const userApi = {
  getProfile:      ()     => privateClient.get('/user/profile'),
  updateProfile:   (data) => privateClient.put('/user/profile', data),
  deleteProfile:   ()     => privateClient.delete('/user/profile'),
  changePassword:  (data) => privateClient.put('/user/password', data),
  createStaff:     (data) => privateClient.post('/user/create-staff-admin', data)
}
export default userApi
