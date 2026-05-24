import { publicClient, privateClient } from "./axiosInstance.js";

const brandApi = {
    getAll:         ()        => publicClient.get('/brands'),
    create:         (data)    => privateClient.post('/brands', data),
    update:         (id, data)=> privateClient.put(`/brands/${id}`, data),
    delete:         (id)      => privateClient.delete(`/brands/${id}`)
}

export default brandApi;