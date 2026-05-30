import { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Button, Rate } from 'antd';
import { useProducts } from '../../context/ProductContext';
import { useCategories } from '../../context/CategoryContext';

const formatPrice = (p) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(p);

function ProductCard ({ product }) {
  const navigate = useNavigate();
  return (
    <div className="product-card" onClick={() => navigate(`/products/${product.id}`)}>
      <div className="product-card-img">
        <img src={product.imageUrls?.[0] || '/placeholder.png'} alt={product.name} />
      </div>
      <div className="p-3.5">
        <p className="text-xs text-gray-500 mb-1">{product.brandName}</p>
        <p className="font-semibold text-sm text-gray-800 mb-1.5 line-clamp-2">{product.name}</p>
        <p className="text-base font-bold text-blue-600">{formatPrice(product.minPrice)}</p>
        <div className="flex items-center gap-1 mt-1.5">
          <Rate disabled defaultValue={product.averageRating} allowHalf className="text-xs" />
          
        </div>
      </div>
    </div>
  );
}

const BG_GRADIENTS = [
  'linear-gradient(135deg,#1e3a5f,#2563eb)',
  'linear-gradient(135deg,#111827,#374151)',
  'linear-gradient(135deg,#1e40af,#7c3aed)',
];

const CAT_ICON_MAP = [
  { keywords: ['điện thoại', 'phone', 'iphone', 'samsung galaxy', 'xiaomi'], icon: '📱' },
  { keywords: ['laptop', 'macbook', 'notebook', 'máy tính xách tay'], icon: '💻' },
  { keywords: ['máy tính bảng', 'tablet', 'ipad'], icon: '📱' },
  { keywords: ['tai nghe', 'airpod', 'headphone', 'earphone'], icon: '🎧' },
  { keywords: ['đồng hồ', 'watch', 'smartwatch'], icon: '⌚' },
  { keywords: ['màn hình', 'monitor', 'display'], icon: '🖥️' },
  { keywords: ['bàn phím', 'keyboard', 'chuột', 'mouse'], icon: '🖱️' },
  { keywords: ['phụ kiện', 'accessory', 'sạc', 'cáp', 'cable'], icon: '🔌' },
  { keywords: ['loa', 'speaker'], icon: '🔊' },
  { keywords: ['máy ảnh', 'camera'], icon: '📷' },
  { keywords: ['gaming', 'game'], icon: '🎮' },
];

const getCatIcon = (name) => {
  const lower = name.toLowerCase();
  const match = CAT_ICON_MAP.find(m => m.keywords.some(k => lower.includes(k)));
  return match ? match.icon : '📦';
};

export default function HomePage() {
  const { getFeaturedProducts,  loading  } = useProducts();
  const featured = getFeaturedProducts();
  const [slide, setSlide] = useState(0);
  const {categories} = useCategories();
  const navigate = useNavigate();
  const slides = featured.slice(0, 3).map((p, i) => ({
    id: p.id,
    title: p.name,
    sub: p.brandName ? `${p.brandName} — Từ ${new Intl.NumberFormat('vi-VN').format(p.minPrice)}₫` : '',
    image: p.imageUrls?.[0] || null,
    cta: 'Mua ngay',
    link: `/products/${p.id}`,
    bg: BG_GRADIENTS[i % BG_GRADIENTS.length],
  }));

  useEffect(() => {
    if (slides.length === 0) return;
    const t = setInterval(() => setSlide(s => (s + 1) % slides.length), 4000);
    return () => clearInterval(t);
  }, [slides.length]);

  return (
    <div>
      {/* Hero Slider */}
      {slides.length > 0 && (
        <section
          className="py-16 px-4 transition-all duration-500 relative overflow-hidden"
          style={{
            background: slides[slide].bg,
            ...(slides[slide].image ? {
              backgroundImage: `linear-gradient(rgba(0,0,0,0.55),rgba(0,0,0,0.55)), url(${slides[slide].image})`,
              backgroundSize: 'cover',
              backgroundPosition: 'center',
            } : {})
          }}
        >
          <div className="container text-center text-white">
            <h1 className="text-4xl font-bold mb-3 md:text-2xl">{slides[slide].title}</h1>
            <p className="text-lg text-blue-100 mb-6">{slides[slide].sub}</p>
            <Link to={slides[slide].link}>
              <Button size="large" className="bg-white text-blue-600 border-0 font-semibold hover:bg-blue-50 px-8">
                {slides[slide].cta}
              </Button>
            </Link>
            <div className="flex justify-center gap-2 mt-8">
              {slides.map((_, i) => (
                <button
                  key={i}
                  onClick={() => setSlide(i)}
                  className={`w-2.5 h-2.5 rounded-full transition-all ${i === slide ? 'bg-white w-6' : 'bg-white/40'}`}
                />
              ))}
            </div>
          </div>
        </section>
      )}

      {/* Categories */}
      <section className="py-12">
        <div className="container">
          <h2 className="text-2xl font-bold text-gray-800 mb-6">Danh mục <span className="text-blue-600">sản phẩm</span></h2>
          <div className="grid grid-cols-[repeat(auto-fill,minmax(120px,1fr))] gap-4">
            {categories.map(cat => (
              <Link
                key={cat.name}
                to={`/products?category=${cat.name}`}
                className="flex flex-col items-center gap-2 p-4 bg-white rounded-xl shadow-sm hover:shadow-md hover:-translate-y-1 transition-all text-center"
              >
                <div className="text-3xl">{getCatIcon(cat.name)}</div>
                <p className="text-sm font-medium text-gray-700">{cat.name}</p>
              </Link>
            ))}
          </div>
        </div>
      </section>

      <section className="py-12 bg-white">
        <div className="container">
          <div className="flex justify-between items-center mb-6">
            <h2 className="text-2xl font-bold text-gray-800">Sản phẩm <span className="text-blue-600">nổi bật</span></h2>
            <Link to="/products"><Button>Xem tất cả →</Button></Link>
          </div>
          {loading ? (
            <div className="grid grid-cols-[repeat(auto-fill,minmax(220px,1fr))] gap-5">
              {Array.from({ length: 8 }).map((_, i) => (
                <div key={i} className="rounded-xl bg-gray-100 animate-pulse h-64" />
              ))}
            </div>
          ) : (
            <div className="grid grid-cols-[repeat(auto-fill,minmax(220px,1fr))] gap-5">
              {featured.map(p => <ProductCard key={p.id} product={p} />)}
            </div>
          )}
        </div>
      </section>

      {/* Features */}
      <section className="py-12">
        <div className="container">
          <h2 className="text-2xl font-bold text-gray-800 text-center mb-8">Tại sao chọn <span className="text-blue-600">TechShop?</span></h2>
          <div className="grid grid-cols-[repeat(auto-fill,minmax(220px,1fr))] gap-5">
            {[
              { icon: '✅', title: 'Hàng chính hãng 100%', desc: 'Cam kết tất cả sản phẩm có nguồn gốc rõ ràng, tem chính hãng' },
              { icon: '🚚', title: 'Giao hàng nhanh', desc: 'Giao hàng trong 24h tại TP.HCM, 2-3 ngày toàn quốc' },
              { icon: '🔄', title: 'Đổi trả 30 ngày', desc: 'Chính sách đổi trả linh hoạt trong vòng 30 ngày' },
              { icon: '🛡️', title: 'Bảo hành tận nơi', desc: 'Hỗ trợ bảo hành tận nhà, không cần mang máy đến shop' },
            ].map((f, i) => (
              <div key={i} className="bg-white rounded-xl p-6 shadow-sm text-center hover:shadow-md transition-shadow">
                <div className="text-4xl mb-3">{f.icon}</div>
                <h3 className="font-semibold text-gray-800 mb-2">{f.title}</h3>
                <p className="text-sm text-gray-500">{f.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>
    </div>
  );
}
