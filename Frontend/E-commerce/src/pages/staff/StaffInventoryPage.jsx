import { useState } from 'react';
import { Table, Tag, Button, Input, InputNumber, Modal, Space } from 'antd';
import { SearchOutlined, WarningOutlined } from '@ant-design/icons';
import { Alert } from 'antd';
import { useProducts } from '../../context/ProductContext';
import variantApi from '../../api/variantApi';

const formatPrice = p => new Intl.NumberFormat('vi-VN',{style:'currency',currency:'VND'}).format(p);

export default function StaffInventoryPage() {
  const { products: productList } = useProducts();
  const [search, setSearch] = useState('');
  const [editingProduct, setEditingProduct] = useState(null); // { product, variants: [{...v, editQty}] }

  const getTotalStock = (p) => p.variants?.reduce((s, v) => s + v.quantity, 0) ?? 0;

  const getStockStatus = (stock) => {
    if (stock === 0) return { label: 'Hết hàng', color: 'red' };
    if (stock < 10) return { label: 'Sắp hết', color: 'orange' };
    return { label: 'Còn hàng', color: 'green' };
  };

  const openEdit = (product) => {
    setEditingProduct({
      product,
      variants: product.variants.map(v => ({ ...v, editQty: v.quantity }))
    });
  };

  const handleSaveVariant = async (variantId, newQty) => {
    try {
      await variantApi.update(variantId, { quantity: newQty });
      setEditingProduct(prev => ({
        ...prev,
        variants: prev.variants.map(v => v.id === variantId ? { ...v, quantity: newQty, editQty: newQty } : v)
      }));
    } catch { }
  };

  const filtered = productList.filter(p =>
    p.name.toLowerCase().includes(search.toLowerCase()) ||
    p.brandName?.toLowerCase().includes(search.toLowerCase())
  ).sort((a, b) => new Date(b.orderDate) - new Date(a.orderDate));
  const lowStockCount = productList.filter(p => getTotalStock(p) < 10).length;

  const columns = [
    { title: 'Sản phẩm', key: 'product', render: (_, r) => (
      <div className="flex items-center gap-2.5">
        {r.imageUrls?.[0]
          ? <img src={r.imageUrls[0]} alt={r.name} className="w-9 h-9 rounded object-cover bg-gray-100" />
          : <div className="w-9 h-9 rounded bg-gray-100" />
        }
        <span className="font-medium">{r.name}</span>
      </div>
    )},
    { title: 'Danh mục', dataIndex: 'categoryName', key: 'cat',
      render: v => <span className="text-sm text-gray-500">{v}</span> },
    { title: 'Giá từ', dataIndex: 'minPrice', key: 'price', render: v => formatPrice(v) },
    { title: 'Tổng tồn kho', key: 'stock',
      render: (_, r) => <span className="font-semibold">{getTotalStock(r)}</span> },
    { title: 'Trạng thái', key: 'status', render: (_, r) => {
      const s = getStockStatus(getTotalStock(r));
      return <Tag color={s.color}>{s.label}</Tag>;
    }},
    { title: 'Cập nhật kho', key: 'action',
      render: (_, r) => <Button size="small" onClick={() => openEdit(r)}>Cập nhật</Button> },
  ];

  return (
    <div className="flex flex-col gap-4">
      {lowStockCount > 0 && (
        <Alert icon={<WarningOutlined />}
          message={<><strong>{lowStockCount} sản phẩm</strong> đang sắp hết hoặc đã hết hàng. Cần nhập thêm!</>}
          type="warning" showIcon />
      )}

      <Input prefix={<SearchOutlined />} placeholder="Tìm sản phẩm..."
        value={search} onChange={e => setSearch(e.target.value)} className="max-w-sm" />

      <Table columns={columns} dataSource={filtered} rowKey="id"
        scroll={{ x: true }} size="small" pagination={{ pageSize: 10 }} />

      <Modal title={`Cập nhật tồn kho: ${editingProduct?.product.name}`}
        open={!!editingProduct} onCancel={() => setEditingProduct(null)} footer={null}>
        {editingProduct && (
          <div className="flex flex-col gap-3">
            {editingProduct.variants.map(v => (
              <div key={v.id} className="flex items-center justify-between gap-3 p-3 bg-gray-50 rounded-lg">
                <span className="text-sm font-medium flex-1">{v.name}</span>
                <InputNumber min={0} defaultValue={v.quantity} className="w-24" size="small"
                  onChange={val => {
                    setEditingProduct(prev => ({
                      ...prev,
                      variants: prev.variants.map(x => x.id === v.id ? { ...x, editQty: val } : x)
                    }));
                  }}
                />
                <Button size="small" type="primary"
                  onClick={() => handleSaveVariant(v.id, editingProduct.variants.find(x => x.id === v.id).editQty)}>
                  Lưu
                </Button>
              </div>
            ))}
          </div>
        )}
      </Modal>
    </div>
  );
}
