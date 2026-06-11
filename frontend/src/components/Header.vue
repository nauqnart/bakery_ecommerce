<template>
  <header class="bg-surface/90 backdrop-blur-md border-b border-outline-variant/30 sticky top-0 z-50 h-[72px] flex items-center">
    <div class="flex justify-between items-center w-full px-margin max-w-[1440px] mx-auto">
      <div class="flex items-center gap-base">
        <router-link to="/" class="font-h2 text-h3 md:text-h2 font-bold text-primary">Hearth &amp; Harvest</router-link>
      </div>

      <!-- Center Navigation -->
      <nav class="hidden md:flex items-center gap-8 font-medium text-on-surface-variant">
        <router-link to="/" class="hover:text-primary transition-colors" active-class="text-primary font-semibold border-b-2 border-primary" exact-active-class="text-primary font-semibold border-b-2 border-primary">Trang chủ</router-link>
        <router-link to="/products" class="hover:text-primary transition-colors" active-class="text-primary font-semibold border-b-2 border-primary">Sản phẩm</router-link>
        <router-link to="/about" class="hover:text-primary transition-colors" active-class="text-primary font-semibold border-b-2 border-primary">Về chúng tôi</router-link>
        <router-link to="/contact" class="hover:text-primary transition-colors" active-class="text-primary font-semibold border-b-2 border-primary">Liên hệ</router-link>
      </nav>

      <div class="flex items-center gap-base">
        <button class="relative p-base text-on-surface-variant hover:bg-surface-container-low transition-all rounded-full flex items-center justify-center" @click="$router.push('/wishlist')" title="Yêu thích">
          <span class="material-symbols-outlined">favorite</span>
        </button>
        <button class="relative p-base text-on-surface-variant hover:bg-surface-container-low transition-all rounded-full flex items-center justify-center" @click="isCartOpen = true" title="Giỏ hàng">
          <span class="material-symbols-outlined">shopping_basket</span>
          <span v-if="cart.totalItems > 0" class="cart-badge">{{ cart.totalItems }}</span>
        </button>
        <button v-if="isAdmin" class="relative p-base text-primary hover:bg-surface-container-low transition-all rounded-full flex items-center justify-center" @click="$router.push('/dashboard')" title="Admin Dashboard">
          <span class="material-symbols-outlined">dashboard</span>
        </button>
        <template v-if="!isLoggedIn">
          <button @click="$router.push('/login')" class="font-body-md bg-primary text-on-primary px-4 py-2 rounded-full font-bold hover:bg-primary/90 transition-colors">Đăng nhập</button>
        </template>
        <template v-else>
          <div class="relative flex items-center justify-center" @click.stop="isAccountMenuOpen = !isAccountMenuOpen">
            <button class="relative p-base text-on-surface-variant hover:bg-surface-container-low transition-all rounded-full flex items-center justify-center" title="Tài khoản">
              <span class="material-symbols-outlined">account_circle</span>
            </button>
            <!-- Account Dropdown -->
            <div v-if="isAccountMenuOpen" class="absolute top-full right-0 mt-2 w-56 bg-surface rounded-xl shadow-xl border border-outline-variant/30 z-50 overflow-hidden flex flex-col py-1">
              <div class="px-4 py-3 border-b border-outline-variant/30 bg-surface-container-low">
                <p class="text-sm font-semibold text-on-surface">{{ userName }}</p>
                <p class="text-xs text-on-surface-variant truncate">{{ userEmail }}</p>
              </div>
              <button v-if="isAdmin" @click="$router.push('/dashboard')" class="text-left px-4 py-2.5 text-sm text-on-surface-variant hover:bg-surface-container-low flex items-center gap-3 transition-colors"><span class="material-symbols-outlined text-[20px]">dashboard</span> Quản trị (Admin)</button>
              <button @click="$router.push('/profile')" class="text-left px-4 py-2.5 text-sm text-on-surface-variant hover:bg-surface-container-low flex items-center gap-3 transition-colors"><span class="material-symbols-outlined text-[20px]">person</span> Trang cá nhân</button>
              <button @click="$router.push('/my-orders')" class="text-left px-4 py-2.5 text-sm text-on-surface-variant hover:bg-surface-container-low flex items-center gap-3 transition-colors"><span class="material-symbols-outlined text-[20px]">receipt_long</span> Lịch sử đơn hàng</button>
              <button @click="handleLogout" class="text-left px-4 py-2.5 text-sm text-error hover:bg-error/10 flex items-center gap-3 border-t border-outline-variant/10 mt-1 transition-colors"><span class="material-symbols-outlined text-[20px]">logout</span> Đăng xuất</button>
            </div>
          </div>
        </template>
      </div>
    </div>

    <Cart v-if="isCartOpen" @close="isCartOpen = false" />
  </header>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue';
import { useRouter } from 'vue-router';
import { useCartStore } from '../stores/cartStore';
import { toast } from 'vue3-toastify';
import Cart from '../screens/Cart.vue';

const isCartOpen = ref(false);
const router  = useRouter();
const cart    = useCartStore();
const isAdmin = ref(false);
const isLoggedIn = ref(false);
const isAccountMenuOpen = ref(false);
const userName = ref('');
const userEmail = ref('');

const closeMenu = () => { isAccountMenuOpen.value = false; };

onMounted(() => {
  const role = localStorage.getItem('role');
  if (role === 'admin' || role === 'Admin') isAdmin.value = true;
  
  const userStr = localStorage.getItem('user');
  if (userStr) {
    try {
      const user = JSON.parse(userStr);
      isLoggedIn.value = true;
      userName.value = user.fullName || 'User';
      userEmail.value = user.email || '';
    } catch(e){}
  }

  document.addEventListener('click', closeMenu);
});

onBeforeUnmount(() => {
  document.removeEventListener('click', closeMenu);
});

const handleLogout = () => {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
  localStorage.removeItem('role');
  cart.items = [];
  localStorage.removeItem('cart');
  isLoggedIn.value = false;
  isAdmin.value = false;
  toast.success('Đã đăng xuất thành công!');
  router.push('/login');
};
</script>

<style scoped>
.cart-badge {
  position: absolute;
  top: 2px; right: 2px;
  background: #b36a3a;
  color: #fff;
  width: 18px; height: 18px;
  border-radius: 50%;
  font-size: .65rem;
  font-weight: 700;
  display: flex;
  align-items: center;
  justify-content: center;
}
.material-symbols-outlined {
  font-family: 'Material Symbols Outlined';
  font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
}
</style>
