import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Card, Form, Input, Button, Alert } from 'antd';
import { UserOutlined, MailOutlined, PhoneOutlined, LockOutlined } from '@ant-design/icons';
import { useAuth } from '../../context/AuthContext';  

export default function RegisterPage() {
  const {register} = useAuth();
  const navigate = useNavigate();
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [form] = Form.useForm();

  const handleSubmit = async (values) => {
    if (values.password !== values.confirm) {
      setError('Mật khẩu xác nhận không khớp');
      return;
    }
    setLoading(true);
    setError("");
    const result = await register(values.name, values.email, values.phone, values.password);
    
    setLoading(false);
    console.log(result);
    if (result.ok) {
      navigate('/login');
    } else {
      setError(result.message);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-blue-100 p-4">
      <Card className="w-full max-w-[420px] shadow-xl rounded-2xl">
        <div className="text-center mb-6">
          <div className="text-5xl mb-3">🛒</div>
          <h2 className="text-2xl font-bold text-gray-800">Tạo tài khoản</h2>
          <p className="text-gray-500 mt-1">Đăng ký để mua sắm dễ dàng hơn</p>
        </div>

        {error && <Alert message={error} type="error" showIcon className="mb-4" />}

        <Form form={form} layout="vertical" onFinish={handleSubmit} size="large">
          <Form.Item label="Họ và tên" name="name" rules={[{ required: true, message: 'Vui lòng nhập họ tên' }]}>
            <Input prefix={<UserOutlined />} placeholder="Nguyễn Văn A" />
          </Form.Item>
          <Form.Item label="Email" name="email" rules={[{ required: true, message: 'Vui lòng nhập email' }, { type: 'email', message: 'Email không hợp lệ' }]}>
            <Input prefix={<MailOutlined />} placeholder="email@example.com" />
          </Form.Item>
          <Form.Item label="Số điện thoại" name="phone" rules={[{ required: true, message: 'Vui lòng nhập số điện thoại' }, { pattern: /^\d{10}$/, message: 'Số điện thoại phải đúng 10 chữ số' }]}>
            <Input prefix={<PhoneOutlined />} placeholder="09xxxxxxxx" />
          </Form.Item>
          <Form.Item label="Mật khẩu" name="password" rules={[{ required: true, message: 'Vui lòng nhập mật khẩu' }, { min: 6, message: 'Tối thiểu 6 ký tự' }, { pattern: /^(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d])/, message: 'Phải có chữ hoa, số và ký tự đặc biệt' }]}>
            <Input.Password prefix={<LockOutlined />} placeholder="Tối thiểu 6 ký tự" />
          </Form.Item>
          <Form.Item label="Xác nhận mật khẩu" name="confirm" rules={[{ required: true, message: 'Vui lòng xác nhận mật khẩu' }]}>
            <Input.Password prefix={<LockOutlined />} placeholder="Nhập lại mật khẩu" />
          </Form.Item>
          <Button type="primary" htmlType="submit" block loading={loading} className="h-11 font-semibold">
            Đăng ký
          </Button>
        </Form>

        <p className="text-center text-sm text-gray-500 mt-4">
          Đã có tài khoản?{' '}
          <Link to="/login" className="text-blue-600 font-semibold hover:underline">Đăng nhập</Link>
        </p>
      </Card>
    </div>
  );
}
