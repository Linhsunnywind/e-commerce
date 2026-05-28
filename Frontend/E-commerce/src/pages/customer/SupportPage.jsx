import { useState, useEffect } from 'react';
import { Tabs, Form, Input, Button, Tag, Alert, Collapse, Card, Empty, Spin } from 'antd';
import { useAuth } from '../../context/AuthContext';
import supportApi from '../../api/supportApi';

const STATUS_MAP = {
  Pending:    { label: 'Chờ xử lý',    color: 'orange' },
  InProgress: { label: 'Đang xử lý',   color: 'blue'   },
  Resolved:   { label: 'Đã giải quyết', color: 'green' },
};

const FAQ_ITEMS = [
  { q: 'Chính sách đổi trả như thế nào?', a: 'Sản phẩm được đổi trả trong vòng 30 ngày kể từ ngày nhận hàng nếu còn nguyên vẹn, đầy đủ phụ kiện và không có dấu hiệu sử dụng.' },
  { q: 'Sản phẩm có được bảo hành không?', a: 'Tất cả sản phẩm đều được bảo hành theo chính sách hãng, thông thường từ 12-24 tháng tùy sản phẩm.' },
  { q: 'Thời gian giao hàng là bao lâu?', a: 'TP.HCM: 24 giờ. Các tỉnh thành khác: 2-4 ngày làm việc.' },
  { q: 'Tôi có thể thanh toán bằng những hình thức nào?', a: 'Hiện tại hỗ trợ: COD (tiền mặt khi nhận), VNPay, MoMo và Chuyển khoản ngân hàng.' },
  { q: 'Làm sao để theo dõi đơn hàng?', a: 'Bạn có thể theo dõi đơn hàng trong mục "Đơn hàng của tôi" sau khi đăng nhập.' },
];

export default function SupportPage() {
  const { user } = useAuth();
  const [submitted, setSubmitted] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [tickets, setTickets] = useState([]);
  const [loadingTickets, setLoadingTickets] = useState(false);
  const [form] = Form.useForm();

  const loadTickets = async () => {
    if (!user) return;
    setLoadingTickets(true);
    try {
      const res = await supportApi.getMyTickets();
      setTickets(res.data || []);
    } catch {
      setTickets([]);
    } finally {
      setLoadingTickets(false);
    }
  };

  useEffect(() => { loadTickets(); }, [user]);

  const handleSubmit = async (values) => {
    setSubmitting(true);
    try {
      await supportApi.create({ subject: values.subject, message: values.message });
      setSubmitted(true);
      form.resetFields();
      await loadTickets();
      setTimeout(() => setSubmitted(false), 3000);
    } catch {
      // silent
    } finally {
      setSubmitting(false);
    }
  };

  const tabItems = [
    {
      key: 'new',
      label: 'Gửi yêu cầu mới',
      children: (
        <Card>
          {submitted && (
            <Alert message="Yêu cầu đã được gửi! Chúng tôi sẽ phản hồi trong 24 giờ." type="success" showIcon className="mb-4" />
          )}
          {!user && (
            <Alert message="Vui lòng đăng nhập để gửi yêu cầu hỗ trợ." type="warning" showIcon className="mb-4" />
          )}
          <Form form={form} layout="vertical" onFinish={handleSubmit} className="max-w-xl">
            <Form.Item label="Chủ đề *" name="subject" rules={[{ required: true, message: 'Bắt buộc' }, { max: 200, message: 'Tối đa 200 ký tự' }]}>
              <Input placeholder="Mô tả ngắn vấn đề của bạn" />
            </Form.Item>
            <Form.Item label="Nội dung chi tiết *" name="message" rules={[{ required: true, message: 'Bắt buộc' }, { max: 2000, message: 'Tối đa 2000 ký tự' }]}>
              <Input.TextArea rows={5} placeholder="Mô tả chi tiết vấn đề bạn đang gặp phải..." />
            </Form.Item>
            <Button type="primary" htmlType="submit" loading={submitting} disabled={!user}>
              Gửi yêu cầu
            </Button>
          </Form>
        </Card>
      ),
    },
    {
      key: 'history',
      label: 'Lịch sử hỗ trợ',
      children: (
        <Spin spinning={loadingTickets}>
          {tickets.length === 0 ? (
            <Empty description="Chưa có yêu cầu hỗ trợ nào" className="py-12" />
          ) : (
            <div className="flex flex-col gap-4">
              {tickets.map(ticket => {
                const s = STATUS_MAP[ticket.status] ?? { label: ticket.status, color: 'default' };
                return (
                  <Card key={ticket.id} size="small">
                    <div className="flex items-start justify-between gap-4 mb-2">
                      <div>
                        <p className="font-semibold text-gray-800">{ticket.subject}</p>
                        <p className="text-xs text-gray-500">
                          {new Date(ticket.createdDate).toLocaleDateString('vi-VN')}
                        </p>
                      </div>
                      <Tag color={s.color}>{s.label}</Tag>
                    </div>
                    <p className="text-sm text-gray-600">{ticket.message}</p>
                  </Card>
                );
              })}
            </div>
          )}
        </Spin>
      ),
    },
    {
      key: 'faq',
      label: 'Câu hỏi thường gặp',
      children: (
        <Collapse
          items={FAQ_ITEMS.map((item, i) => ({ key: i, label: item.q, children: <p className="text-gray-600">{item.a}</p> }))}
          className="bg-white"
        />
      ),
    },
  ];

  return (
    <div className="py-8 pb-16">
      <div className="container max-w-3xl">
        <h1 className="text-2xl font-bold text-gray-800 mb-6">Hỗ trợ khách hàng</h1>
        <Tabs items={tabItems} />
      </div>
    </div>
  );
}
