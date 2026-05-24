import { useState, useMemo } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { Select, Button, Rate, Empty, Radio, InputNumber, Card } from 'antd';
import { FilterOutlined } from '@ant-design/icons';
import { useProducts } from '../../context/ProductContext';
import { useCategories } from '../../context/CategoryContext'

export default function ProductsPage() {
  const { categories } = useCategories();
  const [searchParams, setSearchParams] = useSearchParams();
  const navigate = useNavigate();
  const formatPrice = (p) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(p);

  const categoryParam = searchParams.get('category') || '';
  const query = searchParams.get('q') || '';

  const [sort, setSort] = useState('default');
  const [priceMin, setPriceMin] = useState('');
  const [priceMax, setPriceMax] = useState('');
  const [brandName, setbrandName] = useState('');
  const { products } = useProducts();

  const brandNames = [...new Set(products.map(p => p.brandName))];

  const filtered = useMemo(() => {
    let list = [...products];
    if (categoryParam) list = list.filter(p => p.categoryName === categoryParam);
    if (query) list = list.filter(p => p.name.toLowerCase().includes(query.toLowerCase()) || p.brandName.toLowerCase().includes(query.toLowerCase()));
    if (brandName) list = list.filter(p => p.brandName === brandName);
    if (priceMin) list = list.filter(p => p.minPrice >= Number(priceMin));
    if (priceMax) list = list.filter(p => p.minPrice <= Number(priceMax));
    if (sort === 'price-asc') list.sort((a, b) => a.minPrice - b.minPrice);
    if (sort === 'price-desc') list.sort((a, b) => b.minPrice - a.minPrice);
    if (sort === 'averageRating') list.sort((a, b) => b.averageRating - a.averageRating);
    return list;
  }, [categoryParam, query, sort, priceMin, priceMax, brandName, products]);

  const selectedCategory = categories.find(c => c.name === categoryParam);

  const resetFilters = () => {
    setbrandName(''); setPriceMin(''); setPriceMax(''); setSort('default');
    setSearchParams({});
  };

  return (
    <div className="py-6 pb-12">
      <div className="container">
        {/* Breadcrumb */}
        <nav className="flex items-center gap-1.5 text-sm text-gray-500 mb-5">
          <span onClick={() => navigate('/')} className="cursor-pointer hover:text-blue-600">Trang chủ</span>
          <span>›</span>
          <span className="text-gray-800 font-medium">
            {selectedCategory ? selectedCategory.name : query ? `Tìm: "${query}"` : 'Tất cả sản phẩm'}
          </span>
        </nav>

        <div className="grid grid-cols-[240px_1fr] gap-6 md:grid-cols-1">
          {/* Filter Sidebar */}
          <aside className="sticky top-20 self-start">
            <Card size="small" title={<span className="font-semibold flex items-center gap-2"><FilterOutlined /> Bộ lọc</span>}
              extra={<Button size="small" onClick={resetFilters}>Xóa lọc</Button>}
            >
              <div className="flex flex-col gap-5">
                <div>
                  <p className="text-xs font-semibold text-gray-500 uppercase mb-2">Danh mục</p>
                  <Radio.Group
                    value={categoryParam}
                    onChange={e => e.target.value ? setSearchParams({ category: e.target.value }) : setSearchParams({})}
                    className="flex flex-col gap-1.5"
                  >
                    <Radio value="">Tất cả</Radio>
                    {categories.map(cat => <Radio key={cat.id} value={cat.name}>{cat.name}</Radio>)}
                  </Radio.Group>
                </div>

                <div>
                  <p className="text-xs font-semibold text-gray-500 uppercase mb-2">Thương hiệu</p>
                  <Radio.Group value={brandName} onChange={e => setbrandName(e.target.value)} className="flex flex-col gap-1.5">
                    <Radio value="">Tất cả</Radio>
                    {brandNames.map(b => <Radio key={b} value={b}>{b}</Radio>)}
                  </Radio.Group>
                </div>

                <div>
                  <p className="text-xs font-semibold text-gray-500 uppercase mb-2">Khoảng giá (VND)</p>
                  <div className="flex items-center gap-2">
                    <InputNumber placeholder="Từ" value={priceMin} onChange={v => setPriceMin(v)} className="w-full" formatter={v => v?.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',')} />
                    <span className="text-gray-400">—</span>
                    <InputNumber placeholder="Đến" value={priceMax} onChange={v => setPriceMax(v)} className="w-full" formatter={v => v?.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',')} />
                  </div>
                </div>
              </div>
            </Card>
          </aside>

          {/* Products */}
          <div>
            <div className="flex items-center justify-between mb-4">
              <p className="text-sm text-gray-600">
                {query && <span>Kết quả cho <strong>"{query}"</strong> – </span>}
                <strong>{filtered.length}</strong> sản phẩm
              </p>
              <Select value={sort} onChange={setSort} className="w-44">
                <Select.Option value="default">Mặc định</Select.Option>
                <Select.Option value="price-asc">Giá thấp → cao</Select.Option>
                <Select.Option value="price-desc">Giá cao → thấp</Select.Option>
                <Select.Option value="averageRating">Đánh giá cao nhất</Select.Option>
              </Select>
            </div>

            {filtered.length === 0 ? (
              <Empty description="Không tìm thấy sản phẩm phù hợp" className="py-16">
                <Button type="primary" onClick={resetFilters}>Xóa bộ lọc</Button>
              </Empty>
            ) : (
              <div className="grid grid-cols-[repeat(auto-fill,minmax(200px,1fr))] gap-5">
                {filtered.map(p => (
                  <div key={p.id} className="product-card relative" onClick={() => navigate(`/products/${p.id}`)}>
                    <div className="product-card-img">
                      <img src={p.imageUrls?.[0]} alt={p.name} /> 
                    </div>
                    <div className="p-3.5">
                      <p className="text-xs text-gray-500 mb-1">{p.brandName}</p>
                      <p className="font-semibold text-sm text-gray-800 mb-1.5 line-clamp-2">{p.name}</p>
                      <p className="text-base font-bold text-blue-600">{formatPrice(p.minPrice)}</p>
                      <div className="flex items-center gap-1 mt-1.5">
                        <Rate disabled defaultValue={p.averageRating} allowHalf className="text-xs" />
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
