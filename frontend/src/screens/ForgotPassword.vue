<template>
  <div class="auth-page">
    <main class="flex h-screen overflow-hidden">
      <!-- Image Side (Left) -->
      <section class="hidden lg:block lg:w-1/2 relative overflow-hidden">
        <div class="absolute inset-0 bg-primary/10 mix-blend-multiply z-10"></div>
        <img
          alt="Artisan Sourdough Bread"
          class="absolute inset-0 w-full h-full object-cover transition-transform duration-1000 scale-100"
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
              Quên mật khẩu?
            </h2>
            <p class="font-body-md text-on-surface-variant mt-2">
              Đừng lo lắng! Hãy nhập email bạn đã đăng ký, chúng tôi sẽ gửi liên kết để đặt lại mật khẩu.
            </p>
          </div>

          <!-- Forms -->
          <div class="relative mt-8">
            <form @submit.prevent="handleForgotPassword" class="space-y-sm animate-fade-in">
              <div class="space-y-xs">
                <label class="font-label-caps text-on-surface-variant uppercase" for="email">Email address</label>
                <div class="relative">
                  <input
                    v-model="email"
                    class="w-full px-md py-2 bg-surface-container-low border border-outline-variant rounded-lg focus:ring-2 focus:ring-primary focus:border-primary outline-none transition-all font-body-md text-on-surface"
                    id="email" type="email" placeholder="ten@hearthharvest.vn" required
                  />
                </div>
              </div>
              
              <button :disabled="loading" class="w-full py-2 mt-4 bg-primary text-on-primary font-button rounded-lg shadow-md shadow-primary/20 hover:bg-primary-container active:scale-[0.98] transition-all flex items-center justify-center gap-base disabled:opacity-50" type="submit">
                <span v-if="loading" class="material-symbols-outlined spin">refresh</span>
                {{ loading ? 'Đang gửi...' : 'Gửi link đặt lại mật khẩu' }}
              </button>
            </form>
          </div>

          <div class="relative my-4">
            <div class="absolute inset-0 flex items-center">
              <div class="w-full border-t border-outline-variant"></div>
            </div>
          </div>
          
          <p class="mt-4 text-center font-body-sm text-on-surface-variant">
            Nhớ ra mật khẩu rồi?
            <router-link to="/login" class="text-primary font-semibold hover:underline">
              Quay lại đăng nhập
            </router-link>
          </p>

        </div>
      </section>
    </main>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { toast } from 'vue3-toastify'

const email = ref('')
const loading = ref(false)

const handleForgotPassword = async () => {
  if (!email.value.trim()) return toast.error('Vui lòng nhập email!');
  loading.value = true
  
  try {
    const res = await fetch(import.meta.env.VITE_API_BASE_URL + '/user/forgot-password', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email: email.value })
    })
    const data = await res.json()
    if(data.success) {
      toast.success(data.message || 'Đã gửi link đặt lại mật khẩu. Vui lòng kiểm tra email.')
      email.value = ''
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
