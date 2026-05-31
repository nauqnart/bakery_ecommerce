<template>
  <div class="revenuedashboard">
    
<!-- SideNavBar -->
<aside class="h-screen w-[280px] sticky left-0 top-0 bg-surface-container-low border-r border-outline-variant flex flex-col p-md gap-base shadow-sm">
<div class="mb-lg px-2">
<h2 class="font-h3 text-h3 font-bold text-primary">Quản trị Tiệm bánh</h2>
<p class="font-body-sm text-body-sm text-on-surface-variant">Vận hành Hàng ngày</p>
</div>
<nav class="flex-grow flex flex-col gap-1">
<router-link class="bg-secondary-container text-on-secondary-container font-semibold rounded-lg px-4 py-3 flex items-center gap-3 transition-colors" to="/dashboard">
<span class="material-symbols-outlined" data-icon="dashboard">dashboard</span>
<span class="font-body-md text-body-md">Bảng điều khiển</span>
</router-link>
<router-link class="text-on-surface-variant hover:bg-surface-container-high hover:text-primary rounded-lg px-4 py-3 flex items-center gap-3 transition-all" to="/payments">
<span class="material-symbols-outlined" data-icon="receipt_long">receipt_long</span>
<span class="font-body-md text-body-md">Lịch sử Đơn hàng</span>
</router-link>
<router-link class="text-on-surface-variant hover:bg-surface-container-high hover:text-primary rounded-lg px-4 py-3 flex items-center gap-3 transition-all" to="/users">
<span class="material-symbols-outlined" data-icon="group">group</span>
<span class="font-body-md text-body-md">Quản lý Người dùng</span>
</router-link>
<router-link class="text-on-surface-variant hover:bg-surface-container-high hover:text-primary rounded-lg px-4 py-3 flex items-center gap-3 transition-all" to="/inventory">
<span class="material-symbols-outlined" data-icon="inventory_2">inventory_2</span>
<span class="font-body-md text-body-md">Quản lý Sản phẩm</span>
</router-link>
</nav>
<div class="mt-auto flex flex-col gap-1">
<div class="px-2 mb-md">
<button class="w-full bg-primary text-on-primary font-button px-4 py-3 rounded-lg flex items-center justify-center gap-2 hover:opacity-90 transition-all active:scale-95 shadow-sm">
<span class="material-symbols-outlined">add</span>
                Lô hàng Mới
            </button>
</div><div class="h-[1px] bg-outline-variant w-full mb-base"></div>
<a class="text-on-surface-variant hover:bg-surface-container-high hover:text-primary rounded-lg px-4 py-3 flex items-center gap-3 transition-all" href="#">
<span class="material-symbols-outlined" data-icon="settings">settings</span>
<span class="font-body-sm text-body-sm">Cài đặt</span>
</a>
<a class="text-on-surface-variant hover:bg-surface-container-high hover:text-primary rounded-lg px-4 py-3 flex items-center gap-3 transition-all" href="#">
<span class="material-symbols-outlined" data-icon="help_outline">help_outline</span>
<span class="font-body-sm text-body-sm">Trợ giúp</span>
</a>
<div class="flex items-center gap-3 px-4 py-4 mt-2">
<div class="w-10 h-10 rounded-full overflow-hidden border border-outline-variant shrink-0">
<img alt="Bakery Logo" class="w-full h-full object-cover" src="https://lh3.googleusercontent.com/aida-public/AB6AXuBl4TldgsSzvYzpfAruJkAF5olIinHZzbcoPZx62ditGAFc8vFtBDUJxH4wnhhqzIDoUzZ1Y2tjdFfLSAP9iuVA-ToO4QmIC4hWQ1qFeXmmitswfsaakSfH_NijXWhSUccKGHXnc4FyM8G_wDSl6WNPTV8hrXzMbxPSkOTotDzgdZWCg07tyoZKGV0sqWRRxq_xB0_3Eqy4fDfxs4dFbziC8a35UlR8bfwRaXetX4VHe-kqB-t-4GvMF-kMiFiGzJumFywUax5jQw"/>
</div>
<div class="flex flex-col min-w-0">
<span class="font-label-caps text-[11px] text-on-surface font-bold uppercase truncate">Thợ bánh chính</span>
<span class="font-body-sm text-[12px] text-on-surface-variant truncate">Phiên Quản trị</span>
</div>
</div>
</div>
</aside>
<!-- Main Content Canvas -->
<main class="flex-grow flex flex-col min-h-screen">
<!-- TopAppBar -->
<header class="bg-surface/80 backdrop-blur-md border-b border-outline-variant sticky top-0 z-20">
<div class="flex justify-between items-center w-full px-margin py-md">
<div class="flex items-center gap-md">
<h1 class="font-h2 text-h2 font-bold text-primary">Hearth &amp; Harvest</h1>
</div>
<div class="flex items-center gap-gutter">
<div class="relative hidden lg:block">
<span class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant">search</span>
<input class="pl-10 pr-4 py-2 bg-surface-container border border-outline-variant rounded-full text-body-sm w-64 focus:outline-none focus:ring-1 focus:ring-primary focus:border-primary transition-all" placeholder="Tìm kiếm đơn hàng..." type="text"/>
</div>
<div class="flex items-center gap-2">
<button class="material-symbols-outlined text-primary p-2 hover:bg-surface-container-high rounded-full transition-all">shopping_basket</button>
<button class="material-symbols-outlined text-primary p-2 hover:bg-surface-container-high rounded-full transition-all">notifications</button>
<button class="material-symbols-outlined text-primary p-2 hover:bg-surface-container-high rounded-full transition-all">account_circle</button>
</div>
</div>
</div>
</header>
<!-- Dashboard Content -->
<div class="p-margin max-w-[1440px] mx-auto w-full space-y-lg">
<!-- Filter Tabs -->
<div class="flex items-center gap-xs bg-surface-container-low p-1 rounded-xl w-fit border border-outline-variant">
<button class="px-5 py-1.5 rounded-lg bg-primary text-on-primary font-button text-sm shadow-sm transition-all">Ngày</button>
<button class="px-5 py-1.5 rounded-lg text-on-surface-variant font-button text-sm hover:bg-surface-container-high transition-colors">Tháng</button>
<button class="px-5 py-1.5 rounded-lg text-on-surface-variant font-button text-sm hover:bg-surface-container-high transition-colors">Năm</button>
</div>
<!-- Bento Grid Metrics -->
<section class="grid grid-cols-1 md:grid-cols-3 gap-gutter" v-if="summary">
<!-- Sales Card -->
<div class="bg-surface-container-lowest border border-outline-variant p-md rounded-xl shadow-sm hover:shadow-md transition-shadow group">
<div class="flex justify-between items-start">
<span class="text-label-caps font-label-caps text-on-surface-variant">DOANH THU</span>
<span class="material-symbols-outlined text-primary group-hover:scale-110 transition-transform">payments</span>
</div>
<div class="flex flex-col mt-2">
<h2 class="font-h1 text-h1 text-on-surface">{{ formatPrice(summary.totalRevenue) }}</h2>
</div>
</div>
<!-- Orders Card -->
<div class="bg-surface-container-lowest border border-outline-variant p-md rounded-xl shadow-sm hover:shadow-md transition-shadow group">
<div class="flex justify-between items-start">
<span class="text-label-caps font-label-caps text-on-surface-variant">ĐƠN HÀNG MỚI</span>
<span class="material-symbols-outlined text-primary group-hover:scale-110 transition-transform">bakery_dining</span>
</div>
<div class="flex flex-col mt-2">
<h2 class="font-h1 text-h1 text-on-surface">{{ summary.totalOrders }}</h2>
<div class="flex items-center gap-1 text-on-surface-variant mt-1">
<span class="material-symbols-outlined text-sm">schedule</span>
<span class="text-body-sm">{{ summary.pendingOrders }} đơn đang chờ xử lý</span>
</div>
</div>
</div>
<!-- AOV Card -->
<div class="bg-surface-container-lowest border border-outline-variant p-md rounded-xl shadow-sm hover:shadow-md transition-shadow group">
<div class="flex justify-between items-start">
<span class="text-label-caps font-label-caps text-on-surface-variant">GIÁ TRỊ ĐƠN TRUNG BÌNH</span>
<span class="material-symbols-outlined text-primary group-hover:scale-110 transition-transform">analytics</span>
</div>
<div class="flex flex-col mt-2">
<h2 class="font-h1 text-h1 text-on-surface">{{ formatPrice(summary.avgOrderValue) }}</h2>
</div>
</div>
</section>
<!-- Main Dashboard Layout -->
<div class="grid grid-cols-1 lg:grid-cols-12 gap-gutter items-start">
<!-- Charts Section -->
<section class="lg:col-span-12 grid grid-cols-1 md:grid-cols-2 gap-gutter">
  <div class="bg-surface-container-lowest border border-outline-variant p-md rounded-xl shadow-sm h-[350px] flex flex-col">
    <h3 class="font-h3 text-h3 text-primary mb-md">Doanh thu 7 ngày qua</h3>
    <div class="flex-grow relative w-full h-full" v-if="loaded">
      <Bar :data="revenueChartData" :options="chartOptions" />
    </div>
  </div>
  <div class="bg-surface-container-lowest border border-outline-variant p-md rounded-xl shadow-sm h-[350px] flex flex-col">
    <h3 class="font-h3 text-h3 text-primary mb-md">Tỷ trọng Danh mục</h3>
    <div class="flex-grow relative w-full h-full flex justify-center" v-if="loaded">
      <Pie :data="categoryChartData" :options="pieOptions" />
    </div>
  </div>
</section>
<!-- Top Products -->
<section class="lg:col-span-8 bg-surface-container-lowest border border-outline-variant rounded-xl shadow-sm overflow-hidden" v-if="summary">
<div class="p-md border-b border-outline-variant flex justify-between items-center">
<h3 class="font-h3 text-h3 text-on-surface">Sản phẩm sắp hết hàng</h3>
<router-link to="/inventory" class="text-primary font-button hover:underline decoration-primary">Xem Kho hàng</router-link>
</div>
<div class="divide-y divide-outline-variant">
<div v-for="p in summary.lowStockProducts" :key="p.productId" class="p-md flex items-center justify-between hover:bg-surface-container-low transition-colors group">
<div class="flex items-center gap-md">
<img :alt="p.name" class="w-16 h-16 rounded-lg object-cover border border-outline-variant shadow-sm group-hover:scale-105 transition-transform" :src="imgUrl(p.imageUrl)"/>
<div>
<h4 class="font-body-lg text-body-lg font-bold text-on-surface">{{ p.name }}</h4>
</div>
</div>
<div class="text-right">
<div class="font-h3 text-h3 text-on-surface text-red-600">{{ p.stockQty }}</div>
<div class="text-label-caps text-on-surface-variant">CÒN LẠI</div>
</div>
</div>
<div v-if="summary.lowStockProducts.length === 0" class="p-md text-center text-on-surface-variant">Không có sản phẩm nào sắp hết hàng.</div>
</div>
</section>
<!-- Recent Activity Feed -->
<section class="lg:col-span-4 bg-surface-container-lowest border border-outline-variant rounded-xl shadow-sm flex flex-col" v-if="summary">
<div class="p-md border-b border-outline-variant">
<h3 class="font-h3 text-h3 text-on-surface">Đơn hàng gần đây</h3>
</div>
<div class="p-md space-y-lg flex-grow">
<div v-for="o in summary.recentOrders" :key="o.orderId" class="flex gap-sm relative">
<div class="flex flex-col items-center shrink-0">
<div class="w-2.5 h-2.5 rounded-full z-10 border-2 border-surface" :class="o.orderStatus === 'delivered' ? 'bg-green-500' : 'bg-primary'"></div>
<div class="w-[1px] h-full bg-outline-variant absolute top-1"></div>
</div>
<div class="pb-base">
<p class="font-body-sm text-on-surface leading-tight"><span class="font-bold">Đơn #ORD-{{ o.orderId }}</span> từ {{ o.customerName }}</p>
<span class="text-label-caps text-[10px] text-on-surface-variant mt-1 block">{{ formatDate(o.createdAt) }} - {{ formatPrice(o.totalPrice) }}</span>
</div>
</div>
<div v-if="summary.recentOrders.length === 0" class="text-center text-on-surface-variant">Chưa có đơn hàng nào.</div>
</div>
<div class="p-md pt-0">
<router-link to="/payments" class="w-full py-2.5 border border-outline-variant rounded-lg font-button text-on-surface-variant hover:bg-surface-container-high transition-all text-sm flex justify-center">Xem tất cả đơn hàng</router-link>
</div>
</section>
</div>
</div>
<!-- Footer -->
<footer class="mt-auto bg-surface-container-highest border-t border-outline-variant">
<div class="flex flex-col md:flex-row justify-between items-center w-full px-margin py-lg max-w-[1440px] mx-auto gap-md">
<div class="flex flex-col gap-1 text-center md:text-left">
<h2 class="font-h3 text-h3 font-bold text-primary">Hearth &amp; Harvest</h2>
<p class="font-body-sm text-body-sm text-on-surface-variant">© 2024 Tiệm bánh Hearth &amp; Harvest. Chất lượng thủ công, tươi ngon mỗi ngày.</p>
</div>
<div class="flex flex-wrap justify-center gap-x-lg gap-y-2">
<a class="font-body-sm text-body-sm text-on-surface-variant hover:text-primary transition-colors hover:underline decoration-primary" href="#">Câu chuyện</a>
<a class="font-body-sm text-body-sm text-on-surface-variant hover:text-primary transition-colors hover:underline decoration-primary" href="#">Địa điểm</a>
<a class="font-body-sm text-body-sm text-on-surface-variant hover:text-primary transition-colors hover:underline decoration-primary" href="#">Bán sỉ</a>
<a class="font-body-sm text-body-sm text-on-surface-variant hover:text-primary transition-colors hover:underline decoration-primary" href="#">Liên hệ</a>
</div>
</div>
</footer>
</main>

  </div>
</template>

<script setup>
import AdminSidebar from '../components/AdminSidebar.vue'
import { ref, onMounted } from 'vue';
import { Bar, Pie } from 'vue-chartjs';
import { Chart as ChartJS, Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale, ArcElement } from 'chart.js';

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale, ArcElement);

const revenueChartData = ref({ labels: [], datasets: [] });
const categoryChartData = ref({ labels: [], datasets: [] });
const summary = ref(null);
const loaded = ref(false);

const chartOptions = { responsive: true, maintainAspectRatio: false };
const pieOptions = { responsive: true, maintainAspectRatio: false };

const formatPrice = v => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v);
const formatDate = d => new Date(d).toLocaleString('vi-VN');
const imgUrl = url => {
  if(!url) return 'https://placehold.co/100x100?text=No+Image';
  return url.startsWith('http') ? url : `${import.meta.env.VITE_BASE_URL}\${url}`;
};

const fetchSummary = async () => {
  try {
    const res = await fetch(import.meta.env.VITE_API_BASE_URL + '/dashboard/summary');
    const json = await res.json();
    if(json.success) summary.value = json.data;
  } catch(e) { console.error(e); }
};

const fetchChartData = async () => {
  try {
    const res = await fetch(import.meta.env.VITE_API_BASE_URL + '/dashboard/chart-data');
    const json = await res.json();
    if(json.success) {
      revenueChartData.value = {
        labels: json.data.revenue.labels,
        datasets: [{
          label: 'Doanh thu ($)',
          backgroundColor: '#98421f',
          data: json.data.revenue.data
        }]
      };
      categoryChartData.value = {
        labels: json.data.category.labels,
        datasets: [{
          backgroundColor: ['#98421f', '#d28a5c', '#f0cba1', '#3e2723', '#795548', '#a87c69'],
          data: json.data.category.data
        }]
      };
      loaded.value = true;
    }
  } catch(e) { console.error(e); }
};

onMounted(() => {
  fetchSummary();
  fetchChartData();
});
</script>

<style scoped>

        .material-symbols-outlined {
            font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
        }
        body {
            font-family: 'Inter', sans-serif;
            background-color: #fcf9f8;
        }
        .font-h1 { font-family: 'Noto Serif', serif; }
        .font-h2 { font-family: 'Noto Serif', serif; }
        .font-h3 { font-family: 'Noto Serif', serif; }
    
</style>


