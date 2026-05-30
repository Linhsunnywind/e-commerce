import { useState, useEffect } from 'react';
import { Table, Button, Modal, Input, Space } from 'antd';
import { SearchOutlined, EyeOutlined } from '@ant-design/icons';
import { privateClient } from '../../api/axiosInstance';

const formatPrice = p => new Intl.NumberFormat('vi-VN',{style:'currency',currency:'VND'}).format(p);

export default function CustomerManagementPage() {
  const [customers, setCustomers] = useState([]);
  const [loading, setLoading] = useState(false);
  const [search, setSearch] = useState('');
  const [selected, setSelected] = useState(null);

  useEffect(() => {
    setLoading(true);
    privateClient.get('/admins/customers')
      .then(res => setCustomers(res.data || []))
      .catch(() => setCustomers([]))
      .finally(() => setLoading(false));
  }, []);

  const filtered = customers.filter(c =>
    (c.fullName || '').toLowerCase().includes(search.toLowerCase()) ||
    c.email.toLowerCase().includes(search.toLowerCase()) ||
    (c.phoneNumber || '').includes(search)
  );

  const columns = [
    { title: 'Khách hàng', key: 'name', render: (_, r) => (
      <div className="flex items-center gap-2.5">
        <div className="w-9 h-9 rounded-full bg-blue-600 text-white text-sm font-bold flex items-center justify-center flex-shrink-0">
          {(r.fullName || r.email)[0].toUpperCase()}
        </div>
        <span className="font-medium">{r.fullName || '—'}</span>
      </div>
    )},
    { title: 'Liên hệ', key: 'contact', render: (_, r) => (
      <div><p className="text-sm">{r.email}</p><p className="text-xs text-gray-500">{r.phoneNumber}</p></div>
    )},
    { title: 'Thao tác', key: 'action',
      render: (_, r) => <Button size="small" icon={<EyeOutlined />} onClick={() => setSelected(r)}>Xem</Button> },
  ];

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center gap-4">
        <Input prefix={<SearchOutlined />} placeholder="Tìm tên, email, SĐT..."
          value={search} onChange={e => setSearch(e.target.value)} className="max-w-sm" />
        <span className="text-sm text-gray-500">Tổng: <strong>{customers.length}</strong></span>
      </div>
      <Table columns={columns} dataSource={filtered} rowKey="id"
        loading={loading} scroll={{ x: true }} size="small" pagination={{ pageSize: 10 }} />
      <Modal title="Thông tin khách hàng" open={!!selected} onCancel={() => setSelected(null)} footer={null}>
        {selected && (
          <div className="flex flex-col gap-3">
            <div className="flex items-center gap-4">
              <div className="w-16 h-16 rounded-full bg-blue-600 text-white text-2xl font-bold flex items-center justify-center">
                {(selected.fullName || selected.email)[0].toUpperCase()}
              </div>
              <div><p className="text-xl font-bold">{selected.fullName || '—'}</p><p className="text-sm text-gray-500">{selected.roleName}</p></div>
            </div>
            {[['📧', 'Email', selected.email],['📞', 'SĐT', selected.phoneNumber]].map(([icon, label, val]) => (
              <div key={label} className="flex gap-4 text-sm">
                <span className="w-28 text-gray-500">{icon} {label}</span>
                <span className="font-medium">{val || '—'}</span>
              </div>
            ))}
          </div>
        )}
      </Modal>
    </div>
  );
}
