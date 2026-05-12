<template>
  <div class="um-layout">

    <!-- Sidebar -->
    <aside class="sidebar">
      <div class="sidebar-brand">
        <h2>Quản trị Bakery</h2>
        <p>Vận hành Hàng ngày</p>
      </div>
      <nav class="sidebar-nav">
        <router-link to="/dashboard" class="nav-item">
          <span class="material-symbols-outlined">dashboard</span><span>Bảng điều khiển</span>
        </router-link>
        <router-link to="/payments" class="nav-item">
          <span class="material-symbols-outlined">receipt_long</span><span>Lịch sử Hóa đơn</span>
        </router-link>
        <router-link to="/users" class="nav-item nav-item--active">
          <span class="material-symbols-outlined">group</span><span>Quản lý Người dùng</span>
        </router-link>
        <router-link to="/inventory" class="nav-item">
          <span class="material-symbols-outlined">inventory_2</span><span>Quản lý Sản phẩm</span>
        </router-link>
      </nav>
    </aside>

    <!-- Main -->
    <main class="um-main">
      <!-- Header -->
      <header class="topbar">
        <div>
          <h1>Quản lý Người dùng</h1>
          <p class="topbar-sub">Xem lại và quản lý tài khoản nhân viên &amp; khách hàng.</p>
        </div>
        <div class="topbar-actions">
          <div class="search-wrap">
            <span class="material-symbols-outlined">search</span>
            <input v-model="searchQuery" placeholder="Tìm theo tên, email..." />
          </div>
          <button class="btn-primary" @click="openCreate">
            <span class="material-symbols-outlined">person_add</span> Thêm người dùng
          </button>
        </div>
      </header>

      <!-- Stats -->
      <div class="stats-row">
        <div class="stat-card">
          <div class="stat-icon"><span class="material-symbols-outlined">group</span></div>
          <div>
            <div class="stat-label">Tổng người dùng</div>
            <div class="stat-value">{{ users.length }}</div>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon stat-icon--admin"><span class="material-symbols-outlined">shield</span></div>
          <div>
            <div class="stat-label">Admin</div>
            <div class="stat-value">{{ countByRole('admin') }}</div>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon stat-icon--staff"><span class="material-symbols-outlined">badge</span></div>
          <div>
            <div class="stat-label">Nhân viên</div>
            <div class="stat-value">{{ countByRole('staff') }}</div>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon stat-icon--cust"><span class="material-symbols-outlined">person</span></div>
          <div>
            <div class="stat-label">Khách hàng</div>
            <div class="stat-value">{{ countByRole('customer') }}</div>
          </div>
        </div>
      </div>

      <!-- Filter tabs -->
      <div class="filter-tabs">
        <button
          v-for="t in tabs" :key="t.value"
          :class="['tab-btn', activeTab === t.value && 'tab-btn--active']"
          @click="activeTab = t.value"
        >{{ t.label }}</button>
      </div>

      <!-- Loading / Error -->
      <div v-if="loading" class="state-msg">Đang tải...</div>
      <div v-else-if="fetchError" class="state-msg state-msg--error">{{ fetchError }}</div>

      <!-- Table -->
      <div v-else class="table-card">
        <table class="user-table">
          <thead>
            <tr>
              <th>Người dùng</th>
              <th>Email</th>
              <th>Vai trò</th>
              <th>Trạng thái</th>
              <th class="text-right">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="u in filteredUsers" :key="u.userId" class="user-row">
              <td>
                <div class="user-cell">
                  <div class="avatar" :style="avatarStyle(u)">{{ initials(u.fullName) }}</div>
                  <div>
                    <div class="user-name">{{ u.fullName || '—' }}</div>
                    <div class="user-id">ID #{{ u.userId }}</div>
                  </div>
                </div>
              </td>
              <td class="td-muted">{{ u.email }}</td>
              <td><span :class="['role-badge', `role-badge--${u.role}`]">{{ roleLabel(u.role) }}</span></td>
              <td>
                <button class="status-toggle" :class="u.isActive ? 'status-toggle--on' : 'status-toggle--off'" @click="toggleActive(u)">
                  <span class="material-symbols-outlined">{{ u.isActive ? 'check_circle' : 'cancel' }}</span>
                  {{ u.isActive ? 'Hoạt động' : 'Bị khóa' }}
                </button>
              </td>
              <td class="text-right">
                <div class="action-btns">
                  <button class="btn-icon btn-icon--edit" title="Sửa" @click="openEdit(u)">
                    <span class="material-symbols-outlined">edit</span>
                  </button>
                  <button class="btn-icon btn-icon--del" title="Xóa" @click="confirmDelete(u)">
                    <span class="material-symbols-outlined">delete</span>
                  </button>
                </div>
              </td>
            </tr>
            <tr v-if="filteredUsers.length === 0">
              <td colspan="5" class="empty-row">
                <span class="material-symbols-outlined">search_off</span>
                <p>Không có người dùng nào</p>
              </td>
            </tr>
          </tbody>
        </table>

        <div class="table-footer">
          Hiển thị {{ filteredUsers.length }} / {{ users.length }} người dùng
        </div>
      </div>
    </main>

    <!-- ── MODAL: Create / Edit ── -->
    <div v-if="showModal" class="modal-backdrop" @click.self="closeModal">
      <div class="modal">
        <div class="modal-header">
          <h2>{{ editingId ? 'Sửa người dùng' : 'Thêm người dùng mới' }}</h2>
          <button class="modal-close" @click="closeModal"><span class="material-symbols-outlined">close</span></button>
        </div>
        <form class="modal-form" @submit.prevent="submitForm">
          <div class="form-row">
            <div class="field">
              <label>Họ tên *</label>
              <input v-model="form.fullName" required placeholder="Nguyễn Văn A" />
            </div>
            <div class="field">
              <label>Số điện thoại</label>
              <input v-model="form.phone" placeholder="0901234567" />
            </div>
          </div>

          <div v-if="!editingId" class="field">
            <label>Email *</label>
            <input v-model="form.email" type="email" required placeholder="example@email.com" />
          </div>

          <div v-if="!editingId" class="field">
            <label>Mật khẩu *</label>
            <input v-model="form.password" type="password" required placeholder="••••••••" />
          </div>

          <div class="form-row">
            <div class="field">
              <label>Vai trò</label>
              <select v-model.number="form.roleId">
                <option :value="1">Admin</option>
                <option :value="2">Nhân viên (Staff)</option>
                <option :value="3">Khách hàng</option>
              </select>
            </div>
            <div class="field" v-if="editingId">
              <label>Trạng thái</label>
              <select v-model="form.isActive">
                <option :value="true">Hoạt động</option>
                <option :value="false">Bị khóa</option>
              </select>
            </div>
          </div>

          <p v-if="submitError" class="field-error">{{ submitError }}</p>

          <div class="modal-footer">
            <button type="button" class="btn-cancel" @click="closeModal">Hủy</button>
            <button type="submit" class="btn-save" :disabled="saving">
              <span v-if="saving" class="spin material-symbols-outlined">refresh</span>
              {{ saving ? 'Đang lưu...' : (editingId ? 'Lưu thay đổi' : 'Tạo tài khoản') }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- ── MODAL: Confirm Delete ── -->
    <div v-if="deleteTarget" class="modal-backdrop" @click.self="deleteTarget = null">
      <div class="modal modal--sm">
        <div class="modal-header">
          <h2>Xác nhận xóa</h2>
          <button class="modal-close" @click="deleteTarget = null"><span class="material-symbols-outlined">close</span></button>
        </div>
        <p class="delete-msg">
          Bạn chắc chắn muốn xóa tài khoản của <strong>{{ deleteTarget.fullName || deleteTarget.email }}</strong>?
          Hành động này không thể hoàn tác.
        </p>
        <div class="modal-footer">
          <button class="btn-cancel" @click="deleteTarget = null">Hủy</button>
          <button class="btn-danger" :disabled="saving" @click="doDelete">
            {{ saving ? 'Đang xóa...' : 'Xóa tài khoản' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

const API = 'http://localhost:5072/api/user'

// ── State ─────────────────────────────────────────────
const users       = ref([])
const loading     = ref(false)
const fetchError  = ref('')
const searchQuery = ref('')
const activeTab   = ref('all')
const showModal   = ref(false)
const saving      = ref(false)
const editingId   = ref(null)
const deleteTarget = ref(null)
const submitError  = ref('')

const tabs = [
  { label: 'Tất cả', value: 'all' },
  { label: 'Admin', value: 'admin' },
  { label: 'Nhân viên', value: 'staff' },
  { label: 'Khách hàng', value: 'customer' },
]

const form = ref({ fullName: '', phone: '', email: '', password: '', roleId: 3, isActive: true })

// ── Computed ──────────────────────────────────────────
const filteredUsers = computed(() => {
  const q = searchQuery.value.toLowerCase()
  return users.value.filter(u => {
    const matchTab  = activeTab.value === 'all' || u.role === activeTab.value
    const matchSearch = !q || u.fullName?.toLowerCase().includes(q) || u.email?.toLowerCase().includes(q)
    return matchTab && matchSearch
  })
})

const countByRole = role => users.value.filter(u => u.role === role).length

// ── Helpers ───────────────────────────────────────────
const roleLabel = r => ({ admin: 'Quản trị viên', staff: 'Nhân viên', customer: 'Khách hàng' }[r] ?? r)

const initials = name => name ? name.split(' ').map(w => w[0]).slice(-2).join('').toUpperCase() : '?'

const AVATAR_COLORS = ['#b36a3a','#5b7e6b','#7c6ea0','#3a7ab3','#a03a6a']
const avatarStyle = u => ({
  background: AVATAR_COLORS[u.userId % AVATAR_COLORS.length],
  color: '#fff'
})

// ── Fetch ─────────────────────────────────────────────
async function fetchUsers() {
  loading.value = true; fetchError.value = ''
  try {
    const res  = await fetch(API)
    const json = await res.json()
    if (json.success) users.value = json.data
    else fetchError.value = 'Không tải được dữ liệu.'
  } catch {
    fetchError.value = 'Lỗi kết nối tới server.'
  } finally {
    loading.value = false
  }
}

// ── Modal ─────────────────────────────────────────────
function openCreate() {
  editingId.value = null
  form.value = { fullName: '', phone: '', email: '', password: '', roleId: 3, isActive: true }
  submitError.value = ''; showModal.value = true
}

function openEdit(u) {
  editingId.value = u.userId
  form.value = { fullName: u.fullName || '', phone: u.phone || '', email: u.email, password: '', roleId: u.roleId ?? 3, isActive: u.isActive ?? true }
  submitError.value = ''; showModal.value = true
}

function closeModal() { showModal.value = false }

// ── Submit ────────────────────────────────────────────
async function submitForm() {
  saving.value = true; submitError.value = ''
  try {
    if (editingId.value) {
      const res = await fetch(`${API}/${editingId.value}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ fullName: form.value.fullName, phone: form.value.phone, roleId: form.value.roleId, isActive: form.value.isActive })
      })
      const json = await res.json()
      if (!json.success) throw new Error(json.message)
    } else {
      const res = await fetch(`${API}/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ name: form.value.fullName, email: form.value.email, password: form.value.password })
      })
      const json = await res.json()
      if (!json.success) throw new Error(json.message)
    }
    closeModal(); await fetchUsers()
  } catch (err) {
    submitError.value = err.message || 'Đã xảy ra lỗi'
  } finally {
    saving.value = false
  }
}

// ── Toggle active ─────────────────────────────────────
async function toggleActive(u) {
  try {
    await fetch(`${API}/${u.userId}/toggle-active`, { method: 'PATCH' })
    u.isActive = !u.isActive
  } catch { /* silent */ }
}

// ── Delete ────────────────────────────────────────────
function confirmDelete(u) { deleteTarget.value = u }

async function doDelete() {
  if (!deleteTarget.value) return
  saving.value = true
  try {
    await fetch(`${API}/${deleteTarget.value.userId}`, { method: 'DELETE' })
    deleteTarget.value = null; await fetchUsers()
  } finally { saving.value = false }
}

onMounted(fetchUsers)
</script>

<style scoped>
.material-symbols-outlined { font-family: 'Material Symbols Outlined'; font-size: 20px; line-height: 1; }
.spin { display: inline-block; animation: spin 1s linear infinite; }
@keyframes spin { to { transform: rotate(360deg); } }
.text-right { text-align: right; }

/* Layout */
.um-layout { display: flex; min-height: 100vh; background: #faf8f5; font-family: 'Inter', sans-serif; }

/* Sidebar */
.sidebar { width: 256px; min-height: 100vh; background: #fff; border-right: 1px solid #e8e0d8; display: flex; flex-direction: column; padding: 1.5rem 1rem; gap: .5rem; position: sticky; top: 0; height: 100vh; }
.sidebar-brand h2 { font-size: 1.05rem; font-weight: 700; color: #b36a3a; margin: 0 0 .2rem; }
.sidebar-brand p  { font-size: .75rem; color: #8a7060; margin: 0 0 1.2rem; }
.sidebar-nav { display: flex; flex-direction: column; gap: .25rem; }
.nav-item { display: flex; align-items: center; gap: .75rem; padding: .65rem 1rem; border-radius: 10px; text-decoration: none; color: #6b5c50; font-size: .875rem; transition: background .15s; }
.nav-item:hover    { background: #f5ede6; color: #b36a3a; }
.nav-item--active  { background: #f5ede6; color: #b36a3a; font-weight: 600; }

/* Main */
.um-main { flex: 1; display: flex; flex-direction: column; }

/* Topbar */
.topbar { background: #fff; border-bottom: 1px solid #e8e0d8; padding: 1.25rem 2rem; display: flex; align-items: flex-start; justify-content: space-between; gap: 1rem; flex-wrap: wrap; position: sticky; top: 0; z-index: 30; }
.topbar h1 { font-size: 1.4rem; font-weight: 700; color: #b36a3a; margin: 0 0 .2rem; }
.topbar-sub { font-size: .85rem; color: #8a7060; margin: 0; }
.topbar-actions { display: flex; align-items: center; gap: .75rem; flex-wrap: wrap; }
.search-wrap { position: relative; }
.search-wrap .material-symbols-outlined { position: absolute; left: .75rem; top: 50%; transform: translateY(-50%); color: #aaa; font-size: 18px; }
.search-wrap input { padding: .55rem .75rem .55rem 2.4rem; border: 1px solid #e0d8d0; border-radius: 8px; font-size: .875rem; background: #faf8f5; width: 220px; outline: none; }
.search-wrap input:focus { border-color: #b36a3a; }
.btn-primary { display: flex; align-items: center; gap: .4rem; padding: .6rem 1.25rem; background: #b36a3a; color: #fff; border: none; border-radius: 8px; font-size: .875rem; font-weight: 600; cursor: pointer; white-space: nowrap; }
.btn-primary:hover { opacity: .9; }

/* Stats */
.stats-row { display: flex; gap: 1rem; padding: 1.25rem 2rem 0; flex-wrap: wrap; }
.stat-card { background: #fff; border: 1px solid #e8e0d8; border-radius: 12px; padding: .9rem 1.25rem; display: flex; align-items: center; gap: 1rem; min-width: 150px; }
.stat-icon { width: 40px; height: 40px; border-radius: 10px; background: #f5ede6; display: flex; align-items: center; justify-content: center; color: #b36a3a; }
.stat-icon--admin { background: #ede6f5; color: #7c6ea0; }
.stat-icon--staff { background: #e6f5ed; color: #5b7e6b; }
.stat-icon--cust  { background: #e6ecf5; color: #3a7ab3; }
.stat-label { font-size: .7rem; font-weight: 600; color: #8a7060; text-transform: uppercase; letter-spacing: .04em; }
.stat-value { font-size: 1.6rem; font-weight: 700; color: #b36a3a; }

/* Filter tabs */
.filter-tabs { display: flex; gap: .5rem; padding: 1.25rem 2rem 0; }
.tab-btn { padding: .45rem 1.1rem; border-radius: 20px; border: 1px solid #e0d8d0; background: #fff; color: #8a7060; font-size: .8rem; font-weight: 600; cursor: pointer; transition: all .15s; }
.tab-btn:hover    { border-color: #b36a3a; color: #b36a3a; }
.tab-btn--active  { background: #b36a3a; color: #fff; border-color: #b36a3a; }

/* State */
.state-msg { text-align: center; padding: 3rem; color: #8a7060; }
.state-msg--error { color: #c0392b; }

/* Table */
.table-card { margin: 1.25rem 2rem 2rem; background: #fff; border: 1px solid #e8e0d8; border-radius: 16px; overflow: hidden; }
.user-table { width: 100%; border-collapse: collapse; font-size: .875rem; }
.user-table th { padding: .9rem 1.25rem; font-size: .7rem; font-weight: 700; text-transform: uppercase; letter-spacing: .05em; color: #8a7060; background: #faf8f5; border-bottom: 1px solid #e8e0d8; }
.user-table td { padding: .9rem 1.25rem; border-bottom: 1px solid #f0e8e0; vertical-align: middle; }
.user-row:last-child td { border-bottom: none; }
.user-row:hover { background: #fdf9f6; }
.user-cell { display: flex; align-items: center; gap: .75rem; }
.avatar { width: 38px; height: 38px; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-weight: 700; font-size: .85rem; flex-shrink: 0; }
.user-name { font-weight: 600; color: #2c2018; }
.user-id   { font-size: .72rem; color: #b0a090; }
.td-muted  { color: #6b5c50; }

/* Role badge */
.role-badge { padding: 3px 10px; border-radius: 20px; font-size: .72rem; font-weight: 700; }
.role-badge--admin    { background: #ede6f5; color: #5b4a8a; }
.role-badge--staff    { background: #e6f5ed; color: #2e6649; }
.role-badge--customer { background: #f5ede6; color: #8a4020; }

/* Status toggle */
.status-toggle { display: flex; align-items: center; gap: .3rem; padding: 4px 10px; border-radius: 20px; border: none; font-size: .75rem; font-weight: 600; cursor: pointer; }
.status-toggle--on  { background: #e6f5ed; color: #2e6649; }
.status-toggle--off { background: #f5e6e6; color: #862020; }

/* Action buttons */
.action-btns { display: flex; gap: .4rem; justify-content: flex-end; }
.btn-icon { display: flex; align-items: center; justify-content: center; width: 32px; height: 32px; border-radius: 8px; border: none; cursor: pointer; }
.btn-icon--edit { background: #f5ede6; color: #b36a3a; }
.btn-icon--edit:hover { background: #e8d8c8; }
.btn-icon--del  { background: #f5e6e6; color: #c0392b; }
.btn-icon--del:hover  { background: #e8c8c8; }

/* Empty row */
.empty-row { text-align: center; padding: 3rem; color: #b0a090; }
.empty-row .material-symbols-outlined { font-size: 40px; display: block; margin-bottom: .5rem; }

/* Table footer */
.table-footer { padding: .75rem 1.25rem; border-top: 1px solid #f0e8e0; font-size: .8rem; color: #8a7060; }

/* Modal */
.modal-backdrop { position: fixed; inset: 0; background: rgba(0,0,0,.45); backdrop-filter: blur(3px); display: flex; align-items: center; justify-content: center; z-index: 100; padding: 1rem; }
.modal { background: #fff; border-radius: 20px; width: 100%; max-width: 520px; max-height: 90vh; overflow-y: auto; box-shadow: 0 20px 60px rgba(0,0,0,.2); }
.modal--sm { max-width: 400px; }
.modal-header { display: flex; justify-content: space-between; align-items: center; padding: 1.5rem 1.5rem 0; }
.modal-header h2 { font-size: 1.1rem; font-weight: 700; color: #2c2018; margin: 0; }
.modal-close { background: none; border: none; cursor: pointer; padding: .25rem; border-radius: 6px; display: flex; align-items: center; color: #8a7060; }
.modal-close:hover { background: #f5ede6; }
.modal-form { padding: 1.25rem 1.5rem; display: flex; flex-direction: column; gap: 1rem; }
.form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; }
.field { display: flex; flex-direction: column; gap: .35rem; }
.field label { font-size: .8rem; font-weight: 600; color: #5a4a3a; }
.field input, .field select { padding: .6rem .85rem; border: 1px solid #e0d8d0; border-radius: 8px; font-size: .875rem; outline: none; background: #faf8f5; font-family: inherit; }
.field input:focus, .field select:focus { border-color: #b36a3a; box-shadow: 0 0 0 3px rgba(179,106,58,.1); }
.field-error { color: #c0392b; font-size: .8rem; margin: 0; }
.modal-footer { display: flex; justify-content: flex-end; gap: .75rem; padding: 1rem 1.5rem 1.5rem; border-top: 1px solid #f0e8e0; }
.btn-cancel { padding: .6rem 1.25rem; border: 1px solid #e0d8d0; background: #fff; color: #5a4a3a; border-radius: 8px; font-size: .875rem; font-weight: 600; cursor: pointer; }
.btn-save   { padding: .6rem 1.5rem; background: #b36a3a; color: #fff; border: none; border-radius: 8px; font-size: .875rem; font-weight: 600; cursor: pointer; display: flex; align-items: center; gap: .4rem; }
.btn-save:disabled   { opacity: .6; cursor: not-allowed; }
.btn-danger { padding: .6rem 1.25rem; background: #c0392b; color: #fff; border: none; border-radius: 8px; font-size: .875rem; font-weight: 600; cursor: pointer; }
.btn-danger:disabled { opacity: .6; }
.delete-msg { padding: .5rem 1.5rem 1rem; color: #5a4a3a; font-size: .9rem; line-height: 1.6; }
</style>
