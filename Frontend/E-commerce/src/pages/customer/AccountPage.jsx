import { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, Form, Input, Button, Alert, message, Modal, Tag, Radio, Checkbox, Select } from 'antd';
import { UserOutlined, LockOutlined, LogoutOutlined, ShoppingOutlined, EnvironmentOutlined, PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { useAuth } from '../../context/AuthContext';
import userApi from '../../api/userApi';
import shippingAddressApi from '../../api/shippingAddressApi';

export default function AccountPage() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [tab, setTab] = useState('profile');
  const [saved, setSaved] = useState(false);
  const [isDirty, setIsDirty] = useState(false);
  const [profileForm] = Form.useForm();
  const [pwForm] = Form.useForm();
  const [changingPw, setChangingPw] = useState(false);
  const initialValues = useRef({});
  const [addresses, setAddresses] = useState([]);
  const [addrModal, setAddrModal] = useState(false);
  const [editingAddr, setEditingAddr] = useState(null);
  const [addrForm] = Form.useForm();
  const [provinces, setProvinces] = useState([]);
  const [districts, setDistricts] = useState([]);
  const [wards, setWards] = useState([]);
  const formatPrice = (p) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(p);

  useEffect(() => {
    if (!user) navigate('/login');
  }, [user]);

  const loadAddresses = async () => {
    try {
      const res = await shippingAddressApi.getAll();
      setAddresses(res.data || []);
    } catch { setAddresses([]); }
  };

  useEffect(() => { loadAddresses(); }, []);

  useEffect(() => {
    if (!user) return;
    userApi.getProfile().then(res => {
      const vals = {
        name: res.data.fullName || user.name,
        phone: res.data.phoneNumber || '',
        address: res.data.address || '',
      };
      profileForm.setFieldsValue(vals);
      initialValues.current = vals;
    }).catch(() => {});
  }, []);

  const handleProfileChange = (_, allValues) => {
    const init = initialValues.current;
    setIsDirty(
      allValues.name !== init.name ||
      allValues.phone !== init.phone ||
      allValues.address !== init.address
    );
  };
  

  const handleSave = async (values) => {
    try {
      await userApi.updateProfile({ fullName: values.name, phoneNumber: values.phone, address: values.address });
      initialValues.current = { name: values.name, phone: values.phone, address: values.address };
      setIsDirty(false);
      setSaved(true);
      setTimeout(() => setSaved(false), 2000);
    } catch {
      message.error('Lưu thất bại');
    }
  };

  const handleChangePassword = async (values) => {
    if (values.newPassword !== values.confirm) {
      message.error('Mật khẩu xác nhận không khớp');
      return;
    }
    setChangingPw(true);
    try {
      await userApi.changePassword({
        currentPassword: values.currentPassword,
        newPassword: values.newPassword,
      });
      message.success('Đổi mật khẩu thành công');
      pwForm.resetFields();
    } catch (err) {
      message.error(err?.response?.data?.message || 'Mật khẩu hiện tại không đúng');
    } finally {
      setChangingPw(false);
    }
  };

  const openAddAddr = () => {
    setEditingAddr(null);
    addrForm.resetFields();
    setDistricts([]);
    setWards([]);
    setAddrModal(true);
    if (provinces.length === 0) {
      fetch('https://provinces.open-api.vn/api/p/')
        .then(r => r.json())
        .then(setProvinces);
    }
  };

  const openEditAddr = (addr) => {
    setEditingAddr(addr.id);
    addrForm.setFieldsValue(addr);
    setAddrModal(true);
  };

  const handleDeleteAddr = (id) => {
    Modal.confirm({
      title: 'Xóa địa chỉ này?',
      okButtonProps: { danger: true },
      onOk: async () => {
        await shippingAddressApi.delete(id);
        loadAddresses();
      }
    });
  };

  const handleSaveAddr = async (values) => {
    try {
      if (editingAddr) await shippingAddressApi.update(editingAddr, values);
      else await shippingAddressApi.create(values);
      setAddrModal(false);
      loadAddresses();
      message.success('Đã lưu địa chỉ');
    } catch { message.error('Lưu thất bại'); }
  };

  const sideNav = [
    { key: 'profile', icon: <UserOutlined />, label: 'Thông tin cá nhân' },
    { key: 'address', icon: <EnvironmentOutlined />, label: 'Địa chỉ nhận hàng' },
    { key: 'security', icon: <LockOutlined />, label: 'Bảo mật' },
    { key: 'orders', icon: <ShoppingOutlined />, label: 'Đơn hàng' },
  ];

  if (!user) return null;
  return (
    <div className="py-8 pb-16">
      <div className="container">
        <h1 className="text-2xl font-bold text-gray-800 mb-6">Tài khoản của tôi</h1>

        <div className="grid grid-cols-[220px_1fr] gap-6 md:grid-cols-1">
          {/* Sidebar */}
          <Card className="self-start">
            <div className="text-center mb-4">
              <div className="w-16 h-16 rounded-full bg-blue-600 text-white text-2xl font-bold flex items-center justify-center mx-auto mb-2">
                {user.name?.[0] ?? '?'}
              </div>
              <p className="font-semibold text-gray-800">{user.name}</p>
              <p className="text-sm text-gray-500">{user.email}</p>
            </div>
            <nav className="flex flex-col gap-1">
              {sideNav.map(item => (
                <button
                  key={item.key}
                  onClick={() => item.key === 'orders' ? navigate('/orders') : setTab(item.key)}
                  className={`flex items-center gap-2.5 w-full text-left px-3 py-2.5 rounded-lg text-sm transition-colors ${
                    tab === item.key ? 'bg-blue-50 text-blue-600 font-medium' : 'text-gray-600 hover:bg-gray-50'
                  }`}
                >
                  {item.icon} {item.label}
                </button>
              ))}
              <button
                onClick={() => { logout(); navigate('/'); }}
                className="flex items-center gap-2.5 w-full text-left px-3 py-2.5 rounded-lg text-sm text-red-500 hover:bg-red-50 transition-colors"
              >
                <LogoutOutlined /> Đăng xuất
              </button>
            </nav>
          </Card>

          {/* Content */}
          <div>
            {tab === 'profile' && (
              <Card title="Thông tin cá nhân">
                {saved && <Alert message="Lưu thành công!" type="success" showIcon className="mb-4" />}
                <Form
                  form={profileForm}
                  layout="vertical"
                  onFinish={handleSave}
                  onValuesChange={handleProfileChange}
                >
                  <div className="grid grid-cols-2 gap-4 md:grid-cols-1">
                    <Form.Item label="Họ và tên" name="name" rules={[{ max: 100, message: 'Tối đa 100 ký tự' }]}>
                      <Input />
                    </Form.Item>
                    <Form.Item label="Email">
                      <Input value={user.email} disabled />
                      <p className="text-xs text-gray-400 mt-1">Email không thể thay đổi</p>
                    </Form.Item>
                    <Form.Item label="Số điện thoại" name="phone" rules={[{ pattern: /^\d{10}$/, message: 'Số điện thoại phải đúng 10 chữ số' }]}>
                      <Input placeholder="09xxxxxxxx" />
                    </Form.Item>
                    <Form.Item label="Địa chỉ" name="address" rules={[{ max: 200, message: 'Tối đa 200 ký tự' }]}>
                      <Input placeholder="Địa chỉ của bạn" />
                    </Form.Item>
                  </div>
                  <Button
                    type="primary"
                    htmlType="submit"
                    disabled={!isDirty}
                    style={isDirty ? { background: '#22c55e', borderColor: '#22c55e' } : {}}
                  >
                    Lưu thay đổi
                  </Button>
                </Form>
              </Card>
            )}

            {tab === 'address' && (
              <div className="flex flex-col gap-4">
                <div className="flex justify-between items-center">
                  <span className="text-sm text-gray-500">{addresses.length} địa chỉ đã lưu</span>
                  <Button type="primary" icon={<PlusOutlined />} onClick={openAddAddr}>
                    Thêm địa chỉ mới
                  </Button>
                </div>

                {addresses.length === 0 && (
                  <Card>
                    <div className="text-center py-8 text-gray-400">
                      <EnvironmentOutlined className="text-4xl mb-3" />
                      <p>Chưa có địa chỉ nhận hàng nào</p>
                      <Button type="primary" className="mt-4" onClick={openAddAddr}>Thêm ngay</Button>
                    </div>
                  </Card>
                )}

                {addresses.map(addr => (
                  <Card key={addr.id} size="small">
                    <div className="flex items-start justify-between gap-4">
                      <div className="flex-1">
                        <div className="flex items-center gap-2 mb-1">
                          <span className="font-semibold text-gray-800">{addr.fullName}</span>
                          <span className="text-gray-400">|</span>
                          <span className="text-gray-600 text-sm">{addr.phoneNumber}</span>
                          {addr.isDefault && <Tag color="blue">Mặc định</Tag>}
                        </div>
                        <p className="text-sm text-gray-600">
                          {addr.street}, {addr.ward}, {addr.district}, {addr.province}
                        </p>
                      </div>
                      <div className="flex gap-2 flex-shrink-0">
                        {!addr.isDefault && (
                          <Button
                            size="small"
                            onClick={async () => {
                              await shippingAddressApi.setDefault(addr.id);
                              loadAddresses();
                            }}
                          >
                            Đặt mặc định
                          </Button>
                        )}
                        <Button size="small" icon={<EditOutlined />} onClick={() => openEditAddr(addr)} />
                        <Button size="small" danger icon={<DeleteOutlined />} onClick={() => handleDeleteAddr(addr.id)} />
                      </div>
                    </div>
                  </Card>
                ))}
              </div>
            )}

            {tab === 'security' && (
              <Card title="Đổi mật khẩu">
                <div className="max-w-sm">
                  <Form form={pwForm} layout="vertical" onFinish={handleChangePassword}>
                    <Form.Item
                      label="Mật khẩu hiện tại"
                      name="currentPassword"
                      rules={[{ required: true, message: 'Bắt buộc' }]}
                    >
                      <Input.Password placeholder="••••••••" />
                    </Form.Item>
                    <Form.Item
                      label="Mật khẩu mới"
                      name="newPassword"
                      rules={[
                        { required: true, message: 'Vui lòng nhập mật khẩu mới' },
                        { min: 6, message: 'Tối thiểu 6 ký tự' },
                        { pattern: /^(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d])/, message: 'Phải có chữ hoa, số và ký tự đặc biệt' },
                      ]}
                    >
                      <Input.Password placeholder="••••••••" />
                    </Form.Item>
                    <Form.Item
                      label="Xác nhận mật khẩu mới"
                      name="confirm"
                      rules={[{ required: true, message: 'Bắt buộc' }]}
                    >
                      <Input.Password placeholder="••••••••" />
                    </Form.Item>
                    <Button type="primary" htmlType="submit" loading={changingPw}>
                      Đổi mật khẩu
                    </Button>
                  </Form>
                </div>
              </Card>
            )}
          </div>
        </div>
      </div>

      {/* Modal them/sua dia chi */}
      <Modal
        title={editingAddr ? 'Sửa địa chỉ' : 'Thêm địa chỉ mới'}
        open={addrModal}
        onCancel={() => setAddrModal(false)}
        footer={null}
        width={600}
      >
        <Form form={addrForm} layout="vertical" onFinish={handleSaveAddr} className="mt-4">
          <div className="grid grid-cols-2 gap-x-4">
            <Form.Item label="Họ và tên *" name="fullName" rules={[{ required: true, message: 'Bắt buộc' }, { max: 100, message: 'Tối đa 100 ký tự' }]}>
              <Input placeholder="Nguyễn Văn A" />
            </Form.Item>
            <Form.Item label="Số điện thoại *" name="phoneNumber" rules={[{ required: true, message: 'Bắt buộc' }, { pattern: /^\d{10}$/, message: 'Phải đúng 10 chữ số' }]}>
              <Input placeholder="09xxxxxxxx" />
            </Form.Item>
            <Form.Item label="Tỉnh / Thành phố *" name="province" rules={[{ required: true, message: 'Bắt buộc' }]}>
            <Select
              showSearch
              placeholder="Chọn tỉnh/thành phố"
              filterOption={(input, opt) => opt.label.toLowerCase().includes(input.toLowerCase())}
              options={provinces.map(p => ({ value: p.name, label: p.name, code: p.code }))}
              onChange={(_, opt) => {
                addrForm.setFieldValue('district', undefined);
                addrForm.setFieldValue('ward', undefined);
                setWards([]);
                fetch(`https://provinces.open-api.vn/api/p/${opt.code}?depth=2`)
                  .then(r => r.json())
                  .then(data => setDistricts(data.districts || []));
              }}
            />
          </Form.Item>

          <Form.Item label="Quận / Huyện *" name="district" rules={[{ required: true, message: 'Bắt buộc' }]}>
            <Select
              showSearch
              placeholder="Chọn quận/huyện"
              disabled={districts.length === 0}
              filterOption={(input, opt) => opt.label.toLowerCase().includes(input.toLowerCase())}
              options={districts.map(d => ({ value: d.name, label: d.name, code: d.code }))}
              onChange={(_, opt) => {
                addrForm.setFieldValue('ward', undefined);
                fetch(`https://provinces.open-api.vn/api/d/${opt.code}?depth=2`)
                  .then(r => r.json())
                  .then(data => setWards(data.wards || []));
              }}
            />
          </Form.Item>

          <Form.Item label="Phường / Xã *" name="ward" rules={[{ required: true, message: 'Bắt buộc' }]}>
            <Select
              showSearch
              placeholder="Chọn phường/xã"
              disabled={wards.length === 0}
              filterOption={(input, opt) => opt.label.toLowerCase().includes(input.toLowerCase())}
              options={wards.map(w => ({ value: w.name, label: w.name }))}
            />
          </Form.Item>
            <Form.Item label="Số nhà, tên đường *" name="street" rules={[{ required: true, message: 'Bắt buộc' }]}>
              <Input placeholder="123 Nguyễn Huệ" />
            </Form.Item>
            <Form.Item  name="isDefault" valuePropName="checked" className="col-span-2">
              <Checkbox>Đặt làm địa chỉ mặc định </Checkbox>
            </Form.Item>
          </div>
          <div className="flex justify-end gap-2 mt-2">
            <Button onClick={() => setAddrModal(false)}>Hủy</Button>
            <Button type="primary" htmlType="submit">Lưu địa chỉ</Button>
          </div>
        </Form>
      </Modal>
    </div>
  );
}
