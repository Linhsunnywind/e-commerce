import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Card, Table, Tag } from 'antd';
import { useOrders } from '../../context/OrderContext';
import reportApi from '../../api/reportApi';
import { productAPi } from '../../api/productApi';

const formatPrice = p => new Intl.NumberFormat('vi-VN',{style:'currency',currency:'VND'}).format(p);
const STATUS_COLOR = { 0:'orange', 1:'blue', 2:'cyan', 3:'green', 4:'red' };
const STATUS_LABEL = { 0:'Chờ xử lý', 1:'Đang xử lý', 2:'Đang giao', 3:'Đã giao', 4:'Đã hủy' };

export default function DashboardPage() {
  const { orders, fetchAllOrders } = useOrders();
  const [orderStats, setOrderStats] = useState([]);
  const [topCustomers, setTopCustomers] = useState([]);
  const [totalProducts, setTotalProducts] = useState(0);

  useEffect(() => {
    fetchAllOrders();
    reportApi.getOrders().then(res => setOrderStats(res.data || []));
    reportApi.getCustomers().then(res => setTopCustomers(res.data || []));
    productAPi.getAll({ pageSize: 1 }).then(res => setTotalProducts(res.data?.totalItems ?? 0));
  }, []);

  const totalRevenue = orderStats.filter(o => o.status === 'Đã giao').reduce((s, o) => s + (o.totalRevenue || 0), 0);
  const totalOrders = orderStats.reduce((s, o) => s + (o.count || 0), 0);
  const recentOrders = [...orders].sort((a,b) => new Date(b.orderDate) - new Date(a.orderDate)).slice(0, 5);

  const statsCards = [
    { icon: '💰', label: 'Tổng doanh thu', value: formatPrice(totalRevenue), color: '#2563eb' },
    { icon: '🛒', label: 'Tổng đơn hàng', value: totalOrders, color: '#10b981' },
    { icon: '👥', label: 'Top khách hàng', value: topCustomers.length, color: '#f59e0b' },
    { icon: '📦', label: 'Tổng sản phẩm', value: totalProducts, color: '#8b5cf6' },
  ];

  const topCustColumns = [
    { title: '#', key: 'rank', width: 40, render: (_, __, i) => (
      <div className="w-6 h-6 rounded-full bg-blue-600 text-white text-xs font-bold flex items-center justify-center">{i+1}</div>
    )},
    { title: 'Khách hàng', dataIndex: 'customerName', key: 'name', render: v => <span className="font-medium">{v}</span> },
    { title: 'Đơn hàng', dataIndex: 'orderCount', key: 'orders' },
    { title: 'Chi tiêu', dataIndex: 'totalSpent', key: 'spent', render: v => <span className="text-blue-600 font-semibold">{formatPrice(v)}</span> },
  ];

  const recentCols = [
    { title: 'Mã đơn', dataIndex: 'id', key: 'id',
      render: v => <span className="text-blue-600 font-semibold">#{v.slice(0,8).toUpperCase()}</span> },
    { title: 'Ngày', dataIndex: 'orderDate', key: 'date',
      render: v => <span className="text-gray-500">{new Date(v).toLocaleDateString('vi-VN')}</span> },
    { title: 'Tổng', dataIndex: 'totalAmount', key: 'total',
      render: v => <span className="font-semibold">{formatPrice(v)}</span> },
    { title: 'Trạng thái', dataIndex: 'status', key: 'status',
      render: v => <Tag color={STATUS_COLOR[v]}>{STATUS_LABEL[v]}</Tag> },
  ];

  return (
    <div className="flex flex-col gap-5">
      <div className="grid grid-cols-4 gap-4 lg:grid-cols-2">
        {statsCards.map(s => (
          <Card key={s.label}>
            <div className="flex items-center gap-4">
              <div className="w-12 h-12 rounded-xl flex items-center justify-center text-2xl flex-shrink-0" style={{ background: s.color + '18' }}>{s.icon}</div>
              <div><p className="text-sm text-gray-500">{s.label}</p><p className="text-xl font-bold text-gray-800">{s.value}</p></div>
            </div>
          </Card>
        ))}
      </div>

      <div className="grid grid-cols-2 gap-5 md:grid-cols-1">
        <Card title="Thống kê đơn hàng theo trạng thái">
          <div className="flex flex-col gap-2">
            {orderStats.map(s => (
              <div key={s.status} className="flex items-center justify-between py-1.5 border-b border-gray-50 last:border-0">
                <span className="text-sm text-gray-600">{s.status}</span>
                <div className="flex gap-4 text-sm">
                  <span className="font-semibold">{s.count} đơn</span>
                  <span className="text-blue-600">{formatPrice(s.totalRevenue)}</span>
                </div>
              </div>
            ))}
          </div>
        </Card>
        <Card title="Top khách hàng" extra={<Link to="/admin/customers" className="text-blue-600 text-sm">Xem tất cả</Link>}>
          <Table columns={topCustColumns} dataSource={topCustomers} rowKey="customerId" pagination={false} size="small" />
        </Card>
      </div>

      <Card title="Đơn hàng gần đây" extra={<Link to="/admin/orders" className="text-blue-600 text-sm">Xem tất cả</Link>}>
        <Table columns={recentCols} dataSource={recentOrders} rowKey="id" pagination={false} size="small" scroll={{ x: true }} />
      </Card>
    </div>
  );
}
