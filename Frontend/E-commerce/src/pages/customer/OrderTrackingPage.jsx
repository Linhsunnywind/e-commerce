import { useEffect } from 'react';
import { useLocation } from 'react-router-dom';
import { Tag, Alert, Empty, Steps } from 'antd';
import { useAuth } from '../../context/AuthContext';
import { useOrders } from '../../context/OrderContext';

const STATUS_LABEL = { 0: 'Chờ xử lý', 1: 'Đang xử lý', 2: 'Đang giao', 3: 'Đã giao', 4: 'Đã hủy' };
const STATUS_COLOR = { 0: 'orange', 1: 'blue', 2: 'cyan', 3: 'green', 4: 'red' };
const formatPrice = (p) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(p);

export default function OrderTrackingPage() {
  const { user } = useAuth();
  const location = useLocation();
  const justOrdered = location.state?.justOrdered;
  const { orders, loading, fetchMyOrders } = useOrders();

  useEffect(() => { if (user) fetchMyOrders(); }, [user]);

  return (
    <div className="py-8 pb-16">
      <div className="container">
        <h1 className="text-2xl font-bold text-gray-800 mb-6">Đơn hàng của tôi</h1>

        {justOrdered && (
          <Alert message="Đặt hàng thành công!" description="Chúng tôi sẽ xử lý đơn hàng sớm nhất." type="success" showIcon className="mb-6" />
        )}

        {loading ? (
          <div className="text-center py-16 text-gray-400">Đang tải...</div>
        ) : orders.length === 0 ? (
          <Empty description="Bạn chưa có đơn hàng nào" className="py-16" />
        ) : (
          <div className="flex flex-col gap-4">
            {orders.map(order => (
              <div key={order.id} className="bg-white rounded-xl shadow-sm p-5">
                <div className="flex items-center justify-between mb-4">
                  <div>
                    <span className="font-bold text-blue-600">#{order.id.slice(0,8).toUpperCase()}</span>
                    <span className="text-sm text-gray-500"> · {new Date(order.orderDate).toLocaleDateString('vi-VN')}</span>
                  </div>
                  <Tag color={STATUS_COLOR[order.status]}>{STATUS_LABEL[order.status]}</Tag>
                </div>

                {order.status !== 4 && (
                  <Steps size="small" current={order.status}
                    items={[0,1,2,3].map(s => ({ title: STATUS_LABEL[s] }))}
                    className="mb-4"
                  />
                )}

                <div className="flex flex-col gap-2 mb-4">
                  {(order.items || []).map((item, i) => (
                    <div key={i} className="flex justify-between text-sm text-gray-700">
                      <span>{item.variantName} × {item.orderQuantity}</span>
                      <span className="font-semibold">{formatPrice(item.unitPrice * item.orderQuantity)}</span>
                    </div>
                  ))}
                </div>

                <div className="flex items-center justify-between pt-3 border-t border-gray-100">
                  <span className="text-sm text-gray-500">{order.paymentMethodName}</span>
                  <div className="text-sm">
                    Tổng: <strong className="text-lg text-blue-600">{formatPrice(order.totalAmount)}</strong>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
