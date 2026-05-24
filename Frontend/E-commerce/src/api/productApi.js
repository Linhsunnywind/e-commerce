import { publicClient, privateClient } from "./axiosInstance.js";
export const productAPi = {
    getAll: (params) => publicClient.get('/products', { params }),
    getById: (id) => publicClient.get(`/products/${id}`),
    create: (data) => privateClient.post('/products', data),
    update: (id, data) => privateClient.put(`/products/${id}`, data),
    delete: (id) => privateClient.delete(`/products/${id}`)
}
