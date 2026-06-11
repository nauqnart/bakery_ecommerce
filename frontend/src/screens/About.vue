<template>
  <div class="storefront">
    <Header />

    <main class="py-xl max-w-[1000px] mx-auto px-margin">
      <div class="bg-surface rounded-2xl shadow-sm border border-outline-variant/30 overflow-hidden">
        <div class="h-64 sm:h-80 w-full relative">
          <img src="https://images.unsplash.com/photo-1509440159596-0249088772ff?q=80&w=2072&auto=format&fit=crop" class="w-full h-full object-cover" alt="Bakery interior" />
          <div class="absolute inset-0 bg-black/40 flex items-center justify-center">
            <h1 class="text-4xl md:text-5xl font-bold text-white tracking-wide">Về Chúng Tôi</h1>
          </div>
        </div>
        
        <div class="p-8 md:p-12">
          <h2 class="text-2xl font-bold text-primary mb-4">Câu Chuyện Của Hearth &amp; Harvest</h2>
          <p class="text-on-surface-variant leading-relaxed mb-6">
            Bắt nguồn từ niềm đam mê với những chiếc bánh mì truyền thống lên men tự nhiên (sourdough), Hearth &amp; Harvest được thành lập với mục tiêu mang lại hương vị đích thực của bánh ngọt thủ công đến với mọi gia đình. 
            Chúng tôi tin rằng, một chiếc bánh ngon không chỉ đến từ nguyên liệu đắt tiền, mà còn từ sự kiên nhẫn, tỉ mỉ và tình yêu của người thợ làm bánh.
          </p>
          
          <h2 class="text-2xl font-bold text-primary mb-4 mt-8">Triết Lý Của Chúng Tôi</h2>
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mt-6">
            <div class="bg-surface-container-low p-6 rounded-xl border border-outline-variant/50 text-center hover:-translate-y-1 transition-transform">
              <span class="material-symbols-outlined text-4xl text-primary mb-3">eco</span>
              <h3 class="font-bold text-lg mb-2">Nguyên liệu tự nhiên</h3>
              <p class="text-sm text-on-surface-variant">Tuyển chọn các loại bột mì hảo hạng và nguyên liệu tươi ngon nhất mỗi ngày.</p>
            </div>
            <div class="bg-surface-container-low p-6 rounded-xl border border-outline-variant/50 text-center hover:-translate-y-1 transition-transform">
              <span class="material-symbols-outlined text-4xl text-primary mb-3">bakery_dining</span>
              <h3 class="font-bold text-lg mb-2">Làm hoàn toàn thủ công</h3>
              <p class="text-sm text-on-surface-variant">Từng chiếc bánh được nhào nặn và chăm chút bởi đôi bàn tay khéo léo của các thợ bánh.</p>
            </div>
            <div class="bg-surface-container-low p-6 rounded-xl border border-outline-variant/50 text-center hover:-translate-y-1 transition-transform">
              <span class="material-symbols-outlined text-4xl text-primary mb-3">favorite</span>
              <h3 class="font-bold text-lg mb-2">Đong đầy tình yêu</h3>
              <p class="text-sm text-on-surface-variant">Hương vị của sự ấm áp, như chính căn bếp gia đình của bạn.</p>
            </div>
          </div>
        </div>
      </div>
    </main>

    <Footer />
  </div>
</template>

<script setup>
import Header from '../components/Header.vue';
import Footer from '../components/Footer.vue';
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
