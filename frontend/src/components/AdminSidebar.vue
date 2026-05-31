<template>
    <aside class="sidebar">
      <div class="sidebar-brand">
        <h2>Quản trị Bakery</h2>
        <p>Hệ thống vận hành</p>
      </div>
      <nav class="sidebar-nav">
        <router-link to="/dashboard" class="nav-item" active-class="nav-item--active">
          <span class="material-symbols-outlined">dashboard</span>
          <span>Bảng điều khiển</span>
        </router-link>

        <router-link to="/payments" class="nav-item" active-class="nav-item--active">
          <span class="material-symbols-outlined">receipt_long</span>
          <span>Quản lý Đơn hàng</span>
        </router-link>

        <router-link to="/inventory" class="nav-item" active-class="nav-item--active">
          <span class="material-symbols-outlined">inventory_2</span>
          <span>Quản lý Sản phẩm</span>
        </router-link>

        <router-link to="/categories" class="nav-item" active-class="nav-item--active">
          <span class="material-symbols-outlined">category</span>
          <span>Quản lý Danh mục</span>
        </router-link>

        <router-link v-if="isAdmin" to="/users" class="nav-item" active-class="nav-item--active">
          <span class="material-symbols-outlined">group</span>
          <span>Quản lý Người dùng</span>
        </router-link>

      </nav>
      <button class="nav-item btn-logout" @click="logout">
        <span class="material-symbols-outlined">logout</span>
        <span>Đăng xuất</span>
      </button>
    </aside>
</template>

<script setup>
import { computed } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();

const isAdmin = computed(() => {
  const role = localStorage.getItem('role');
  return role === 'Admin' || role === 'admin';
});

const logout = () => {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
  localStorage.removeItem('role');
  localStorage.removeItem('cart');
  router.push('/login');
};
</script>

<style scoped>
/* Sidebar */
.sidebar { width:260px; background:#2c2018; display:flex; flex-direction:column; padding:1.5rem 1rem; gap:.5rem; position:sticky; top:0; height:100vh; box-shadow: 4px 0 20px rgba(0,0,0,0.05); z-index: 40;}
.sidebar-brand { padding: 0 0.5rem; }
.sidebar-brand h2 { font-size:1.15rem; font-weight:800; color:#e8d8c8; margin:0 0 .2rem; letter-spacing: 0.02em; }
.sidebar-brand p  { font-size:.75rem; color:#a89888; margin:0 0 1.5rem; text-transform: uppercase; letter-spacing: 0.05em; font-weight: 600; }
.sidebar-nav { display:flex; flex-direction:column; gap:.35rem; }
.nav-item { display:flex; align-items:center; gap:.75rem; padding:.75rem 1rem; border-radius:12px; text-decoration:none; color:#a89888; font-size:.9rem; font-weight: 500; transition:all .2s ease; border:none; background:transparent; cursor:pointer; width:100%; text-align:left; font-family:inherit; }
.nav-item:hover    { background:rgba(255,255,255,0.08); color:#e8d8c8; transform: translateX(4px); }
.nav-item--active  { background: linear-gradient(135deg, #b36a3a 0%, #8a4a20 100%); color:#fff; font-weight:600; box-shadow: 0 4px 12px rgba(179,106,58,0.3); }
.nav-item .material-symbols-outlined { font-size: 20px; }
.btn-logout { margin-top: auto; color: #e57373 !important; }
.btn-logout:hover { background: rgba(229,115,115,0.1) !important; color: #ffcdd2 !important; }
</style>
