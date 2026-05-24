import { useState, useEffect } from 'react';
import { Table, Button, Modal, Form, Input, Space } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import staffApi from '../../api/staffApi';

export default function StaffManagementPage() {
  const [staffs, setStaffs] = useState([]);
  const [loading, setLoading] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [editing, setEditing] = useState(null); // null = add, object = edit
  const [form] = Form.useForm();

  const load = () => {
    setLoading(true);
    staffApi.getAll().then(res => setStaffs(res.data || [])).finally(() => setLoading(false));
  };
  useEffect(load, []);

  const openAdd = () => { setEditing(null); form.resetFields(); setShowModal(true); };
  const openEdit = (s) => {
    setEditing(s);
    form.setFieldsValue({ name: s.name, fullName: s.fullName, phoneNumber: s.phoneNumber });
    setShowModal(true);
  };

  const handleDelete = (id) => {
    Modal.confirm({ title: 'Xóa nhân viên này?', okButtonProps: { danger: true },
      onOk: async () => { await staffApi.delete(id); load(); }
    });
  };

  const handleSave = async (values) => {
    try {
      if (editing) {
        await staffApi.update(editing.id, { name: values.name, fullName: values.fullName, phoneNumber: values.phoneNumber });
      } else {
        await staffApi.create({ name: values.name, email: values.email, password: values.password, fullName: values.fullName, phoneNumber: values.phoneNumber });
      }
      setShowModal(false);
      load();
    } catch { /* handle error */ }
  };

  const columns = [
    { title: 'Nhân viên', key: 'name', render: (_, r) => (
      <div className="flex items-center gap-2.5">
        <div className="w-9 h-9 rounded-full bg-blue-600 text-white text-sm font-bold flex items-center justify-center">
          {(r.fullName || r.name || 'S')[0].toUpperCase()}
        </div>
        <div><p className="font-medium">{r.fullName || r.name}</p><p className="text-xs text-gray-500">{r.name}</p></div>
      </div>
    )},
    { title: 'Email', dataIndex: 'email', key: 'email', render: v => <span className="text-sm">{v}</span> },
    { title: 'SĐT', dataIndex: 'phoneNumber', key: 'phone', render: v => <span className="text-sm">{v}</span> },
    { title: 'Thao tác', key: 'action', render: (_, r) => (
      <Space>
        <Button size="small" icon={<EditOutlined />} onClick={() => openEdit(r)}>Sửa</Button>
        <Button size="small" danger icon={<DeleteOutlined />} onClick={() => handleDelete(r.id)}>Xóa</Button>
      </Space>
    )},
  ];

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <span className="text-sm text-gray-500">Tổng <strong>{staffs.length}</strong> nhân viên</span>
        <Button type="primary" icon={<PlusOutlined />} onClick={openAdd}>Thêm nhân viên</Button>
      </div>
      <Table columns={columns} dataSource={staffs} rowKey="id" loading={loading} size="small" pagination={{ pageSize: 10 }} />

      <Modal title={editing ? 'Sửa nhân viên' : 'Thêm nhân viên mới'}
        open={showModal} onCancel={() => setShowModal(false)} footer={null}>
        <Form form={form} layout="vertical" onFinish={handleSave} className="mt-4">
          <Form.Item label="Tên đăng nhập *" name="name" rules={[{ required: true }]}><Input /></Form.Item>
          {!editing && <>
            <Form.Item label="Email *" name="email" rules={[{ required: true, type: 'email' }]}><Input /></Form.Item>
            <Form.Item label="Mật khẩu *" name="password" rules={[{ required: true, min: 8, message: 'Tối thiểu 8 ký tự' }]}>
              <Input.Password />
            </Form.Item>
          </>}
          <Form.Item label="Họ và tên" name="fullName"><Input /></Form.Item>
          <Form.Item label="Số điện thoại *" name="phoneNumber" rules={[{ required: true }]}><Input /></Form.Item>
          <div className="flex justify-end gap-2">
            <Button onClick={() => setShowModal(false)}>Hủy</Button>
            <Button type="primary" htmlType="submit">{editing ? 'Lưu' : 'Thêm'}</Button>
          </div>
        </Form>
      </Modal>
    </div>
  );
}
