import { publicClient } from './axiosInstance.js'

const authApi = {
  login:    (data) => publicClient.post('/auth/login', data),
  register: (data) => publicClient.post('/auth/register', data)
}
export default authApi
