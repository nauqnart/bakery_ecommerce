<template>
  <div class="register">
    
<main class="flex min-h-screen">
<!-- Left Side: Visual/Pastry Image -->
<section class="hidden lg:block lg:w-1/2 relative overflow-hidden">
<img alt="Artisanal Pastries" class="absolute inset-0 w-full h-full object-cover" data-alt="A warm, atmospheric wide shot of a rustic bakery table overflowing with hand-crafted pastries including flaky croissants, sourdough loaves, and dusted danishes. The lighting is soft and golden, evocative of early morning sun streaming through a flour-dusted window. The color palette features rich earthy browns, terracotta, and soft creams, creating a tactile and premium artisanal aesthetic. The focus is sharp on the textures of the golden crusts and delicate layers." src="https://lh3.googleusercontent.com/aida-public/AB6AXuBCF_iEgFioNn_92uhG1rOh--4IA3Ttk5n6JcZBLwBtJwHRWDGLkuS9zRf_GwP6R2qYlxwUekNwNAz6qrX52wUZR1HY_RdUyRkafHSKMKDQt8i4cTksiLr553as6A4gLq2zFC9JRmddvH0yqLahaoofCdfW2FN5-Xsvm2CrvsyTnm5jEuhU4cVAgo1CLiDPUDSlK1uy97VJStpz6Klad0YJ-muXk5oiUjzCzbaTGom04KTUGkiUyTn533imLZBliUHGR0-4ezR2lA"/>
<div class="absolute inset-0 bg-primary/10 mix-blend-multiply"></div>
<div class="absolute bottom-xl left-xl right-xl z-10 text-white">
<h2 class="font-h2 text-h2 mb-base">Hearth &amp; Harvest</h2>
<p class="font-body-lg text-body-lg max-w-md opacity-90">Mang hương vị bánh thủ công nồng nàn vào gian bếp của bạn mỗi ngày.</p>
</div>
</section>
<!-- Right Side: Sign Up Form -->
<section class="w-full lg:w-1/2 flex items-center justify-center p-margin bg-surface">
<div class="w-full max-w-[480px]">
<!-- Header -->
<header class="mb-lg">
<div class="flex items-center gap-base mb-md">
<span class="material-symbols-outlined text-primary text-[32px]">bakery_dining</span>
<h1 class="font-h1 text-h1 text-primary">Hearth &amp; Harvest</h1>
</div>
<h2 class="font-h2 text-h3 text-on-surface mb-xs">Chào mừng bạn mới</h2>
<p class="font-body-md text-body-md text-on-surface-variant">Bắt đầu hành trình thưởng thức nghệ thuật làm bánh cùng chúng tôi.</p>
</header>
<!-- Form -->
<form class="space-y-md">
<!-- Name Field -->
<div class="space-y-xs">
<label class="font-label-caps text-label-caps text-on-surface-variant uppercase" for="full-name">Họ và tên</label>
<div class="relative">
<span class="material-symbols-outlined absolute left-4 top-1/2 -translate-y-1/2 text-on-surface-variant opacity-60">person</span>
<input class="w-full pl-12 pr-4 py-3 bg-surface-container-low border-transparent focus:border-primary focus:ring-0 rounded-lg font-body-md transition-all duration-200 rustic-shadow" id="full-name" name="full-name" placeholder="Nguyễn Văn A" type="text"/>
</div>
</div>
<!-- Email Field -->
<div class="space-y-xs">
<label class="font-label-caps text-label-caps text-on-surface-variant uppercase" for="email">Email</label>
<div class="relative">
<span class="material-symbols-outlined absolute left-4 top-1/2 -translate-y-1/2 text-on-surface-variant opacity-60">mail</span>
<input class="w-full pl-12 pr-4 py-3 bg-surface-container-low border-transparent focus:border-primary focus:ring-0 rounded-lg font-body-md transition-all duration-200 rustic-shadow" id="email" name="email" placeholder="email@vi-du.com" type="email"/>
</div>
</div>
<!-- Password Field -->
<div class="space-y-xs">
<label class="font-label-caps text-label-caps text-on-surface-variant uppercase" for="password">Mật khẩu</label>
<div class="relative">
<span class="material-symbols-outlined absolute left-4 top-1/2 -translate-y-1/2 text-on-surface-variant opacity-60">lock</span>
<input class="w-full pl-12 pr-4 py-3 bg-surface-container-low border-transparent focus:border-primary focus:ring-0 rounded-lg font-body-md transition-all duration-200 rustic-shadow" id="password" name="password" placeholder="••••••••" type="password"/>
</div>
</div>
<!-- Newsletter Checkbox -->
<div class="flex items-start gap-md py-xs">
<div class="pt-1">
<input class="w-5 h-5 rounded border-outline text-primary focus:ring-primary/20 bg-surface" id="newsletter" type="checkbox"/>
</div>
<label class="font-body-sm text-body-sm text-on-surface-variant cursor-pointer" for="newsletter">
                            Tham gia <span class="text-primary font-semibold">Vòng tròn Bột</span> (Flour Circle) để nhận các ưu đãi đặc quyền và bí quyết làm bánh hàng tuần.
                        </label>
</div>
<!-- Submit Button -->
<button class="w-full bg-primary hover:bg-primary-container text-white font-button text-button py-4 rounded-lg shadow-md shadow-primary/10 transition-all duration-150 active:scale-[0.98] mt-base" type="submit">
                        Đăng ký
                    </button>
</form>
<!-- Footer Links -->
<footer class="mt-lg text-center">
<p class="font-body-sm text-body-sm text-on-surface-variant">
                        Bạn đã có tài khoản? 
                        <router-link class="text-primary font-semibold hover:underline transition-all" to="#">Đăng nhập ngay</router-link>
</p>
<div class="mt-xl flex flex-wrap justify-center gap-md border-t border-outline-variant pt-lg">
<a class="font-label-caps text-label-caps text-on-surface-variant hover:text-primary uppercase tracking-widest transition-colors" href="#">Về chúng tôi</a>
<a class="font-label-caps text-label-caps text-on-surface-variant hover:text-primary uppercase tracking-widest transition-colors" href="#">Điều khoản</a>
<a class="font-label-caps text-label-caps text-on-surface-variant hover:text-primary uppercase tracking-widest transition-colors" href="#">Hỗ trợ</a>
</div>
<div class="mt-md">
<p class="font-body-sm text-[11px] text-on-surface-variant opacity-60 uppercase tracking-tighter">
                            © 2024 Hearth &amp; Harvest Bakery. Nghệ thuật từ bột mì và lòng tâm huyết.
                        </p>
</div>
</footer>
</div>
</section>
</main>

  </div>
</template>

<script setup>
</script>

<style scoped>

        .material-symbols-outlined {
            font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
        }
        .batch-progress-bar {
            height: 6px;
            background: #e4e2e1;
            border-radius: 999px;
            position: relative;
            overflow: hidden;
        }
        .batch-progress-fill {
            height: 100%;
            background: #98421f;
            width: 65%;
            border-radius: 999px;
        }
        .rustic-shadow {
            box-shadow: 0 1px 2px 0 rgba(152, 66, 31, 0.05);
        }
    
</style>
