import { useEffect, useState } from 'react';
import { Table, Tag, Button, Modal, Tabs, Input, Space } from 'antd';
import { SearchOutlined } from '@ant-design/icons';
import { useOrders } from '../../context/OrderContext';

const STATUS_LABEL = { 0:'Chờ xử lý', 1:'Đang xử lý', 2:'Đang giao', 3:'Đã giao', 4:'Đã hủy' };
const STATUS_COLOR = { 0:'orange', 1:'blue', 2:'cyan', 3:'green', 4:'red' };
const formatPrice = p => new Intl.NumberFormat('vi-VN',{style:'currency',currency:'VND'}).format(p);

export default function OrderManagementPage() {
  const { orders, loading, fetchAllOrders, updateStatus } = useOrders();
  const [filter, setFilter] = useState('all');
  const [search, setSearch] = useState('');
  const [selected, setSelected] = useState(null);

  useEffect(() => { fetchAllOrders(); }, []);

  const filtered = orders.filter(o => {
    const matchStatus = filter === 'all' || o.status === Number(filter);
    const matchSearch = !search || o.id.toLowerCase().includes(search.toLowerCase()) ||
      o.shippingAddress?.toLowerCase().includes(search.toLowerCase());
    return matchStatus && matchSearch;
  }).sort((a, b) => new Date(b.orderDate) - new Date(a.orderDate));;

  const tabItems = [
    { key: 'all', label: `Tất cả (${orders.length})` },
    ...Object.entries(STATUS_LABEL).map(([k, label]) => ({
      key: String(k), label: `${label} (${orders.filter(o => o.status === Number(k)).length})`
    }))
  ];

  const columns = [
    { title: 'Mã đơn', dataIndex: 'id', key: 'id',
      render: v => <span className="text-blue-600 font-semibold">#{v.slice(0,8).toUpperCase()}</span> },
    { title: 'Địa chỉ', dataIndex: 'shippingAddress', key: 'addr',
      render: v => <span className="text-sm text-gray-600 truncate max-w-xs block">{v}</span> },
    { title: 'Ngày', dataIndex: 'orderDate', key: 'date',
      render: v => <span className="text-gray-500">{new Date(v).toLocaleDateString('vi-VN')}</span> },
    { title: 'Tổng', dataIndex: 'totalAmount', key: 'total',
      render: v => <span className="font-semibold">{formatPrice(v)}</span> },
    { title: 'Thanh toán', dataIndex: 'paymentMethodName', key: 'pay', render: v => <Tag>{v}</Tag> },
    { title: 'Trạng thái', dataIndex: 'status', key: 'status',
      render: v => <Tag color={STATUS_COLOR[v]}>{STATUS_LABEL[v]}</Tag> },
    { title: 'Thao tác', key: 'action', render: (_, r) => (
      <Button size="small" onClick={() => setSelected(r)}>Chi tiết</Button>
    )},
  ];

  return (
    <div className="flex flex-col gap-4">
      <Tabs activeKey={filter} onChange={setFilter} items={tabItems} />
      <Input prefix={<SearchOutlined />} placeholder="Tìm mã đơn, địa chỉ..."
        value={search} onChange={e => setSearch(e.target.value)} className="max-w-sm" />
      <Table columns={columns} dataSource={filtered} rowKey="id" loading={loading}
        scroll={{ x: true }} size="small" pagination={{ pageSize: 10 }} />

      <Modal title={`Chi tiết đơn #${selected?.id?.slice(0,8).toUpperCase()}`}
        open={!!selected} onCancel={() => setSelected(null)} footer={null} width={680}>
        {selected && (
          <div className="flex flex-col gap-4">
            <div className="grid grid-cols-2 gap-4">
              <div><p className="text-xs font-semibold text-gray-500 uppercase mb-1">Giao hàng</p>
                <p className="text-sm">{selected.shippingAddress}</p></div>
              <div><p className="text-xs font-semibold text-gray-500 uppercase mb-1">Thanh toán</p>
                <p className="text-sm">{selected.paymentMethodName}</p></div>
            </div>
            <div>
              {(selected.items || []).map((item, i) => (
                <div key={i} className="flex justify-between py-1.5 border-b border-gray-100 last:border-0 text-sm">
                  <span className="flex-1">{item.productName} - {item.variantName}</span>
                  <span className="text-gray-500 mx-4">×{item.orderQuantity}</span>
                  <span className="font-semibold">{formatPrice(item.unitPrice * item.orderQuantity)}</span>
                </div>
              ))}
              <div className="flex justify-between font-bold text-base pt-3 border-t border-gray-200 mt-2">
                <span>Tổng cộng</span>
                <span className="text-blue-600">{formatPrice(selected.totalAmount)}</span>
              </div>
            </div>
            <div>
              <p className="text-xs font-semibold text-gray-500 uppercase mb-2">Cập nhật trạng thái</p>
              <div className="flex gap-2 flex-wrap">
                {Object.entries(STATUS_LABEL).map(([k, label]) => (
                  <Button key={k} size="small"
                    type={selected.status === Number(k) ? 'primary' : 'default'}
                    onClick={() => { updateStatus(selected.id, Number(k)); setSelected(p => ({...p, status: Number(k)})); }}>
                    {label}
                  </Button>
                ))}
              </div>
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
}
