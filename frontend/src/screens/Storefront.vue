<template>
  <div class="storefront">

<!-- Toast notification -->
<transition name="toast">
  <div v-if="toast" class="toast-notif">
    <span class="material-symbols-outlined">check_circle</span> {{ toast }}
  </div>
</transition>

<!-- TopAppBar -->
<header class="bg-surface/90 backdrop-blur-md border-b border-outline-variant/30 sticky top-0 z-50">
<div class="flex justify-between items-center w-full px-margin py-base max-w-[1440px] mx-auto">
<div class="flex items-center gap-base">
<router-link to="/" class="font-h2 text-h3 md:text-h2 font-bold text-primary">Hearth &amp; Harvest</router-link>
</div>
<nav class="hidden md:flex items-center gap-lg">
<router-link to="/" class="text-primary font-bold border-b-2 border-primary pb-1 font-body-md">Cửa hàng</router-link>
<a class="text-on-surface-variant font-medium hover:text-primary transition-colors font-body-md" href="#story">Câu chuyện của chúng tôi</a>
<a class="text-on-surface-variant font-medium hover:text-primary transition-colors font-body-md" href="#location">Địa điểm</a>
</nav>
<div class="flex items-center gap-base">
<button class="relative p-base text-on-surface-variant hover:bg-surface-container-low transition-all rounded-full" @click="isCartOpen = true">
  <span class="material-symbols-outlined">shopping_basket</span>
  <span v-if="cart.totalItems > 0" class="cart-badge">{{ cart.totalItems }}</span>
</button>
<button v-if="isAdmin" class="material-symbols-outlined p-base text-primary hover:bg-surface-container-low transition-all rounded-full" @click="$router.push('/dashboard')">dashboard</button>
<button class="material-symbols-outlined p-base text-on-surface-variant hover:bg-surface-container-low transition-all rounded-full" @click="handleAccountClick">account_circle</button>
</div>
</div>
</header>
<main>
<!-- Hero Section -->
<section class="relative w-full h-[600px] flex items-center overflow-hidden">
<div class="absolute inset-0 z-0">
<img class="w-full h-full object-cover brightness-75" data-alt="A warm and inviting close-up shot of a rustic sourdough bread loaf with a perfectly crackled, golden-brown crust." src="https://lh3.googleusercontent.com/aida-public/AB6AXuDgEk1by2syfmSzk1FDqoggA2Sz8ePtOdJ5fOAoxKwwbdjqnrcEz9ELgh46cXDrtXUn394pYVOyL9nRiDNq1QSRTrZROQBlGJ4grJpgUcNTyn7Wa36srI7sxhhTrr_APtaLitzGuf2ct3PKc3U2vLSWpoeLgh5N74newi3_QSaKEGYL9bnPw3PkPO6I-pE9UhcuNHPKBnFKD0GDkJ_6WyEQStaJ7T7_En2kfRIh3poL2JpQ7GeUSVwk8M2g1X9u3Iv3dPQx2255_w"/>
</div>
<div class="relative z-10 max-w-[1440px] mx-auto px-margin w-full">
<div class="max-w-2xl bg-surface/80 backdrop-blur-sm p-lg rounded-xl shadow-lg border border-outline-variant/30">
<span class="font-label-caps text-label-caps text-primary uppercase tracking-widest mb-base block">Est. 2024 — Hearth &amp; Harvest</span>
<h1 class="font-h1 text-h1 text-on-surface mb-md">Chất lượng thủ công, nướng mới cho bàn ăn của bạn mỗi sáng.</h1>
<p class="font-body-lg text-body-lg text-on-surface-variant mb-lg">Trải nghiệm linh hồn của nghệ thuật làm bánh truyền thống với bánh mì sourdough lên men tự nhiên và bánh ngọt thủ công.</p>
<div class="flex gap-md">
<button class="bg-primary text-on-primary px-lg py-md rounded-lg font-button hover:opacity-90 transition-all shadow-md shadow-primary/20">Mua ngay</button>
<button class="border border-outline text-on-surface-variant px-lg py-md rounded-lg font-button hover:bg-surface-container-low transition-all">Quy trình</button>
</div>
</div>
</div>
</section>
<!-- Featured Categories (Bento Style) -->
<section class="max-w-[1440px] mx-auto px-margin py-xl">
<div class="mb-lg border-b border-outline-variant/30 pb-sm">
<h2 class="font-h2 text-h2 text-on-surface">Khám phá gian bếp</h2>
</div>
<div class="bento-grid h-[600px]">
<!-- Daily Breads -->
<div class="col-span-12 md:col-span-8 group relative overflow-hidden rounded-xl border border-outline-variant/20 shadow-sm transition-shadow hover:shadow-md">
<img class="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105" data-alt="A collection of diverse artisanal breads." src="https://lh3.googleusercontent.com/aida-public/AB6AXuBNYk2Hw0s-D7OTIHpRrRdcVBxkTn391y7g_-oZMk60KYuK2rsjcQM8RBolt8yv0CI_zxT9BUClHkj1FlMtXrtJIJRzaqxEvm3G24VaHIGuF0WQpSS4KPX68QdYEpM5XZoEcJiFxmR7exorEV2xZzdizyOZuV0r0_55usClGUA7v4ad4eY-uSdgYS-KQ9JdhQwUGQlrhDtUBpRmWSGOFR-2ltUXya7Y8kreM71J6GdpM22cBT_MMz5kHIvOBzrGlNqrt6Sr6b2EGw"/>
<div class="absolute inset-0 bg-gradient-to-t from-on-background/80 via-on-background/20 to-transparent flex flex-col justify-end p-lg">
<h3 class="font-h3 text-h3 text-white">Bánh mì hàng ngày</h3>
<p class="text-white/80 font-body-md mb-md">Ngũ cốc xay đá và lên men dài ngày.</p>
<a class="text-white font-button flex items-center gap-xs hover:gap-sm transition-all" href="#">Xem bộ sưu tập <span class="material-symbols-outlined text-[18px]">arrow_forward</span></a>
</div>
</div>
<!-- Hand-crafted Pastries -->
<div class="col-span-12 md:col-span-4 group relative overflow-hidden rounded-xl border border-outline-variant/20 shadow-sm transition-shadow hover:shadow-md">
<img class="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105" data-alt="Buttery, flaky croissants." src="https://lh3.googleusercontent.com/aida-public/AB6AXuAmp0zco70Ra6QFM7jwww2rUT2YCwS5EnQGfvst8HQYSNGrdnWYdwyDtiDuv5RJcZCUuGZ6LqHht2Ly-LEpdhDuOYOnAZmHwH1dUc368n8jTMJvcTRROg9nKbICTfayoF0UKJu1pxSpbampEotsLcK_-PGAGcPxKkkCyeU9CQ6YiAizgKY_jQDuiw1UFGZsQtPncBMoeveYhc34mx3yYjHxan8A0IndyxH9eMWOF05r8DssZrilFx5GZAwWwhzJziTpOuFvVuFPWQ"/>
<div class="absolute inset-0 bg-gradient-to-t from-on-background/80 via-on-background/20 to-transparent flex flex-col justify-end p-lg">
<h3 class="font-h3 text-h3 text-white">Bánh ngọt thủ công</h3>
<a class="text-white font-button flex items-center gap-xs hover:gap-sm transition-all mt-base" href="#">Khám phá <span class="material-symbols-outlined text-[18px]">arrow_forward</span></a>
</div>
</div>
<!-- Custom Cakes -->
<div class="col-span-12 group relative overflow-hidden rounded-xl border border-outline-variant/20 shadow-sm transition-shadow hover:shadow-md h-[250px]">
<img class="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105" data-alt="Minimalist, elegantly tiered cake." src="https://lh3.googleusercontent.com/aida-public/AB6AXuCaCPYdwS7DuC7WmRivzXeHAmPhAr_wyJEJru-JJ2U__o2VwfQdRcsr2J-plF-GlwpFVTkh3aQWMHUfMwB9n8SOg_nGg5LcvwDiLRh7RvcEFu6cX9kt7V0llfTMrOJb6U5FYZ-nAsCJtO1rA4I0jbfEfses7mbQ4azC8Zc2F9LRhgQcUYoFIGRwzUQXocVEv9ReXVxPgEaZTl6qV7Oi2ujHDrHS_pKZ08sPK7v6QE1hzBqAjJrbIPurONahx9UOHy4m6Zr6foYLCQ"/>
<div class="absolute inset-0 bg-gradient-to-t from-on-background/80 via-on-background/10 to-transparent flex flex-col justify-end p-lg">
<h3 class="font-h3 text-h3 text-white">Bánh kem theo yêu cầu</h3>
<p class="text-white/80 font-body-md mb-md">Dành cho các dịp lễ, làm theo đơn đặt hàng.</p>
<a class="text-white font-button flex items-center gap-xs hover:gap-sm transition-all" href="#">Yêu cầu ngay <span class="material-symbols-outlined text-[18px]">arrow_forward</span></a>
</div>
</div>
</div>
</section>
<!-- New Arrivals (Real Products) -->
<section class="bg-surface-container-low py-xl">
<div class="max-w-[1440px] mx-auto px-margin">
<div class="flex justify-between items-end mb-lg">
<div>
<span class="font-label-caps text-label-caps text-primary mb-xs block">Vừa ra khỏi lò</span>
<h2 class="font-h2 text-h2 text-on-surface">Sản phẩm mới</h2>
</div>
</div>

<div v-if="loadingProd" class="text-center py-8 text-on-surface-variant">Đang tải sản phẩm...</div>

<div v-else-if="products.length === 0" class="text-center py-8 text-on-surface-variant">Chưa có sản phẩm nào.</div>

<div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-gutter">
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
  </div>
  <div class="p-md">
    <h4 class="font-h3 text-body-lg font-bold text-on-surface mb-xs group-hover:text-primary transition-colors">{{ product.name }}</h4>
    <p class="text-on-surface-variant font-body-sm mb-md h-12 overflow-hidden">{{ product.baseDescription || product.description }}</p>
    <div class="flex items-center justify-between">
      <span class="text-primary font-h3 text-h3">{{ fmtMoney(minPrice(product)) }}</span>
      <button @click.stop.prevent="addToCart(product)"
        class="bg-on-surface-variant/10 text-on-surface-variant w-10 h-10 rounded-lg flex items-center justify-center hover:bg-primary hover:text-white transition-all active:scale-95">
        <span class="material-symbols-outlined">add_shopping_cart</span>
      </button>
    </div>
  </div>
</router-link>



</div>
</div>
</section>
<!-- Newsletter / Community -->

<!-- Newsletter / Community -->
<section class="max-w-[1440px] mx-auto px-margin py-xl">
<div class="bg-primary-fixed rounded-2xl p-xl flex flex-col md:flex-row items-center gap-xl relative overflow-hidden">
<div class="absolute top-0 right-0 w-64 h-64 bg-primary/10 rounded-full -translate-y-1/2 translate-x-1/2"></div>
<div class="flex-1 z-10">
<h2 class="font-h2 text-h1 text-on-primary-fixed mb-md">Tham gia "Vòng tròn Bột"</h2>
<p class="font-body-lg text-on-primary-fixed-variant max-w-lg">Nhận thông tin sớm về các mẻ bánh mới hàng tuần, ưu đãi theo mùa và mẹo nướng bánh thủ công gửi thẳng đến hộp thư của bạn.</p>
</div>
<div class="flex-1 w-full max-w-md z-10">
<form class="flex flex-col sm:flex-row gap-base">
<input class="flex-grow bg-surface border-none rounded-lg px-md py-md font-body-md focus:ring-2 focus:ring-primary shadow-sm" placeholder="Nhập email của bạn" type="email"/>
<button class="bg-primary text-on-primary px-lg py-md rounded-lg font-button hover:opacity-90 transition-all">Đăng ký</button>
</form>
<p class="font-body-sm text-on-primary-fixed-variant mt-md opacity-70 italic">Nướng tươi mới, gửi mỗi tuần một lần. Không rác, chỉ có sourdough.</p>
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
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import Cart from './Cart.vue';
import { useCartStore } from '../stores/cartStore';

const isCartOpen = ref(false);
const router  = useRouter();
const cart    = useCartStore();
const isAdmin = ref(false);

// Products from API
const products    = ref([]);
const loadingProd = ref(false);
const toast       = ref('');

onMounted(async () => {
  const role = localStorage.getItem('role');
  if (role === 'admin' || role === 'Admin') isAdmin.value = true;

  loadingProd.value = true;
  try {
    const res  = await fetch('http://localhost:5072/api/product');
    const json = await res.json();
    if (json.success && json.data.length > 0) {
      products.value = json.data;
    } else {
      products.value = mockProducts;
    }
  } catch {
    products.value = mockProducts;
  }
  finally { loadingProd.value = false; }
});

const mockProducts = [
  {
    productId: 1,
    name: 'Bánh mì Sourdough truyền thống',
    baseDescription: 'Lên men tự nhiên 48h, vỏ giòn tan, ruột mềm mọng.',
    imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuDgEk1by2syfmSzk1FDqoggA2Sz8ePtOdJ5fOAoxKwwbdjqnrcEz9ELgh46cXDrtXUn394pYVOyL9nRiDNq1QSRTrZROQBlGJ4grJpgUcNTyn7Wa36srI7sxhhTrr_APtaLitzGuf2ct3PKc3U2vLSWpoeLgh5N74newi3_QSaKEGYL9bnPw3PkPO6I-pE9UhcuNHPKBnFKD0GDkJ_6WyEQStaJ7T7_En2kfRIh3poL2JpQ7GeUSVwk8M2g1X9u3Iv3dPQx2255_w',
    isNew: true,
    variants: [{ price: 85000, stockQty: 10 }]
  },
  {
    productId: 2,
    name: 'Bánh Croissant Bơ Pháp',
    baseDescription: 'Nhiều lớp bơ béo ngậy, nướng vàng đều.',
    imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuAmp0zco70Ra6QFM7jwww2rUT2YCwS5EnQGfvst8HQYSNGrdnWYdwyDtiDuv5RJcZCUuGZ6LqHht2Ly-LEpdhDuOYOnAZmHwH1dUc368n8jTMJvcTRROg9nKbICTfayoF0UKJu1pxSpbampEotsLcK_-PGAGcPxKkkCyeU9CQ6YiAizgKY_jQDuiw1UFGZsQtPncBMoeveYhc34mx3yYjHxan8A0IndyxH9eMWOF05r8DssZrilFx5GZAwWwhzJziTpOuFvVuFPWQ',
    variants: [{ price: 45000, stockQty: 15 }]
  },
  {
    productId: 3,
    name: 'Bánh Scone Việt quất',
    baseDescription: 'Việt quất tươi mọng trong lớp bánh bơ xốp.',
    imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuDiACamZtXJe8Qdx8r-hiiOWokVnej1l0ALcWwWuajHWzXvXGXRQXFHRpb6JWesNIfcXUtgI_QrJJiTEA5v20o8wxEFpzBYKFEA_4jAN0rUE-1NQyyYrZHD9-HfATZ_4xOZ2yNzTmQaYXzFwGzaO3iYsmDrqn8qviBVK0R1PHGzc-OKmOTrkdQjePj7XrxRcZM7qIPrWHKPlOWOxtUWNCcJF-QzsBWd7UvrqDEHsNxaHmjPlZZqIx-rgM3r2Pjq1hlzRjijVQ3-vA',
    variants: [{ price: 55000, stockQty: 8 }]
  },
  {
    productId: 4,
    name: 'Bánh Focaccia Cà chua',
    baseDescription: 'Mềm xốp với hương vị thảo mộc và cà chua bi.',
    imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuBn-ltScn6WKKgYWldzMDhtmkwqoOHUpbhygz_q3eWkdauGuSOYqYZlI9xscHlZ37WiD6tvPEjaAIPvYHymJ9o6Zmmpy1lBEwzQjFXPzcDGtxft-CThpJWCw-rlX-ltuMkULKtT7w-avkfMv2h4QJH9Lspyti8iDYfb38P-2cTele8JscQJSkgXKHgHvsGxODGwHA2WpfoAswHotbzEzwhgrIps8sE7QoiBnQVdUQ7ArgbK3BXUEzryPh9s_5lTHuenWdnNTIjW_A',
    variants: [{ price: 70000, stockQty: 5 }]
  }
];


function addToCart(product) {
  // Lấy variant đầu tiên có hàng
  const variant = product.variants?.find(v => v.stockQty > 0) || product.variants?.[0];
  if (!variant) { showToast('Sản phẩm tạm hết hàng!'); return; }
  cart.addItem(product, variant);
  showToast(`Đã thêm "${product.name}" vào giỏ hàng!`);
}

function showToast(msg) {
  toast.value = msg;
  setTimeout(() => toast.value = '', 2500);
}

const imgUrl = url => url?.startsWith('http') ? url : `http://localhost:5072${url}`;
const fmtMoney = v => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v);
const minPrice = product => {
  const prices = product.variants?.map(v => v.price) || [];
  return prices.length ? Math.min(...prices) : 0;
};

const handleAccountClick = () => {
  const user = localStorage.getItem('user');
  if (user) { isAdmin.value ? router.push('/dashboard') : alert('Bạn đã đăng nhập!'); }
  else router.push('/login');
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
        /* Toast */
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

