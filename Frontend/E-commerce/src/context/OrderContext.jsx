import { createContext, useContext, useState } from 'react'
import orderApi from '../api/orderApi.js'

const OrderContext = createContext(null)

export function OrderProvider({ children }) {
  const [orders, setOrders] = useState([])
  const [loading, setLoading] = useState(false)

  const fetchMyOrders = async () => {
    setLoading(true)
    try {
      const res = await orderApi.getMyOrders()
      setOrders(res.data)
    } finally { setLoading(false) }
  }

  const fetchAllOrders = async () => {
    setLoading(true)
    try {
      const res = await orderApi.getAll()
      setOrders(res.data)
    } finally { setLoading(false) }
  }

  const updateStatus = async (id, status) => {
    await orderApi.updateStatus(id, { orderStatus: status })
    setOrders(prev => prev.map(o => o.id === id ? { ...o, status } : o))
  }

  const cancelOrder = async (id) => {
    await orderApi.cancel(id)
    setOrders(prev => prev.map(o => o.id === id ? { ...o, status: 'Cancelled' } : o))
  }

  return (
    <OrderContext.Provider value={{ orders, loading, fetchMyOrders, fetchAllOrders, updateStatus, cancelOrder }}>
      {children}
    </OrderContext.Provider>
  )
}

export const useOrders = () => useContext(OrderContext)
