import { useState, useEffect } from 'react';
import { Card, Table } from 'antd';
import reportApi from '../../api/reportApi';

const formatPrice = p => new Intl.NumberFormat('vi-VN',{style:'currency',currency:'VND'}).format(p);

export default function ReportPage() {
  const [revenue, setRevenue] = useState(null);
  const [orderStats, setOrderStats] = useState([]);
  const [topCustomers, setTopCustomers] = useState([]);

  useEffect(() => {
    const now = new Date();
    const start = new Date(now.getFullYear(), 0, 1).toISOString();
    const end = now.toISOString();
    reportApi.getRevenue({ params: { startDate: start, endDate: end } })
      .then(res => setRevenue(res.data))
      .catch(() => {});
    reportApi.getOrders().then(res => setOrderStats(res.data || []));
    reportApi.getCustomers().then(res => setTopCustomers(res.data || []));
  }, []);

  const summaryStats = [
    { icon: '💰', label: `Doanh thu năm ${new Date().getFullYear()}`, value: formatPrice(revenue?.totalRevenue || 0) },
    { icon: '🛒', label: 'Tổng đơn hàng', value: revenue?.totalOrders || orderStats.reduce((s,o)=>s+o.count,0) },
    { icon: '💡', label: 'Trung bình / đơn', value: formatPrice(revenue?.totalOrders ? (revenue.totalRevenue / revenue.totalOrders) : 0) },
  ];

  const orderCols = [
    { title: 'Trạng thái', dataIndex: 'status', key: 'status', render: v => <span className="font-medium">{v}</span> },
    { title: 'Số đơn', dataIndex: 'count', key: 'count', render: v => <span className="font-semibold">{v}</span> },
    { title: 'Doanh thu', dataIndex: 'totalRevenue', key: 'rev', render: v => <span className="text-blue-600">{formatPrice(v)}</span> },
    { title: 'TB / đơn', dataIndex: 'averageOrderValue', key: 'avg', render: v => formatPrice(v) },
  ];
  const custCols = [
    { title: '#', key: 'r', width: 40, render: (_, __, i) => (
      <div className="w-6 h-6 rounded-full bg-blue-600 text-white text-xs font-bold flex items-center justify-center">{i+1}</div>
    )},
    { title: 'Khách hàng', dataIndex: 'customerName', key: 'name', render: v => <span className="font-medium">{v}</span> },
    { title: 'Email', dataIndex: 'email', key: 'email', render: v => <span className="text-sm text-gray-500">{v}</span> },
    { title: 'Số đơn', dataIndex: 'orderCount', key: 'orders' },
    { title: 'Chi tiêu', dataIndex: 'totalSpent', key: 'spent', render: v => <span className="text-blue-600 font-semibold">{formatPrice(v)}</span> },
  ];

  return (
    <div className="flex flex-col gap-5">
      <div className="grid grid-cols-3 gap-4">
        {summaryStats.map(s => (
          <Card key={s.label}>
            <div className="flex items-center gap-4">
              <div className="text-3xl">{s.icon}</div>
              <div><p className="text-sm text-gray-500">{s.label}</p><p className="text-xl font-bold">{s.value}</p></div>
            </div>
          </Card>
        ))}
      </div>
      <div className="grid grid-cols-2 gap-5 md:grid-cols-1">
        <Card title="Thống kê theo trạng thái đơn hàng">
          <Table columns={orderCols} dataSource={orderStats} rowKey="status" pagination={false} size="small" />
        </Card>
        <Card title="Top khách hàng chi tiêu nhiều nhất">
          <Table columns={custCols} dataSource={topCustomers} rowKey="customerId" pagination={false} size="small" />
        </Card>
      </div>
    </div>
  );
}
