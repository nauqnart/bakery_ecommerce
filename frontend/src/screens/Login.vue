<template>
  <div class="auth-page">
    <main class="flex h-screen overflow-hidden">
      <!-- Image Side (Left) -->
      <section class="hidden lg:block lg:w-1/2 relative overflow-hidden">
        <div class="absolute inset-0 bg-primary/10 mix-blend-multiply z-10"></div>
        <img
          alt="Artisan Sourdough Bread"
          class="absolute inset-0 w-full h-full object-cover transition-transform duration-1000"
          :class="isLoginMode ? 'scale-100' : 'scale-105'"
          src="https://lh3.googleusercontent.com/aida-public/AB6AXuDQnF0D1NMGobQ4NsbrIyIiz2npzqMq8yH-mbVemWAD1iPmi5sTcNUI7gmijkEWfzTzhNE2aoK74HmPLC4p5jp2Ocbm3zTtphdE0ax8PaSauC3jjrYBKxV5qw0oT73YqClZCh8sA_Lnfg1ltIy3oHQfmWDcPq5acKUSiIB85YTFBWiQe6SnieMS1tkNfNyQG9qyLPal824sLEQX78IlogU6GjL_dnJ643jqNi2Zda6qXia2fGGlFaJEABBC_1axoUnbVj3-FEO35w"
        />
        <!-- Branding Overlay -->
        <div class="absolute inset-0 flex flex-col justify-end p-xl z-20 bg-gradient-to-t from-black/60 to-transparent">
          <h1 class="font-h1 text-[64px] leading-tight text-white mb-base">Hearth &amp; Harvest</h1>
          <p class="font-body-lg text-white/90 max-w-md">
            Nghệ thuật làm bánh truyền thống, mang hương vị ấm áp đến từng căn bếp Việt.
          </p>
        </div>
      </section>

      <!-- Form Side (Right) -->
      <section class="w-full lg:w-1/2 flex items-center justify-center p-md bg-surface relative overflow-y-auto">
        <div class="grainy-overlay absolute inset-0"></div>
        <div class="w-full max-w-[440px] z-10">
          
          <div class="lg:hidden mb-lg text-center">
            <h2 class="font-h2 text-primary">Hearth &amp; Harvest</h2>
          </div>

          <!-- Header -->
          <div class="mb-sm">
            <h2 class="font-h2 text-h3 text-on-surface mb-0">
              {{ isLoginMode ? 'Chào mừng bạn quay lại' : 'Chào mừng bạn mới' }}
            </h2>
            <p class="font-body-md text-on-surface-variant">
              {{ isLoginMode ? 'Đăng nhập để quản lý lò bánh của bạn.' : 'Bắt đầu hành trình thưởng thức nghệ thuật làm bánh.' }}
            </p>
          </div>

          <!-- Forms -->
          <div class="relative">
            <!-- LOGIN FORM -->
            <form v-if="isLoginMode" @submit.prevent="handleLogin" class="space-y-sm animate-fade-in">
              <div class="space-y-xs">
                <label class="font-label-caps text-on-surface-variant uppercase" for="email">Email</label>
                <div class="relative">
                  <input
                    v-model="email"
                    class="w-full px-md py-2 bg-surface-container-low border border-outline-variant rounded-lg focus:ring-2 focus:ring-primary focus:border-primary outline-none transition-all font-body-md text-on-surface"
                    id="email" type="email" placeholder="ten@hearthharvest.vn" required
                  />
                </div>
              </div>
              <div class="space-y-xs">
                <label class="font-label-caps text-on-surface-variant uppercase" for="password">Mật khẩu</label>
                <div class="relative">
                  <input
                    v-model="password"
                    class="w-full px-md py-2 pr-10 bg-surface-container-low border border-outline-variant rounded-lg focus:ring-2 focus:ring-primary focus:border-primary outline-none transition-all font-body-md text-on-surface"
                    id="password" :type="showLoginPassword ? 'text' : 'password'" placeholder="••••••••" required
                  />
                  <button type="button" @click="showLoginPassword = !showLoginPassword" class="absolute right-3 top-1/2 -translate-y-1/2 text-on-surface-variant hover:text-primary transition-colors flex items-center" tabindex="-1">
                    <span class="material-symbols-outlined text-[20px]">{{ showLoginPassword ? 'visibility_off' : 'visibility' }}</span>
                  </button>
                </div>
              </div>
              <div class="flex items-center justify-between text-sm">
                <label class="flex items-center gap-xs cursor-pointer group">
                  <input class="w-4 h-4 rounded border-outline-variant text-primary focus:ring-primary bg-surface-container-low transition-colors" type="checkbox" />
                  <span class="font-body-sm text-on-surface-variant group-hover:text-on-surface transition-colors">Ghi nhớ đăng nhập</span>
                </label>
                <router-link to="/forgot-password" class="font-body-sm text-primary font-semibold hover:underline">Quên mật khẩu?</router-link>
              </div>
              
              <button :disabled="isLoading" class="w-full py-2 mt-2 bg-primary text-on-primary font-button rounded-lg shadow-md shadow-primary/20 hover:bg-primary-container active:scale-[0.98] transition-all flex items-center justify-center gap-base disabled:opacity-50" type="submit">
                <span v-if="isLoading" class="material-symbols-outlined spin">refresh</span>
                {{ isLoading ? 'Đang xử lý...' : 'Đăng nhập' }}
              </button>
            </form>

            <!-- REGISTER FORM -->
            <form v-else @submit.prevent="handleRegister" class="space-y-sm animate-fade-in">
              <div class="space-y-xs">
                <label class="font-label-caps text-on-surface-variant uppercase" for="reg-name">Họ và tên</label>
                <div class="relative">
                  <input
                    v-model="regFullName"
                    class="w-full px-md py-2 bg-surface-container-low border border-outline-variant rounded-lg focus:ring-2 focus:ring-primary focus:border-primary outline-none transition-all font-body-md text-on-surface"
                    id="reg-name" type="text" placeholder="Nguyễn Văn A" required
                  />
                </div>
              </div>
              <div class="space-y-xs">
                <label class="font-label-caps text-on-surface-variant uppercase" for="reg-email">Email</label>
                <div class="relative">
                  <input
                    v-model="regEmail"
                    class="w-full px-md py-2 bg-surface-container-low border border-outline-variant rounded-lg focus:ring-2 focus:ring-primary focus:border-primary outline-none transition-all font-body-md text-on-surface"
                    id="reg-email" type="email" placeholder="email@vi-du.com" required
                  />
                </div>
              </div>
              <div class="space-y-xs">
                <label class="font-label-caps text-on-surface-variant uppercase" for="reg-password">Mật khẩu</label>
                <div class="relative">
                  <input
                    v-model="regPassword"
                    class="w-full px-md py-2 pr-10 bg-surface-container-low border border-outline-variant rounded-lg focus:ring-2 focus:ring-primary focus:border-primary outline-none transition-all font-body-md text-on-surface"
                    id="reg-password" :type="showRegPassword ? 'text' : 'password'" placeholder="••••••••" required
                  />
                  <button type="button" @click="showRegPassword = !showRegPassword" class="absolute right-3 top-1/2 -translate-y-1/2 text-on-surface-variant hover:text-primary transition-colors flex items-center" tabindex="-1">
                    <span class="material-symbols-outlined text-[20px]">{{ showRegPassword ? 'visibility_off' : 'visibility' }}</span>
                  </button>
                </div>
              </div>
              <div class="space-y-xs">
                <label class="font-label-caps text-on-surface-variant uppercase" for="reg-confirm-password">Xác nhận mật khẩu</label>
                <div class="relative">
                  <input
                    v-model="regConfirmPassword"
                    class="w-full px-md py-2 pr-10 bg-surface-container-low border border-outline-variant rounded-lg focus:ring-2 focus:ring-primary focus:border-primary outline-none transition-all font-body-md text-on-surface"
                    id="reg-confirm-password" :type="showRegConfirmPassword ? 'text' : 'password'" placeholder="••••••••" required
                  />
                  <button type="button" @click="showRegConfirmPassword = !showRegConfirmPassword" class="absolute right-3 top-1/2 -translate-y-1/2 text-on-surface-variant hover:text-primary transition-colors flex items-center" tabindex="-1">
                    <span class="material-symbols-outlined text-[20px]">{{ showRegConfirmPassword ? 'visibility_off' : 'visibility' }}</span>
                  </button>
                </div>
              </div>
              
              <button :disabled="isLoading" class="w-full py-2 mt-2 bg-primary text-on-primary font-button rounded-lg shadow-md shadow-primary/20 hover:bg-primary-container active:scale-[0.98] transition-all flex items-center justify-center gap-base disabled:opacity-50" type="submit">
                <span v-if="isLoading" class="material-symbols-outlined spin">refresh</span>
                {{ isLoading ? 'Đang xử lý...' : 'Đăng ký' }}
              </button>
            </form>
          </div>

          <!-- Toggle Mode -->
          <p class="mt-4 text-center font-body-sm text-on-surface-variant">
            {{ isLoginMode ? 'Chưa có tài khoản?' : 'Đã có tài khoản?' }}
            <button class="text-primary font-semibold hover:underline bg-transparent border-none cursor-pointer" @click="isLoginMode = !isLoginMode">
              {{ isLoginMode ? 'Đăng ký thành viên mới' : 'Đăng nhập ngay' }}
            </button>
          </p>

        </div>
      </section>
    </main>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useAuthStore } from '../stores/authStore';
import { useCartStore } from '../stores/cartStore';
import { toast } from 'vue3-toastify';

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();
const cartStore = useCartStore();

const isLoginMode = ref(true);

// Login State
const email = ref('');
const password = ref('');
const showLoginPassword = ref(false);

// Register State
const regFullName = ref('');
const regEmail = ref('');
const regPassword = ref('');
const regConfirmPassword = ref('');
const showRegPassword = ref(false);
const showRegConfirmPassword = ref(false);

const isLoading = ref(false);

const handleLogin = async () => {
  if (!email.value.trim()) return toast.error('Email không được để trống!');
  if (!password.value) return toast.error('Mật khẩu không được để trống!');
  
  isLoading.value = true;
  try {
    const response = await fetch(import.meta.env.VITE_API_BASE_URL + '/user/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email: email.value, password: password.value })
    });
    const result = await response.json();
    if (response.ok && result.success) {
      const user = result.data;
      authStore.login(user, user.token);
      localStorage.setItem('role', user.role);
      
      // Merge local cart to backend if items exist
      if (cartStore.items.length > 0) {
        await cartStore.syncLocalCart();
      }
      await cartStore.fetchCart();
      
      if (user.role.toLowerCase() === 'admin' || user.role.toLowerCase() === 'staff') {
        router.push('/dashboard');
      } else {
        const redirectPath = route.query.redirect || '/';
        router.push(redirectPath);
      }
    } else {
      toast.error(result.message || 'Đăng nhập thất bại.');
    }
  } catch (error) {
    toast.error('Lỗi kết nối đến máy chủ.');
  } finally {
    isLoading.value = false;
  }
};

const handleRegister = async () => {
  if (!regFullName.value.trim()) return toast.error('Họ tên không được để trống!');
  if (!regEmail.value.trim()) return toast.error('Email không được để trống!');
  if (!regPassword.value) return toast.error('Mật khẩu không được để trống!');
  if (regPassword.value.length < 6) return toast.error('Mật khẩu phải từ 6 ký tự trở lên!');
  if (regPassword.value !== regConfirmPassword.value) return toast.error('Mật khẩu xác nhận không khớp!');
  
  isLoading.value = true;
  try {
    const response = await fetch(import.meta.env.VITE_API_BASE_URL + '/user/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ name: regFullName.value, email: regEmail.value, password: regPassword.value })
    });
    const result = await response.json();
    if (response.ok && result.success) {
      toast.success('Đăng ký thành công! Vui lòng đăng nhập.');
      isLoginMode.value = true; // Switch back to login
      email.value = regEmail.value; // Pre-fill email
      password.value = '';
      regConfirmPassword.value = '';
      regFullName.value = '';
      regEmail.value = '';
      regPassword.value = '';
    } else {
      let errorMsg = result.message || 'Đăng ký thất bại.';
      // Check for .NET Validation error map
      if (result.errors) {
         errorMsg = Object.values(result.errors).flat().join(', ');
      }
      toast.error(errorMsg);
    }
  } catch (error) {
    toast.error('Lỗi kết nối đến máy chủ.');
  } finally {
    isLoading.value = false;
  }
};
</script>

<style scoped>
.material-symbols-outlined {
  font-variation-settings: "FILL" 0, "wght" 400, "GRAD" 0, "opsz" 24;
  display: inline-block;
  line-height: 1;
}
.grainy-overlay {
  background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)'/%3E%3C/svg%3E");
  opacity: 0.03;
  pointer-events: none;
}
.spin { display: inline-block; animation: spin 1s linear infinite; }
@keyframes spin { to { transform: rotate(360deg); } }
.animate-fade-in {
  animation: fadeIn 0.3s ease-in-out;
}
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>
