<template>
  <div class="storefront">
    <Header />

    <main class="py-xl max-w-[1200px] mx-auto px-margin">
      <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
        
        <!-- Contact Info -->
        <div class="bg-surface p-8 rounded-2xl shadow-sm border border-outline-variant/30">
          <h1 class="text-3xl font-bold text-primary mb-6">Liên Hệ Với Chúng Tôi</h1>
          <p class="text-on-surface-variant mb-8">
            Chúng tôi luôn sẵn sàng lắng nghe mọi ý kiến đóng góp, thắc mắc về đơn hàng hoặc các yêu cầu đặt bánh theo yêu cầu riêng của bạn.
          </p>
          
          <div class="space-y-6">
            <div class="flex items-start gap-4">
              <span class="material-symbols-outlined text-primary text-3xl bg-surface-container-low p-3 rounded-full">location_on</span>
              <div>
                <h3 class="font-bold text-on-surface">Địa chỉ</h3>
                <p class="text-on-surface-variant text-sm mt-1">123 Đường Bánh Mì, Quận Ngọt Ngào, TP. HCM</p>
              </div>
            </div>
            
            <div class="flex items-start gap-4">
              <span class="material-symbols-outlined text-primary text-3xl bg-surface-container-low p-3 rounded-full">call</span>
              <div>
                <h3 class="font-bold text-on-surface">Điện thoại</h3>
                <p class="text-on-surface-variant text-sm mt-1">0123 456 789 (8:00 - 20:00 mỗi ngày)</p>
              </div>
            </div>
            
            <div class="flex items-start gap-4">
              <span class="material-symbols-outlined text-primary text-3xl bg-surface-container-low p-3 rounded-full">mail</span>
              <div>
                <h3 class="font-bold text-on-surface">Email</h3>
                <p class="text-on-surface-variant text-sm mt-1">hello@hearthandharvest.vn</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Contact Form -->
        <div class="bg-surface p-8 rounded-2xl shadow-sm border border-outline-variant/30">
          <h2 class="text-2xl font-bold text-on-surface mb-6">Gửi tin nhắn</h2>
          <form @submit.prevent="submitForm" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-on-surface-variant mb-1">Họ và tên</label>
              <input type="text" required class="w-full px-4 py-2 border border-outline-variant/50 rounded-lg focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary" placeholder="Tên của bạn">
            </div>
            <div>
              <label class="block text-sm font-medium text-on-surface-variant mb-1">Email</label>
              <input type="email" required class="w-full px-4 py-2 border border-outline-variant/50 rounded-lg focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary" placeholder="email@example.com">
            </div>
            <div>
              <label class="block text-sm font-medium text-on-surface-variant mb-1">Tin nhắn</label>
              <textarea required rows="4" class="w-full px-4 py-2 border border-outline-variant/50 rounded-lg focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary" placeholder="Bạn muốn nhắn gửi điều gì?"></textarea>
            </div>
            <button type="submit" class="w-full bg-primary text-on-primary py-3 rounded-lg font-bold hover:bg-primary/90 transition-colors mt-4">
              Gửi tin nhắn
            </button>
          </form>
        </div>

      </div>
    </main>

    <Footer />
  </div>
</template>

<script setup>
import { toast } from 'vue3-toastify';
import Header from '../components/Header.vue';
import Footer from '../components/Footer.vue';

const submitForm = () => {
  toast.success('Cảm ơn bạn! Tin nhắn đã được gửi.');
};
</script>

<style scoped>
.material-symbols-outlined {
  font-family: 'Material Symbols Outlined';
  font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
}
.cart-badge {
  position: absolute;
  top: 2px; right: 2px;
  background: #b36a3a;
  color: #fff;
  width: 18px; height: 18px;
  border-radius: 50%;
  font-size: .65rem;
  font-weight: 700;
  display: flex;
  align-items: center;
  justify-content: center;
}
.storefront {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  background-color: #fcf9f8;
}
</style>
