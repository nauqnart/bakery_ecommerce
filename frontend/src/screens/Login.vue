<template>
  <div class="login">
    <main class="flex min-h-screen">
      <!-- Image Side (Left) -->
      <section class="hidden lg:block lg:w-1/2 relative overflow-hidden">
        <div
          class="absolute inset-0 bg-primary/10 mix-blend-multiply z-10"
        ></div>
        <img
          alt="Artisan Sourdough Bread"
          class="absolute inset-0 w-full h-full object-cover"
          data-alt="A rustic, close-up shot of a freshly baked artisan sourdough loaf on a flour-dusted wooden table. Soft morning sunlight streams through a nearby window, casting long, gentle shadows and highlighting the crispy, golden-brown texture of the bread's crust. The atmosphere is warm and inviting, evoking a sense of artisanal craftsmanship and morning freshness. The color palette is dominated by rich earth tones, warm ambers, and soft creams, perfectly aligning with the Hearth and Harvest brand identity."
          src="https://lh3.googleusercontent.com/aida-public/AB6AXuDQnF0D1NMGobQ4NsbrIyIiz2npzqMq8yH-mbVemWAD1iPmi5sTcNUI7gmijkEWfzTzhNE2aoK74HmPLC4p5jp2Ocbm3zTtphdE0ax8PaSauC3jjrYBKxV5qw0oT73YqClZCh8sA_Lnfg1ltIy3oHQfmWDcPq5acKUSiIB85YTFBWiQe6SnieMS1tkNfNyQG9qyLPal824sLEQX78IlogU6GjL_dnJ643jqNi2Zda6qXia2fGGlFaJEABBC_1axoUnbVj3-FEO35w"
        />
        <!-- Branding Overlay -->
        <div
          class="absolute inset-0 flex flex-col justify-end p-xl z-20 bg-gradient-to-t from-black/60 to-transparent"
        >
          <h1 class="font-h1 text-[64px] leading-tight text-white mb-base">
            Hearth &amp; Harvest
          </h1>
          <p class="font-body-lg text-white/90 max-w-md">
            Nghệ thuật làm bánh truyền thống, mang hương vị ấm áp đến từng căn
            bếp Việt.
          </p>
        </div>
      </section>
      <!-- Form Side (Right) -->
      <section
        class="w-full lg:w-1/2 flex items-center justify-center p-margin bg-surface relative"
      >
        <div class="grainy-overlay absolute inset-0"></div>
        <div class="w-full max-w-[440px] z-10">
          <!-- Mobile Branding -->
          <div class="lg:hidden mb-lg text-center">
            <h2 class="font-h2 text-primary">Hearth &amp; Harvest</h2>
          </div>
          <div class="mb-lg">
            <h2 class="font-h2 text-on-surface mb-xs">
              Chào mừng bạn quay lại
            </h2>
            <p class="font-body-md text-on-surface-variant">
              Đăng nhập để quản lý lò bánh của bạn.
            </p>
          </div>
          <form @submit.prevent="handleLogin" class="space-y-md">
            <div class="space-y-xs">
              <label
                class="font-label-caps text-on-surface-variant uppercase"
                for="email"
                >Email</label
              >
              <div class="relative">
                <input
                  v-model="email"
                  class="w-full px-md py-base bg-surface-container-low border border-outline-variant rounded-lg focus:ring-2 focus:ring-primary focus:border-primary outline-none transition-all font-body-md text-on-surface"
                  id="email"
                  name="email"
                  placeholder="ten@hearthharvest.vn"
                  type="email"
                  required
                />
              </div>
            </div>
            <div class="space-y-xs">
              <label
                class="font-label-caps text-on-surface-variant uppercase"
                for="password"
                >Mật khẩu</label
              >
              <div class="relative">
                <input
                  v-model="password"
                  class="w-full px-md py-base bg-surface-container-low border border-outline-variant rounded-lg focus:ring-2 focus:ring-primary focus:border-primary outline-none transition-all font-body-md text-on-surface"
                  id="password"
                  name="password"
                  placeholder="••••••••"
                  type="password"
                  required
                />
              </div>
            </div>
            <div class="flex items-center justify-between">
              <label class="flex items-center gap-base cursor-pointer group">
                <input
                  class="w-5 h-5 rounded border-outline-variant text-primary focus:ring-primary bg-surface-container-low transition-colors"
                  type="checkbox"
                />
                <span
                  class="font-body-sm text-on-surface-variant group-hover:text-on-surface transition-colors"
                  >Ghi nhớ đăng nhập</span
                >
              </label>
              <a
                class="font-body-sm text-primary font-semibold hover:underline"
                href="#"
                >Quên mật khẩu?</a
              >
            </div>
            <p v-if="errorMessage" class="text-red-500 font-body-sm">{{ errorMessage }}</p>
            <button
              :disabled="isLoading"
              class="w-full py-md bg-primary text-on-primary font-button rounded-lg shadow-md shadow-primary/20 hover:bg-primary-container active:scale-[0.98] transition-all flex items-center justify-center gap-base disabled:opacity-50"
              type="submit"
            >
              {{ isLoading ? 'Đang xử lý...' : 'Đăng nhập' }}
            </button>
          </form>
          <div class="relative my-lg">
            <div class="absolute inset-0 flex items-center">
              <div class="w-full border-t border-outline-variant"></div>
            </div>
            <div class="relative flex justify-center text-label-caps">
              <span class="bg-surface px-md text-on-surface-variant uppercase"
                >Hoặc tiếp tục với</span
              >
            </div>
          </div>
          <button
            class="w-full py-base border border-outline-variant bg-surface-container-lowest text-on-surface font-button rounded-lg hover:bg-surface-container-low active:scale-[0.98] transition-all flex items-center justify-center gap-md"
          >
            <svg class="w-5 h-5" viewbox="0 0 24 24">
              <path
                d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
                fill="#4285F4"
              ></path>
              <path
                d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
                fill="#34A853"
              ></path>
              <path
                d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
                fill="#FBBC05"
              ></path>
              <path
                d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
                fill="#EA4335"
              ></path>
            </svg>
            Đăng nhập bằng Google
          </button>
          <p class="mt-xl text-center font-body-sm text-on-surface-variant">
            Chưa có tài khoản?
            <a class="text-primary font-semibold hover:underline" href="#"
              >Đăng ký thành viên mới</a
            >
          </p>
        </div>
      </section>
    </main>
    <!-- Simple Footer for Legal -->
    <footer
      class="absolute bottom-0 w-full lg:w-1/2 right-0 p-md bg-transparent flex justify-center items-center text-on-surface-variant/60 font-body-sm pointer-events-none lg:pointer-events-auto"
    >
      <div class="flex gap-md">
        <span>© 2024 Hearth &amp; Harvest</span>
        <a
          class="hover:text-primary transition-colors pointer-events-auto"
          href="#"
          >Điều khoản</a
        >
        <a
          class="hover:text-primary transition-colors pointer-events-auto"
          href="#"
          >Bảo mật</a
        >
      </div>
    </footer>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const email = ref('');
const password = ref('');
const errorMessage = ref('');
const isLoading = ref(false);

const handleLogin = async () => {
  errorMessage.value = '';
  isLoading.value = true;
  try {
    const response = await fetch('http://localhost:5072/api/user/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ email: email.value, password: password.value })
    });

    const result = await response.json();

    if (response.ok && result.success) {
      const user = result.data;
      localStorage.setItem('user', JSON.stringify(user));
      localStorage.setItem('role', user.role);
      
      // Chuyển hướng
      if (user.role === 'Admin') {
        router.push('/dashboard');
      } else {
        router.push('/');
      }
    } else {
      errorMessage.value = result.message || 'Đăng nhập thất bại.';
    }
  } catch (error) {
    errorMessage.value = 'Lỗi kết nối đến máy chủ.';
  } finally {
    isLoading.value = false;
  }
};
</script>

<style scoped>
.material-symbols-outlined {
  font-variation-settings:
    "FILL" 0,
    "wght" 400,
    "GRAD" 0,
    "opsz" 24;
  display: inline-block;
  line-height: 1;
  text-transform: none;
  letter-spacing: normal;
  word-wrap: normal;
  white-space: nowrap;
  direction: ltr;
}
.grainy-overlay {
  background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)'/%3E%3C/svg%3E");
  opacity: 0.03;
  pointer-events: none;
}
</style>
