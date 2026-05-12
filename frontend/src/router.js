import { createRouter, createWebHistory } from 'vue-router'

import Storefront      from './screens/Storefront.vue'
import Dashboard       from './screens/Dashboard.vue'
import InventoryControl from './screens/InventoryControl.vue'
import UserManagement  from './screens/UserManagement.vue'
import RevenueDashboard from './screens/RevenueDashboard.vue'
import PaymentHistory  from './screens/PaymentHistory.vue'
import Login           from './screens/Login.vue'
import Register        from './screens/Register.vue'
import Cart            from './screens/Cart.vue'
import Checkout        from './screens/Checkout.vue'

import ProductDetails  from './screens/ProductDetails.vue'

const routes = [
  { path: '/',          component: Storefront },
  { path: '/dashboard', component: Dashboard },
  { path: '/inventory', component: InventoryControl },
  { path: '/users',     component: UserManagement },
  { path: '/revenue',   component: RevenueDashboard },
  { path: '/payments',  component: PaymentHistory },
  { path: '/login',     component: Login },
  { path: '/register',  component: Register },
  { path: '/cart',      component: Cart },
  { path: '/checkout',  component: Checkout },
  { path: '/product/:id', component: ProductDetails },
]


export const router = createRouter({
  history: createWebHistory(),
  routes
})

