<template>
  <div class="storefront">
    <Header />

    <main>
      <!-- Slider Banner Section -->
      <section class="relative w-full h-[500px] overflow-hidden group">
        <div class="absolute inset-0 transition-transform duration-700 ease-in-out flex" :style="{ transform: `translateX(-${currentSlide * 100}%)` }">
          <div v-for="(slide, index) in slides" :key="index" class="min-w-full h-full relative">
            <img :src="slide.image" class="w-full h-full object-cover brightness-75" />
            <div class="absolute inset-0 flex items-center justify-center">
              <div class="text-center text-white px-4 max-w-3xl">
                <h2 class="text-4xl md:text-5xl font-bold mb-4 drop-shadow-lg" style="text-shadow: 0 4px 12px rgba(0,0,0,0.4);">{{ slide.title }}</h2>
                <p class="text-lg md:text-xl drop-shadow-md text-white/90">{{ slide.desc }}</p>
              </div>
            </div>
          </div>
        </div>
        
        <!-- Prev/Next buttons -->
        <button @click="prevSlide" class="absolute left-6 top-1/2 -translate-y-1/2 bg-black/40 hover:bg-black/70 text-white p-3 rounded-full opacity-0 group-hover:opacity-100 transition-opacity backdrop-blur-sm">
          <span class="material-symbols-outlined text-[28px]">chevron_left</span>
        </button>
        <button @click="nextSlide" class="absolute right-6 top-1/2 -translate-y-1/2 bg-black/40 hover:bg-black/70 text-white p-3 rounded-full opacity-0 group-hover:opacity-100 transition-opacity backdrop-blur-sm">
          <span class="material-symbols-outlined text-[28px]">chevron_right</span>
        </button>
        
        <!-- Indicators -->
        <div class="absolute bottom-6 left-1/2 -translate-x-1/2 flex gap-3">
          <button v-for="(s, i) in slides" :key="i" @click="currentSlide = i" :class="['w-3 h-3 rounded-full transition-all duration-300', currentSlide === i ? 'bg-white w-8' : 'bg-white/50 hover:bg-white/80']"></button>
        </div>
      </section>

      <!-- Top 5 Mới Nhất -->
      <section class="bg-surface-container-low py-xl">
        <div class="max-w-[1440px] mx-auto px-margin">
          <div class="flex justify-between items-end mb-8">
            <div>
              <h2 class="text-3xl font-bold text-primary mb-2">✨ Mới Ra Lò</h2>
              <p class="text-on-surface-variant">Những chiếc bánh vừa được chúng tôi thêm vào menu</p>
            </div>
            <router-link to="/products?sort=newest" class="text-primary font-bold hover:underline flex items-center gap-1">Xem tất cả <span class="material-symbols-outlined text-sm">arrow_forward</span></router-link>
          </div>

          <div v-if="loadingNew" class="text-center py-8 text-on-surface-variant">Đang tải...</div>
          <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-6">
            <router-link v-for="product in newProducts" :key="product.productId"
              :to="`/product/${product.productId}`"
              class="bg-surface rounded-xl overflow-hidden border border-outline-variant/30 shadow-sm hover:shadow-md transition-all group cursor-pointer hover:border-primary/30 flex flex-col">
              <div class="relative h-48 overflow-hidden shrink-0">
                <img v-if="product.imageUrl"
                  :src="imgUrl(product.imageUrl)" :alt="product.name"
                  class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500" />
                <div v-else class="w-full h-full bg-surface-container-low flex items-center justify-center">
                  <span class="material-symbols-outlined text-[48px] text-on-surface-variant/30">bakery_dining</span>
                </div>
                <span class="absolute top-2 right-2 bg-primary text-on-primary font-label-caps text-[10px] px-2 py-1 rounded-full">MỚI</span>
                <button @click.stop.prevent="toggleWishlist(product.productId)" 
                  class="absolute top-2 left-2 bg-surface/80 p-1.5 rounded-full text-on-surface-variant hover:text-red-500 hover:bg-surface transition-all">
                  <span class="material-symbols-outlined text-[18px]" :class="{'text-red-500 fill-current': isWishlisted(product.productId)}">favorite</span>
                </button>
              </div>
              <div class="p-4 flex flex-col flex-1">
                <h4 class="font-bold text-on-surface text-sm mb-1 group-hover:text-primary transition-colors line-clamp-2">{{ product.name }}</h4>
                <div class="flex items-center justify-between mt-auto pt-3">
                  <span class="text-primary font-bold">{{ displayPrice(product) }}</span>
                  <button @click.stop.prevent="addToCart(product)"
                    class="bg-on-surface-variant/10 text-on-surface-variant w-8 h-8 rounded-lg flex items-center justify-center hover:bg-primary hover:text-white transition-all active:scale-95">
                    <span class="material-symbols-outlined text-[18px]">{{ product.variants && product.variants.length > 1 ? 'visibility' : 'add_shopping_cart' }}</span>
                  </button>
                </div>
              </div>
            </router-link>
          </div>
        </div>
      </section>

      <!-- Top 5 Bán Chạy -->
      <section class="bg-surface py-xl">
        <div class="max-w-[1440px] mx-auto px-margin">
          <div class="flex justify-between items-end mb-8">
            <div>
              <h2 class="text-3xl font-bold text-primary mb-2">🔥 Bán Chạy Nhất</h2>
              <p class="text-on-surface-variant">Được khách hàng yêu thích và lựa chọn nhiều nhất</p>
            </div>
            <router-link to="/products?sort=hot" class="text-primary font-bold hover:underline flex items-center gap-1">Xem tất cả <span class="material-symbols-outlined text-sm">arrow_forward</span></router-link>
          </div>

          <div v-if="loadingHot" class="text-center py-8 text-on-surface-variant">Đang tải...</div>
          <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-6">
            <router-link v-for="product in hotProducts" :key="product.productId"
              :to="`/product/${product.productId}`"
              class="bg-surface rounded-xl overflow-hidden border border-outline-variant/30 shadow-sm hover:shadow-md transition-all group cursor-pointer hover:border-primary/30 flex flex-col">
              <div class="relative h-48 overflow-hidden shrink-0">
                <img v-if="product.imageUrl"
                  :src="imgUrl(product.imageUrl)" :alt="product.name"
                  class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500" />
                <div v-else class="w-full h-full bg-surface-container-low flex items-center justify-center">
                  <span class="material-symbols-outlined text-[48px] text-on-surface-variant/30">bakery_dining</span>
                </div>
                <button @click.stop.prevent="toggleWishlist(product.productId)" 
                  class="absolute top-2 left-2 bg-surface/80 p-1.5 rounded-full text-on-surface-variant hover:text-red-500 hover:bg-surface transition-all">
                  <span class="material-symbols-outlined text-[18px]" :class="{'text-red-500 fill-current': isWishlisted(product.productId)}">favorite</span>
                </button>
              </div>
              <div class="p-4 flex flex-col flex-1">
                <h4 class="font-bold text-on-surface text-sm mb-1 group-hover:text-primary transition-colors line-clamp-2">{{ product.name }}</h4>
                <div class="flex items-center justify-between mt-auto pt-3">
                  <span class="text-primary font-bold">{{ displayPrice(product) }}</span>
                  <button @click.stop.prevent="addToCart(product)"
                    class="bg-on-surface-variant/10 text-on-surface-variant w-8 h-8 rounded-lg flex items-center justify-center hover:bg-primary hover:text-white transition-all active:scale-95">
                    <span class="material-symbols-outlined text-[18px]">{{ product.variants && product.variants.length > 1 ? 'visibility' : 'add_shopping_cart' }}</span>
                  </button>
                </div>
              </div>
            </router-link>
          </div>
        </div>
      </section>

    </main>

    <Footer />

    <QuickViewModal v-if="quickViewProduct" :product="quickViewProduct" @close="quickViewProduct = null" />
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue';
import Header from '../components/Header.vue';
import Footer from '../components/Footer.vue';
import QuickViewModal from '../components/QuickViewModal.vue';
import { useCartStore } from '../stores/cartStore';
import { toast } from 'vue3-toastify';

const cart = useCartStore();
const quickViewProduct = ref(null);

const newProducts = ref([]);
const hotProducts = ref([]);
const loadingNew  = ref(false);
const loadingHot  = ref(false);
const wishlistIds = ref([]);

// Slider data
const slides = [
  {
    image: 'https://lh3.googleusercontent.com/aida-public/AB6AXuDgEk1by2syfmSzk1FDqoggA2Sz8ePtOdJ5fOAoxKwwbdjqnrcEz9ELgh46cXDrtXUn394pYVOyL9nRiDNq1QSRTrZROQBlGJ4grJpgUcNTyn7Wa36srI7sxhhTrr_APtaLitzGuf2ct3PKc3U2vLSWpoeLgh5N74newi3_QSaKEGYL9bnPw3PkPO6I-pE9UhcuNHPKBnFKD0GDkJ_6WyEQStaJ7T7_En2kfRIh3poL2JpQ7GeUSVwk8M2g1X9u3Iv3dPQx2255_w',
    title: 'Hương Vị Sourdough Đích Thực',
    desc: 'Bánh mì lên men tự nhiên, nướng mới mỗi ngày'
  },
  {
    image: 'https://lh3.googleusercontent.com/aida-public/AB6AXuBNYk2Hw0s-D7OTIHpRrRdcVBxkTn391y7g_-oZMk60KYuK2rsjcQM8RBolt8yv0CI_zxT9BUClHkj1FlMtXrtJIJRzaqxEvm3G24VaHIGuF0WQpSS4KPX68QdYEpM5XZoEcJiFxmR7exorEV2xZzdizyOZuV0r0_55usClGUA7v4ad4eY-uSdgYS-KQ9JdhQwUGQlrhDtUBpRmWSGOFR-2ltUXya7Y8kreM71J6GdpM22cBT_MMz5kHIvOBzrGlNqrt6Sr6b2EGw',
    title: 'Bánh Ngọt Thủ Công',
    desc: 'Làm từ bơ Pháp và tình yêu nghệ thuật làm bánh'
  },
  {
    image: 'https://lh3.googleusercontent.com/aida-public/AB6AXuCaCPYdwS7DuC7WmRivzXeHAmPhAr_wyJEJru-JJ2U__o2VwfQdRcsr2J-plF-GlwpFVTkh3aQWMHUfMwB9n8SOg_nGg5LcvwDiLRh7RvcEFu6cX9kt7V0llfTMrOJb6U5FYZ-nAsCJtO1rA4I0jbfEfses7mbQ4azC8Zc2F9LRhgQcUYoFIGRwzUQXocVEv9ReXVxPgEaZTl6qV7Oi2ujHDrHS_pKZ08sPK7v6QE1hzBqAjJrbIPurONahx9UOHy4m6Zr6foYLCQ',
    title: 'Bánh Kem Kỷ Niệm',
    desc: 'Dành riêng cho những khoảnh khắc đáng nhớ của bạn'
  }
];
const currentSlide = ref(0);
let slideInterval;
const nextSlide = () => { currentSlide.value = (currentSlide.value + 1) % slides.length; };
const prevSlide = () => { currentSlide.value = (currentSlide.value - 1 + slides.length) % slides.length; };

onMounted(() => {
  fetchWishlist();
  fetchNewProducts();
  fetchHotProducts();
  
  slideInterval = setInterval(nextSlide, 5000);
});

onBeforeUnmount(() => {
  if (slideInterval) clearInterval(slideInterval);
});

async function fetchWishlist() {
  const token = localStorage.getItem('token');
  if(!token) return;
  try {
    const res = await fetch(import.meta.env.VITE_API_BASE_URL + '/wishlist', {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    const json = await res.json();
    if(json.success) {
      wishlistIds.value = json.data.map(w => w.productId);
    }
  } catch(e) { console.error(e); }
}

function isWishlisted(productId) {
  return wishlistIds.value.includes(productId);
}

async function toggleWishlist(productId) {
  const token = localStorage.getItem('token');
  if(!token) {
    toast.warning('Vui lòng đăng nhập để lưu sản phẩm yêu thích!');
    return;
  }
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
    if(json.success) {
      if(json.isAdded) {
        wishlistIds.value.push(productId);
        toast.success('Đã thêm vào mục Yêu thích!');
      } else {
        wishlistIds.value = wishlistIds.value.filter(id => id !== productId);
        toast.info('Đã xóa khỏi mục Yêu thích!');
      }
    }
  } catch(e) { console.error(e); }
}

async function fetchNewProducts() {
  loadingNew.value = true;
  try {
    const res  = await fetch(`${import.meta.env.VITE_API_BASE_URL}/product?page=1&pageSize=5&sortBy=newest`);
    const json = await res.json();
    if (json.success) {
        newProducts.value = (json.data || []).map(p => ({
          ...p,
          imageUrl: p.variants?.find(v => v?.imageUrl)?.imageUrl || p.imageUrl
        }));
    }
  } catch(e) { console.error(e); }
  finally { loadingNew.value = false; }
}

async function fetchHotProducts() {
  loadingHot.value = true;
  try {
    const res  = await fetch(`${import.meta.env.VITE_API_BASE_URL}/product?page=1&pageSize=5&sortBy=hot`);
    const json = await res.json();
    if (json.success) {
        hotProducts.value = (json.data || []).map(p => ({
          ...p,
          imageUrl: p.variants?.find(v => v?.imageUrl)?.imageUrl || p.imageUrl
        }));
    }
  } catch(e) { console.error(e); }
  finally { loadingHot.value = false; }
}

const addToCart = product => {
  if (product.variants && product.variants.length > 1) {
    quickViewProduct.value = product;
    return;
  }
  const variant = product.variants?.find(v => v.stockQty > 0) || product.variants?.[0];
  if (!variant || variant.stockQty <= 0) { toast.error('Sản phẩm tạm hết hàng!'); return; }
  cart.addItem(product, variant);
  toast.success(`Đã thêm "${product.name}" vào giỏ hàng!`);
}

const imgUrl = url => url?.startsWith('http') ? url : `${import.meta.env.VITE_BASE_URL}\${url}`;
const fmtMoney = v => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v);
const displayPrice = product => {
  const prices = product.variants?.map(v => v.price) || [];
  if (prices.length === 0) return fmtMoney(0);
  const min = Math.min(...prices);
  return fmtMoney(min);
};
</script>

<style scoped>
.material-symbols-outlined {
  font-family: 'Material Symbols Outlined';
  font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
}
.storefront {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  background-color: #fcf9f8;
}
</style>
