import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button, Card, Divider, InputNumber, Empty, Tag, Checkbox } from 'antd';
import { DeleteOutlined, ShoppingOutlined } from '@ant-design/icons';
import { useCart } from '../../context/CartContext';
import { useAuth } from '../../context/AuthContext';

export default function CartPage() {
  const { items, removeFromCart, updateQuantity } = useCart();
  const navigate = useNavigate();
  const { user } = useAuth();
  const [selectedIds, setSelectedIds] = useState([]);

  const formatPrice = (p) =>
    new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(p);

  const selectedItems = items.filter(i => selectedIds.includes(i.id));
  const subtotal = selectedItems.reduce((s, i) => s + i.price * i.quantity, 0);
  const shipping = subtotal > 0 && subtotal >= 5000000 ? 0 : subtotal > 0 ? 30000 : 0;
  const total = subtotal + shipping;

  const toggleItem = (id) =>
    setSelectedIds(prev => prev.includes(id) ? prev.filter(x => x !== id) : [...prev, id]);

  const allChecked = items.length > 0 && selectedIds.length === items.length;
  const toggleAll = () =>
    setSelectedIds(allChecked ? [] : items.map(i => i.id));

  const handleRemove = (id, variantId) => {
    setSelectedIds(prev => prev.filter(x => x !== id));
    removeFromCart(variantId);
  };

  const handleCheckout = () => {
    if (selectedIds.length === 0) return;
    navigate('/checkout', { state: { selectedItemIds: selectedIds } });
  };

  if (items.length === 0) {
    return (
      <div className="py-8 pb-16">
        <div className="container">
          <h1 className="text-2xl font-bold text-gray-800 mb-6">Giỏ hàng</h1>
          <Empty
            image={<ShoppingOutlined className="text-6xl text-gray-300" />}
            description={user ? 'Giỏ hàng của bạn đang trống' : 'Vui lòng đăng nhập để xem giỏ hàng.'}
            className="py-16"
          >
            <Button type="primary" onClick={() => navigate('/products')}>Tiếp tục mua sắm</Button>
          </Empty>
        </div>
      </div>
    );
  }

  return (
    <div className="py-8 pb-16">
      <div className="container">
        <h1 className="text-2xl font-bold text-gray-800 mb-6">Giỏ hàng ({items.length} sản phẩm)</h1>

        <div className="grid grid-cols-[1fr_320px] gap-6 md:grid-cols-1">
          {/* Items */}
          <div className="flex flex-col gap-3">
            {/* Select all header */}
            <div className="bg-white rounded-xl shadow-sm px-4 py-2.5 flex items-center gap-3">
              <Checkbox checked={allChecked} onChange={toggleAll} />
              <span className="text-sm text-gray-600">Chọn tất cả ({items.length})</span>
              {selectedIds.length > 0 && (
                <span className="text-xs text-blue-600 ml-auto">Đã chọn {selectedIds.length} sản phẩm</span>
              )}
            </div>

            {items.map(item => (
              <div
                key={item.id}
                className={`bg-white rounded-xl shadow-sm p-4 flex items-center gap-4 transition-opacity ${
                  !selectedIds.includes(item.id) ? 'opacity-60' : ''
                }`}
              >
                <Checkbox
                  checked={selectedIds.includes(item.id)}
                  onChange={() => toggleItem(item.id)}
                />
                <div className="w-16 h-16 rounded-lg bg-gray-100 flex-shrink-0 flex items-center justify-center text-gray-400 text-2xl">
                  🛍️
                </div>
                <div className="flex-1 min-w-0">
                  <p className="font-semibold text-gray-800 truncate">{item.productName || item.productVariantName}</p>
                  <p className="text-xs text-gray-500 mt-0.5">{item.productVariantName}</p>
                  <p className="text-blue-600 font-bold mt-1">{formatPrice(item.price)}</p>
                </div>
                <InputNumber
                  min={1}
                  value={item.quantity}
                  onChange={v => updateQuantity(item.productVariantId, v || 1)}
                  className="w-20"
                  size="small"
                />
                <p className="font-bold text-gray-800 w-24 text-right">{formatPrice(item.price * item.quantity)}</p>
                <Button
                  type="text"
                  danger
                  icon={<DeleteOutlined />}
                  onClick={() => handleRemove(item.id, item.productVariantId)}
                />
              </div>
            ))}
          </div>

          {/* Summary */}
          <div className="self-start">
            <Card title="Tóm tắt đơn hàng">
              <div className="flex flex-col gap-2">
                <div className="flex justify-between text-sm">
                  <span className="text-gray-600">Tạm tính ({selectedIds.length} sp)</span>
                  <span>{formatPrice(subtotal)}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-gray-600">Phí vận chuyển</span>
                  <span>
                    {subtotal === 0 ? '—' : shipping === 0
                      ? <Tag color="green">Miễn phí</Tag>
                      : formatPrice(shipping)
                    }
                  </span>
                </div>
                {shipping === 0 && subtotal > 0 && (
                  <p className="text-xs text-green-600">✓ Miễn phí vận chuyển đơn từ 5 triệu</p>
                )}
                <Divider className="my-2" />
                <div className="flex justify-between font-bold text-lg">
                  <span>Tổng cộng</span>
                  <span className="text-blue-600">{formatPrice(total)}</span>
                </div>
                <Button
                  type="primary"
                  size="large"
                  block
                  className="mt-4"
                  disabled={selectedIds.length === 0}
                  onClick={handleCheckout}
                >
                  Thanh toán {selectedIds.length > 0 ? `(${selectedIds.length})` : ''} →
                </Button>
                <Button block onClick={() => navigate('/products')}>
                  Tiếp tục mua sắm
                </Button>
              </div>
            </Card>
          </div>
        </div>
      </div>
    </div>
  );
}
