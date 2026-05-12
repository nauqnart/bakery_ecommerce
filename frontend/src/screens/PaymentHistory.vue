<template>
  <div class="ph-layout">

    <!-- Sidebar -->
    <aside class="sidebar">
      <div class="sidebar-brand">
        <h2>Quản trị Bakery</h2>
        <p>Vận hành Hàng ngày</p>
      </div>
      <nav class="sidebar-nav">
        <router-link to="/dashboard" class="nav-item"><span class="material-symbols-outlined">dashboard</span><span>Bảng điều khiển</span></router-link>
        <router-link to="/payments"  class="nav-item nav-item--active"><span class="material-symbols-outlined">receipt_long</span><span>Quản lý Đơn hàng</span></router-link>
        <router-link to="/users"     class="nav-item"><span class="material-symbols-outlined">group</span><span>Quản lý Người dùng</span></router-link>
        <router-link to="/inventory" class="nav-item"><span class="material-symbols-outlined">inventory_2</span><span>Quản lý Sản phẩm</span></router-link>
      </nav>
    </aside>

    <!-- Main -->
    <main class="ph-main">
      <header class="topbar">
        <div>
          <h1>Quản lý Đơn hàng &amp; Hóa đơn</h1>
          <p class="topbar-sub">Xem, cập nhật trạng thái và theo dõi thanh toán.</p>
        </div>
        <div class="topbar-right">
          <div class="search-wrap">
            <span class="material-symbols-outlined">search</span>
            <input v-model="search" placeholder="Tìm mã đơn, tên khách..." />
          </div>
        </div>
      </header>

      <!-- Stats -->
      <div class="stats-row">
        <div class="stat-card" v-for="s in stats" :key="s.label">
          <div :class="['stat-icon', s.cls]"><span class="material-symbols-outlined">{{ s.icon }}</span></div>
          <div><div class="stat-label">{{ s.label }}</div><div class="stat-value">{{ s.value }}</div></div>
        </div>
      </div>

      <!-- Filter tabs -->
      <div class="filter-tabs">
        <button v-for="t in tabs" :key="t.v" :class="['tab', activeTab===t.v&&'tab--on']" @click="activeTab=t.v">{{ t.label }}</button>
      </div>

      <!-- Loading / Error -->
      <div v-if="loading" class="state-msg">Đang tải...</div>
      <div v-else-if="err" class="state-msg state-err">{{ err }}</div>

      <!-- Table -->
      <div v-else class="table-wrap">
        <table class="tbl">
          <thead>
            <tr>
              <th>Mã đơn</th><th>Khách hàng</th><th>Ngày tạo</th>
              <th>Tổng tiền</th><th>Trạng thái đơn</th><th>Thanh toán</th><th></th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="o in filtered" :key="o.orderId" class="tbl-row" @click="openDetail(o)">
              <td class="td-id">#ORD-{{ o.orderId }}</td>
              <td>{{ o.user?.fullName || 'Khách' }}</td>
              <td class="td-muted">{{ fmtDate(o.createdAt) }}</td>
              <td class="td-price">{{ fmtMoney(o.totalPrice) }}</td>
              <td><span :class="['badge', 'badge-order--'+o.orderStatus]">{{ orderLabel(o.orderStatus) }}</span></td>
              <td><span :class="['badge', 'badge-pay--'+(o.invoice?.paymentStatus||'unpaid')]">{{ payLabel(o.invoice?.paymentStatus) }}</span></td>
              <td class="td-action"><span class="material-symbols-outlined">chevron_right</span></td>
            </tr>
            <tr v-if="filtered.length===0"><td colspan="7" class="empty-cell"><span class="material-symbols-outlined">inbox</span><p>Không có đơn hàng</p></td></tr>
          </tbody>
        </table>
      </div>
    </main>

    <!-- ═══ DETAIL DRAWER ═══ -->
    <transition name="drawer">
      <div v-if="selected" class="drawer-backdrop" @click.self="selected=null">
        <aside class="drawer">
          <div class="drawer-header">
            <div>
              <h2>#ORD-{{ selected.orderId }}</h2>
              <p class="drawer-sub">{{ fmtDate(selected.createdAt) }}</p>
            </div>
            <button class="close-btn" @click="selected=null"><span class="material-symbols-outlined">close</span></button>
          </div>

          <!-- Customer info -->
          <section class="drawer-section">
            <div class="section-title">Thông tin khách hàng</div>
            <div class="info-grid">
              <div class="info-item"><span class="info-label">Tên</span><span>{{ selected.user?.fullName || '—' }}</span></div>
              <div class="info-item"><span class="info-label">Email</span><span>{{ selected.user?.email || '—' }}</span></div>
              <div class="info-item info-item--full"><span class="info-label">Địa chỉ giao hàng</span><span>{{ selected.shippingAddress }}</span></div>
              <div class="info-item info-item--full" v-if="selected.note"><span class="info-label">Ghi chú</span><span>{{ selected.note }}</span></div>
            </div>
          </section>

          <!-- Items -->
          <section class="drawer-section">
            <div class="section-title">Sản phẩm ({{ selected.items?.length || 0 }})</div>
            <div class="items-list">
              <div class="item-row" v-for="it in selected.items" :key="it.orderItemId">
                <div class="item-name">{{ it.variant?.product?.name || `Variant #${it.variantId}` }}</div>
                <div class="item-qty">x{{ it.quantity }}</div>
                <div class="item-price">{{ fmtMoney(it.priceAtPurchase * it.quantity) }}</div>
              </div>
              <div class="item-total">
                <span>Tổng cộng</span>
                <strong>{{ fmtMoney(selected.totalPrice) }}</strong>
              </div>
            </div>
          </section>

          <!-- Order status buttons -->
          <section class="drawer-section">
            <div class="section-title">Cập nhật trạng thái đơn hàng</div>
            <div class="status-flow">
              <button
                v-for="s in orderStatuses" :key="s.v"
                :class="['status-btn', selected.orderStatus===s.v && 'status-btn--active', s.cls]"
                :disabled="saving || selected.orderStatus===s.v"
                @click="updateOrderStatus(s.v)"
              >
                <span class="material-symbols-outlined">{{ s.icon }}</span>
                {{ s.label }}
              </button>
            </div>
          </section>

          <!-- Invoice -->
          <section class="drawer-section" v-if="selected.invoice">
            <div class="section-title">Hóa đơn {{ selected.invoice.invoiceNumber }}</div>
            <div class="info-grid">
              <div class="info-item"><span class="info-label">Tạm tính</span><span>{{ fmtMoney(selected.invoice.subtotal) }}</span></div>
              <div class="info-item"><span class="info-label">Thuế (10%)</span><span>{{ fmtMoney(selected.invoice.taxAmount) }}</span></div>
              <div class="info-item"><span class="info-label">Tổng hóa đơn</span><strong>{{ fmtMoney(selected.invoice.totalAmount) }}</strong></div>
              <div class="info-item"><span class="info-label">Trạng thái TT</span>
                <span :class="['badge','badge-pay--'+selected.invoice.paymentStatus]">{{ payLabel(selected.invoice.paymentStatus) }}</span>
              </div>
            </div>

            <!-- Payment action buttons -->
            <div class="pay-actions">
              <button
                v-if="selected.invoice.paymentStatus==='unpaid'"
                class="pay-btn pay-btn--cod"
                :disabled="saving"
                @click="markPaid('cod')"
              ><span class="material-symbols-outlined">payments</span> Xác nhận COD</button>

              <button
                v-if="selected.invoice.paymentStatus==='unpaid'"
                class="pay-btn pay-btn--bank"
                :disabled="saving"
                @click="markPaid('bank_transfer')"
              ><span class="material-symbols-outlined">account_balance</span> Chuyển khoản</button>

              <button
                v-if="selected.invoice.paymentStatus==='paid'"
                class="pay-btn pay-btn--refund"
                :disabled="saving"
                @click="markPaid('refunded')"
              ><span class="material-symbols-outlined">assignment_return</span> Hoàn tiền</button>
            </div>
          </section>

          <p v-if="saveErr" class="drawer-err">{{ saveErr }}</p>
        </aside>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

const API_ORDER   = 'http://localhost:5072/api/order'
const API_INVOICE = 'http://localhost:5072/api/invoice'

const orders  = ref([])
const loading = ref(false)
const err     = ref('')
const search  = ref('')
const activeTab = ref('all')
const selected  = ref(null)
const saving    = ref(false)
const saveErr   = ref('')

const tabs = [
  { label: 'Tất cả',      v: 'all' },
  { label: 'Chờ xử lý',  v: 'pending' },
  { label: 'Đang xử lý', v: 'processing' },
  { label: 'Đang giao',  v: 'shipped' },
  { label: 'Hoàn thành', v: 'delivered' },
  { label: 'Đã hủy',     v: 'cancelled' },
]

const orderStatuses = [
  { v: 'pending',    label: 'Chờ xử lý',  icon: 'schedule',          cls: 'osb-pending'    },
  { v: 'processing', label: 'Xác nhận',   icon: 'thumb_up',           cls: 'osb-processing' },
  { v: 'shipped',    label: 'Giao vận',   icon: 'local_shipping',     cls: 'osb-shipped'    },
  { v: 'delivered',  label: 'Hoàn thành', icon: 'check_circle',       cls: 'osb-delivered'  },
  { v: 'cancelled',  label: 'Hủy đơn',   icon: 'cancel',             cls: 'osb-cancelled'  },
]

const stats = computed(() => [
  { label: 'Tổng đơn',    value: orders.value.length,                                         icon: 'receipt_long',  cls: '' },
  { label: 'Đang xử lý', value: orders.value.filter(o=>o.orderStatus==='processing').length,  icon: 'pending',       cls: 'si-proc' },
  { label: 'Đang giao',  value: orders.value.filter(o=>o.orderStatus==='shipped').length,     icon: 'local_shipping',cls: 'si-ship' },
  { label: 'Chưa TT',    value: orders.value.filter(o=>o.invoice?.paymentStatus==='unpaid').length, icon: 'payments', cls: 'si-warn' },
])

const filtered = computed(() => {
  const q = search.value.toLowerCase()
  return orders.value.filter(o => {
    const matchTab = activeTab.value === 'all' || o.orderStatus === activeTab.value
    const matchQ   = !q || String(o.orderId).includes(q) || o.user?.fullName?.toLowerCase().includes(q)
    return matchTab && matchQ
  })
})

async function fetchOrders() {
  loading.value = true; err.value = ''
  try {
    const res  = await fetch(API_ORDER)
    const json = await res.json()
    orders.value = json.success ? json.data : []
    if (!json.success) err.value = 'Không tải được đơn hàng.'
  } catch { err.value = 'Lỗi kết nối server.' }
  finally { loading.value = false }
}

function openDetail(o) { selected.value = { ...o }; saveErr.value = '' }

async function updateOrderStatus(status) {
  if (!selected.value) return
  saving.value = true; saveErr.value = ''
  try {
    const res = await fetch(`${API_ORDER}/${selected.value.orderId}/status`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(status)
    })
    const json = await res.json()
    if (!json.success) throw new Error(json.message)
    selected.value.orderStatus = status
    const o = orders.value.find(x => x.orderId === selected.value.orderId)
    if (o) o.orderStatus = status
  } catch (e) { saveErr.value = e.message }
  finally { saving.value = false }
}

async function markPaid(method) {
  if (!selected.value?.invoice) return
  saving.value = true; saveErr.value = ''
  try {
    const invId = selected.value.invoice.invoiceId
    const url = method === 'refunded'
      ? `${API_INVOICE}/${invId}/pay`
      : `${API_INVOICE}/${invId}/pay`
    const body = method === 'refunded' ? 'refunded' : method
    const res  = await fetch(url, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(body)
    })
    const json = await res.json()
    if (!json.success) throw new Error(json.message)
    selected.value.invoice.paymentStatus = method === 'refunded' ? 'refunded' : 'paid'
    selected.value.invoice.paymentMethod = method
    const o = orders.value.find(x => x.orderId === selected.value.orderId)
    if (o?.invoice) { o.invoice.paymentStatus = selected.value.invoice.paymentStatus }
  } catch (e) { saveErr.value = e.message }
  finally { saving.value = false }
}

const fmtDate  = d => d ? new Date(d).toLocaleDateString('vi-VN') : '—'
const fmtMoney = v => v != null ? new Intl.NumberFormat('vi-VN',{style:'currency',currency:'VND'}).format(v) : '—'
const orderLabel = s => ({ pending:'Chờ xử lý', processing:'Đang xử lý', shipped:'Đang giao', delivered:'Hoàn thành', cancelled:'Đã hủy' }[s] ?? s)
const payLabel   = s => ({ unpaid:'Chưa TT', paid:'Đã TT', refunded:'Hoàn tiền', void:'Vô hiệu' }[s] ?? '—')

onMounted(fetchOrders)
</script>

<style scoped>
.material-symbols-outlined { font-family:'Material Symbols Outlined'; font-size:20px; line-height:1; }

/* Layout */
.ph-layout { display:flex; min-height:100vh; background:#faf8f5; font-family:'Inter',sans-serif; }

/* Sidebar */
.sidebar { width:252px; background:#fff; border-right:1px solid #e8e0d8; display:flex; flex-direction:column; padding:1.5rem 1rem; gap:.5rem; position:sticky; top:0; height:100vh; }
.sidebar-brand h2 { font-size:1.05rem; font-weight:700; color:#b36a3a; margin:0 0 .2rem; }
.sidebar-brand p  { font-size:.75rem; color:#8a7060; margin:0 0 1.2rem; }
.sidebar-nav { display:flex; flex-direction:column; gap:.25rem; }
.nav-item { display:flex; align-items:center; gap:.75rem; padding:.65rem 1rem; border-radius:10px; text-decoration:none; color:#6b5c50; font-size:.875rem; transition:background .15s; }
.nav-item:hover   { background:#f5ede6; color:#b36a3a; }
.nav-item--active { background:#f5ede6; color:#b36a3a; font-weight:600; }

/* Main */
.ph-main { flex:1; display:flex; flex-direction:column; }
.topbar { background:#fff; border-bottom:1px solid #e8e0d8; padding:1.25rem 2rem; display:flex; align-items:flex-start; justify-content:space-between; gap:1rem; flex-wrap:wrap; position:sticky; top:0; z-index:30; }
.topbar h1 { font-size:1.4rem; font-weight:700; color:#b36a3a; margin:0 0 .2rem; }
.topbar-sub { font-size:.85rem; color:#8a7060; margin:0; }
.topbar-right { display:flex; align-items:center; gap:.75rem; }
.search-wrap { position:relative; }
.search-wrap .material-symbols-outlined { position:absolute; left:.75rem; top:50%; transform:translateY(-50%); color:#aaa; font-size:18px; }
.search-wrap input { padding:.55rem .75rem .55rem 2.4rem; border:1px solid #e0d8d0; border-radius:8px; font-size:.875rem; background:#faf8f5; width:240px; outline:none; }
.search-wrap input:focus { border-color:#b36a3a; }

/* Stats */
.stats-row { display:flex; gap:1rem; padding:1.25rem 2rem 0; flex-wrap:wrap; }
.stat-card { background:#fff; border:1px solid #e8e0d8; border-radius:12px; padding:.9rem 1.25rem; display:flex; align-items:center; gap:1rem; min-width:140px; }
.stat-icon { width:40px; height:40px; border-radius:10px; background:#f5ede6; display:flex; align-items:center; justify-content:center; color:#b36a3a; }
.si-proc { background:#e6ecf5; color:#3a7ab3; }
.si-ship { background:#e6f5ed; color:#5b7e6b; }
.si-warn { background:#fff0e6; color:#b36a3a; }
.stat-label { font-size:.7rem; font-weight:600; color:#8a7060; text-transform:uppercase; letter-spacing:.04em; }
.stat-value { font-size:1.6rem; font-weight:700; color:#2c2018; }

/* Tabs */
.filter-tabs { display:flex; gap:.5rem; padding:1.25rem 2rem 0; flex-wrap:wrap; }
.tab { padding:.4rem 1rem; border-radius:20px; border:1px solid #e0d8d0; background:#fff; color:#8a7060; font-size:.8rem; font-weight:600; cursor:pointer; transition:all .15s; }
.tab:hover  { border-color:#b36a3a; color:#b36a3a; }
.tab--on    { background:#b36a3a; color:#fff; border-color:#b36a3a; }

/* State */
.state-msg { text-align:center; padding:3rem; color:#8a7060; }
.state-err { color:#c0392b; }

/* Table */
.table-wrap { margin:1.25rem 2rem 2rem; background:#fff; border:1px solid #e8e0d8; border-radius:16px; overflow:hidden; }
.tbl { width:100%; border-collapse:collapse; font-size:.875rem; }
.tbl th { padding:.85rem 1rem; font-size:.7rem; font-weight:700; text-transform:uppercase; letter-spacing:.05em; color:#8a7060; background:#faf8f5; border-bottom:1px solid #e8e0d8; text-align:left; }
.tbl td { padding:.85rem 1rem; border-bottom:1px solid #f0e8e0; vertical-align:middle; }
.tbl-row { cursor:pointer; transition:background .1s; }
.tbl-row:hover { background:#fdf6f0; }
.tbl-row:last-child td { border-bottom:none; }
.td-id    { font-weight:700; color:#b36a3a; }
.td-muted { color:#8a7060; font-size:.82rem; }
.td-price { font-weight:700; color:#2c2018; }
.td-action { color:#b0a090; text-align:right; }
.empty-cell { text-align:center; padding:3rem; color:#b0a090; }
.empty-cell .material-symbols-outlined { font-size:40px; display:block; margin-bottom:.5rem; }

/* Badges */
.badge { padding:3px 10px; border-radius:20px; font-size:.7rem; font-weight:700; display:inline-block; }
.badge-order--pending    { background:#fff3e0; color:#b36a3a; }
.badge-order--processing { background:#e3f2fd; color:#1565c0; }
.badge-order--shipped    { background:#e8f5e9; color:#2e7d32; }
.badge-order--delivered  { background:#e8f5e9; color:#1b5e20; }
.badge-order--cancelled  { background:#fce4ec; color:#b71c1c; }
.badge-pay--unpaid   { background:#fff3e0; color:#e65100; }
.badge-pay--paid     { background:#e8f5e9; color:#2e7d32; }
.badge-pay--refunded { background:#fce4ec; color:#b71c1c; }
.badge-pay--void     { background:#f5f5f5; color:#757575; }

/* Drawer */
.drawer-backdrop { position:fixed; inset:0; background:rgba(0,0,0,.4); backdrop-filter:blur(3px); z-index:200; display:flex; justify-content:flex-end; }
.drawer { width:480px; max-width:100%; height:100vh; background:#fff; box-shadow:-8px 0 40px rgba(0,0,0,.15); display:flex; flex-direction:column; overflow-y:auto; }
.drawer-header { display:flex; justify-content:space-between; align-items:flex-start; padding:1.5rem; border-bottom:1px solid #f0e8e0; position:sticky; top:0; background:#fff; z-index:5; }
.drawer-header h2 { font-size:1.2rem; font-weight:700; color:#b36a3a; margin:0; }
.drawer-sub { font-size:.8rem; color:#8a7060; margin:.2rem 0 0; }
.close-btn { background:none; border:none; cursor:pointer; padding:.25rem; border-radius:6px; display:flex; color:#8a7060; }
.close-btn:hover { background:#f5ede6; }
.drawer-section { padding:1.25rem 1.5rem; border-bottom:1px solid #f5ede6; }
.section-title { font-size:.7rem; font-weight:700; text-transform:uppercase; letter-spacing:.06em; color:#b36a3a; margin-bottom:.85rem; }
.info-grid { display:grid; grid-template-columns:1fr 1fr; gap:.75rem; }
.info-item { display:flex; flex-direction:column; gap:.2rem; }
.info-item--full { grid-column:1/-1; }
.info-label { font-size:.72rem; color:#8a7060; font-weight:600; }
.items-list { display:flex; flex-direction:column; gap:.4rem; }
.item-row { display:flex; align-items:center; gap:.5rem; padding:.5rem; background:#faf8f5; border-radius:8px; font-size:.85rem; }
.item-name { flex:1; font-weight:600; color:#2c2018; }
.item-qty  { color:#8a7060; }
.item-price{ font-weight:700; color:#b36a3a; }
.item-total{ display:flex; justify-content:space-between; padding:.5rem; border-top:1px solid #e8e0d8; font-size:.9rem; color:#2c2018; margin-top:.25rem; }

/* Status buttons */
.status-flow { display:flex; flex-wrap:wrap; gap:.5rem; }
.status-btn { display:flex; align-items:center; gap:.35rem; padding:.5rem 1rem; border-radius:8px; border:2px solid transparent; font-size:.8rem; font-weight:700; cursor:pointer; transition:all .15s; background:#f5f5f5; color:#5a4a3a; }
.status-btn:disabled { opacity:.5; cursor:default; }
.status-btn--active  { opacity:1 !important; cursor:default; }
.osb-pending    { border-color:#f0c070; background:#fff8e8; color:#8a5a00; }
.osb-pending.status-btn--active { background:#f0c070; }
.osb-processing { border-color:#90caf9; background:#e3f2fd; color:#1565c0; }
.osb-processing.status-btn--active { background:#90caf9; }
.osb-shipped    { border-color:#80cbc4; background:#e0f2f1; color:#00695c; }
.osb-shipped.status-btn--active { background:#80cbc4; }
.osb-delivered  { border-color:#a5d6a7; background:#e8f5e9; color:#2e7d32; }
.osb-delivered.status-btn--active { background:#a5d6a7; }
.osb-cancelled  { border-color:#ef9a9a; background:#fce4ec; color:#b71c1c; }
.osb-cancelled.status-btn--active { background:#ef9a9a; }

/* Pay action */
.pay-actions { display:flex; flex-wrap:wrap; gap:.5rem; margin-top:1rem; }
.pay-btn { display:flex; align-items:center; gap:.4rem; padding:.55rem 1.1rem; border:none; border-radius:8px; font-size:.82rem; font-weight:700; cursor:pointer; transition:opacity .15s; }
.pay-btn:disabled { opacity:.6; cursor:not-allowed; }
.pay-btn--cod   { background:#4caf50; color:#fff; }
.pay-btn--bank  { background:#1976d2; color:#fff; }
.pay-btn--refund{ background:#e53935; color:#fff; }

.drawer-err { color:#c0392b; font-size:.82rem; padding:.5rem 1.5rem 1rem; }

/* Transition */
.drawer-enter-active,.drawer-leave-active { transition:transform .3s ease; }
.drawer-enter-from,.drawer-leave-to { transform:translateX(100%); }
.drawer-enter-to,.drawer-leave-from { transform:translateX(0); }
</style>
