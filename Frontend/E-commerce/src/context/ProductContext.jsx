import { createContext, useContext, useState, useEffect } from 'react';
import { productAPi } from '../api/productApi.js';

const ProductContext = createContext(null);

export function ProductProvider({ children }) {
	const [products, setProducts] = useState([]);
	const [loading, setLoading] = useState(true);
	useEffect(() => {
		const fetchProducts = async () => {
			try {
				const res = await productAPi.getAll();
				setProducts(res.data.items);
			}
			catch (err) {
				console.error("Failed to fetch products: ", err);
			}
			finally {
				setLoading(false);
			}
		};
		fetchProducts();
	}, []);
	const addProduct = async (data) => {
		await productAPi.create(data);
		// reload lại products
		const res = await productAPi.getAll();
		setProducts(res.data?.items || []);
	};
	const updateProduct = async (id, data) => {
		await productAPi.update(id, data);
		const res = await productAPi.getAll({ pageSize: 100 });
		setProducts(res.data?.items || []);
	};

	const deleteProduct = async (id) => {
		await productAPi.delete(id);
		setProducts(prev => prev.filter(p => p.id !== id));
	};

	const getFeaturedProducts = () => products.slice(0, 8);
	const getProductBySlug = (slug) => products.find(p => p.slug === slug);
	const getProductById = (id) => products.find(p => p.id === id)

	return (
		<ProductContext.Provider value={{ products, loading, getProductById, getFeaturedProducts, addProduct, updateProduct, deleteProduct }}>
			{children}
		</ProductContext.Provider>
	);
}

export const useProducts = () => useContext(ProductContext);
