<template>
  <div class="wishlist bg-surface min-h-screen pb-xl pt-24">
    <!-- TopAppBar (Minimal) -->
    <header class="bg-surface/90 backdrop-blur-md border-b border-outline-variant/30 fixed top-0 w-full z-50">
      <div class="flex justify-between items-center w-full px-margin py-base max-w-[1440px] mx-auto">
        <router-link to="/" class="font-h2 text-h3 md:text-h2 font-bold text-primary flex items-center gap-2">
          <span class="material-symbols-outlined">arrow_back</span>
          Tiếp tục mua sắm
        </router-link>
      </div>
    </header>

    <!-- Toast notification -->
    <transition name="toast">
      <div v-if="toast" class="toast-notif">
        <span class="material-symbols-outlined">check_circle</span> {{ toast }}
      </div>
    </transition>

    <main class="max-w-[1440px] mx-auto px-margin pt-xl">
      <h1 class="font-h1 text-h1 text-on-surface mb-xs">Danh sách yêu thích</h1>
      <p class="text-on-surface-variant font-body-lg mb-xl">Những chiếc bánh mà bạn đã "thả tim" ❤️</p>

      <div v-if="loading" class="text-center py-8 text-on-surface-variant">Đang tải danh sách...</div>

      <div v-else-if="wishlists.length === 0" class="text-center py-16">
        <span class="material-symbols-outlined text-[80px] text-on-surface-variant/30 mb-md">heart_broken</span>
        <h3 class="font-h3 text-h3 text-on-surface mb-sm">Danh sách yêu thích trống</h3>
        <p class="text-on-surface-variant mb-md">Bạn chưa lưu sản phẩm nào vào danh sách yêu thích.</p>
        <router-link to="/" class="bg-primary text-on-primary px-lg py-md rounded-lg font-button hover:opacity-90 transition-all inline-block">
          Khám phá cửa hàng
        </router-link>
      </div>

      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-gutter">
        <div v-for="item in wishlists" :key="item.wishlistId"
          class="bg-surface rounded-xl overflow-hidden border border-outline-variant/30 shadow-sm relative group">
          <div class="relative h-64 overflow-hidden cursor-pointer" @click="$router.push(`/product/${item.productId}`)">
            <img v-if="item.product.variants && item.product.variants.length > 0 && item.product.variants[0].imageUrl"
              :src="imgUrl(item.product.variants[0].imageUrl)" :alt="item.product.name"
              class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500" />
            <div v-else class="w-full h-full bg-surface-container-low flex items-center justify-center">
              <span class="material-symbols-outlined text-[64px] text-on-surface-variant/30">bakery_dining</span>
            </div>
            
            <button @click.stop.prevent="removeWishlist(item.productId)" 
              class="absolute top-base right-base bg-surface/80 p-2 rounded-full text-red-500 hover:bg-surface transition-all">
              <span class="material-symbols-outlined fill-current">delete</span>
            </button>
          </div>
          <div class="p-md">
            <h4 class="font-h3 text-body-lg font-bold text-on-surface mb-xs">{{ item.product.name }}</h4>
            <div class="flex items-center justify-between mt-md">
              <span class="text-primary font-h3 text-h3">{{ fmtMoney(minPrice(item.product)) }}</span>
              <button @click="addToCart(item.product)"
                class="bg-primary/10 text-primary w-10 h-10 rounded-lg flex items-center justify-center hover:bg-primary hover:text-white transition-all active:scale-95">
                <span class="material-symbols-outlined">add_shopping_cart</span>
              </button>
            </div>
          </div>
        </div>
      </div>
    </main>
    <Cart v-if="isCartOpen" @close="isCartOpen = false" />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useCartStore } from '../stores/cartStore';
import Cart from './Cart.vue';

const router = useRouter();
const cart = useCartStore();
const wishlists = ref([]);
const loading = ref(true);
const toast = ref('');
const isCartOpen = ref(false);

onMounted(() => {
  fetchWishlists();
});

async function fetchWishlists() {
  const token = localStorage.getItem('token');
  if (!token) {
    router.push('/login');
    return;
  }
  loading.value = true;
  try {
    const res = await fetch(import.meta.env.VITE_API_BASE_URL + '/wishlist', {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    const json = await res.json();
    if (json.success) {
      wishlists.value = json.data;
    }
  } catch (error) {
    console.error('Error fetching wishlists:', error);
  } finally {
    loading.value = false;
  }
}

async function removeWishlist(productId) {
  const token = localStorage.getItem('token');
  if (!token) return;
  try {
    const res = await fetch(import.meta.env.VITE_API_BASE_URL + '/wishlist/toggle', {
      method: 'POST',
      headers: { 
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ productId })
    });
    const json = await res.json();
    if (json.success) {
      wishlists.value = wishlists.value.filter(w => w.productId !== productId);
      showToast('Đã xóa khỏi danh sách yêu thích!');
    }
  } catch (e) {
    console.error(e);
  }
}

function addToCart(product) {
  const variant = product.variants?.find(v => v.stockQty > 0) || product.variants?.[0];
  if (!variant) {
    showToast('Sản phẩm tạm hết hàng!');
    return;
  }
  cart.addItem(product, variant);
  showToast(`Đã thêm "${product.name}" vào giỏ hàng!`);
  isCartOpen.value = true;
}

function showToast(msg) {
  toast.value = msg;
  setTimeout(() => toast.value = '', 2500);
}

const imgUrl = url => url?.startsWith('http') ? url : `${import.meta.env.VITE_BASE_URL}\${url}`;
const fmtMoney = v => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v);
const minPrice = product => {
  const prices = product.variants?.map(v => v.price) || [];
  return prices.length ? Math.min(...prices) : 0;
};
</script>

<style scoped>
.material-symbols-outlined {
  font-family: 'Material Symbols Outlined';
  font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
}
.toast-notif {
  position: fixed;
  bottom: 2rem; left: 50%;
  transform: translateX(-50%);
  background: #2c2018;
  color: #fff;
  padding: .75rem 1.5rem;
  border-radius: 50px;
  font-size: .875rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: .5rem;
  z-index: 9999;
  box-shadow: 0 8px 24px rgba(0,0,0,.25);
}
.toast-enter-active, .toast-leave-active { transition: all .3s ease; }
.toast-enter-from, .toast-leave-to { opacity: 0; transform: translateX(-50%) translateY(16px); }
</style>
