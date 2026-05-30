import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { CartProvider } from './context/CartContext';
import { ProductProvider } from './context/ProductContext';
import { CategoryProvider } from './context/CategoryContext'
import { OrderProvider } from './context/OrderContext'


import CustomerLayout from './layouts/CustomerLayout';
import AdminLayout from './layouts/AdminLayout';
import StaffLayout from './layouts/StaffLayout';

// Auth
import LoginPage from './pages/auth/LoginPage';
import RegisterPage from './pages/auth/RegisterPage';

// Customer
import HomePage from './pages/customer/HomePage';
import ProductsPage from './pages/customer/ProductsPage';
import ProductDetailPage from './pages/customer/ProductDetailPage';
import CartPage from './pages/customer/CartPage';
import CheckoutPage from './pages/customer/CheckoutPage';
import OrderTrackingPage from './pages/customer/OrderTrackingPage';
import AccountPage from './pages/customer/AccountPage';
import SupportPage from './pages/customer/SupportPage';

// Admin
import DashboardPage from './pages/admin/DashboardPage';
import ProductManagementPage from './pages/admin/ProductManagementPage';
import OrderManagementPage from './pages/admin/OrderManagementPage';
import CustomerManagementPage from './pages/admin/CustomerManagementPage';
import CategoryManagementPage from './pages/admin/CategoryManagementPage';
import PromotionManagementPage from './pages/admin/PromotionManagementPage';
import ReportPage from './pages/admin/ReportPage';
import PaymentMethodPage from './pages/admin/PaymentMethodPage';
import StaffManagementPage from './pages/admin/StaffManagementPage';

// Staff
import StaffOrderPage from './pages/staff/StaffOrderPage';
import StaffSupportPage from './pages/staff/StaffSupportPage';
import StaffInventoryPage from './pages/staff/StaffInventoryPage';

export default function App() {
return (
	<BrowserRouter>
		<AuthProvider>
			<CategoryProvider>
				<ProductProvider>
					<CartProvider>
						<OrderProvider>
							<Routes>
								{/* Auth */}
								<Route path="/login" element={<LoginPage />} />
								<Route path="/register" element={<RegisterPage />} />

								{/* Customer */}
								<Route element={<CustomerLayout />}>
								<Route path="/" element={<HomePage />} />
								<Route path="/products" element={<ProductsPage />} />
								<Route path="/products/:id" element={<ProductDetailPage />} />
								<Route path="/cart" element={<CartPage />} />
								<Route path="/checkout" element={<CheckoutPage />} />
								<Route path="/orders" element={<OrderTrackingPage />} />
								<Route path="/account" element={<AccountPage />} />
								<Route path="/support" element={<SupportPage />} />
								</Route>

								{/* Admin */}
								<Route path="/admin" element={<AdminLayout />}>
								<Route index element={<Navigate to="/admin/dashboard" replace />} />
								<Route path="dashboard" element={<DashboardPage />} />
								<Route path="products" element={<ProductManagementPage />} />
								<Route path="orders" element={<OrderManagementPage />} />
								<Route path="customers" element={<CustomerManagementPage />} />
								<Route path="categories" element={<CategoryManagementPage />} />
								<Route path="promotions" element={<PromotionManagementPage />} />
								<Route path="reports" element={<ReportPage />} />
								<Route path="payment-methods" element={<PaymentMethodPage />} />
								<Route path="staff" element={<StaffManagementPage />} />
								</Route>

								{/* Staff */}
								<Route path="/staff" element={<StaffLayout />}>
								<Route index element={<Navigate to="/staff/orders" replace />} />
								<Route path="orders" element={<StaffOrderPage />} />
								<Route path="support" element={<StaffSupportPage />} />
								<Route path="inventory" element={<StaffInventoryPage />} />
								</Route>

								<Route path="*" element={<Navigate to="/" replace />} />
							</Routes>
						</OrderProvider>
					</CartProvider>
				</ProductProvider>
			</CategoryProvider>
		</AuthProvider>
	</BrowserRouter>
);
}
