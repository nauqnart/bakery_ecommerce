import { createRouter, createWebHistory } from 'vue-router'

import Home            from './screens/Home.vue'
import Dashboard       from './screens/Dashboard.vue'
import InventoryControl from './screens/InventoryControl.vue'
import CategoryManagement from './screens/CategoryManagement.vue'
import UserManagement  from './screens/UserManagement.vue'
import RevenueDashboard from './screens/RevenueDashboard.vue'
import PaymentHistory  from './screens/PaymentHistory.vue'
import Login           from './screens/Login.vue'
import Cart            from './screens/Cart.vue'
import Checkout        from './screens/Checkout.vue'
import ForgotPassword  from './screens/ForgotPassword.vue'
import ResetPassword   from './screens/ResetPassword.vue'
import MyOrders        from './screens/MyOrders.vue'
import Profile         from './screens/Profile.vue'

import ProductDetails  from './screens/ProductDetails.vue'
import Wishlist        from './screens/Wishlist.vue'
import About           from './screens/About.vue'
import Contact         from './screens/Contact.vue'
import Products        from './screens/Products.vue'

const routes = [
  { path: '/',          component: Home },
  { path: '/products',  component: Products },
  { path: '/dashboard', component: Dashboard, meta: { requiresAuth: true, requiresStaff: true } },
  { path: '/categories', component: CategoryManagement, meta: { requiresAuth: true, requiresStaff: true } },
  { path: '/inventory', component: InventoryControl, meta: { requiresAuth: true, requiresStaff: true } },
  { path: '/users',     component: UserManagement, meta: { requiresAuth: true, requiresAdmin: true } },
  { path: '/revenue',   component: RevenueDashboard, meta: { requiresAuth: true, requiresAdmin: true } },
  { path: '/payments',  component: PaymentHistory, meta: { requiresAuth: true, requiresStaff: true } },
  { path: '/login',     component: Login },
  { path: '/cart',      component: Cart },
  { path: '/checkout',  component: Checkout, meta: { requiresAuth: true } },
  { path: '/product/:id', component: ProductDetails },
  { path: '/forgot-password', component: ForgotPassword },
  { path: '/reset-password', component: ResetPassword },
  { path: '/my-orders', component: MyOrders, meta: { requiresAuth: true } },
  { path: '/profile', component: Profile, meta: { requiresAuth: true } },
  { path: '/wishlist', component: Wishlist, meta: { requiresAuth: true } },
  { path: '/about', component: About },
  { path: '/contact', component: Contact },
  { path: '/:pathMatch(.*)*', name: 'NotFound', component: () => import('./screens/NotFound.vue') },
]


export const router = createRouter({
  history: createWebHistory(),
  routes
})

function isTokenExpired(token) {
  try {
    const payloadBase64 = token.split('.')[1];
    const decodedJson = atob(payloadBase64);
    const decoded = JSON.parse(decodedJson);
    const exp = decoded.exp;
    const now = Math.floor(Date.now() / 1000);
    return exp < now;
  } catch (e) {
    return true; // If we can't parse it, treat it as expired
  }
}

router.beforeEach((to, from, next) => {
  const userStr = localStorage.getItem('user');
  const user = userStr ? JSON.parse(userStr) : null;
  const isAuthenticated = user && user.token && !isTokenExpired(user.token);
  const isAdmin = user && user.role && user.role.toLowerCase() === 'admin';
  const isStaff = user && user.role && user.role.toLowerCase() === 'staff';

  if (!isAuthenticated && user && user.token) {
    // Token expired, clear storage
    localStorage.removeItem('user');
  }

  if (to.meta.requiresAuth && !isAuthenticated) {
    // If not authenticated (or token expired), go to login
    next({ path: '/login', query: { redirect: to.fullPath } });
  } else if (to.meta.requiresAdmin && !isAdmin) {
    // If authenticated but not admin trying to access admin pages
    next({ path: '/' });
  } else if (to.meta.requiresStaff && !isAdmin && !isStaff) {
    // If authenticated but not admin/staff trying to access staff pages
    next({ path: '/' });
  } else {
    next();
  }
})
