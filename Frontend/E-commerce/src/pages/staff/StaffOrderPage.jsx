import { useEffect, useState } from 'react';
import { Table, Tag, Button, Modal, Tabs, Space } from 'antd';
import { useOrders } from '../../context/OrderContext';

const STATUS_LABEL = { 0:'Chờ xử lý', 1:'Đang xử lý', 2:'Đang giao', 3:'Đã giao', 4:'Đã hủy' };
const STATUS_COLOR = { 0:'orange', 1:'blue', 2:'cyan', 3:'green', 4:'red' };
const ALLOWED_NEXT = { 0:[1,4], 1:[2,4], 2:[3], 3:[], 4:[] };
const ALL_TABS = [null, 0, 1, 2, 3, 4];
const formatPrice = p => new Intl.NumberFormat('vi-VN',{style:'currency',currency:'VND'}).format(p);

export default function StaffOrderPage() {
  const { orders, loading, fetchAllOrders, updateStatus } = useOrders();
  const [statusFilter, setStatusFilter] = useState('all');
  const [selected, setSelected] = useState(null);

  useEffect(() => { fetchAllOrders(); }, []);

  const filtered = statusFilter === 'all' ? orders : orders.filter(o => o.status === Number(statusFilter))
    .sort((a, b) => new Date(b.orderDate) - new Date(a.orderDate));

  const tabItems = [
    { key: 'all', label: `Tất cả (${orders.length})` },
    ...Object.entries(STATUS_LABEL).map(([k, label]) => ({
      key: String(k),
      label: `${label} (${orders.filter(o => o.status === Number(k)).length})`
    }))
  ];

  const columns = [
    { title: 'Mã đơn', dataIndex: 'id', key: 'id',
      render: v => <span className="text-blue-600 font-semibold">#{v.slice(0,8).toUpperCase()}</span> },
    { title: 'Địa chỉ', dataIndex: 'shippingAddress', key: 'addr',
      render: v => <span className="text-sm text-gray-600 truncate max-w-xs block">{v}</span> },
    { title: 'Ngày', dataIndex: 'orderDate', key: 'date',
      render: v => <span className="text-sm text-gray-500">{new Date(v).toLocaleDateString('vi-VN')}</span> },
    { title: 'Tổng', dataIndex: 'totalAmount', key: 'total',
      render: v => <span className="font-semibold">{formatPrice(v)}</span> },
    { title: 'Thanh toán', dataIndex: 'paymentMethodName', key: 'payment',
      render: v => <Tag>{v}</Tag> },
    { title: 'Trạng thái', dataIndex: 'status', key: 'status',
      render: v => <Tag color={STATUS_COLOR[v]}>{STATUS_LABEL[v]}</Tag> },
    { title: 'Hành động', key: 'action', render: (_, r) => (
      <Space wrap>
        <Button size="small" onClick={() => setSelected(r)}>Chi tiết</Button>
        {ALLOWED_NEXT[r.status]?.map(next => (
          <Button key={next} size="small" type="primary" onClick={() => updateStatus(r.id, next)}>
            → {STATUS_LABEL[next]}
          </Button>
        ))}
      </Space>
    )},
  ];

  return (
    <div className="flex flex-col gap-4">
      <Tabs activeKey={statusFilter} onChange={setStatusFilter} items={tabItems} />
      <Table
        columns={columns} dataSource={filtered} rowKey="id"
        loading={loading} scroll={{ x: true }} size="small"
        pagination={{ pageSize: 10 }}
      />

      <Modal title={`Chi tiết đơn #${selected?.id?.slice(0,8).toUpperCase()}`}
        open={!!selected} onCancel={() => setSelected(null)} footer={null}>
        {selected && (
          <div className="flex flex-col gap-3">
            <div>
              <p className="text-sm text-gray-500">{selected.shippingAddress}</p>
              <p className="text-xs text-gray-400">{selected.paymentMethodName}</p>
            </div>
            <div>
              {(selected.items || []).map((item, i) => (
                <div key={i} className="flex justify-between text-sm py-1.5 border-b border-gray-100 last:border-0">
                  <span className="flex-1">{item.variantName}</span>
                  <span className="text-gray-500 mx-3">×{item.orderQuantity}</span>
                  <span className="font-semibold">{formatPrice(item.unitPrice * item.orderQuantity)}</span>
                </div>
              ))}
            </div>
            <div className="flex justify-between font-bold text-base pt-2 border-t border-gray-200">
              <span>Tổng cộng</span>
              <span className="text-blue-600">{formatPrice(selected.totalAmount)}</span>
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
}
