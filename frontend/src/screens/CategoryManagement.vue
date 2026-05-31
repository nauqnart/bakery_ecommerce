<template>
  <div class="um-layout">

    <!-- Sidebar -->
    <AdminSidebar />

    <!-- Main -->
    <main class="um-main">
      <!-- Header -->
      <header class="topbar">
        <div>
          <h1>Quản lý Danh mục</h1>
          <p class="topbar-sub">Xem lại và quản lý danh mục sản phẩm.</p>
        </div>
        <div class="topbar-actions">
          <div class="search-wrap">
            <span class="material-symbols-outlined">search</span>
            <input v-model="searchQuery" placeholder="Tìm theo tên..." />
          </div>
          <button class="btn-primary" @click="openCreate">
            <span class="material-symbols-outlined">add_circle</span> Thêm danh mục
          </button>
        </div>
      </header>

      <!-- Stats -->
      <div class="stats-row">
        <div class="stat-card">
          <div class="stat-icon"><span class="material-symbols-outlined">category</span></div>
          <div>
            <div class="stat-label">Tổng danh mục</div>
            <div class="stat-value">{{ categories.length }}</div>
          </div>
        </div>
      </div>

      <!-- Loading / Error -->
      <div v-if="loading" class="state-msg">Đang tải...</div>
      <div v-else-if="fetchError" class="state-msg state-msg--error">{{ fetchError }}</div>

      <!-- Table -->
      <div v-else class="table-card">
        <table class="user-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Tên danh mục</th>
              <th>Mô tả</th>
              <th class="text-right">Sản phẩm</th>
              <th class="text-right">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="c in filteredCategories" :key="c.categoryId" class="user-row">
              <td class="td-muted">#{{ c.categoryId }}</td>
              <td><div class="user-name">{{ c.name }}</div></td>
              <td class="td-muted" style="max-width: 250px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;">{{ c.description || '—' }}</td>
              <td class="text-right">{{ c.productCount }}</td>
              <td class="text-right">
                <div class="action-btns">
                  <button class="btn-icon btn-icon--edit" title="Sửa" @click="openEdit(c)">
                    <span class="material-symbols-outlined">edit</span>
                  </button>
                  <button class="btn-icon btn-icon--del" title="Xóa" @click="confirmDelete(c)">
                    <span class="material-symbols-outlined">delete</span>
                  </button>
                </div>
              </td>
            </tr>
            <tr v-if="filteredCategories.length === 0">
              <td colspan="5" class="empty-row">
                <span class="material-symbols-outlined">search_off</span>
                <p>Không có danh mục nào</p>
              </td>
            </tr>
          </tbody>
        </table>

        <div class="table-footer">
          Hiển thị {{ filteredCategories.length }} / {{ categories.length }} danh mục
        </div>
      </div>
    </main>

    <!-- ── MODAL: Create / Edit ── -->
    <div v-if="showModal" class="modal-backdrop" @click.self="closeModal">
      <div class="modal">
        <div class="modal-header">
          <h2>{{ editingId ? 'Sửa danh mục' : 'Thêm danh mục mới' }}</h2>
          <button class="modal-close" @click="closeModal"><span class="material-symbols-outlined">close</span></button>
        </div>
        <form class="modal-form" @submit.prevent="submitForm">
          <div class="field">
            <label>Tên danh mục *</label>
            <input v-model="form.name" required placeholder="Nhập tên danh mục..." />
          </div>

          <div class="field">
            <label>Mô tả</label>
            <textarea v-model="form.description" rows="3" placeholder="Nhập mô tả danh mục..."></textarea>
          </div>

          <p v-if="submitError" class="field-error">{{ submitError }}</p>

          <div class="modal-footer">
            <button type="button" class="btn-cancel" @click="closeModal">Hủy</button>
            <button type="submit" class="btn-save" :disabled="saving">
              <span v-if="saving" class="material-symbols-outlined spin">refresh</span>
              {{ saving ? 'Đang lưu...' : (editingId ? 'Lưu thay đổi' : 'Tạo danh mục') }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- ── MODAL: Delete ── -->
    <div v-if="deleteTarget" class="modal-backdrop" @click.self="deleteTarget = null">
      <div class="modal modal--sm">
        <div class="modal-header">
          <h2>Xóa danh mục</h2>
          <button class="modal-close" @click="deleteTarget = null"><span class="material-symbols-outlined">close</span></button>
        </div>
        <div style="padding: 1rem 1.5rem;">
          <p>Bạn chắc chắn muốn xóa danh mục <strong>{{ deleteTarget.name }}</strong>?</p>
          <p class="field-error" style="margin-top:0.5rem;" v-if="deleteTarget.productCount > 0">Không thể xóa vì danh mục đang chứa {{ deleteTarget.productCount }} sản phẩm.</p>
        </div>
        <div class="modal-footer">
          <button class="btn-cancel" @click="deleteTarget = null">Hủy</button>
          <button class="btn-delete-confirm" :disabled="saving || deleteTarget.productCount > 0" @click="doDelete">
            {{ saving ? 'Đang xóa...' : 'Xóa' }}
          </button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import AdminSidebar from '../components/AdminSidebar.vue'

const API = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5072/api'

const categories = ref([])
const loading = ref(true)
const fetchError = ref('')
const searchQuery = ref('')

const showModal = ref(false)
const editingId = ref(null)
const form = ref({ name: '', description: '' })
const saving = ref(false)
const submitError = ref('')

const deleteTarget = ref(null)

const filteredCategories = computed(() => {
  const q = searchQuery.value.toLowerCase()
  return categories.value.filter(c => c.name?.toLowerCase().includes(q))
})

onMounted(fetchCategories)

async function fetchCategories() {
  loading.value = true; fetchError.value = ''
  try {
    const res = await fetch(`${API}/category`)
    if (!res.ok) throw new Error('Không thể tải danh sách danh mục')
    const json = await res.json()
    categories.value = json.data || []
  } catch (e) {
    fetchError.value = e.message
  } finally {
    loading.value = false
  }
}

function openCreate() {
  editingId.value = null
  form.value = { name: '', description: '' }
  submitError.value = ''
  showModal.value = true
}

function openEdit(c) {
  editingId.value = c.categoryId
  form.value = { name: c.name, description: c.description || '' }
  submitError.value = ''
  showModal.value = true
}

function closeModal() {
  showModal.value = false
}

async function submitForm() {
  saving.value = true
  submitError.value = ''
  try {
    const token = localStorage.getItem('token')
    const url = editingId.value ? `${API}/category/${editingId.value}` : `${API}/category`
    const method = editingId.value ? 'PUT' : 'POST'
    const res = await fetch(url, {
      method,
      headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
      body: JSON.stringify({ name: form.value.name, description: form.value.description })
    })
    const json = await res.json()
    if (!res.ok || !json.success) throw new Error(json.message || 'Lỗi khi lưu danh mục')
    
    await fetchCategories()
    closeModal()
  } catch (e) {
    submitError.value = e.message
  } finally {
    saving.value = false
  }
}

function confirmDelete(c) {
  deleteTarget.value = c
}

async function doDelete() {
  if (!deleteTarget.value) return
  saving.value = true
  try {
    const token = localStorage.getItem('token')
    const res = await fetch(`${API}/category/${deleteTarget.value.categoryId}`, {
      method: 'DELETE',
      headers: { 'Authorization': `Bearer ${token}` }
    })
    const json = await res.json()
    if (!res.ok || !json.success) throw new Error(json.message || 'Xóa thất bại')
    
    await fetchCategories()
    deleteTarget.value = null
  } catch (e) {
    alert(e.message)
  } finally {
    saving.value = false
  }
}
</script>

<style scoped>
.um-layout { display: flex; min-height: 100vh; background: #fcf9f8; }
.um-main { flex: 1; padding: 2rem; display: flex; flex-direction: column; gap: 1.5rem; overflow-y: auto; }
.topbar { display: flex; justify-content: space-between; align-items: flex-start; gap: 1rem; flex-wrap: wrap; }
.topbar h1 { font-size: 1.75rem; font-weight: 800; color: #2c2018; margin: 0 0 0.5rem; letter-spacing: -0.02em; }
.topbar-sub { color: #8a7060; margin: 0; font-size: 0.95rem; }
.topbar-actions { display: flex; gap: 1rem; align-items: center; }
.search-wrap { display: flex; align-items: center; gap: 0.5rem; background: #fff; border: 1px solid #e0d8d0; padding: 0.5rem 1rem; border-radius: 99px; transition: border-color 0.2s; box-shadow: 0 2px 8px rgba(0,0,0,0.02); }
.search-wrap:focus-within { border-color: #b36a3a; box-shadow: 0 0 0 3px rgba(179,106,58,0.1); }
.search-wrap .material-symbols-outlined { color: #a09080; font-size: 20px; }
.search-wrap input { border: none; background: transparent; outline: none; font-size: 0.9rem; color: #2c2018; min-width: 200px; font-family: inherit; }
.btn-primary { background: #b36a3a; color: #fff; border: none; padding: 0.65rem 1.25rem; border-radius: 99px; font-weight: 600; cursor: pointer; display: flex; align-items: center; gap: 0.5rem; transition: background 0.2s; font-size: 0.9rem; }
.btn-primary:hover { background: #96562d; }

.stats-row { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 1rem; }
.stat-card { background: #fff; padding: 1.25rem; border-radius: 16px; border: 1px solid #f0e8e0; display: flex; align-items: center; gap: 1rem; box-shadow: 0 2px 10px rgba(0,0,0,0.02); }
.stat-icon { width: 48px; height: 48px; border-radius: 12px; background: #f5ede6; color: #b36a3a; display: flex; align-items: center; justify-content: center; }
.stat-label { font-size: 0.8rem; font-weight: 600; color: #8a7060; text-transform: uppercase; letter-spacing: 0.05em; margin-bottom: 0.25rem; }
.stat-value { font-size: 1.5rem; font-weight: 800; color: #2c2018; line-height: 1; }

.table-card { background: #fff; border-radius: 16px; border: 1px solid #f0e8e0; overflow: hidden; box-shadow: 0 2px 15px rgba(0,0,0,0.02); display: flex; flex-direction: column; }
.user-table { width: 100%; border-collapse: collapse; text-align: left; }
.user-table th { background: #faf8f5; font-weight: 600; font-size: 0.8rem; text-transform: uppercase; letter-spacing: 0.05em; color: #8a7060; padding: 1rem 1.5rem; border-bottom: 2px solid #f0e8e0; }
.user-table td { padding: 1rem 1.5rem; border-bottom: 1px solid #f0e8e0; vertical-align: middle; }
.user-row:hover td { background: #fdfbf9; }
.td-muted { color: #8a7060; font-size: 0.9rem; }
.text-right { text-align: right; }
.user-name { font-weight: 600; color: #2c2018; font-size: 0.95rem; margin-bottom: 0.15rem; }
.action-btns { display: flex; gap: 0.5rem; justify-content: flex-end; }
.btn-icon { width: 32px; height: 32px; border-radius: 8px; border: none; cursor: pointer; display: flex; align-items: center; justify-content: center; transition: all 0.2s; background: transparent; }
.btn-icon .material-symbols-outlined { font-size: 18px; }
.btn-icon--edit { color: #3498db; }
.btn-icon--edit:hover { background: rgba(52, 152, 219, 0.1); }
.btn-icon--del { color: #e74c3c; }
.btn-icon--del:hover { background: rgba(231, 76, 60, 0.1); }
.empty-row { text-align: center; padding: 3rem !important; color: #8a7060; }
.empty-row .material-symbols-outlined { font-size: 48px; opacity: 0.5; margin-bottom: 1rem; }
.table-footer { padding: 1rem 1.5rem; font-size: 0.85rem; color: #8a7060; border-top: 1px solid #f0e8e0; background: #faf8f5; text-align: center; }

/* MODAL */
.modal-backdrop { position: fixed; inset: 0; background: rgba(0,0,0,0.5); backdrop-filter: blur(4px); display: flex; align-items: center; justify-content: center; z-index: 100; padding: 1rem; }
.modal { background: #fff; border-radius: 20px; width: 100%; max-width: 500px; box-shadow: 0 20px 60px rgba(0,0,0,0.2); overflow: hidden; }
.modal--sm { max-width: 400px; }
.modal-header { display: flex; justify-content: space-between; align-items: center; padding: 1.5rem 1.5rem 1rem; border-bottom: 1px solid #f0e8e0; }
.modal-header h2 { font-size: 1.25rem; font-weight: 700; color: #2c2018; margin: 0; }
.modal-close { background: none; border: none; cursor: pointer; padding: 0.25rem; border-radius: 8px; color: #8a7060; display: flex; transition: background 0.2s; }
.modal-close:hover { background: #f5ede6; }
.modal-form { padding: 1.5rem; display: flex; flex-direction: column; gap: 1rem; }
.field { display: flex; flex-direction: column; gap: 0.4rem; }
.field label { font-size: 0.85rem; font-weight: 600; color: #5a4a3a; }
.field input, .field select, .field textarea { padding: 0.75rem 1rem; border: 1px solid #e0d8d0; border-radius: 10px; font-size: 0.95rem; outline: none; font-family: inherit; transition: border-color 0.2s; background: #faf8f5; }
.field input:focus, .field select:focus, .field textarea:focus { border-color: #b36a3a; box-shadow: 0 0 0 3px rgba(179,106,58,0.1); background: #fff; }
.field-error { color: #e74c3c; font-size: 0.85rem; margin: 0; }
.modal-footer { display: flex; justify-content: flex-end; gap: 0.75rem; padding: 1.5rem; border-top: 1px solid #f0e8e0; background: #faf8f5; }
.btn-cancel { padding: 0.65rem 1.25rem; border: 1px solid #e0d8d0; background: #fff; color: #5a4a3a; border-radius: 10px; font-size: 0.95rem; font-weight: 600; cursor: pointer; transition: all 0.2s; }
.btn-cancel:hover { background: #f5ede6; }
.btn-save { padding: 0.65rem 1.5rem; background: #b36a3a; color: #fff; border: none; border-radius: 10px; font-size: 0.95rem; font-weight: 600; cursor: pointer; display: flex; align-items: center; gap: 0.5rem; transition: background 0.2s; }
.btn-save:hover:not(:disabled) { background: #96562d; }
.btn-save:disabled { opacity: 0.6; cursor: not-allowed; }
.btn-delete-confirm { padding: 0.65rem 1.5rem; background: #e74c3c; color: #fff; border: none; border-radius: 10px; font-size: 0.95rem; font-weight: 600; cursor: pointer; }
.btn-delete-confirm:disabled { opacity: 0.6; cursor: not-allowed; }
.spin { animation: spin 1s linear infinite; }
@keyframes spin { 100% { transform: rotate(360deg); } }
.state-msg { text-align: center; padding: 3rem; color: #8a7060; font-size: 1.1rem; }
.state-msg--error { color: #e74c3c; }
</style>
