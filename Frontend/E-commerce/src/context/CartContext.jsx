import { createContext, useContext, useState, useEffect } from 'react';
import cartApi from '../api/cartApi.js';
import { useAuth } from './AuthContext';

const CartContext = createContext(null);

export function CartProvider({ children }) {
	const {user} = useAuth();
	const [items, setItems] = useState([]);

	useEffect (() => {
		if (!user){
			setItems([]);
			return;
		}
		const fetchCart = async () => {
			try {
				const res = await cartApi.getCart();
				setItems(res.data.items || []);
			}catch (err){
				setItems([]);
			}
		}
		fetchCart()
	}, [user])

	const addToCart = async(variantId, quantity) => {
		await cartApi.addItem({productVariantId: variantId, quantity });
		const res = await cartApi.getCart();
		setItems(res.data.items || []);
    	return res.data.items || [];
	}

	const updateQuantity = async(variantId, quantity) => {
    await cartApi.updateItem({ productVariantId: variantId, quantity });
		const res = await cartApi.getCart();
		setItems(res.data.items || []);
	}
	const removeFromCart = async (variantId) => {
		await cartApi.deleteItem(variantId)
		setItems(prev => prev.filter(i => i.productVariantId !== variantId))
	}

	const clearCart = async () => {
		await cartApi.clearCart()
		setItems([])
	}

	const totalItems = items.reduce((s, i) => s + i.quantity, 0)
  	const subtotal = items.reduce((s, i) => s + i.price * i.quantity, 0)

	return (
		<CartContext.Provider value={{ items, addToCart, removeFromCart, updateQuantity, clearCart, totalItems, subtotal }}>
		{children}
		</CartContext.Provider>
	);
}

export const useCart = () => useContext(CartContext);
