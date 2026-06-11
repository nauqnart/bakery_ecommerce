<template>
  <div class="storefront">
    <Header />

    <main class="bg-surface-container-low py-xl relative">
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
        <div class="w-full min-h-[500px]">
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
          <div class="flex-1"></div>
          
          <!-- Pagination UI -->
          <div v-if="!loadingProd && totalPages > 1" class="flex justify-center items-center gap-2">
            <button @click="changePage(currentPage - 1)" :disabled="currentPage === 1" class="px-4 py-2 border border-outline-variant/50 rounded hover:bg-surface-container-low disabled:opacity-50 font-medium">Trước</button>
            <button v-for="p in totalPages" :key="p" @click="changePage(p)" :class="{'bg-[#b36a3a] text-white border-[#b36a3a]': currentPage === p, 'border border-outline-variant/50 text-on-surface-variant hover:bg-surface-container-low': currentPage !== p}" class="px-4 py-2 rounded font-medium">
              {{ p }}
            </button>
            <button @click="changePage(currentPage + 1)" :disabled="currentPage === totalPages" class="px-4 py-2 border border-outline-variant/50 rounded hover:bg-surface-container-low disabled:opacity-50 font-medium">Sau</button>
          </div>
          <div v-else class="flex-1"></div>

          <!-- Page Size -->
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
    </main>

    <Footer />
    
    <QuickViewModal v-if="quickViewProduct" :product="quickViewProduct" @close="quickViewProduct = null" />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import Header from '../components/Header.vue';
import Footer from '../components/Footer.vue';
import QuickViewModal from '../components/QuickViewModal.vue';
import { useCartStore } from '../stores/cartStore';
import { toast } from 'vue3-toastify';

const route = useRoute();
const cart = useCartStore();
const quickViewProduct = ref(null);

const products    = ref([]);
const loadingProd = ref(false);
const currentPage = ref(1);
const pageSize    = ref(20);
const totalPages  = ref(1);
const categories  = ref([]);
const selectedCategory = ref(null);
const searchQuery = ref('');
const sortBy      = ref('newest');
const wishlistIds = ref([]);

onMounted(() => {
  if (route.query.sort) {
    sortBy.value = route.query.sort;
  }
  fetchCategories();
  fetchProducts(currentPage.value);
  fetchWishlist();
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
    }
  } catch(e) {
    console.error('Fetch error', e);
  }
  finally { loadingProd.value = false; }
}

function changePage(page) {
  if(page < 1 || page > totalPages.value) return;
  fetchProducts(page);
  window.scrollTo({ top: 0, behavior: 'smooth' });
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
