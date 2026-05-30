import { useState, useEffect } from 'react';
import { Table, Button, Tag, Modal, Form, Input, Select, InputNumber, Space, DatePicker, Switch, Spin, message } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import voucherApi from '../../api/voucherApi';

// DiscountType: 0 = Percentage, 1 = FixedAmount
const DISCOUNT_TYPE_LABEL = { 0: 'Phần trăm (%)', 1: 'Cố định (VND)' };

const formatPrice = (p) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(p);

const getStatus = (v) => {
  const now = new Date();
  if (!v.isActive) return { label: 'Tắt', color: 'default' };
  if (new Date(v.endDate) < now) return { label: 'Hết hạn', color: 'default' };
  if (new Date(v.startDate) > now) return { label: 'Sắp diễn ra', color: 'blue' };
  return { label: 'Đang hoạt động', color: 'green' };
};

export default function PromotionManagementPage() {
  const [vouchers, setVouchers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editing, setEditing] = useState(null);
  const [saving, setSaving] = useState(false);
  const [form] = Form.useForm();
  const discountType = Form.useWatch('discountType', form);
  const vndFormatter = val => `${val}`.replace(/\B(?=(\d{3})+(?!\d))/g, '.');
  const vndParser = val => val.replace(/\./g, '');

  const loadVouchers = async () => {
    setLoading(true);
    try {
      const res = await voucherApi.getAll();
      setVouchers(res.data || []);
    } catch {
      setVouchers([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { loadVouchers(); }, []);

  const openAdd = () => {
    setEditing(null);
    form.resetFields();
    form.setFieldsValue({ discountType: 0, isActive: true });
    setShowModal(true);
  };

  const openEdit = (v) => {
    setEditing(v.id);
    form.setFieldsValue({
      ...v,
      startDate: dayjs(v.startDate),
      endDate: dayjs(v.endDate),
    });
    setShowModal(true);
  };

  const handleDelete = (id) => {
    Modal.confirm({
      title: 'Xóa voucher này?',
      okButtonProps: { danger: true },
      onOk: async () => {
        try {
          await voucherApi.delete(id);
          message.success('Đã xóa');
          loadVouchers();
        } catch {
          message.error('Xóa thất bại');
        }
      },
    });
  };

  const handleSave = async (values) => {
    setSaving(true);
    const payload = {
      ...values,
      startDate: values.startDate.toISOString(),
      endDate: values.endDate.toISOString(),
    };
    try {
      if (editing) {
        await voucherApi.update(editing, payload);
        message.success('Đã cập nhật');
      } else {
        await voucherApi.create(payload);
        message.success('Đã tạo voucher');
      }
      setShowModal(false);
      loadVouchers();
    } catch {
      message.error('Lưu thất bại');
    } finally {
      setSaving(false);
    }
  };

  const columns = [
    {
      title: 'Mã',
      dataIndex: 'code',
      render: v => (
        <span className="font-mono bg-gray-100 text-blue-600 font-bold px-2 py-0.5 rounded text-sm">{v}</span>
      ),
    },
    {
      title: 'Loại',
      dataIndex: 'discountType',
      render: v => DISCOUNT_TYPE_LABEL[v] ?? v,
    },
    {
      title: 'Giá trị',
      render: (_, r) => (
        <span className="font-semibold">
          {r.discountType === 0 ? `${r.discountValue}%` : formatPrice(r.discountValue)}
        </span>
      ),
    },
    {
      title: 'Đơn tối thiểu',
      dataIndex: 'minOrderAmount',
      render: v => <span className="text-sm">{formatPrice(v)}</span>,
    },
    {
      title: 'Đã dùng / Tổng',
      render: (_, r) => <span className="text-sm">{r.usedCount}/{r.totalQuantity}</span>,
    },
    {
      title: 'Hiệu lực',
      render: (_, r) => (
        <span className="text-xs text-gray-500">
          {dayjs(r.startDate).format('DD/MM/YYYY')} → {dayjs(r.endDate).format('DD/MM/YYYY')}
        </span>
      ),
    },
    {
      title: 'Trạng thái',
      render: (_, r) => {
        const s = getStatus(r);
        return <Tag color={s.color}>{s.label}</Tag>;
      },
    },
    {
      title: 'Thao tác',
      render: (_, r) => (
        <Space>
          <Button size="small" icon={<EditOutlined />} onClick={() => openEdit(r)} />
          <Button size="small" danger icon={<DeleteOutlined />} onClick={() => handleDelete(r.id)} />
        </Space>
      ),
    },
  ];

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <span className="text-sm text-gray-500">Tổng <strong>{vouchers.length}</strong> voucher</span>
        <Button type="primary" icon={<PlusOutlined />} onClick={openAdd}>Tạo voucher mới</Button>
      </div>

      <Spin spinning={loading}>
        <Table
          columns={columns}
          dataSource={vouchers}
          rowKey="id"
          scroll={{ x: true }}
          size="small"
          pagination={{ pageSize: 10 }}
        />
      </Spin>

      <Modal
        title={editing ? 'Sửa voucher' : 'Tạo voucher mới'}
        open={showModal}
        onCancel={() => setShowModal(false)}
        footer={null}
        width={640}
      >
        <Form form={form} layout="vertical" onFinish={handleSave} className="mt-4">
          <div className="grid grid-cols-2 gap-x-4">
            <Form.Item label="Mã voucher *" name="code" rules={[{ required: true }]}>
              <Input
                style={{ textTransform: 'uppercase' }}
                onChange={e => form.setFieldValue('code', e.target.value.toUpperCase())}
              />
            </Form.Item>
            <Form.Item label="Loại giảm giá *" name="discountType" rules={[{ required: true }]}>
              <Select>
                <Select.Option value={0}>Phần trăm (%)</Select.Option>
                <Select.Option value={1}>Cố định (VND)</Select.Option>
              </Select>
            </Form.Item>
            <Form.Item label="Giá trị giảm *" name="discountValue" rules={[{ required: true, message: 'Bắt buộc' }, { type: 'number', min: 0.01, message: 'Phải lớn hơn 0' }]}>
              <InputNumber className="w-full" min={0.01}
                formatter={vndFormatter}
                parser={vndParser}
              />
            </Form.Item>
            <Form.Item label="Giảm tối đa (VND)" name="maxDiscountAmount">
              <InputNumber className="w-full" min={0}
                formatter={vndFormatter}
                parser={vndParser}
              />
            </Form.Item>
            <Form.Item label="Đơn tối thiểu (VND)" name="minOrderAmount">
              <InputNumber className="w-full" min={0}
                formatter={vndFormatter}
                parser={vndParser}
              />
            </Form.Item>
            <Form.Item label="Số lượng *" name="totalQuantity" rules={[{ required: true }]}>
              <InputNumber className="w-full" min={1}
                formatter={vndFormatter}
                parser={vndParser}
              />
            </Form.Item>
            <Form.Item label="Ngày bắt đầu *" name="startDate" rules={[{ required: true }]}>
              <DatePicker className="w-full" />
            </Form.Item>
            <Form.Item label="Ngày kết thúc *" name="endDate" rules={[{ required: true, message: 'Bắt buộc' }, ({ getFieldValue }) => ({
              validator(_, value) {
                if (!value || !getFieldValue('startDate') || value.isAfter(getFieldValue('startDate'))) return Promise.resolve();
                return Promise.reject(new Error('Ngày kết thúc phải sau ngày bắt đầu'));
              },
            })]}>
              <DatePicker className="w-full" />
            </Form.Item>
            <Form.Item label="Kích hoạt" name="isActive" valuePropName="checked" className="col-span-2">
              <Switch />
            </Form.Item>
          </div>
          <div className="flex justify-end gap-2 mt-2">
            <Button onClick={() => setShowModal(false)}>Hủy</Button>
            <Button type="primary" htmlType="submit" loading={saving}>
              {editing ? 'Lưu thay đổi' : 'Tạo voucher'}
            </Button>
          </div>
        </Form>
      </Modal>
    </div>
  );
}
