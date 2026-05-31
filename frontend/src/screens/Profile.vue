<template>
  <div class="max-w-3xl mx-auto px-4 py-12 min-h-screen">
    <div class="bg-white shadow overflow-hidden sm:rounded-lg">
      <div class="px-4 py-5 sm:px-6">
        <div class="flex items-center gap-4 mb-4">
          <router-link to="/" class="p-2 rounded-full hover:bg-stone-100 text-stone-500 transition-colors flex items-center justify-center">
            <span class="material-symbols-outlined">arrow_back</span>
          </router-link>
          <h3 class="text-lg leading-6 font-medium text-gray-900">Hồ Sơ Cá Nhân</h3>
        </div>
        <p class="mt-1 max-w-2xl text-sm text-gray-500">Cập nhật thông tin tài khoản của bạn.</p>
      </div>
      <div class="border-t border-gray-200 px-4 py-5 sm:p-0">
        <form @submit.prevent="updateProfile" class="sm:divide-y sm:divide-gray-200">
          <div class="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
            <dt class="text-sm font-medium text-gray-500">Họ và tên</dt>
            <dd class="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              <input v-model="profile.fullName" class="border border-gray-300 rounded px-3 py-2 w-full max-w-md focus:ring-[#b36a3a] focus:border-[#b36a3a]" />
            </dd>
          </div>
          <div class="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
            <dt class="text-sm font-medium text-gray-500">Email</dt>
            <dd class="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              <input :value="profile.email" disabled class="bg-gray-100 border border-gray-300 rounded px-3 py-2 w-full max-w-md" />
            </dd>
          </div>
          <div class="py-4 sm:py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
            <dt class="text-sm font-medium text-gray-500">Số điện thoại</dt>
            <dd class="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              <input v-model="profile.phone" class="border border-gray-300 rounded px-3 py-2 w-full max-w-md focus:ring-[#b36a3a] focus:border-[#b36a3a]" />
            </dd>
          </div>
          
          <div class="py-4 sm:py-5 px-6 flex justify-between items-center">
            <button type="button" @click="showPasswordModal = true" class="text-sm text-blue-600 hover:text-blue-800 font-medium border border-blue-200 px-3 py-1.5 rounded bg-blue-50 hover:bg-blue-100 transition-colors flex items-center gap-1">
              <span class="material-symbols-outlined align-middle text-[18px]">lock</span> Đổi mật khẩu
            </button>
            <button type="submit" :disabled="saving" class="bg-[#b36a3a] text-white px-4 py-2 rounded shadow hover:bg-[#8a7060] disabled:opacity-50">
              {{ saving ? 'Đang lưu...' : 'Lưu thay đổi' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Sổ địa chỉ -->
    <div class="mt-8 bg-white shadow overflow-hidden sm:rounded-lg">
      <div class="px-4 py-5 sm:px-6 flex justify-between items-center">
        <div>
          <h3 class="text-lg leading-6 font-medium text-gray-900">Sổ địa chỉ</h3>
          <p class="mt-1 max-w-2xl text-sm text-gray-500">Quản lý các địa chỉ nhận hàng của bạn.</p>
        </div>
        <button @click="openAddressModal()" class="bg-[#b36a3a] text-white px-3 py-1.5 rounded text-sm hover:bg-[#8a7060] flex items-center gap-1">
          <span class="material-symbols-outlined text-[18px]">add</span> Thêm địa chỉ mới
        </button>
      </div>
      <div class="border-t border-gray-200">
        <div v-if="loadingAddresses" class="p-6 text-center text-gray-500">Đang tải sổ địa chỉ...</div>
        <div v-else-if="addresses.length === 0" class="p-6 text-center text-gray-500">Bạn chưa thêm địa chỉ nào.</div>
        <div v-else class="divide-y divide-gray-200">
          <div v-for="addr in addresses" :key="addr.userAddressId" class="p-4 sm:px-6 flex flex-col sm:flex-row sm:justify-between sm:items-start gap-4 hover:bg-gray-50 transition-colors">
            <div class="flex-1">
              <div class="flex items-center gap-2 mb-1">
                <span class="font-semibold text-gray-900">{{ addr.fullName }}</span>
                <span v-if="addr.isDefault" class="px-2 py-0.5 rounded text-xs font-medium bg-blue-100 text-blue-800">Mặc định</span>
              </div>
              <div class="text-sm text-gray-600 space-y-1">
                <p><span class="text-gray-400 w-16 inline-block">SĐT:</span> {{ addr.phone }}</p>
                <p><span class="text-gray-400 w-16 inline-block">Địa chỉ:</span> {{ addr.addressLine }}</p>
              </div>
            </div>
            <div class="flex items-center gap-2">
              <button @click="openAddressModal(addr)" class="text-stone-500 hover:text-[#b36a3a] px-2 py-1 text-sm font-medium">Sửa</button>
              <button @click="deleteAddress(addr.userAddressId)" class="text-stone-500 hover:text-red-600 px-2 py-1 text-sm font-medium">Xóa</button>
              <button v-if="!addr.isDefault" @click="setDefaultAddress(addr.userAddressId)" class="text-stone-500 hover:text-blue-600 px-2 py-1 text-sm font-medium border border-gray-200 rounded ml-2">Đặt làm mặc định</button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Address Modal -->
    <div v-if="showAddressModal" class="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" @click="closeAddressModal"></div>
      <div class="relative bg-white rounded-xl shadow-xl w-full max-w-md overflow-hidden flex flex-col">
        <div class="px-6 py-4 border-b border-gray-100 flex justify-between items-center">
          <h3 class="font-bold text-lg text-gray-900">{{ editAddressId ? 'Sửa địa chỉ' : 'Thêm địa chỉ mới' }}</h3>
          <button @click="closeAddressModal" class="text-gray-400 hover:text-gray-600"><span class="material-symbols-outlined">close</span></button>
        </div>
        <form @submit.prevent="saveAddress" class="p-6 space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Họ và tên người nhận</label>
            <input v-model="addressForm.fullName" required class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-[#b36a3a] focus:border-[#b36a3a]" placeholder="Nguyễn Văn A" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Số điện thoại</label>
            <input v-model="addressForm.phone" required class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-[#b36a3a] focus:border-[#b36a3a]" placeholder="0901234567" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Địa chỉ chi tiết</label>
            <textarea v-model="addressForm.addressLine" required rows="3" class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-[#b36a3a] focus:border-[#b36a3a]" placeholder="Số nhà, tên đường, phường/xã, quận/huyện, tỉnh/thành phố"></textarea>
          </div>
          <div class="flex items-center gap-2 pt-2">
            <input type="checkbox" id="isDefault" v-model="addressForm.isDefault" class="rounded text-[#b36a3a] focus:ring-[#b36a3a]" />
            <label for="isDefault" class="text-sm text-gray-700 cursor-pointer">Đặt làm địa chỉ mặc định</label>
          </div>
          <div class="pt-4 flex justify-end gap-3">
            <button type="button" @click="closeAddressModal" class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50">Hủy</button>
            <button type="submit" :disabled="savingAddress" class="px-4 py-2 text-sm font-medium text-white bg-[#b36a3a] rounded-lg hover:bg-[#8a7060] disabled:opacity-50">
              {{ savingAddress ? 'Đang lưu...' : 'Lưu địa chỉ' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Change Password Modal -->
    <div v-if="showPasswordModal" class="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" @click="closePasswordModal"></div>
      <div class="relative bg-white rounded-xl shadow-xl w-full max-w-md overflow-hidden flex flex-col">
        <div class="px-6 py-4 border-b border-gray-100 flex justify-between items-center">
          <h3 class="font-bold text-lg text-gray-900">Đổi mật khẩu</h3>
          <button type="button" @click="closePasswordModal" class="text-gray-400 hover:text-gray-600"><span class="material-symbols-outlined">close</span></button>
        </div>
        <form @submit.prevent="changePassword" class="p-6 space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Mật khẩu hiện tại</label>
            <input v-model="passwordForm.currentPassword" type="password" required class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-[#b36a3a] focus:border-[#b36a3a]" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Mật khẩu mới</label>
            <input v-model="passwordForm.newPassword" type="password" required class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-[#b36a3a] focus:border-[#b36a3a]" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Nhập lại mật khẩu mới</label>
            <input v-model="passwordForm.confirmPassword" type="password" required class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-[#b36a3a] focus:border-[#b36a3a]" />
          </div>
          <div class="pt-4 flex justify-end gap-3">
            <button type="button" @click="closePasswordModal" class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50">Hủy</button>
            <button type="submit" :disabled="savingPassword" class="px-4 py-2 text-sm font-medium text-white bg-[#b36a3a] rounded-lg hover:bg-[#8a7060] disabled:opacity-50">
              {{ savingPassword ? 'Đang đổi...' : 'Đổi mật khẩu' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { toast } from 'vue3-toastify'

const profile = ref({
  fullName: '',
  email: '',
  phone: ''
})
const saving = ref(false)
const userId = ref(null)
const token = ref(null)

const addresses = ref([])
const loadingAddresses = ref(false)
const showAddressModal = ref(false)
const savingAddress = ref(false)
const editAddressId = ref(null)
const addressForm = ref({ fullName: '', phone: '', addressLine: '', isDefault: false })

const showPasswordModal = ref(false)
const savingPassword = ref(false)
const passwordForm = ref({ currentPassword: '', newPassword: '', confirmPassword: '' })

function closePasswordModal() {
  showPasswordModal.value = false;
  passwordForm.value = { currentPassword: '', newPassword: '', confirmPassword: '' };
}

async function changePassword() {
  if (passwordForm.value.newPassword !== passwordForm.value.confirmPassword) {
    return toast.error("Mật khẩu nhập lại không khớp!");
  }
  if (passwordForm.value.newPassword.length < 6) {
    return toast.error("Mật khẩu mới phải có ít nhất 6 ký tự.");
  }
  savingPassword.value = true;
  try {
    const res = await fetch(`${import.meta.env.VITE_API_BASE_URL}/user/change-password`, {
      method: 'POST',
      headers: { 
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token.value}`
      },
      body: JSON.stringify({
        currentPassword: passwordForm.value.currentPassword,
        newPassword: passwordForm.value.newPassword
      })
    });
    const data = await res.json();
    if(data.success) {
      toast.success(data.message || 'Đổi mật khẩu thành công');
      closePasswordModal();
    } else {
      toast.error(data.message || 'Lỗi đổi mật khẩu');
    }
  } catch(e) {
    toast.error('Lỗi kết nối');
  } finally {
    savingPassword.value = false;
  }
}

onMounted(async () => {
  const userStr = localStorage.getItem('user')
  if (!userStr) {
    window.location.href = '/login'
    return
  }
  const user = JSON.parse(userStr)
  userId.value = user.userId
  token.value = user.token

  try {
    const res = await fetch(`${import.meta.env.VITE_API_BASE_URL}/user/${userId.value}`, {
      headers: { 'Authorization': `Bearer ${token.value}` }
    })
    const data = await res.json()
    if(data.success) {
      profile.value.fullName = data.data.fullName
      profile.value.email = data.data.email
      // Assuming phone is added to DTO, or else we fetch it
      profile.value.phone = data.data.phone || ''
    }
  } catch (e) {
    console.error(e)
  }

  fetchAddresses()
})

async function fetchAddresses() {
  loadingAddresses.value = true;
  try {
    const res = await fetch(`${import.meta.env.VITE_API_BASE_URL}/useraddress`, {
      headers: { 'Authorization': `Bearer ${token.value}` }
    })
    const data = await res.json()
    if(data.success) {
      addresses.value = data.data
    }
  } catch(e) {
    console.error(e)
  } finally {
    loadingAddresses.value = false;
  }
}

function openAddressModal(addr = null) {
  if (addr) {
    editAddressId.value = addr.userAddressId;
    addressForm.value = { ...addr };
  } else {
    editAddressId.value = null;
    addressForm.value = { fullName: profile.value.fullName || '', phone: profile.value.phone || '', addressLine: '', isDefault: addresses.value.length === 0 };
  }
  showAddressModal.value = true;
}

function closeAddressModal() {
  showAddressModal.value = false;
  addressForm.value = { fullName: '', phone: '', addressLine: '', isDefault: false };
}

async function saveAddress() {
  savingAddress.value = true;
  const isEdit = !!editAddressId.value;
  const url = isEdit ? `${import.meta.env.VITE_API_BASE_URL}/useraddress/${editAddressId.value}` : `${import.meta.env.VITE_API_BASE_URL}/useraddress`;
  const method = isEdit ? 'PUT' : 'POST';

  try {
    const res = await fetch(url, {
      method,
      headers: { 
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token.value}`
      },
      body: JSON.stringify(addressForm.value)
    });
    const data = await res.json();
    if(data.success) {
      toast.success(isEdit ? 'Đã cập nhật địa chỉ' : 'Đã thêm địa chỉ mới');
      closeAddressModal();
      fetchAddresses();
    } else {
      toast.error(data.message || 'Lỗi lưu địa chỉ');
    }
  } catch(e) {
    toast.error('Lỗi kết nối');
  } finally {
    savingAddress.value = false;
  }
}

async function deleteAddress(id) {
  if(!confirm('Bạn có chắc chắn muốn xóa địa chỉ này?')) return;
  try {
    const res = await fetch(`${import.meta.env.VITE_API_BASE_URL}/useraddress/${id}`, {
      method: 'DELETE',
      headers: { 'Authorization': `Bearer ${token.value}` }
    });
    const data = await res.json();
    if(data.success) {
      toast.success('Đã xóa địa chỉ');
      fetchAddresses();
    }
  } catch(e) {
    toast.error('Lỗi kết nối');
  }
}

async function setDefaultAddress(id) {
  try {
    const res = await fetch(`${import.meta.env.VITE_API_BASE_URL}/useraddress/${id}/set-default`, {
      method: 'PUT',
      headers: { 'Authorization': `Bearer ${token.value}` }
    });
    const data = await res.json();
    if(data.success) {
      toast.success('Đã đặt làm mặc định');
      fetchAddresses();
    }
  } catch(e) {
    toast.error('Lỗi kết nối');
  }
}

async function updateProfile() {
  if (!profile.value.fullName.trim()) return toast.error("Họ tên không được để trống!");

  saving.value = true;
  try {
    const res = await fetch(`${import.meta.env.VITE_API_BASE_URL}/user/${userId.value}`, {
      method: 'PUT',
      headers: { 
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token.value}`
      },
      body: JSON.stringify({
        fullName: profile.value.fullName,
        phone: profile.value.phone,
        roleId: 3, // hardcode customer
        isActive: true
      })
    })
    const data = await res.json()
    if(data.success) {
      toast.success('Đã lưu thành công!')
      // update localstorage name if needed
      const userStr = localStorage.getItem('user')
      if (userStr) {
        let u = JSON.parse(userStr)
        u.fullName = profile.value.fullName
        localStorage.setItem('user', JSON.stringify(u))
      }
    } else {
      toast.error(data.message || 'Lỗi lưu thông tin')
    }
  } catch(e) {
    toast.error('Lỗi kết nối')
  } finally {
    saving.value = false
  }
}
</script>
