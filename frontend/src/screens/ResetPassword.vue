<template>
  <div class="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
    <div class="sm:mx-auto sm:w-full sm:max-w-md">
      <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">Đặt Lại Mật Khẩu</h2>
    </div>

    <div class="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
      <div class="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
        <form class="space-y-6" @submit.prevent="handleResetPassword">
          <div>
            <label for="password" class="block text-sm font-medium text-gray-700">Mật khẩu mới</label>
            <div class="mt-1">
              <input v-model="password" id="password" type="password" required class="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-[#8B5E34] focus:border-[#8B5E34] sm:text-sm">
            </div>
          </div>
          
          <div>
            <label for="confirm" class="block text-sm font-medium text-gray-700">Nhập lại mật khẩu mới</label>
            <div class="mt-1">
              <input v-model="confirm" id="confirm" type="password" required class="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-[#8B5E34] focus:border-[#8B5E34] sm:text-sm">
            </div>
          </div>


          <div>
            <button type="submit" :disabled="loading" class="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-[#8B5E34] hover:bg-[#7A522D] focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-[#8B5E34]">
              <span v-if="loading">Đang xử lý...</span>
              <span v-else>Đổi Mật Khẩu</span>
            </button>
          </div>
        </form>

        <div class="mt-6 text-center">
            <router-link to="/login" class="text-sm font-medium text-[#8B5E34] hover:text-[#7A522D]">
              Đi đến Đăng nhập
            </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRoute } from 'vue-router'
import { toast } from 'vue3-toastify'

const route = useRoute()
const password = ref('')
const confirm = ref('')
const loading = ref(false)

const handleResetPassword = async () => {
  if (!password.value) return toast.error("Vui lòng nhập mật khẩu!");
  if (password.value !== confirm.value) {
    return toast.error("Mật khẩu không khớp!");
  }

  loading.value = true
  
  const token = route.query.token
  if(!token) {
    loading.value = false
    return toast.error("Link không hợp lệ hoặc đã hết hạn.");
  }

  try {
    const res = await fetch(import.meta.env.VITE_API_BASE_URL + '/user/reset-password', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ token, newPassword: password.value })
    })
    const data = await res.json()
    if(data.success) {
      toast.success(data.message)
      password.value = ''
      confirm.value = ''
    } else {
      toast.error(data.message || 'Có lỗi xảy ra.')
    }
  } catch(err) {
    toast.error("Không thể kết nối đến máy chủ.")
  } finally {
    loading.value = false
  }
}
</script>
