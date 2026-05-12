<template>
  <div class="inventorycontrol" style="display:flex;min-height:100vh;">

    <!-- SideNavBar -->
    <aside class="sidebar">
      <div class="sidebar-brand">
        <h2>Quản trị Bakery</h2>
        <p>Vận hành Hàng ngày</p>
      </div>
      <nav class="sidebar-nav">
        <router-link to="/dashboard" class="nav-item">
          <span class="material-symbols-outlined">dashboard</span>
          <span>Bảng điều khiển</span>
        </router-link>
        <router-link to="/payments" class="nav-item">
          <span class="material-symbols-outlined">receipt_long</span>
          <span>Lịch sử Hóa đơn</span>
        </router-link>
        <router-link to="/users" class="nav-item">
          <span class="material-symbols-outlined">group</span>
          <span>Quản lý Người dùng</span>
        </router-link>
        <router-link to="/inventory" class="nav-item nav-item--active">
          <span class="material-symbols-outlined">inventory_2</span>
          <span>Quản lý Sản phẩm</span>
        </router-link>
      </nav>
      <div class="sidebar-footer">
        <button class="btn-add-new" @click="openCreate">
          <span class="material-symbols-outlined">add</span> Thêm sản phẩm
        </button>
      </div>
    </aside>

    <!-- Main -->
    <main style="flex:1;display:flex;flex-direction:column;">
      <!-- Header -->
      <header class="topbar">
        <h1>Kiểm soát Kho hàng</h1>
        <div class="topbar-right">
          <div class="search-wrap">
            <span class="material-symbols-outlined">search</span>
            <input v-model="searchQuery" placeholder="Tìm kiếm sản phẩm..." />
          </div>
        </div>
      </header>

      <!-- Stats bar -->
      <div class="stats-bar">
        <div class="stat-card">
          <span class="stat-label">Tổng sản phẩm</span>
          <span class="stat-value">{{ products.length }}</span>
        </div>
        <div class="stat-card stat-card--warn">
          <span class="stat-label">Sắp hết hàng</span>
          <span class="stat-value">{{ lowStockCount }}</span>
        </div>
      </div>

      <!-- Loading / Error -->
      <div v-if="loading" class="state-msg">Đang tải sản phẩm...</div>
      <div v-else-if="fetchError" class="state-msg state-msg--error">{{ fetchError }}</div>

      <!-- Product Grid -->
      <div v-else class="product-grid">
        <div
          v-for="p in filteredProducts"
          :key="p.productId"
          class="product-card"
        >
          <!-- Image -->
          <div class="card-img-wrap">
            <img
              :src="imageUrl(p)"
              :alt="p.name"
              class="card-img"
            />
            <span
              class="stock-badge"
              :class="stockBadgeClass(p)"
            >{{ stockLabel(p) }}</span>
          </div>

          <!-- Info -->
          <div class="card-body">
            <div class="card-title-row">
              <h3>{{ p.name }}</h3>
              <span class="card-price">{{ formatPrice(firstVariant(p)?.price) }}</span>
            </div>
            <p class="card-desc">{{ p.baseDescription }}</p>

            <div class="card-stock" :class="{ 'card-stock--low': isLow(p) }">
              <div>
                <span class="stock-lbl">Tồn kho</span>
                <strong>{{ firstVariant(p)?.stockQty ?? 0 }}</strong>
              </div>
              <span class="material-symbols-outlined">{{ isLow(p) ? 'warning' : 'inventory_2' }}</span>
            </div>

            <div class="card-actions">
              <button class="btn-edit" @click="openEdit(p)">
                <span class="material-symbols-outlined">edit</span> Sửa
              </button>
              <button class="btn-delete" @click="confirmDelete(p)">
                <span class="material-symbols-outlined">delete</span> Xóa
              </button>
            </div>
          </div>
        </div>

        <!-- Empty state -->
        <div v-if="filteredProducts.length === 0" class="empty-state">
          <span class="material-symbols-outlined">inbox</span>
          <p>Không có sản phẩm nào</p>
        </div>
      </div>
    </main>

    <!-- ══════════════ MODAL: Create / Edit ══════════════ -->
    <div v-if="showModal" class="modal-backdrop" @click.self="closeModal">
      <div class="modal">
        <div class="modal-header">
          <h2>{{ editingId ? 'Sửa sản phẩm' : 'Thêm sản phẩm mới' }}</h2>
          <button class="modal-close" @click="closeModal">
            <span class="material-symbols-outlined">close</span>
          </button>
        </div>

        <form class="modal-form" @submit.prevent="submitForm">
          <!-- Image preview + upload -->
          <div class="img-upload-area" @click="triggerFileInput" @dragover.prevent @drop.prevent="onDrop">
            <img v-if="imagePreview" :src="imagePreview" class="img-preview" alt="preview" />
            <div v-else class="img-placeholder">
              <span class="material-symbols-outlined">cloud_upload</span>
              <span>Kéo thả hoặc bấm để chọn ảnh</span>
              <span class="img-hint">JPG, PNG, WebP — tối đa 5 MB</span>
            </div>
            <input
              ref="fileInput"
              type="file"
              accept=".jpg,.jpeg,.png,.webp,.gif"
              style="display:none"
              @change="onFileChange"
            />
          </div>
          <p v-if="uploadError" class="field-error">{{ uploadError }}</p>

          <div class="form-row">
            <div class="field">
              <label>Tên sản phẩm *</label>
              <input v-model="form.name" required placeholder="VD: Sourdough Artisan" />
            </div>
            <div class="field">
              <label>SKU *</label>
              <input v-model="form.sku" required placeholder="VD: BREAD-001" />
            </div>
          </div>

          <div class="form-row">
            <div class="field">
              <label>Giá (VND) *</label>
              <input v-model.number="form.price" type="number" min="0" step="500" required placeholder="85000" />
            </div>
            <div class="field">
              <label>Tồn kho *</label>
              <input v-model.number="form.stockQuantity" type="number" min="0" required placeholder="20" />
            </div>
          </div>

          <div class="field">
            <label>Mô tả</label>
            <textarea v-model="form.baseDescription" rows="3" placeholder="Mô tả sản phẩm..."></textarea>
          </div>

          <p v-if="submitError" class="field-error">{{ submitError }}</p>

          <div class="modal-footer">
            <button type="button" class="btn-cancel" @click="closeModal">Hủy</button>
            <button type="submit" class="btn-save" :disabled="saving">
              <span v-if="saving" class="material-symbols-outlined spin">refresh</span>
              {{ saving ? 'Đang lưu...' : (editingId ? 'Lưu thay đổi' : 'Tạo sản phẩm') }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- ══════════════ MODAL: Confirm Delete ══════════════ -->
    <div v-if="deleteTarget" class="modal-backdrop" @click.self="deleteTarget = null">
      <div class="modal modal--sm">
        <div class="modal-header">
          <h2>Xác nhận xóa</h2>
          <button class="modal-close" @click="deleteTarget = null">
            <span class="material-symbols-outlined">close</span>
          </button>
        </div>
        <p style="padding:0 1.5rem 1rem;">
          Bạn chắc chắn muốn xóa <strong>{{ deleteTarget.name }}</strong>? Hành động này không thể hoàn tác.
        </p>
        <div class="modal-footer">
          <button class="btn-cancel" @click="deleteTarget = null">Hủy</button>
          <button class="btn-delete-confirm" :disabled="saving" @click="doDelete">
            {{ saving ? 'Đang xóa...' : 'Xóa sản phẩm' }}
          </button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

const API = 'http://localhost:5072/api'

// ── State ─────────────────────────────────────────────
const products     = ref([])
const loading      = ref(false)
const fetchError   = ref('')
const searchQuery  = ref('')
const showModal    = ref(false)
const saving       = ref(false)
const editingId    = ref(null)
const deleteTarget = ref(null)
const submitError  = ref('')
const uploadError  = ref('')
const imagePreview = ref('')
const fileInput    = ref(null)
const selectedFile = ref(null)

const form = ref({
  name: '', sku: '', price: 0,
  stockQuantity: 0, baseDescription: '', imageUrl: ''
})

// ── Computed ──────────────────────────────────────────
const filteredProducts = computed(() => {
  const q = searchQuery.value.toLowerCase()
  return products.value.filter(p =>
    p.name?.toLowerCase().includes(q) ||
    p.baseDescription?.toLowerCase().includes(q)
  )
})

const lowStockCount = computed(() =>
  products.value.filter(p => (firstVariant(p)?.stockQty ?? 0) <= 5).length
)

// ── Helpers ───────────────────────────────────────────
const firstVariant = p => p.variants?.[0] ?? null

const imageUrl = p => {
  const url = firstVariant(p)?.imageUrl
  if (!url) return 'https://placehold.co/400x300?text=No+Image'
  return url.startsWith('http') ? url : `http://localhost:5072${url}`
}

const isLow = p => (firstVariant(p)?.stockQty ?? 0) <= 5

const stockLabel = p => isLow(p) ? 'SẮP HẾT' : 'CÒN HÀNG'

const stockBadgeClass = p => isLow(p) ? 'stock-badge--low' : 'stock-badge--ok'

const formatPrice = v =>
  v != null ? new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v) : '—'

// ── API calls ─────────────────────────────────────────
async function fetchProducts() {
  loading.value = true; fetchError.value = ''
  try {
    const res = await fetch(`${API}/product`)
    const json = await res.json()
    if (json.success) products.value = json.data
    else fetchError.value = 'Không tải được sản phẩm.'
  } catch {
    fetchError.value = 'Lỗi kết nối tới server.'
  } finally {
    loading.value = false
  }
}

// ── Modal open/close ──────────────────────────────────
function openCreate() {
  editingId.value = null
  form.value = { name:'', sku:'', price:0, stockQuantity:0, baseDescription:'', imageUrl:'' }
  imagePreview.value = ''; selectedFile.value = null; submitError.value = ''; uploadError.value = ''
  showModal.value = true
}

function openEdit(p) {
  editingId.value = p.productId
  const v = firstVariant(p)
  form.value = {
    name: p.name, sku: v?.sku ?? '', price: v?.price ?? 0,
    stockQuantity: v?.stockQty ?? 0, baseDescription: p.baseDescription ?? '', imageUrl: v?.imageUrl ?? ''
  }
  imagePreview.value = v?.imageUrl ? imageUrl(p) : ''
  selectedFile.value = null; submitError.value = ''; uploadError.value = ''
  showModal.value = true
}

function closeModal() { showModal.value = false }

// ── Image handling ────────────────────────────────────
function triggerFileInput() { fileInput.value?.click() }

function onFileChange(e) { handleFile(e.target.files[0]) }

function onDrop(e) { handleFile(e.dataTransfer.files[0]) }

function handleFile(file) {
  uploadError.value = ''
  if (!file) return
  const allowed = ['image/jpeg','image/png','image/webp','image/gif']
  if (!allowed.includes(file.type)) { uploadError.value = 'Chỉ nhận JPG, PNG, WebP, GIF'; return }
  if (file.size > 5 * 1024 * 1024) { uploadError.value = 'File không được vượt quá 5 MB'; return }
  selectedFile.value = file
  imagePreview.value = URL.createObjectURL(file)
}

// ── Submit form ───────────────────────────────────────
async function submitForm() {
  saving.value = true; submitError.value = ''
  try {
    if (editingId.value) {
      // 1. Update product info
      await fetch(`${API}/product/${editingId.value}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(form.value)
      })
      // 2. Upload image if changed
      if (selectedFile.value) {
        const fd = new FormData()
        fd.append('image', selectedFile.value)
        await fetch(`${API}/product/${editingId.value}/upload-image`, { method: 'POST', body: fd })
      }
    } else {
      // Create with image (multipart)
      const fd = new FormData()
      fd.append('Name', form.value.name)
      fd.append('Sku', form.value.sku)
      fd.append('Price', form.value.price)
      fd.append('StockQuantity', form.value.stockQuantity)
      fd.append('BaseDescription', form.value.baseDescription)
      if (selectedFile.value) fd.append('image', selectedFile.value)

      const res = await fetch(`${API}/product/with-image`, { method: 'POST', body: fd })
      const json = await res.json()
      if (!json.success) throw new Error(json.message || 'Tạo thất bại')
    }
    closeModal()
    await fetchProducts()
  } catch (err) {
    submitError.value = err.message || 'Đã xảy ra lỗi'
  } finally {
    saving.value = false
  }
}

// ── Delete ────────────────────────────────────────────
function confirmDelete(p) { deleteTarget.value = p }

async function doDelete() {
  if (!deleteTarget.value) return
  saving.value = true
  try {
    await fetch(`${API}/product/${deleteTarget.value.productId}`, { method: 'DELETE' })
    deleteTarget.value = null
    await fetchProducts()
  } finally {
    saving.value = false
  }
}

onMounted(fetchProducts)
</script>

<style scoped>
.material-symbols-outlined { font-family: 'Material Symbols Outlined'; font-size: 20px; }
.spin { display:inline-block; animation: spin 1s linear infinite; }
@keyframes spin { to { transform: rotate(360deg); } }

/* Layout */
.inventorycontrol { display: flex; min-height: 100vh; background: #faf8f5; font-family: 'Inter', sans-serif; }

/* Sidebar */
.sidebar { width: 260px; background: #fff; border-right: 1px solid #e8e0d8; display: flex; flex-direction: column; padding: 1.5rem 1rem; gap: 0.5rem; position: sticky; top: 0; height: 100vh; }
.sidebar-brand h2 { font-size: 1.1rem; font-weight: 700; color: #b36a3a; margin: 0 0 0.2rem; }
.sidebar-brand p  { font-size: 0.75rem; color: #8a7060; margin: 0 0 1.2rem; }
.sidebar-nav { display: flex; flex-direction: column; gap: 0.25rem; flex: 1; }
.nav-item { display: flex; align-items: center; gap: 0.75rem; padding: 0.7rem 1rem; border-radius: 10px; text-decoration: none; color: #6b5c50; font-size: 0.9rem; transition: background 0.15s; }
.nav-item:hover { background: #f5ede6; color: #b36a3a; }
.nav-item--active { background: #f5ede6; color: #b36a3a; font-weight: 600; }
.sidebar-footer { padding-top: 1rem; border-top: 1px solid #eee; }
.btn-add-new { width: 100%; background: #b36a3a; color: #fff; border: none; border-radius: 10px; padding: 0.75rem 1rem; font-size: 0.9rem; font-weight: 600; cursor: pointer; display: flex; align-items: center; gap: 0.5rem; justify-content: center; transition: opacity .15s; }
.btn-add-new:hover { opacity: 0.9; }

/* Topbar */
.topbar { background: #fff; border-bottom: 1px solid #e8e0d8; padding: 1rem 2rem; display: flex; align-items: center; justify-content: space-between; position: sticky; top: 0; z-index: 30; }
.topbar h1 { font-size: 1.4rem; font-weight: 700; color: #b36a3a; margin: 0; }
.topbar-right { display: flex; gap: 0.75rem; align-items: center; }
.search-wrap { position: relative; }
.search-wrap .material-symbols-outlined { position: absolute; left: 0.75rem; top: 50%; transform: translateY(-50%); color: #aaa; font-size: 18px; }
.search-wrap input { padding: 0.5rem 0.75rem 0.5rem 2.5rem; border: 1px solid #e0d8d0; border-radius: 8px; font-size: 0.875rem; outline: none; background: #faf8f5; width: 220px; }
.search-wrap input:focus { border-color: #b36a3a; box-shadow: 0 0 0 3px rgba(179,106,58,.1); }

/* Stats */
.stats-bar { display: flex; gap: 1rem; padding: 1.25rem 2rem 0; }
.stat-card { background: #fff; border: 1px solid #e8e0d8; border-radius: 12px; padding: 1rem 1.5rem; display: flex; flex-direction: column; gap: 0.25rem; min-width: 140px; }
.stat-card--warn { border-color: #f5c6a0; background: #fff8f3; }
.stat-label { font-size: 0.7rem; font-weight: 600; color: #8a7060; text-transform: uppercase; letter-spacing: .05em; }
.stat-value { font-size: 1.75rem; font-weight: 700; color: #b36a3a; }

/* State msg */
.state-msg { text-align: center; padding: 3rem; color: #8a7060; font-size: 1rem; }
.state-msg--error { color: #c0392b; }

/* Product Grid */
.product-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(240px, 1fr)); gap: 1.25rem; padding: 1.5rem 2rem 2rem; }

/* Product Card */
.product-card { background: #fff; border: 1px solid #e8e0d8; border-radius: 16px; overflow: hidden; display: flex; flex-direction: column; box-shadow: 0 2px 8px rgba(0,0,0,.04); transition: transform .2s, box-shadow .2s; }
.product-card:hover { transform: translateY(-4px); box-shadow: 0 8px 24px rgba(0,0,0,.08); }
.card-img-wrap { height: 180px; position: relative; overflow: hidden; background: #f5ede6; }
.card-img { width: 100%; height: 100%; object-fit: cover; transition: transform .5s; }
.product-card:hover .card-img { transform: scale(1.06); }
.stock-badge { position: absolute; top: 10px; right: 10px; padding: 3px 10px; border-radius: 20px; font-size: 0.68rem; font-weight: 700; letter-spacing: .04em; }
.stock-badge--ok  { background: rgba(200,240,210,.9); color: #276937; }
.stock-badge--low { background: rgba(255,220,200,.9); color: #a03020; }
.card-body { padding: 1rem; display: flex; flex-direction: column; flex: 1; gap: 0.5rem; }
.card-title-row { display: flex; justify-content: space-between; align-items: flex-start; }
.card-title-row h3 { font-size: 0.95rem; font-weight: 700; color: #2c2018; margin: 0; }
.card-price { font-weight: 700; color: #b36a3a; font-size: 0.95rem; white-space: nowrap; }
.card-desc { font-size: 0.8rem; color: #8a7060; flex: 1; }
.card-stock { display: flex; align-items: center; justify-content: space-between; background: #f5ede6; border-radius: 8px; padding: 0.5rem 0.75rem; }
.card-stock--low { background: #fff0ec; }
.stock-lbl { display: block; font-size: 0.65rem; font-weight: 700; text-transform: uppercase; letter-spacing: .05em; color: #8a7060; }
.card-stock strong { font-size: 1.1rem; color: #2c2018; }
.card-actions { display: grid; grid-template-columns: 1fr 1fr; gap: 0.5rem; margin-top: 0.25rem; }
.btn-edit   { display: flex; align-items: center; justify-content: center; gap: 0.3rem; padding: 0.5rem; border: 1px solid #e0d8d0; background: #fff; color: #5a4a3a; border-radius: 8px; cursor: pointer; font-size: 0.8rem; font-weight: 600; transition: background .15s; }
.btn-edit:hover { background: #f5ede6; }
.btn-delete { display: flex; align-items: center; justify-content: center; gap: 0.3rem; padding: 0.5rem; border: 1px solid #f5c6c6; background: #fff5f5; color: #c0392b; border-radius: 8px; cursor: pointer; font-size: 0.8rem; font-weight: 600; transition: background .15s; }
.btn-delete:hover { background: #ffe8e8; }
.empty-state { grid-column: 1/-1; text-align: center; padding: 4rem; color: #b0a090; }
.empty-state .material-symbols-outlined { font-size: 48px; display: block; margin-bottom: 0.75rem; }

/* Modal */
.modal-backdrop { position: fixed; inset: 0; background: rgba(0,0,0,.45); backdrop-filter: blur(3px); display: flex; align-items: center; justify-content: center; z-index: 100; padding: 1rem; }
.modal { background: #fff; border-radius: 20px; width: 100%; max-width: 560px; max-height: 90vh; overflow-y: auto; box-shadow: 0 20px 60px rgba(0,0,0,.2); }
.modal--sm { max-width: 400px; }
.modal-header { display: flex; justify-content: space-between; align-items: center; padding: 1.5rem 1.5rem 0; }
.modal-header h2 { font-size: 1.1rem; font-weight: 700; color: #2c2018; margin: 0; }
.modal-close { background: none; border: none; cursor: pointer; padding: 0.25rem; border-radius: 6px; display: flex; align-items: center; color: #8a7060; }
.modal-close:hover { background: #f5ede6; }
.modal-form { padding: 1.25rem 1.5rem; display: flex; flex-direction: column; gap: 1rem; }
.form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; }
.field { display: flex; flex-direction: column; gap: 0.35rem; }
.field label { font-size: 0.8rem; font-weight: 600; color: #5a4a3a; }
.field input, .field textarea { padding: 0.6rem 0.85rem; border: 1px solid #e0d8d0; border-radius: 8px; font-size: 0.875rem; outline: none; background: #faf8f5; font-family: inherit; resize: vertical; transition: border-color .15s; }
.field input:focus, .field textarea:focus { border-color: #b36a3a; box-shadow: 0 0 0 3px rgba(179,106,58,.1); }
.field-error { color: #c0392b; font-size: 0.8rem; margin: 0; }

/* Image upload */
.img-upload-area { border: 2px dashed #d8c8b8; border-radius: 12px; overflow: hidden; cursor: pointer; min-height: 160px; display: flex; align-items: center; justify-content: center; transition: border-color .2s; background: #faf8f5; }
.img-upload-area:hover { border-color: #b36a3a; background: #f5ede6; }
.img-placeholder { display: flex; flex-direction: column; align-items: center; gap: 0.4rem; color: #8a7060; padding: 2rem; text-align: center; }
.img-placeholder .material-symbols-outlined { font-size: 40px; color: #b36a3a; }
.img-hint { font-size: 0.72rem; color: #b0a090; }
.img-preview { width: 100%; height: 200px; object-fit: cover; }

/* Modal footer */
.modal-footer { display: flex; justify-content: flex-end; gap: 0.75rem; padding: 1rem 1.5rem 1.5rem; border-top: 1px solid #f0e8e0; }
.btn-cancel { padding: 0.6rem 1.25rem; border: 1px solid #e0d8d0; background: #fff; color: #5a4a3a; border-radius: 8px; font-size: 0.875rem; font-weight: 600; cursor: pointer; }
.btn-save { padding: 0.6rem 1.5rem; background: #b36a3a; color: #fff; border: none; border-radius: 8px; font-size: 0.875rem; font-weight: 600; cursor: pointer; display: flex; align-items: center; gap: 0.4rem; }
.btn-save:disabled { opacity: 0.6; cursor: not-allowed; }
.btn-delete-confirm { padding: 0.6rem 1.25rem; background: #c0392b; color: #fff; border: none; border-radius: 8px; font-size: 0.875rem; font-weight: 600; cursor: pointer; }
.btn-delete-confirm:disabled { opacity: 0.6; }
</style>
