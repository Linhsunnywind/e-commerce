import { useState, useEffect } from 'react';
import { Card, Switch, Tag } from 'antd';
import paymentMethodApi from '../../api/paymentMethodApi';

export default function PaymentMethodPage() {
  const [methods, setMethods] = useState([]);

  useEffect(() => {
    paymentMethodApi.getAll().then(res => setMethods(res.data || []));
  }, []);

  const toggle = async (m) => {
    await paymentMethodApi.update(m.id, { name: m.name, description: m.description, isActive: !m.isActive });
    setMethods(prev => prev.map(x => x.id === m.id ? { ...x, isActive: !x.isActive } : x));
  };

  return (
    <div className="flex flex-col gap-4">
      <p className="text-sm text-gray-500">Quản lý phương thức thanh toán hiển thị cho khách hàng.</p>
      <div className="flex flex-col gap-3">
        {methods.map(m => (
          <Card key={m.id} size="small">
            <div className="flex items-center gap-4">
              <div className="flex-1">
                <p className="font-semibold text-gray-800">{m.name}</p>
                <p className="text-sm text-gray-500">{m.description}</p>
              </div>
              <div className="flex items-center gap-3">
                <Tag color={m.isActive ? 'green' : 'default'}>{m.isActive ? 'Đang hoạt động' : 'Tắt'}</Tag>
                <Switch checked={m.isActive} onChange={() => toggle(m)} />
              </div>
            </div>
          </Card>
        ))}
      </div>
    </div>
  );
}
