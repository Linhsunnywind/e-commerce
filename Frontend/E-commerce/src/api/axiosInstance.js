import axios from 'axios'

const BASE_URL = import.meta.env.VITE_API_URL

export const publicClient = axios.create({
  baseURL: BASE_URL,
})

publicClient.interceptors.response.use(res => res.data)

export const privateClient = axios.create({
  baseURL: BASE_URL,
})

privateClient.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

privateClient.interceptors.response.use(
  res => res.data,
  err => {
    if (err.response?.status === 401 || err.response?.status === 403) {
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      // if (window.location.pathname !== '/login' && window.location.pathname !== '/register') {
      //   window.location.href = '/login'
      // }
    }
    return Promise.reject(err)
  }
)
