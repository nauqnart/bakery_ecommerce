<template>
  <div class="product-details-page">
    <!-- TopAppBar -->
    <header class="bg-surface/90 backdrop-blur-md border-b border-outline-variant/30 sticky top-0 z-50">
      <div class="flex justify-between items-center w-full px-margin py-base max-w-[1440px] mx-auto">
        <div class="flex items-center gap-base">
          <router-link to="/" class="font-h2 text-h3 md:text-h2 font-bold text-primary">Hearth &amp; Harvest</router-link>
        </div>
        <nav class="hidden md:flex items-center gap-lg">
          <router-link to="/" class="text-on-surface-variant font-medium hover:text-primary transition-colors font-body-md">Cửa hàng</router-link>
          <a class="text-on-surface-variant font-medium hover:text-primary transition-colors font-body-md" href="#story">Câu chuyện</a>
          <a class="text-on-surface-variant font-medium hover:text-primary transition-colors font-body-md" href="#location">Địa điểm</a>
        </nav>
        <div class="flex items-center gap-base">
          <button class="relative p-base text-on-surface-variant hover:bg-surface-container-low transition-all rounded-full" @click="isCartOpen = true">
            <span class="material-symbols-outlined">shopping_basket</span>
            <span v-if="cart.totalItems > 0" class="cart-badge">{{ cart.totalItems }}</span>
          </button>
          <button class="material-symbols-outlined p-base text-on-surface-variant hover:bg-surface-container-low transition-all rounded-full" @click="handleAccountClick">account_circle</button>
        </div>
      </div>
    </header>

    <main v-if="product" class="max-w-[1200px] mx-auto px-margin py-xl">
      <div class="grid grid-cols-1 md:grid-cols-2 gap-xl">
        <!-- Image Section -->
        <div class="product-image-container">
          <div class="aspect-square bg-surface-container-low rounded-2xl overflow-hidden border border-outline-variant/30 shadow-md group">
            <img v-if="product.imageUrl" :src="imgUrl(product.imageUrl)" :alt="product.name" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-700" />
            <div v-else class="w-full h-full flex items-center justify-center text-on-surface-variant/30">
              <span class="material-symbols-outlined text-[120px]">bakery_dining</span>
            </div>
          </div>
        </div>

        <!-- Info Section -->
        <div class="product-info-container flex flex-col gap-md">
          <div>
            <span class="font-label-caps text-primary uppercase tracking-widest mb-xs block">Vừa ra khỏi lò</span>
            <h1 class="font-h1 text-on-surface mb-xs">{{ product.name }}</h1>
            <p class="font-h2 text-primary font-bold">{{ fmtMoney(minPrice(product)) }}</p>
          </div>

          <p class="font-body-lg text-on-surface-variant leading-relaxed">
            {{ product.baseDescription || product.description || 'Chưa có mô tả cho sản phẩm này.' }}
          </p>


          <div class="flex flex-col gap-sm py-md border-y border-outline-variant/30">
            <div class="flex items-center gap-md">
              <div class="flex items-center border border-outline rounded-lg overflow-hidden bg-surface">
                <button @click="quantity > 1 && quantity--" class="w-10 h-10 flex items-center justify-center text-on-surface-variant hover:bg-surface-container transition-colors">
                  <span class="material-symbols-outlined text-[18px]">remove</span>
                </button>
                <span class="w-10 text-center font-bold text-on-surface">{{ quantity }}</span>
                <button @click="quantity++" class="w-10 h-10 flex items-center justify-center text-on-surface-variant hover:bg-surface-container transition-colors">
                  <span class="material-symbols-outlined text-[18px]">add</span>
                </button>
              </div>
              <button @click="addToCart" class="flex-1 bg-primary text-on-primary py-3 px-lg rounded-lg font-button hover:opacity-90 transition-all shadow-md shadow-primary/20 flex items-center justify-center gap-sm">
                <span class="material-symbols-outlined">add_shopping_cart</span>
                Thêm vào giỏ
              </button>
            </div>
          </div>

          <!-- Accordions -->
          <div class="space-y-sm">
            <div v-for="(detail, index) in details" :key="index" class="border-b border-outline-variant/30">
              <button @click="activeDetail = activeDetail === index ? null : index" class="w-full py-md flex items-center justify-between text-on-surface hover:text-primary transition-colors">
                <span class="font-bold text-sm uppercase tracking-wider">{{ detail.label }}</span>
                <span class="material-symbols-outlined transition-transform duration-300" :class="activeDetail === index && 'rotate-180'">expand_more</span>
              </button>
              <transition name="fade">
                <div v-if="activeDetail === index" class="pb-md text-on-surface-variant font-body-md animate-fade-down">
                  {{ detail.content }}
                </div>
              </transition>
            </div>
          </div>
        </div>
      </div>

      <!-- Related Products -->
      <section class="mt-xl pt-xl border-t border-outline-variant/30">
        <h2 class="font-h2 text-on-surface mb-lg">Có thể bạn sẽ thích</h2>
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-gutter">
          <div v-for="rel in related" :key="rel.productId" @click="goToProduct(rel.productId)" class="bg-surface rounded-xl overflow-hidden border border-outline-variant/30 shadow-sm hover:shadow-md transition-all group cursor-pointer">
            <div class="relative h-48 overflow-hidden">
              <img v-if="rel.imageUrl" :src="imgUrl(rel.imageUrl)" :alt="rel.name" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500" />
              <div v-else class="w-full h-full bg-surface-container-low flex items-center justify-center">
                <span class="material-symbols-outlined text-[48px] text-on-surface-variant/30">bakery_dining</span>
              </div>
            </div>
            <div class="p-md">
              <h4 class="font-bold text-on-surface mb-xs truncate">{{ rel.name }}</h4>
              <p class="text-primary font-bold">{{ fmtMoney(minPrice(rel)) }}</p>
            </div>
          </div>
        </div>
      </section>
    </main>

    <div v-else-if="loading" class="min-h-[60vh] flex flex-col items-center justify-center gap-md">
      <div class="w-12 h-12 border-4 border-primary/20 border-t-primary rounded-full animate-spin"></div>
      <p class="text-on-surface-variant font-medium">Đang tải chi tiết sản phẩm...</p>
    </div>

    <div v-else class="min-h-[60vh] flex flex-col items-center justify-center gap-md text-center px-margin">
      <span class="material-symbols-outlined text-[64px] text-on-surface-variant/30">error</span>
      <h2 class="font-h2 text-on-surface">Không tìm thấy sản phẩm</h2>
      <p class="text-on-surface-variant max-w-md">Rất tiếc, chúng tôi không thể tìm thấy thông tin sản phẩm này hoặc có lỗi xảy ra. Vui lòng thử lại sau.</p>
      <router-link to="/" class="mt-md bg-primary text-on-primary py-3 px-lg rounded-lg font-button hover:opacity-90 transition-all">
        Quay lại cửa hàng
      </router-link>
    </div>


    <!-- Footer -->
    <footer class="bg-surface-container-highest border-t border-outline-variant/30 mt-xl">
      <div class="max-w-[1440px] mx-auto px-margin py-lg">
        <div class="flex flex-col md:flex-row justify-between items-center gap-md">
          <p class="font-body-sm text-on-surface-variant/70">© 2024 Tiệm bánh Hearth &amp; Harvest.</p>
          <div class="flex gap-lg">
            <a class="text-on-surface-variant hover:text-primary transition-all opacity-80" href="#"><span class="material-symbols-outlined">public</span></a>
            <a class="text-on-surface-variant hover:text-primary transition-all opacity-80" href="#"><span class="material-symbols-outlined">mail</span></a>
          </div>
        </div>
      </div>
    </footer>

    <Cart v-if="isCartOpen" @close="isCartOpen = false" />
    
    <!-- Toast -->
    <transition name="toast">
      <div v-if="toast" class="toast-notif">
        <span class="material-symbols-outlined">check_circle</span> {{ toast }}
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useCartStore } from '../stores/cartStore';
import Cart from './Cart.vue';

const route = useRoute();
const router = useRouter();
const cart = useCartStore();

const product = ref(null);
const related = ref([]);
const loading = ref(true);
const quantity = ref(1);
const isCartOpen = ref(false);
const toast = ref('');
const activeDetail = ref(0);

const details = ref([
  { label: 'Thành phần', content: 'Bột mì hữu cơ cao cấp, men tự nhiên lên men 48h, nước lọc tinh khiết và muối biển tự nhiên. Không chứa chất bảo quản hay phụ gia thực phẩm.' },
  { label: 'Dị ứng', content: 'Sản phẩm có chứa Gluten. Được sản xuất trên dây chuyền có thể tiếp xúc với các loại hạt và sữa.' },
  { label: 'Bảo quản', content: 'Giữ sản phẩm ở nơi khô ráo, thoáng mát. Sử dụng tốt nhất trong vòng 2 ngày kể từ khi ra lò. Có thể trữ đông và nướng lại để giữ được độ giòn của vỏ bánh.' }
]);

const fetchProduct = async (id) => {
  loading.value = true;
  try {
    const res = await fetch(`http://localhost:5072/api/product/${id}`);
    const json = await res.json();
    if (json.success) {
      product.value = json.data;
      // Fetch related products (just a few from all products for now)
      const allRes = await fetch('http://localhost:5072/api/product');
      const allJson = await allRes.json();
      if (allJson.success) {
        related.value = allJson.data.filter(p => p.productId != id).slice(0, 4);
      }
    }
  } catch (err) {
    console.error(err);
  } finally {
    loading.value = false;
  }
};

onMounted(() => fetchProduct(route.params.id));
watch(() => route.params.id, (newId) => fetchProduct(newId));

const addToCart = () => {
  if (!product.value) return;
  const variant = product.value.variants?.find(v => v.stockQty > 0) || product.value.variants?.[0];
  if (!variant) {
    showToast('Sản phẩm tạm hết hàng!');
    return;
  }
  // Logic to add multiple quantity
  for(let i=0; i<quantity.value; i++) {
    cart.addItem(product.value, variant);
  }
  showToast(`Đã thêm ${quantity.value} "${product.value.name}" vào giỏ hàng!`);
  quantity.value = 1;
};

const showToast = (msg) => {
  toast.value = msg;
  setTimeout(() => toast.value = '', 2500);
};

const goToProduct = (id) => {
  router.push(`/product/${id}`);
  window.scrollTo({ top: 0, behavior: 'smooth' });
};

const imgUrl = url => url?.startsWith('http') ? url : `http://localhost:5072${url}`;
const fmtMoney = v => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v);
const minPrice = p => {
  const prices = p.variants?.map(v => v.price) || [];
  return prices.length ? Math.min(...prices) : 0;
};

const handleAccountClick = () => {
  const user = localStorage.getItem('user');
  if (user) router.push('/dashboard');
  else router.push('/login');
};
</script>

<style scoped>
.material-symbols-outlined {
  font-family: 'Material Symbols Outlined';
  font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
}

.product-details-page {
  background-color: #fcf9f8;
  min-height: 100vh;
}

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

@keyframes fade-down {
  from { opacity: 0; transform: translateY(-10px); }
  to { opacity: 1; transform: translateY(0); }
}

.animate-fade-down {
  animation: fade-down 0.3s ease-out;
}
</style>
