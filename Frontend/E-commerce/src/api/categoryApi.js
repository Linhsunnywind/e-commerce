import { publicClient, privateClient } from "./axiosInstance.js";
const categoryApi = {
    getAll: () => publicClient.get('/categories'),
    create: (data) => privateClient.post('/categories', data),
    update: (id, data) => privateClient.put(`/categories/${id}`, data),
    delete: (id) => privateClient.delete(`/categories/${id}`)
}
export default categoryApi;
