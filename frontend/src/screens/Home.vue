<template>
  <div class="storefront">



<!-- TopAppBar -->
<header class="bg-surface/90 backdrop-blur-md border-b border-outline-variant/30 sticky top-0 z-50 h-[72px] flex items-center">
<div class="flex justify-between items-center w-full px-margin max-w-[1440px] mx-auto">
<div class="flex items-center gap-base">
<router-link to="/" class="font-h2 text-h3 md:text-h2 font-bold text-primary">Hearth &amp; Harvest</router-link>
</div>

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
</header>
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

<!-- Products -->
<section class="bg-surface-container-low py-xl relative">
<div class="max-w-[1440px] mx-auto px-margin">
  
  <!-- Sticky Top Bar Filter -->
  <div class="sticky top-[72px] z-40 bg-surface-container-low/95 backdrop-blur-md pt-4 pb-4 border-b border-outline-variant/30 mb-8 -mx-margin px-margin">
    <div class="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 max-w-[1440px] mx-auto">
      
      <!-- Left: Search -->
      <div class="w-full md:w-80 relative">
        <span class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant text-[20px]">search</span>
        <input v-model="searchQuery" @keyup.enter="fetchProducts(1)" type="text" placeholder="Tìm kiếm bánh..." class="w-full pl-10 pr-3 py-2 border border-outline-variant/50 rounded-lg bg-surface focus:outline-none focus:border-primary shadow-sm" />
      </div>
      
      <!-- Center: Category Chips -->
      <div class="flex items-center gap-2 overflow-x-auto w-full md:flex-1 scrollbar-hide py-1">
        <button @click="selectedCategory = null; fetchProducts(1)" :class="['px-4 py-2 rounded-full text-sm font-medium whitespace-nowrap transition-all', selectedCategory === null ? 'bg-[#b36a3a] text-white shadow-sm border border-[#b36a3a]' : 'bg-surface border border-outline-variant/50 text-on-surface-variant hover:bg-surface-container-low']">Tất cả</button>
        <button v-for="cat in categories" :key="cat.categoryId" @click="selectedCategory = cat.categoryId; fetchProducts(1)" :class="['px-4 py-2 rounded-full text-sm font-medium whitespace-nowrap transition-all', selectedCategory === cat.categoryId ? 'bg-[#b36a3a] text-white shadow-sm border border-[#b36a3a]' : 'bg-surface border border-outline-variant/50 text-on-surface-variant hover:bg-surface-container-low']">{{ cat.name }}</button>
      </div>
      
      <!-- Right: Sort -->
      <div class="flex-shrink-0">
        <select v-model="sortBy" @change="fetchProducts(1)" class="border border-outline-variant/50 rounded-lg px-3 py-2 bg-surface text-sm focus:outline-none min-w-[150px] shadow-sm font-medium">
          <option value="hot">🔥 Bán chạy</option>
          <option value="newest">✨ Mới nhất</option>
          <option value="price_asc">📈 Giá tăng dần</option>
          <option value="price_desc">📉 Giá giảm dần</option>
        </select>
      </div>
    </div>
  </div>

  <!-- Product List -->
  <div class="w-full">
    <div v-if="loadingProd" class="text-center py-8 text-on-surface-variant">Đang tải sản phẩm...</div>
    <div v-else-if="products.length === 0" class="text-center py-8 text-on-surface-variant">Không tìm thấy sản phẩm nào phù hợp.</div>
    <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
<router-link v-for="product in products" :key="product.productId"
  :to="`/product/${product.productId}`"
  class="bg-surface rounded-xl overflow-hidden border border-outline-variant/30 shadow-sm hover:shadow-md transition-all group cursor-pointer hover:border-primary/30 block">
  <div class="relative h-64 overflow-hidden">
    <img v-if="product.imageUrl"
      :src="imgUrl(product.imageUrl)" :alt="product.name"
      class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500" />
    <div v-else class="w-full h-full bg-surface-container-low flex items-center justify-center">
      <span class="material-symbols-outlined text-[64px] text-on-surface-variant/30">bakery_dining</span>
    </div>
    <span v-if="product.isNew" class="absolute top-base right-base bg-primary text-on-primary font-label-caps text-label-caps px-sm py-xs rounded-full">Vừa ra khỏi lò</span>
    <button @click.stop.prevent="toggleWishlist(product.productId)" 
      class="absolute top-base left-base bg-surface/80 p-2 rounded-full text-on-surface-variant hover:text-red-500 hover:bg-surface transition-all">
      <span class="material-symbols-outlined" :class="{'text-red-500 fill-current': isWishlisted(product.productId)}">favorite</span>
    </button>
  </div>
  <div class="p-md">
    <h4 class="font-h3 text-body-lg font-bold text-on-surface mb-xs group-hover:text-primary transition-colors">{{ product.name }}</h4>
    <p class="text-on-surface-variant font-body-sm mb-md overflow-hidden break-words" style="display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; min-height: 2.5rem; word-break: break-word;">{{ product.baseDescription || product.description }}</p>
    <div class="flex items-center justify-between mt-auto">
      <span class="text-primary font-h3 text-h3 font-bold">{{ displayPrice(product) }}</span>
      <button @click.stop.prevent="addToCart(product)"
        class="bg-on-surface-variant/10 text-on-surface-variant w-10 h-10 rounded-lg flex items-center justify-center hover:bg-primary hover:text-white transition-all active:scale-95">
        <span class="material-symbols-outlined">{{ product.variants && product.variants.length > 1 ? 'visibility' : 'add_shopping_cart' }}</span>
      </button>
    </div>
  </div>
</router-link>

</div>
  </div>

<!-- Bottom Actions (Pagination & Page Size) -->
<div class="flex flex-col md:flex-row justify-between items-center mt-xl gap-4">
  <!-- Spacer for flex alignment if pagination doesn't exist -->
  <div class="flex-1"></div>

  <!-- Pagination UI -->
  <div v-if="!loadingProd && totalPages > 1" class="flex justify-center items-center gap-2">
    <button @click="changePage(currentPage - 1)" :disabled="currentPage === 1" class="px-4 py-2 border rounded hover:bg-gray-100 disabled:opacity-50">Trước</button>
    <button v-for="p in totalPages" :key="p" @click="changePage(p)" :class="{'bg-[#b36a3a] text-white': currentPage === p, 'border text-gray-700 hover:bg-gray-100': currentPage !== p}" class="px-4 py-2 rounded">
      {{ p }}
    </button>
    <button @click="changePage(currentPage + 1)" :disabled="currentPage === totalPages" class="px-4 py-2 border rounded hover:bg-gray-100 disabled:opacity-50">Sau</button>
  </div>
  <div v-else class="flex-1"></div>

  <!-- Page Size at bottom right -->
  <div class="flex-1 flex justify-end">
    <div class="flex items-center gap-3 text-sm">
      <span class="text-on-surface-variant font-medium">Hiển thị:</span>
      <select v-model="pageSize" @change="fetchProducts(1)" class="border border-outline-variant/50 hover:border-primary/70 rounded-lg pl-3 pr-8 py-1.5 w-[76px] bg-surface text-on-surface font-semibold focus:outline-none focus:ring-1 focus:ring-primary transition-all cursor-pointer shadow-sm">
        <option :value="20">20</option>
        <option :value="50">50</option>
        <option :value="100">100</option>
      </select>
    </div>
  </div>
</div>

</div>
</section>
</main>
<!-- Footer -->
<footer class="bg-surface-container-highest border-t border-outline-variant/30">
<div class="max-w-[1440px] mx-auto px-margin py-lg">
<div class="flex flex-col md:flex-row justify-between items-start gap-lg mb-lg">
<div class="flex flex-col items-start max-w-xs">
<span class="font-h3 text-h3 font-bold text-primary mb-sm">Hearth &amp; Harvest</span>
<p class="font-body-sm text-on-surface-variant">2024 Tiệm bánh Hearth &amp; Harvest. Chất lượng thủ công, tươi mới mỗi ngày mang linh hồn bánh truyền thống đến bàn ăn hiện đại.</p>
</div>
<div class="grid grid-cols-2 sm:grid-cols-3 gap-lg">
<div class="flex flex-col gap-sm">
<span class="font-label-caps text-on-surface font-bold">Khám phá</span>
<a class="font-body-sm text-on-surface-variant hover:text-primary transition-colors" href="#">Tất cả sản phẩm</a>
<a class="font-body-sm text-on-surface-variant hover:text-primary transition-colors" href="#">Sản phẩm mới</a>
<a class="font-body-sm text-on-surface-variant hover:text-primary transition-colors" href="#">Bán sỉ</a>
</div>
<div class="flex flex-col gap-sm">
<span class="font-label-caps text-on-surface font-bold">Về chúng tôi</span>
<a class="font-body-sm text-on-surface-variant hover:text-primary transition-colors" href="#">Câu chuyện của chúng tôi</a>
<a class="font-body-sm text-on-surface-variant hover:text-primary transition-colors" href="#">Địa điểm</a>
<a class="font-body-sm text-on-surface-variant hover:text-primary transition-colors" href="#">Liên hệ</a>
</div>
<div class="flex flex-col gap-sm">
<span class="font-label-caps text-on-surface font-bold">Pháp lý</span>
<a class="font-body-sm text-on-surface-variant hover:text-primary transition-colors" href="#">Chính sách bảo mật</a>
<a class="font-body-sm text-on-surface-variant hover:text-primary transition-colors" href="#">Điều khoản dịch vụ</a>
</div>
</div>
</div>
<div class="pt-lg border-t border-outline-variant/20 flex flex-col sm:flex-row justify-between items-center gap-md">
<p class="font-body-sm text-on-surface-variant/70">© 2024 Tiệm bánh Hearth &amp; Harvest.</p>
<div class="flex gap-lg">
<a class="text-on-surface-variant hover:text-primary transition-all opacity-80" href="#"><span class="material-symbols-outlined">public</span></a>
<a class="text-on-surface-variant hover:text-primary transition-all opacity-80" href="#"><span class="material-symbols-outlined">mail</span></a>
<a class="text-on-surface-variant hover:text-primary transition-all opacity-80" href="#"><span class="material-symbols-outlined">favorite</span></a>
</div>
</div>
</div>
</footer>

    <Cart v-if="isCartOpen" @close="isCartOpen = false" />
    <QuickViewModal v-if="quickViewProduct" :product="quickViewProduct" @close="quickViewProduct = null" />
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue';
import { useRouter } from 'vue-router';
import Cart from './Cart.vue';
import QuickViewModal from '../components/QuickViewModal.vue';
import { useCartStore } from '../stores/cartStore';
import { toast } from 'vue3-toastify';

const isCartOpen = ref(false);
const quickViewProduct = ref(null);
const router  = useRouter();
const cart    = useCartStore();
const isAdmin = ref(false);
const isLoggedIn = ref(false);
const isAccountMenuOpen = ref(false);
const userName = ref('');
const userEmail = ref('');

// Products from API
const products    = ref([]);
const loadingProd = ref(false);
const currentPage = ref(1);
const pageSize    = ref(20); // Default to 20 items per page
const totalPages  = ref(1);
const categories  = ref([]);
const selectedCategory = ref(null);
const searchQuery = ref('');
const minPrice    = ref(null);
const maxPrice    = ref(null);
const sortBy      = ref('newest');
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

  // Bấm ra ngoài sẽ đóng menu account
  document.addEventListener('click', () => {
    isAccountMenuOpen.value = false;
  });

  fetchCategories();
  fetchProducts(currentPage.value);
  fetchWishlist();
  
  // Bật tự động trượt slider
  slideInterval = setInterval(nextSlide, 5000);
});

onBeforeUnmount(() => {
  if (slideInterval) clearInterval(slideInterval);
});

async function fetchCategories() {
  try {
    const res = await fetch(import.meta.env.VITE_API_BASE_URL + '/category');
    const json = await res.json();
    if(json.success) categories.value = json.data;
  } catch(e) { console.error('Error fetching categories'); }
}

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

async function fetchProducts(page) {
  loadingProd.value = true;
  try {
    let url = `${import.meta.env.VITE_API_BASE_URL}/product?page=${page}&pageSize=${pageSize.value}&sortBy=${sortBy.value}`;
    if (selectedCategory.value) url += `&categoryId=${selectedCategory.value}`;
    if (searchQuery.value) url += `&search=${encodeURIComponent(searchQuery.value)}`;
    
    const res  = await fetch(url);
    const json = await res.json();
    if (json.success) {
        products.value = (json.data || []).map(p => ({
          ...p,
          imageUrl: p.variants?.find(v => v?.imageUrl)?.imageUrl || p.imageUrl
        }));
      totalPages.value = Math.ceil(json.totalCount / pageSize.value) || 1;
      currentPage.value = page;
    } else {
      console.error('API Error:', json.message);
    }
  } catch(e) {
    console.error('Fetch error', e);
  }
  finally { loadingProd.value = false; }
}

function changePage(page) {
  if(page < 1 || page > totalPages.value) return;
  fetchProducts(page);
}




const addToCart = product => {
  if (product.variants && product.variants.length > 1) {
    quickViewProduct.value = product;
    return;
  }
  // Lấy variant đầu tiên có hàng
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

const handleLogout = () => {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
  localStorage.removeItem('role');
  cart.items = []; // Xóa giỏ hàng local khi đăng xuất
  localStorage.removeItem('cart');
  isLoggedIn.value = false;
  isAdmin.value = false;
  toast.success('Đã đăng xuất thành công!');
  router.push('/login');
};
</script>

<style scoped>
        .material-symbols-outlined {
            font-family: 'Material Symbols Outlined';
            font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
        }
        body { background-color: #fcf9f8; color: #1b1c1c; }
        .bento-grid {
            display: grid;
            grid-template-columns: repeat(12, 1fr);
            gap: 24px;
        }
        /* Cart badge */
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

</style>

