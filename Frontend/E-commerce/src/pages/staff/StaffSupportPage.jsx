import { useState, useEffect } from 'react';
import { Table, Tag, Button, Modal, Spin, Select, message } from 'antd';
import supportApi from '../../api/supportApi';

const STATUS_LABEL = { Pending: 'Chờ xử lý', InProgress: 'Đang xử lý', Resolved: 'Đã giải quyết' };
const STATUS_COLOR = { Pending: 'orange', InProgress: 'blue', Resolved: 'green' };
const STATUS_OPTIONS = [
  { value: 'Pending',    label: 'Chờ xử lý' },
  { value: 'InProgress', label: 'Đang xử lý' },
  { value: 'Resolved',   label: 'Đã giải quyết' },
];

export default function StaffSupportPage() {
  const [tickets, setTickets] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selected, setSelected] = useState(null);
  const [updating, setUpdating] = useState(false);

  useEffect(() => {
    const fetch = async () => {
      try {
        const res = await supportApi.getAll();
        setTickets(res.data || []);
      } catch {
        setTickets([]);
      } finally {
        setLoading(false);
      }
    };
    fetch();
  }, []);

  const handleStatusChange = async (newStatus) => {
    setUpdating(true);
    try {
      const res = await supportApi.updateStatus(selected.id, newStatus);
      const updated = res.data;
      setTickets(prev => prev.map(t => t.id === updated.id ? updated : t));
      setSelected(updated);
      message.success('Đã cập nhật trạng thái');
    } catch {
      message.error('Cập nhật thất bại');
    } finally {
      setUpdating(false);
    }
  };

  const columns = [
    {
      title: 'Khách hàng',
      dataIndex: 'customerName',
      key: 'name',
      render: v => <span className="font-medium">{v}</span>,
    },
    { title: 'Chủ đề', dataIndex: 'subject', key: 'subject' },
    {
      title: 'Ngày tạo',
      dataIndex: 'createdDate',
      key: 'createdDate',
      render: v => <span className="text-sm text-gray-500">{new Date(v).toLocaleDateString('vi-VN')}</span>,
    },
    {
      title: 'Trạng thái',
      dataIndex: 'status',
      key: 'status',
      render: v => <Tag color={STATUS_COLOR[v] ?? 'default'}>{STATUS_LABEL[v] ?? v}</Tag>,
    },
    {
      title: 'Thao tác',
      key: 'action',
      render: (_, r) => (
        <Button size="small" onClick={() => setSelected(r)}>Xem chi tiết</Button>
      ),
    },
  ];

  return (
    <div className="flex flex-col gap-4">
      <Spin spinning={loading}>
        <Table
          columns={columns}
          dataSource={tickets}
          rowKey="id"
          scroll={{ x: true }}
          size="small"
          pagination={{ pageSize: 10 }}
        />
      </Spin>

      <Modal
        title="Chi tiết yêu cầu hỗ trợ"
        open={!!selected}
        onCancel={() => setSelected(null)}
        footer={<Button onClick={() => setSelected(null)}>Đóng</Button>}
        width={600}
      >
        {selected && (
          <div className="flex flex-col gap-4 py-2">
            <div className="bg-gray-50 rounded-lg p-4">
              <p className="font-semibold mb-1">{selected.subject}</p>
              <p className="text-xs text-gray-500 mb-3">
                Từ: {selected.customerName} · {new Date(selected.createdDate).toLocaleDateString('vi-VN')}
              </p>
              <p className="text-sm text-gray-700">{selected.message}</p>
            </div>
            <div className="flex items-center gap-2">
              <span className="text-sm text-gray-600">Trạng thái:</span>
              <Select
                value={selected.status}
                onChange={handleStatusChange}
                loading={updating}
                options={STATUS_OPTIONS}
                style={{ width: 160 }}
              />
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
}
