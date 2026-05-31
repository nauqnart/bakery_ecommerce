<template>
  <div class="db-layout">

    <AdminSidebar />

    <!-- Main -->
    <main class="db-main">
      <header class="topbar">
        <div><h1>Bảng Điều Khiển</h1><p class="topbar-sub">Tổng quan hoạt động tiệm bánh hôm nay.</p></div>
        <button class="btn-refresh" @click="fetchData" :disabled="loading">
          <span :class="['material-symbols-outlined', loading && 'spin']">refresh</span>
          Làm mới
        </button>
      </header>

      <div v-if="loading" class="state-msg">Đang tải dữ liệu...</div>

      <div v-else class="db-content">

        <!-- KPI Cards -->
        <section class="kpi-grid">
          <div class="kpi-card kpi-card--revenue">
            <div class="kpi-icon"><span class="material-symbols-outlined">payments</span></div>
            <div class="kpi-body">
              <div class="kpi-label">Tổng doanh thu</div>
              <div class="kpi-value">{{ fmtMoney(data.totalRevenue) }}</div>
              <div class="kpi-sub">Từ đơn hoàn thành</div>
            </div>
          </div>
          <div class="kpi-card kpi-card--orders">
            <div class="kpi-icon"><span class="material-symbols-outlined">receipt_long</span></div>
            <div class="kpi-body">
              <div class="kpi-label">Tổng đơn hàng</div>
              <div class="kpi-value">{{ data.totalOrders }}</div>
              <div class="kpi-sub"><span class="badge-pending">{{ data.pendingOrders }} chờ xử lý</span></div>
            </div>
          </div>
          <div class="kpi-card kpi-card--avg">
            <div class="kpi-icon"><span class="material-symbols-outlined">analytics</span></div>
            <div class="kpi-body">
              <div class="kpi-label">Giá trị đơn TB</div>
              <div class="kpi-value">{{ fmtMoney(data.avgOrderValue) }}</div>
              <div class="kpi-sub">Trên {{ data.totalOrders }} đơn</div>
            </div>
          </div>
          <div class="kpi-card kpi-card--customers">
            <div class="kpi-icon"><span class="material-symbols-outlined">group</span></div>
            <div class="kpi-body">
              <div class="kpi-label">Khách hàng</div>
              <div class="kpi-value">{{ data.activeCustomers }}</div>
              <div class="kpi-sub">Tài khoản đang hoạt động</div>
            </div>
          </div>
          <div class="kpi-card kpi-card--products">
            <div class="kpi-icon"><span class="material-symbols-outlined">inventory_2</span></div>
            <div class="kpi-body">
              <div class="kpi-label">Sản phẩm</div>
              <div class="kpi-value">{{ data.totalProducts }}</div>
              <div class="kpi-sub" :class="data.lowStockCount > 0 && 'kpi-sub--warn'">
                <span class="material-symbols-outlined" v-if="data.lowStockCount > 0">warning</span>
                {{ data.lowStockCount }} sắp hết hàng
              </div>
            </div>
          </div>
        </section>

        <!-- Bottom grid: recent orders + low stock -->
        <section class="bottom-grid">

          <!-- Recent Orders -->
          <div class="panel">
            <div class="panel-header">
              <h3>Đơn hàng gần đây</h3>
              <router-link to="/payments" class="panel-link">Xem tất cả →</router-link>
            </div>
            <div v-if="data.recentOrders.length === 0" class="panel-empty">Chưa có đơn hàng nào.</div>
            <table v-else class="mini-table">
              <thead><tr><th>Mã đơn</th><th>Khách hàng</th><th>Tổng tiền</th><th>Trạng thái</th></tr></thead>
              <tbody>
                <tr v-for="o in data.recentOrders" :key="o.orderId" class="mini-row">
                  <td class="td-id">#ORD-{{ o.orderId }}</td>
                  <td>{{ o.customerName }}</td>
                  <td class="td-price">{{ fmtMoney(o.totalPrice) }}</td>
                  <td><span :class="['badge','badge-order--'+o.orderStatus]">{{ orderLabel(o.orderStatus) }}</span></td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- Low Stock -->
          <div class="panel">
            <div class="panel-header">
              <h3>Sắp hết hàng</h3>
              <router-link to="/inventory" class="panel-link">Quản lý kho →</router-link>
            </div>
            <div v-if="data.lowStockProducts.length === 0" class="panel-empty panel-empty--ok">
              <span class="material-symbols-outlined">check_circle</span> Kho hàng ổn định
            </div>
            <div v-else class="low-stock-list">
              <div v-for="p in data.lowStockProducts" :key="p.productId" class="low-stock-row">
                <div class="ls-img-wrap">
                  <img v-if="p.imageUrl" :src="absImg(p.imageUrl)" :alt="p.name" class="ls-img" />
                  <div v-else class="ls-img-ph"><span class="material-symbols-outlined">bakery_dining</span></div>
                </div>
                <div class="ls-info">
                  <div class="ls-name">{{ p.sku ? `${p.name} - ${p.sku}` : p.name }}</div>
                  <div class="ls-qty" :class="p.stockQty === 0 && 'ls-qty--zero'">
                    {{ p.stockQty === 0 ? 'HẾT HÀNG' : `Còn ${p.stockQty}` }}
                  </div>
                </div>
                <div class="ls-bar-wrap">
                  <div class="ls-bar" :style="{ width: Math.min(p.stockQty * 20, 100) + '%' }"></div>
                </div>
              </div>
            </div>
          </div>

        </section>

        <!-- Charts Grid -->
        <section class="charts-grid">
          <div class="panel">
            <div class="panel-header"><h3>Biểu đồ Doanh thu (7 ngày)</h3></div>
            <div class="panel-body chart-container">
              <Line v-if="!loadingCharts && revenueChartData" :data="revenueChartData" :options="chartOptions" />
            </div>
          </div>
          <div class="panel">
            <div class="panel-header"><h3>Top 5 Bán Chạy</h3></div>
            <div class="panel-body chart-container">
              <Bar v-if="!loadingCharts && topProductsChartData" :data="topProductsChartData" :options="chartOptions" />
            </div>
          </div>
        </section>

      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import AdminSidebar from '../components/AdminSidebar.vue'
import { Line, Bar } from 'vue-chartjs'
import { toast } from 'vue3-toastify'
import 'vue3-toastify/dist/index.css'
import { Chart as ChartJS, Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale, PointElement, LineElement } from 'chart.js'

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale, PointElement, LineElement)

const API  = import.meta.env.VITE_API_BASE_URL + '/dashboard/summary'
const API_REVENUE = import.meta.env.VITE_API_BASE_URL + '/dashboard/revenue'
const API_TOP_PRODUCTS = import.meta.env.VITE_API_BASE_URL + '/dashboard/top-products'

const getAuthHeaders = () => {
  const token = JSON.parse(localStorage.getItem('user'))?.token
  return {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
}

const router = useRouter()

const data = ref({
  totalRevenue: 0, totalOrders: 0, pendingOrders: 0,
  totalProducts: 0, lowStockCount: 0, activeCustomers: 0, avgOrderValue: 0,
  recentOrders: [], lowStockProducts: []
})
const loading = ref(false)
const loadingCharts = ref(false)
const isAdmin = ref(false)

const revenueChartData = ref(null)
const topProductsChartData = ref(null)

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: { legend: { display: false } }
}

async function fetchData() {
  const role = localStorage.getItem('role')
  if (role && role.toLowerCase() === 'admin') isAdmin.value = true

  loading.value = true
  loadingCharts.value = true
  try {
    const res  = await fetch(API, { headers: getAuthHeaders() })
    const json = await res.json()
    if (json.success) data.value = json.data
    else toast.error('Không tải được dữ liệu.')

    const resRev = await fetch(API_REVENUE, { headers: getAuthHeaders() })
    const jsonRev = await resRev.json()
    if (jsonRev.success) {
      revenueChartData.value = {
        labels: jsonRev.data.labels,
        datasets: [{
          label: 'Doanh thu',
          data: jsonRev.data.data,
          borderColor: '#b36a3a',
          backgroundColor: 'rgba(179, 106, 58, 0.1)',
          fill: true,
          tension: 0.4
        }]
      }
    }

    const resTop = await fetch(API_TOP_PRODUCTS, { headers: getAuthHeaders() })
    const jsonTop = await resTop.json()
    if (jsonTop.success) {
      topProductsChartData.value = {
        labels: jsonTop.data.labels,
        datasets: [{
          label: 'Đã bán',
          data: jsonTop.data.data,
          backgroundColor: '#e09060'
        }]
      }
    }

  } catch { toast.error('Lỗi kết nối server.') }
  finally { 
    loading.value = false
    loadingCharts.value = false 
  }
}

const fmtMoney  = v => new Intl.NumberFormat('vi-VN',{style:'currency',currency:'VND'}).format(v ?? 0)
const absImg    = url => url?.startsWith('http') ? url : `${import.meta.env.VITE_BASE_URL}\${url}`
const orderLabel = s => ({pending:'Chờ xử lý',processing:'Đang xử lý',shipped:'Đang giao',delivered:'Hoàn thành',cancelled:'Đã hủy'}[s] ?? s)

onMounted(fetchData)
</script>

<style scoped>
.material-symbols-outlined { font-family:'Material Symbols Outlined'; font-size:20px; line-height:1; }
.spin { animation: spin 1s linear infinite; }
@keyframes spin { to { transform: rotate(360deg); } }

.db-layout { display:flex; min-height:100vh; background:#faf8f5; font-family:'Inter',sans-serif; }

/* Main */
.db-main { flex:1; display:flex; flex-direction:column; }
.topbar { background:#fff; border-bottom:1px solid #e8e0d8; padding:1.25rem 2rem; display:flex; align-items:center; justify-content:space-between; position:sticky; top:0; z-index:30; }
.topbar h1 { font-size:1.4rem; font-weight:700; color:#b36a3a; margin:0 0 .15rem; }
.topbar-sub { font-size:.85rem; color:#8a7060; margin:0; }
.btn-refresh { display:flex; align-items:center; gap:.4rem; padding:.55rem 1.1rem; border:1px solid #e0d8d0; background:#fff; color:#5a4a3a; border-radius:8px; font-size:.85rem; font-weight:600; cursor:pointer; }
.btn-refresh:hover { background:#f5ede6; border-color:#b36a3a; color:#b36a3a; }
.btn-refresh:disabled { opacity:.6; cursor:not-allowed; }

.state-msg { text-align:center; padding:4rem; color:#8a7060; font-size:1rem; }
.state-err { color:#c0392b; }

.db-content { display:flex; flex-direction:column; gap:1.5rem; padding:1.5rem 2rem 2rem; }

/* KPI */
.kpi-grid { display:grid; grid-template-columns:repeat(5, 1fr); gap:1rem; }
@media (max-width: 1200px) { .kpi-grid { grid-template-columns:repeat(3, 1fr); } }
@media (max-width: 768px) { .kpi-grid { grid-template-columns:repeat(2, 1fr); } }
@media (max-width: 480px) { .kpi-grid { grid-template-columns:1fr; } }
.kpi-card { background:#fff; border:1px solid #e8e0d8; border-radius:16px; padding:1.25rem; display:flex; align-items:flex-start; gap:1rem; box-shadow:0 2px 8px rgba(0,0,0,.04); transition:box-shadow .2s; }
.kpi-card:hover { box-shadow:0 6px 20px rgba(0,0,0,.08); }
.kpi-icon { width:44px; height:44px; border-radius:12px; display:flex; align-items:center; justify-content:center; flex-shrink:0; }
.kpi-card--revenue   .kpi-icon { background:#fff3e0; color:#b36a3a; }
.kpi-card--orders    .kpi-icon { background:#e3f2fd; color:#1565c0; }
.kpi-card--avg       .kpi-icon { background:#f3e5f5; color:#7b1fa2; }
.kpi-card--customers .kpi-icon { background:#e8f5e9; color:#2e7d32; }
.kpi-card--products  .kpi-icon { background:#fce4ec; color:#c62828; }
.kpi-label { font-size:.68rem; font-weight:700; text-transform:uppercase; letter-spacing:.05em; color:#8a7060; }
.kpi-value { font-size:1.55rem; font-weight:800; color:#2c2018; margin:.2rem 0; line-height:1.2; }
.kpi-sub   { font-size:.75rem; color:#8a7060; display:flex; align-items:center; gap:.25rem; }
.kpi-sub--warn { color:#c0392b; }
.badge-pending { background:#fff3e0; color:#b36a3a; padding:2px 8px; border-radius:12px; font-size:.72rem; font-weight:700; }

/* Bottom grid */
.bottom-grid { display:grid; grid-template-columns:1fr 1fr; gap:1rem; }
@media(max-width:900px){ .bottom-grid { grid-template-columns:1fr; } }
.panel { background:#fff; border:1px solid #e8e0d8; border-radius:16px; overflow:hidden; }
.panel-header { display:flex; justify-content:space-between; align-items:center; padding:1rem 1.25rem; border-bottom:1px solid #f0e8e0; }
.panel-header h3 { font-size:.95rem; font-weight:700; color:#2c2018; margin:0; }
.panel-link { font-size:.8rem; color:#b36a3a; text-decoration:none; font-weight:600; }
.panel-link:hover { text-decoration:underline; }
.panel-empty { text-align:center; padding:2rem; color:#b0a090; font-size:.875rem; }
.panel-empty--ok { color:#5b7e6b; display:flex; align-items:center; justify-content:center; gap:.4rem; }

/* Mini table */
.mini-table { width:100%; border-collapse:collapse; font-size:.82rem; }
.mini-table th { padding:.65rem 1rem; font-size:.68rem; font-weight:700; text-transform:uppercase; letter-spacing:.05em; color:#8a7060; background:#faf8f5; border-bottom:1px solid #f0e8e0; text-align:left; }
.mini-table td { padding:.7rem 1rem; border-bottom:1px solid #f5ede6; vertical-align:middle; }
.mini-row:last-child td { border-bottom:none; }
.td-id    { font-weight:700; color:#b36a3a; }
.td-price { font-weight:700; }

/* Badges */
.badge { padding:2px 8px; border-radius:12px; font-size:.68rem; font-weight:700; }
.badge-order--pending    { background:#fff3e0; color:#b36a3a; }
.badge-order--processing { background:#e3f2fd; color:#1565c0; }
.badge-order--shipped    { background:#e0f2f1; color:#00695c; }
.badge-order--delivered  { background:#e8f5e9; color:#2e7d32; }
.badge-order--cancelled  { background:#fce4ec; color:#b71c1c; }

/* Low stock list */
.low-stock-list { display:flex; flex-direction:column; }
.low-stock-row { display:flex; align-items:center; gap:.75rem; padding:.75rem 1.25rem; border-bottom:1px solid #f5ede6; }
.low-stock-row:last-child { border-bottom:none; }
.ls-img-wrap { width:40px; height:40px; border-radius:8px; overflow:hidden; flex-shrink:0; background:#f5ede6; }
.ls-img { width:100%; height:100%; object-fit:cover; }
.ls-img-ph { width:100%; height:100%; display:flex; align-items:center; justify-content:center; color:#b36a3a; }
.ls-info { flex:1; min-width:0; }
.ls-name { font-size:.85rem; font-weight:600; color:#2c2018; white-space:nowrap; overflow:hidden; text-overflow:ellipsis; }
.ls-qty  { font-size:.75rem; color:#8a7060; font-weight:600; }
.ls-qty--zero { color:#c0392b; }
.ls-bar-wrap { width:60px; height:6px; background:#f0e8e0; border-radius:3px; overflow:hidden; }
.ls-bar { height:100%; background:#e09060; border-radius:3px; transition:width .3s; }

/* Charts */
.charts-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; margin-top: 1.5rem; }
@media(max-width:900px){ .charts-grid { grid-template-columns:1fr; } }
.chart-container { height: 300px; padding: 1rem; }
</style>

