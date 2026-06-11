<template>
<footer class="bg-surface-container-highest border-t border-outline-variant/30 mt-auto">
  <div class="max-w-[1440px] mx-auto px-margin py-lg">
    <div class="flex flex-col md:flex-row justify-between items-start gap-lg mb-lg">
      <div class="flex flex-col items-start max-w-xs">
        <span class="font-h3 text-h3 font-bold text-primary mb-sm">Hearth &amp; Harvest</span>
        <p class="font-body-sm text-on-surface-variant">2024 Tiệm bánh Hearth &amp; Harvest. Chất lượng thủ công, tươi mới mỗi ngày mang linh hồn bánh truyền thống đến bàn ăn hiện đại.</p>
      </div>
      <div class="grid grid-cols-2 sm:grid-cols-3 gap-lg">
        <div class="flex flex-col gap-sm">
          <span class="font-label-caps text-on-surface font-bold">Khám phá</span>
          <router-link to="/products" class="font-body-sm text-on-surface-variant hover:text-primary transition-colors">Tất cả sản phẩm</router-link>
          <router-link to="/products?sort=newest" class="font-body-sm text-on-surface-variant hover:text-primary transition-colors">Sản phẩm mới</router-link>
        </div>
        <div class="flex flex-col gap-sm">
          <span class="font-label-caps text-on-surface font-bold">Về chúng tôi</span>
          <router-link to="/about" class="font-body-sm text-on-surface-variant hover:text-primary transition-colors">Câu chuyện của chúng tôi</router-link>
          <a class="font-body-sm text-on-surface-variant hover:text-primary transition-colors" href="#">Địa điểm</a>
          <router-link to="/contact" class="font-body-sm text-on-surface-variant hover:text-primary transition-colors">Liên hệ</router-link>
        </div>
        <div class="flex flex-col gap-sm">
          <span class="font-label-caps text-on-surface font-bold">Pháp lý</span>
          <a class="font-body-sm text-on-surface-variant hover:text-primary transition-colors" href="#">Chính sách bảo mật</a>
          <a class="font-body-sm text-on-surface-variant hover:text-primary transition-colors" href="#">Điều khoản dịch vụ</a>
        </div>
      </div>
    </div>
    <div class="pt-lg border-t border-outline-variant/20 flex flex-col sm:flex-row justify-between items-center gap-md">
      <p class="font-body-sm text-on-surface-variant/70">© 2024 Tiệm bánh Hearth &amp; Harvest.</p>
      <div class="flex gap-lg">
        <a class="text-on-surface-variant hover:text-primary transition-all opacity-80" href="#"><span class="material-symbols-outlined">public</span></a>
        <a class="text-on-surface-variant hover:text-primary transition-all opacity-80" href="#"><span class="material-symbols-outlined">mail</span></a>
        <a class="text-on-surface-variant hover:text-primary transition-all opacity-80" href="#"><span class="material-symbols-outlined">favorite</span></a>
      </div>
    </div>
  </div>
</footer>
</template>

<style scoped>
.material-symbols-outlined {
  font-family: 'Material Symbols Outlined';
  font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
}
</style>
