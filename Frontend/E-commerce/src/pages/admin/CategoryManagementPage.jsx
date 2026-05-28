import { useState, useEffect } from 'react';
import { Table, Button, Modal, Form, Input, Space } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import categoryApi from '../../api/categoryApi';

export default function CategoryManagementPage() {
  const [cats, setCats] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [editing, setEditing] = useState(null);
  const [form] = Form.useForm();

  const load = () => categoryApi.getAll().then(res => setCats(res.data || []));
  useEffect(() => { load(); }, []);

  const openAdd = () => { setEditing(null); form.resetFields(); setShowModal(true); };
  const openEdit = (c) => { setEditing(c); form.setFieldsValue({ name: c.name }); setShowModal(true); };

  const handleDelete = (id) => {
    Modal.confirm({ title: 'Xóa danh mục này?', okButtonProps: { danger: true },
      onOk: async () => { await categoryApi.delete(id); load(); }
    });
  };

  const handleSave = async (values) => {
    if (editing) {
      await categoryApi.update(editing.id, values);
    } else {
      await categoryApi.create(values);
    }
    setShowModal(false);
    load();
  };

  const columns = [
    { title: 'Tên danh mục', dataIndex: 'name', key: 'name', render: v => <span className="font-medium">{v}</span> },
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
        <span className="text-sm text-gray-500">Tổng <strong>{cats.length}</strong> danh mục</span>
        <Button type="primary" icon={<PlusOutlined />} onClick={openAdd}>Thêm danh mục</Button>
      </div>
      <Table columns={columns} dataSource={cats} rowKey="id" size="small" pagination={{ pageSize: 10 }} />
      <Modal title={editing ? 'Sửa danh mục' : 'Thêm danh mục'} open={showModal}
        onCancel={() => setShowModal(false)} footer={null}>
        <Form form={form} layout="vertical" onFinish={handleSave} className="mt-4">
          <Form.Item label="Tên danh mục *" name="name" rules={[{ required: true, message: 'Vui lòng nhập tên danh mục' }, { max: 100, message: 'Tối đa 100 ký tự' }]}><Input /></Form.Item>
          <div className="flex justify-end gap-2">
            <Button onClick={() => setShowModal(false)}>Hủy</Button>
            <Button type="primary" htmlType="submit">{editing ? 'Lưu' : 'Thêm'}</Button>
          </div>
        </Form>
      </Modal>
    </div>
  );
}
