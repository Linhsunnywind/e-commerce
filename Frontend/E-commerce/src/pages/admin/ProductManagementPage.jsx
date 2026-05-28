import { useState, useEffect } from 'react';
import { Table, Button, Modal, Form, Input, Select, Tag, Space, InputNumber, Rate, message, Upload } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, SearchOutlined } from '@ant-design/icons';
import { useProducts } from '../../context/ProductContext';
import { productAPi } from '../../api/productApi.js';
import { useCategories } from '../../context/CategoryContext';
import brandApi from '../../api/brandApi';
import { privateClient } from '../../api/axiosInstance';


const emptyVariant = () => ({ id: Date.now() + Math.random(), label: '', price: '', stock: '' });

export default function ProductManagementPage() {
	const { categories } = useCategories();
	const { products: productList, addProduct, updateProduct, deleteProduct } = useProducts();
	const [search, setSearch] = useState('');
	const [catFilter, setCatFilter] = useState('');
	const [showModal, setShowModal] = useState(false);
	const [editing, setEditing] = useState(null);
	const [variants, setVariants] = useState([emptyVariant()]);
	const [form] = Form.useForm();
	const [totalProducts, setTotalProducts] = useState(0);
	const [allProducts, setAllProducts] = useState([]);
	const [brands, setBrands] = useState([]);
	const [imageUrls, setImageUrls] = useState([]);

	const filtered = allProducts.filter(p => {
		const matchSearch = !search ||
		p.name.toLowerCase().includes(search.toLowerCase()) ||
		p.brandName?.toLowerCase().includes(search.toLowerCase());
		const matchCat = !catFilter || p.categoryName === catFilter;
		return matchSearch && matchCat;
	});
	const formatPrice = (p) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(p);

	useEffect(() => {
		brandApi.getAll().then(res => setBrands(res.data || []));
	}, []);

	useEffect(() => {
		productAPi.getAll({ pageSize: 100 }).then(res => setAllProducts(res.data?.items || []));
	}, []);

	const openAdd = () => {
		setEditing(null);
		form.resetFields();
		setVariants([emptyVariant()]);
		setImageUrls([]); 
		setShowModal(true);
	};

	const openEdit = (p) => {
		setEditing(p.id);
		const cat = categories.find(c => c.name === p.categoryName); 
		const brand = brands.find(b => b.name === p.brandName);  // thêm dòng này
		setImageUrls(p.imageUrls || []); 
		form.setFieldsValue({
			name: p.name,
			brandId: brand?.id,   
			categoryId: cat?.id,
			basePrice: p.minPrice,
			description: p.description,
		});
		if (p.variants?.length > 0) {
			setVariants(p.variants.map(v => ({
				id: v.id,         
				label: v.name,
				price: v.price,
				stock: v.quantity,
			})));
		} else {
			setVariants([emptyVariant()]);
		}
		setShowModal(true);
	};

	const handleDelete = (id) => {
		Modal.confirm({
			title: 'Xóa sản phẩm này?',
			okButtonProps: { danger: true },
			onOk: async () => {
				await deleteProduct(id);
				setAllProducts(prev => prev.filter(p => p.id !== id)); // 
			}
		});	
	};

	const handleSave = async (values) => {
		const cleanVariants = variants
			.filter(v => v.label?.trim())
			.map(v => ({
				id: typeof v.id === 'string' ? v.id : undefined,
				name: v.label,
				price: Number(v.price),
				quantity: Number(v.stock),
			}));

		try {
			if (editing) {
				await updateProduct(editing, {
					name: values.name,
					description: values.description,
					categoryId: values.categoryId,
					brandId: values.brandId,
					variants: cleanVariants,
					imageUrls: imageUrls,
				});
			} else {
				await addProduct({
					name: values.name,
					description: values.description,
					categoryId: values.categoryId,
					brandId: values.brandId,
					variants: cleanVariants,
					imageUrls: imageUrls,
				});
			}
			const res = await productAPi.getAll({ pageSize: 100 });
			setAllProducts(res.data?.items || []);
			setShowModal(false);
			message.success(editing ? 'Cập nhật thành công!' : 'Thêm sản phẩm thành công!');
		} catch (err) {
			console.error('Save error:', err);
			message.error(err?.response?.data?.message || 'Lỗi khi lưu sản phẩm');
		}
	};

	const handleImageUpload = async ({ file, onSuccess, onError }) => {
    const formData = new FormData();
		formData.append('file', file);
		try {
			const res = await privateClient.post('/images/upload', formData, {
				headers: { 'Content-Type': 'multipart/form-data' }
			});
			const url = res.data; // URL string từ backend
			setImageUrls(prev => [...prev, url]);
			onSuccess();
		} catch {
			onError();
		}
	};

	const getCatName = (id) => categories.find(c => c.id === id)?.name || '—';

	const columns = [
		{
			title: 'Ảnh',
			dataIndex: 'imageUrls',
			width: 70,
			render: (imgs) => (
			<img src={imgs?.[0]} alt="" className="w-12 h-12 object-cover rounded-lg bg-gray-100" />
			),
		},
		{
			title: 'Tên sản phẩm',
			dataIndex: 'name',
			render: (name, r) => (
			<div>
				<div className="font-medium text-sm">{name}</div>
				<div className="text-xs text-gray-400">{r.brandName} · {r.categoryName}</div>
			</div>
			),
		},
		{
			title: 'Giá thấp nhất',
			dataIndex: 'minPrice',
			width: 140,
			render: (p) => (
			<span className="font-semibold text-blue-600">
				{new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(p)}
			</span>
			),
		},
		{
			title: 'Tổng tồn kho',
			width: 110,
			render: (_, r) => {
			const total = r.variants?.reduce((s, v) => s + v.quantity, 0) ?? 0;
			return (
				<Tag color={total > 0 ? 'green' : 'red'}>
				{total} sp ({r.variants?.length || 0} phiên bản)
				</Tag>
			);
			},
		},
		{
			title: 'Đánh giá',
			dataIndex: 'averageRating',
			width: 110,
			render: (rating) => (
			<div className="flex items-center gap-1">
				<span className="text-xs text-gray-500">{rating?.toFixed(1)}</span>
			</div>
			),
		},
		{
			title: 'Hành động',
			width: 100,
			render: (_, r) => (
			<Space>
				<Button
				size="small"
				icon={<EditOutlined />}
				onClick={() => openEdit(r)}
				/>
				<Button
				size="small"
				danger
				icon={<DeleteOutlined />}
				onClick={() => handleDelete(r.id)}
				/>
			</Space>
			),
		},
		];


	return (
		<div className="flex flex-col gap-4">
			<div className="flex gap-3 flex-wrap">
				<Input prefix={<SearchOutlined />} placeholder="Tìm sản phẩm..." value={search} onChange={e => setSearch(e.target.value)} className="w-64" />
				<Select placeholder="Tất cả danh mục" value={catFilter || undefined} onChange={v => setCatFilter(v || '')} allowClear className="w-48">
					{categories.map(c => <Select.Option key={c.id} value={String(c.name)}>{c.name}</Select.Option>)}
				</Select>
				<Button type="primary" icon={<PlusOutlined />} onClick={openAdd}>Thêm sản phẩm</Button>
			</div>

			<Table columns={columns} dataSource={filtered} rowKey="id" scroll={{ x: true }} size="small"
				pagination={{ pageSize: 10 }}
				locale={{ emptyText: '📦 Không có sản phẩm nào' }}
			/>

			<Modal
				title={editing ? 'Chỉnh sửa sản phẩm' : 'Thêm sản phẩm mới'}
				open={showModal}
				onCancel={() => setShowModal(false)}
				footer={null}
				width={720}
			>
				<Form form={form} layout="vertical" onFinish={handleSave}>
					<div className="grid grid-cols-2 gap-x-4">
						<Form.Item label="Tên sản phẩm *" name="name" rules={[{ required: true }]}><Input /></Form.Item>
						<Form.Item label="Thương hiệu *" name="brandId" rules={[{ required: true }]}>
							<Select placeholder="Chọn thương hiệu" showSearch
								filterOption={(input, opt) => opt.label.toLowerCase().includes(input.toLowerCase())}
								options={brands.map(b => ({ value: b.id, label: b.name }))}
							/>
						</Form.Item>
						<Form.Item label="Danh mục *" name="categoryId" rules={[{ required: true }]}>
							<Select placeholder="Chọn danh mục">
								{categories.map(c => <Select.Option key={c.id} value={c.id}>{c.name}</Select.Option>)}
							</Select>
						</Form.Item>
						<Form.Item label="Hình ảnh">
							<Upload customRequest={handleImageUpload} listType="picture-card" accept="image/*">
								<div>+ Upload</div>
							</Upload>
						</Form.Item>
						<Form.Item label="Mô tả" name="description" className="col-span-2"><Input.TextArea rows={2} /></Form.Item>
					</div>

					<div className="border-t border-gray-100 pt-4 mt-2">
						<div className="flex justify-between items-center mb-3">
							<p className="text-xs font-semibold text-gray-500 uppercase">Biến thể ({variants.length})</p>
							<Button size="small" onClick={() => setVariants(v => [...v, emptyVariant()])}>+ Thêm biến thể</Button>
						</div>
						<div className="grid grid-cols-[1fr_140px_100px_32px] gap-2 mb-2">
							<span className="text-xs text-gray-400">Tên biến thể</span>
							<span className="text-xs text-gray-400">Giá (VND)</span>
							<span className="text-xs text-gray-400">Kho</span>
							<span />
						</div>
						{variants.map((v, idx) => (
							<div key={v.id} className="grid grid-cols-[1fr_140px_100px_32px] gap-2 mb-2 items-center">
								<Input placeholder={`Biến thể ${idx + 1}`} value={v.label} onChange={e => setVariants(vl => vl.map(vt => vt.id === v.id ? { ...vt, label: e.target.value } : vt))} size="small" />
								<InputNumber placeholder="Giá" value={v.price} onChange={val => setVariants(vl => vl.map(vt => vt.id === v.id ? { ...vt, price: val } : vt))} size="small" className="w-full" min={0.01} formatter={val => `${val}`.replace(/\B(?=(\d{3})+(?!\d))/g, '.')} parser={val => val.replace(/\./g, '')}/>
								<InputNumber placeholder="Kho" value={v.stock} onChange={val => setVariants(vl => vl.map(vt => vt.id === v.id ? { ...vt, stock: val } : vt))} size="small" className="w-full" min={0} formatter={val => `${val}`.replace(/\B(?=(\d{3})+(?!\d))/g, '.')} parser={val => val.replace(/\./g, '')}/>
								<Button size="small" danger type="text" disabled={variants.length === 1} onClick={() => setVariants(vl => vl.filter(vt => vt.id !== v.id))}>✕</Button>
							</div>
						))}
					</div>

					<div className="flex justify-end gap-2 mt-4 pt-4 border-t border-gray-100">
						<Button onClick={() => setShowModal(false)}>Hủy</Button>
						<Button type="primary" htmlType="submit">{editing ? 'Lưu thay đổi' : 'Thêm sản phẩm'}</Button>
					</div>
				</Form>
			</Modal>
		</div>
	);
}
