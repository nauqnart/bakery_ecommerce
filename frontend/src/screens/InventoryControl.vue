<template>
  <div class="inventorycontrol" style="display:flex;min-height:100vh;">

<AdminSidebar />

    <!-- Main -->
    <main style="flex:1;display:flex;flex-direction:column;">
      <!-- Header -->
      <header class="topbar">
        <h1>Kiểm soát Kho hàng</h1>
        <div class="topbar-right" style="display:flex; gap:1rem; align-items:center;">
          <div class="search-wrap">
            <span class="material-symbols-outlined">search</span>
            <input v-model="searchQuery" placeholder="Tìm kiếm sản phẩm..." />
          </div>
          <button class="btn-primary" @click="openCreate" style="display:flex; align-items:center; gap:0.4rem; padding:0.6rem 1.25rem; background:#b36a3a; color:#fff; border:none; border-radius:8px; font-size:0.875rem; font-weight:600; cursor:pointer;">
            <span class="material-symbols-outlined">add</span> Thêm sản phẩm
          </button>
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
            <div style="flex: 1; display: flex; flex-direction: column; align-items: flex-start;">
              <p class="card-desc" :style="!p.isDescExpanded ? 'display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; margin-bottom: 0; word-break: break-word; flex: none;' : 'word-break: break-word; margin-bottom: 0; flex: none;'">{{ p.baseDescription }}</p>
              <button v-if="(p.baseDescription || '').length > 60" @click="p.isDescExpanded = !p.isDescExpanded" style="background: none; border: none; color: #b36a3a; font-size: 0.75rem; padding: 0; cursor: pointer; margin-top: 4px; font-weight: 600;">{{ p.isDescExpanded ? 'Thu gọn' : 'Xem thêm' }}</button>
            </div>

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
          <div class="modal-body">
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
              <input v-model="form.name" required @blur="regenerateVariants" />
            </div>
            <div class="field">
              <label>Danh mục</label>
              <select v-model="form.categoryId">
                <option :value="null">Không phân loại</option>
                <option v-for="c in categories" :key="c.categoryId" :value="c.categoryId">{{ c.name }}</option>
              </select>
            </div>
          </div>

          <div class="field-toggle" style="margin: 1.5rem 0 1rem; display:flex; align-items:center; gap: 0.5rem; cursor:pointer;" @click="toggleVariations">
            <div :class="['toggle-switch', hasVariations ? 'toggle-on' : 'toggle-off']" style="width: 44px; height: 24px; border-radius: 12px; position: relative; transition: background-color 0.2s;" :style="{ background: hasVariations ? '#b36a3a' : '#ccc' }">
               <div class="toggle-knob" style="width: 20px; height: 20px; background: white; border-radius: 50%; position: absolute; top: 2px; transition: left 0.2s;" :style="{ left: hasVariations ? '22px' : '2px' }"></div>
            </div>
            <span style="font-weight:600; color:#5a4a3a; user-select:none;">Sản phẩm này có nhiều lựa chọn (Kích cỡ, Hương vị...)</span>
          </div>

          <div v-if="!hasVariations" class="form-row" style="margin-bottom: 1.5rem;">
            <div class="field">
              <label>Mã sản phẩm</label>
              <input v-model="form.variants[0].sku" disabled style="background: #e8e0d8; cursor:not-allowed; color:#8a7060;" />
            </div>
            <div class="field">
              <label>Giá bán (VND) *</label>
              <input v-model.number="form.variants[0].price" type="number" min="0" required placeholder="0" />
            </div>
            <div class="field">
              <label>Tồn kho *</label>
              <input v-model.number="form.variants[0].stockQuantity" type="number" min="0" required placeholder="0" />
            </div>
          </div>

          <div v-if="hasVariations">
            <hr style="margin: 1rem 0; border: none; border-top: 1px solid #e0d8d0;" />
            <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom: 1rem;">
              <label style="margin:0; font-weight:600; font-size:16px;">Nhóm Phân loại (Tối đa 2)</label>
              <button type="button" @click="addTier" v-if="form.tierVariations.length < 2" style="padding: 4px 12px; font-size:13px; border-radius: 4px; display:flex; align-items:center; background:#b36a3a; color:white; border:none; cursor:pointer;">
                <span class="material-symbols-outlined" style="font-size:16px;">add</span> Thêm nhóm
              </button>
            </div>

            <div v-for="(tier, tIdx) in form.tierVariations" :key="tIdx" style="border: 1px solid #e0d8d0; border-radius: 8px; padding: 1rem; margin-bottom: 1rem; position: relative; background: #faf8f5;">
              <button type="button" @click="removeTier(tIdx)" style="position:absolute; top: 12px; right: 12px; color: #dc2626; background: none; border: none; cursor: pointer;">
                <span class="material-symbols-outlined">close</span>
              </button>
              <div class="field">
                <label>Tên nhóm (VD: Kích cỡ) *</label>
                <input v-model="tier.name" required placeholder="Nhập tên nhóm" @input="regenerateVariants" />
              </div>
              <div class="field" style="margin-top: 0.5rem;">
                <label>Tùy chọn (Ví dụ: S, M, Đỏ...)</label>
                <div style="display:flex; flex-wrap:wrap; gap:8px; margin-bottom: 8px;">
                  <span v-for="(val, vIdx) in tier.values" :key="vIdx" style="background:#b36a3a; color:white; padding:4px 12px; border-radius:16px; font-size:13px; display:flex; align-items:center; gap:4px;">
                    {{ val }}
                    <span class="material-symbols-outlined" style="font-size:14px; cursor:pointer;" @click="removeTierValue(tIdx, vIdx)">close</span>
                  </span>
                </div>
                <div style="display:flex; gap: 8px;">
                  <input v-model="tier.tempValue" @keydown.enter.prevent="addTierValue(tIdx)" placeholder="Nhập tên tùy chọn..." style="flex:1; padding: 0.6rem; border: 1px solid #e0d8d0; border-radius: 8px;" />
                  <button type="button" @click="addTierValue(tIdx)" style="padding: 0 16px; border-radius: 4px; background:#b36a3a; color:#fff; border:none; cursor:pointer;">Thêm</button>
                </div>
              </div>
            </div>

            <div v-if="form.variants.length > 0" style="margin-top: 1.5rem; margin-bottom: 1.5rem;">
              <label style="font-weight:600; font-size:14px; margin-bottom: 0.5rem; display:block;">Thiết lập Giá & Tồn kho</label>
              <table style="width:100%; border-collapse: collapse; font-size: 13px;">
                <thead>
                  <tr style="background: #faf8f5; border-bottom: 1px solid #e0d8d0;">
                    <th style="padding: 8px; text-align:left;">Tên (Phân loại)</th>
                    <th style="padding: 8px; text-align:left;">Mã sản phẩm *</th>
                    <th style="padding: 8px; text-align:left;">Giá (VND) *</th>
                    <th style="padding: 8px; text-align:left;">Tồn kho *</th>
                    <th style="padding: 8px; text-align:center; width: 50px;">Xóa</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(v, idx) in form.variants" :key="idx" style="border-bottom: 1px solid #e0d8d0;">
                    <td style="padding: 8px; font-weight:600;">{{ v.variantName }}</td>
                    <td style="padding: 8px;"><input v-model="v.sku" disabled style="width:100%; padding:4px; border:1px solid #ccc; border-radius:4px; background: #e8e0d8; cursor:not-allowed;" /></td>
                    <td style="padding: 8px;"><input v-model.number="v.price" type="number" min="0" required placeholder="Giá" style="width:100%; padding:4px; border:1px solid #ccc; border-radius:4px;" /></td>
                    <td style="padding: 8px;"><input v-model.number="v.stockQuantity" type="number" min="0" required placeholder="Kho" style="width:100%; padding:4px; border:1px solid #ccc; border-radius:4px;" /></td>
                    <td style="padding: 8px; text-align:center;">
                      <button type="button" v-if="form.variants.length > 1" @click="removeVariant(idx)" style="color:#dc2626; background:none; border:none; cursor:pointer;" title="Xóa biến thể">
                        <span class="material-symbols-outlined" style="font-size:20px;">delete</span>
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <div class="field">
            <label>Mô tả</label>
            <textarea v-model="form.baseDescription" rows="3" placeholder="Mô tả sản phẩm..."></textarea>
          </div>

          <p v-if="submitError" class="field-error">{{ submitError }}</p>
          </div>

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
import AdminSidebar from '../components/AdminSidebar.vue'
import { ref, computed, onMounted } from 'vue'

const API = import.meta.env.VITE_API_BASE_URL + ''

const products     = ref([])
const categories   = ref([])
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
const hasVariations= ref(false)

const form = ref({
  name: '', baseDescription: '', imageUrl: '', categoryId: null,
  tierVariations: [],
  variants: [{ variantId: null, variantName: 'Mặc định', sku: '', price: 0, stockQuantity: 0, tierValues: [] }]
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
  return url.startsWith('http') ? url : `${import.meta.env.VITE_BASE_URL}\${url}`
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
    const [resProd, resCat] = await Promise.all([
      fetch(`${API}/product`),
      fetch(`${API}/category`)
    ])
    const jsonProd = await resProd.json()
    const jsonCat = await resCat.json()
    if (jsonProd.success) products.value = jsonProd.data
    else fetchError.value = 'Không tải được sản phẩm.'
    
    if (jsonCat.success) categories.value = jsonCat.data
  } catch {
    fetchError.value = 'Lỗi kết nối tới server.'
  } finally {
    loading.value = false
  }
}

// ── Modal open/close ──────────────────────────────────
function openCreate() {
  editingId.value = null
  hasVariations.value = false
  form.value = { 
    name:'', baseDescription:'', imageUrl:'', categoryId: null,
    tierVariations: [],
    variants: [{ variantId: null, variantName: 'Mặc định', sku: '', price: 0, stockQuantity: 0, tierValues: [] }]
  }
  imagePreview.value = ''; selectedFile.value = null; submitError.value = ''; uploadError.value = ''
  showModal.value = true
}

function openEdit(p) {
  editingId.value = p.productId
  let parsedTiers = [];
  try {
    if (p.tierVariationsJson) parsedTiers = JSON.parse(p.tierVariationsJson);
  } catch(e) {}
  
  form.value = {
    name: p.name,
    categoryId: p.categoryId || null,
    baseDescription: p.baseDescription || '',
    imageUrl: firstVariant(p)?.imageUrl || '',
    tierVariations: parsedTiers.map(t => ({ name: t.name || t.Name || '', values: t.values || t.Values || [], tempValue: '' })),
    variants: p.variants && p.variants.length > 0 
      ? JSON.parse(JSON.stringify(p.variants)).map(v => {
          let parsedTV = [];
          try { if (v.tierValuesJson) parsedTV = JSON.parse(v.tierValuesJson); } catch(e) {}
          return {...v, stockQuantity: v.stockQty ?? 0, tierValues: parsedTV};
        })
      : [{ variantId: null, variantName: 'Mặc định', sku: '', price: 0, stockQuantity: 0, tierValues: [] }]
  }
  hasVariations.value = parsedTiers.length > 0
  imagePreview.value = imageUrl(p)
  selectedFile.value = null; submitError.value = ''; uploadError.value = ''
  showModal.value = true
}

function closeModal() { showModal.value = false }

// ── Variations logic ──────────────────────────────────
function toggleVariations() {
  hasVariations.value = !hasVariations.value;
  if (hasVariations.value) {
    if (form.value.tierVariations.length === 0) {
      addTier();
    }
  } else {
    form.value.tierVariations = [];
    regenerateVariants();
  }
}
function addTier() {
  if (form.value.tierVariations.length < 2) {
    form.value.tierVariations.push({ name: '', values: [], tempValue: '' })
    regenerateVariants()
  }
}
function removeTier(idx) {
  form.value.tierVariations.splice(idx, 1)
  regenerateVariants()
}
function addTierValue(tIdx) {
  const tier = form.value.tierVariations[tIdx]
  const val = tier.tempValue.trim()
  if (val && !tier.values.includes(val)) {
    tier.values.push(val)
    regenerateVariants()
  }
  tier.tempValue = ''
}
function removeTierValue(tIdx, vIdx) {
  form.value.tierVariations[tIdx].values.splice(vIdx, 1)
  regenerateVariants()
}
function removeVariant(idx) {
  form.value.variants.splice(idx, 1)
}

const regenerateVariants = async () => {
  if (form.value.tierVariations.length === 0 || form.value.tierVariations.every(t => t.values.length === 0)) {
    const old = form.value.variants[0] || {};
    form.value.variants = [{ 
      variantId: old.variantId, 
      variantName: 'Mặc định', 
      sku: old.sku || '', 
      price: old.price || 0, 
      stockQuantity: old.stockQuantity || 0, 
      imageUrl: form.value.imageUrl,
      tierValues: [] 
    }];
  } else {
    const groups = form.value.tierVariations.map(t => t.values.length > 0 ? t.values : ['']);
    const combos = groups.reduce((a, b) => a.reduce((r, v) => r.concat(b.map(w => [].concat(v, w))), []));
    
    const newVars = combos.map(combo => {
      const comboArr = Array.isArray(combo) ? combo : [combo];
      const comboStr = comboArr.filter(c => c !== '').join(' - ');
      const old = form.value.variants.find(v => JSON.stringify(v.tierValues) === JSON.stringify(comboArr));
      return old ? { ...old, variantName: comboStr } : { 
        variantId: null, 
        variantName: comboStr, 
        sku: '', 
        price: form.value.variants[0]?.price || 0, 
        stockQuantity: 0, 
        imageUrl: form.value.imageUrl,
        tierValues: comboArr 
      };
    });
    form.value.variants = newVars;
  }

  if (form.value.name) {
    for (let i = 0; i < form.value.variants.length; i++) {
      let v = form.value.variants[i];
      if (!v.sku) {
        try {
          const vName = v.variantName === 'Mặc định' ? '' : v.variantName;
          const res = await fetch(`${API}/product/generate-sku?name=${encodeURIComponent(form.value.name)}&variantName=${encodeURIComponent(vName)}`);
          const json = await res.json();
          if (json.success && !form.value.variants[i].sku) {
             form.value.variants[i].sku = json.sku;
          }
        } catch (e) {
          console.error('Failed to generate SKU', e);
        }
      }
    }
  }
}

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
    const token = localStorage.getItem('token') || ''
    const headers = {
       ...(token ? { 'Authorization': `Bearer ${token}` } : {})
    };

    if (editingId.value) {
      await fetch(`${API}/product/${editingId.value}`, {
        method: 'PUT',
        headers: { ...headers, 'Content-Type': 'application/json' },
        body: JSON.stringify({
          name: form.value.name,
          categoryId: form.value.categoryId,
          baseDescription: form.value.baseDescription,
          imageUrl: form.value.imageUrl,
          tierVariations: form.value.tierVariations.map(t => ({ name: t.name, values: t.values })),
          variants: form.value.variants
        })
      })
      if (selectedFile.value) {
        const fd = new FormData()
        fd.append('image', selectedFile.value)
        await fetch(`${API}/product/${editingId.value}/upload-image`, { method: 'POST', headers, body: fd })
      }
    } else {
      const fd = new FormData()
      fd.append('Name', form.value.name)
      if (form.value.categoryId) fd.append('CategoryId', form.value.categoryId)
      fd.append('BaseDescription', form.value.baseDescription)
      
      form.value.tierVariations.forEach((t, i) => {
         fd.append(`TierVariations[${i}].Name`, t.name)
         t.values.forEach((v, j) => {
            fd.append(`TierVariations[${i}].Values[${j}]`, v)
         })
      })

      form.value.variants.forEach((v, i) => {
         fd.append(`Variants[${i}].VariantName`, v.variantName)
         fd.append(`Variants[${i}].Sku`, v.sku)
         fd.append(`Variants[${i}].Price`, v.price)
         fd.append(`Variants[${i}].StockQuantity`, v.stockQuantity)
         if (v.tierValues) {
            v.tierValues.forEach((tv, j) => {
                fd.append(`Variants[${i}].TierValues[${j}]`, tv)
            })
         }
      })
      
      if (selectedFile.value) fd.append('image', selectedFile.value)

      const res = await fetch(`${API}/product/with-image`, {
        method: 'POST',
        headers,
        body: fd
      })
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
    const token = localStorage.getItem('token') || ''
    const res = await fetch(`${API}/product/${deleteTarget.value.productId}`, { 
      method: 'DELETE',
      headers: { 'Authorization': `Bearer ${token}` }
    })
    
    if (!res.ok) {
      const data = await res.json()
      alert(data.message || 'Lỗi khi xóa sản phẩm');
    } else {
      const data = await res.json()
      if (!data.success) {
        alert(data.message || 'Lỗi khi xóa sản phẩm');
      } else {
        deleteTarget.value = null
        await fetchProducts()
      }
    }
  } catch (err) {
    alert('Đã xảy ra lỗi kết nối');
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
.modal { background: #fff; border-radius: 20px; width: 100%; max-width: 560px; max-height: 90vh; display: flex; flex-direction: column; overflow: hidden; box-shadow: 0 20px 60px rgba(0,0,0,.2); }
.modal--sm { max-width: 400px; }
.modal-header { display: flex; justify-content: space-between; align-items: center; padding: 1.5rem 1.5rem 1rem; flex-shrink: 0; border-bottom: 1px solid #f0e8e0; }
.modal-header h2 { font-size: 1.1rem; font-weight: 700; color: #2c2018; margin: 0; }
.modal-close { background: none; border: none; cursor: pointer; padding: 0.25rem; border-radius: 6px; display: flex; align-items: center; color: #8a7060; }
.modal-close:hover { background: #f5ede6; }
.modal-form { display: flex; flex-direction: column; flex: 1; overflow: hidden; padding: 0; }
.modal-body { flex: 1; overflow-y: auto; padding: 1.25rem 1.5rem; display: flex; flex-direction: column; gap: 1rem; }
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
.modal-footer { display: flex; justify-content: flex-end; gap: 0.75rem; padding: 1rem 1.5rem; border-top: 1px solid #f0e8e0; flex-shrink: 0; background: #fff; }
.btn-cancel { padding: 0.6rem 1.25rem; border: 1px solid #e0d8d0; background: #fff; color: #5a4a3a; border-radius: 8px; font-size: 0.875rem; font-weight: 600; cursor: pointer; }
.btn-save { padding: 0.6rem 1.5rem; background: #b36a3a; color: #fff; border: none; border-radius: 8px; font-size: 0.875rem; font-weight: 600; cursor: pointer; display: flex; align-items: center; gap: 0.4rem; }
.btn-save:disabled { opacity: 0.6; cursor: not-allowed; }
.btn-delete-confirm { padding: 0.6rem 1.25rem; background: #c0392b; color: #fff; border: none; border-radius: 8px; font-size: 0.875rem; font-weight: 600; cursor: pointer; }
.btn-delete-confirm:disabled { opacity: 0.6; }
</style>
