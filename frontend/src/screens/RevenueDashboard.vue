<template>
  <div class="revenuedashboard">
    
<!-- SideNavBar -->
<aside class="h-screen w-[280px] sticky left-0 top-0 bg-surface-container-low border-r border-outline-variant flex flex-col p-md gap-base shadow-sm">
<div class="mb-lg px-2">
<h2 class="font-h3 text-h3 font-bold text-primary">Quản trị Tiệm bánh</h2>
<p class="font-body-sm text-body-sm text-on-surface-variant">Vận hành Hàng ngày</p>
</div>
<nav class="flex-grow flex flex-col gap-1">
<router-link class="bg-secondary-container text-on-secondary-container font-semibold rounded-lg px-4 py-3 flex items-center gap-3 transition-colors" to="/dashboard">
<span class="material-symbols-outlined" data-icon="dashboard">dashboard</span>
<span class="font-body-md text-body-md">Bảng điều khiển</span>
</router-link>
<router-link class="text-on-surface-variant hover:bg-surface-container-high hover:text-primary rounded-lg px-4 py-3 flex items-center gap-3 transition-all" to="/payments">
<span class="material-symbols-outlined" data-icon="receipt_long">receipt_long</span>
<span class="font-body-md text-body-md">Lịch sử Đơn hàng</span>
</router-link>
<router-link class="text-on-surface-variant hover:bg-surface-container-high hover:text-primary rounded-lg px-4 py-3 flex items-center gap-3 transition-all" to="/users">
<span class="material-symbols-outlined" data-icon="group">group</span>
<span class="font-body-md text-body-md">Quản lý Người dùng</span>
</router-link>
<router-link class="text-on-surface-variant hover:bg-surface-container-high hover:text-primary rounded-lg px-4 py-3 flex items-center gap-3 transition-all" to="/inventory">
<span class="material-symbols-outlined" data-icon="inventory_2">inventory_2</span>
<span class="font-body-md text-body-md">Quản lý Sản phẩm</span>
</router-link>
</nav>
<div class="mt-auto flex flex-col gap-1">
<div class="px-2 mb-md">
<button class="w-full bg-primary text-on-primary font-button px-4 py-3 rounded-lg flex items-center justify-center gap-2 hover:opacity-90 transition-all active:scale-95 shadow-sm">
<span class="material-symbols-outlined">add</span>
                Lô hàng Mới
            </button>
</div><div class="h-[1px] bg-outline-variant w-full mb-base"></div>
<a class="text-on-surface-variant hover:bg-surface-container-high hover:text-primary rounded-lg px-4 py-3 flex items-center gap-3 transition-all" href="#">
<span class="material-symbols-outlined" data-icon="settings">settings</span>
<span class="font-body-sm text-body-sm">Cài đặt</span>
</a>
<a class="text-on-surface-variant hover:bg-surface-container-high hover:text-primary rounded-lg px-4 py-3 flex items-center gap-3 transition-all" href="#">
<span class="material-symbols-outlined" data-icon="help_outline">help_outline</span>
<span class="font-body-sm text-body-sm">Trợ giúp</span>
</a>
<div class="flex items-center gap-3 px-4 py-4 mt-2">
<div class="w-10 h-10 rounded-full overflow-hidden border border-outline-variant shrink-0">
<img alt="Bakery Logo" class="w-full h-full object-cover" src="https://lh3.googleusercontent.com/aida-public/AB6AXuBl4TldgsSzvYzpfAruJkAF5olIinHZzbcoPZx62ditGAFc8vFtBDUJxH4wnhhqzIDoUzZ1Y2tjdFfLSAP9iuVA-ToO4QmIC4hWQ1qFeXmmitswfsaakSfH_NijXWhSUccKGHXnc4FyM8G_wDSl6WNPTV8hrXzMbxPSkOTotDzgdZWCg07tyoZKGV0sqWRRxq_xB0_3Eqy4fDfxs4dFbziC8a35UlR8bfwRaXetX4VHe-kqB-t-4GvMF-kMiFiGzJumFywUax5jQw"/>
</div>
<div class="flex flex-col min-w-0">
<span class="font-label-caps text-[11px] text-on-surface font-bold uppercase truncate">Thợ bánh chính</span>
<span class="font-body-sm text-[12px] text-on-surface-variant truncate">Phiên Quản trị</span>
</div>
</div>
</div>
</aside>
<!-- Main Content Canvas -->
<main class="flex-grow flex flex-col min-h-screen">
<!-- TopAppBar -->
<header class="bg-surface/80 backdrop-blur-md border-b border-outline-variant sticky top-0 z-20">
<div class="flex justify-between items-center w-full px-margin py-md">
<div class="flex items-center gap-md">
<h1 class="font-h2 text-h2 font-bold text-primary">Hearth &amp; Harvest</h1>
</div>
<div class="flex items-center gap-gutter">
<div class="relative hidden lg:block">
<span class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant">search</span>
<input class="pl-10 pr-4 py-2 bg-surface-container border border-outline-variant rounded-full text-body-sm w-64 focus:outline-none focus:ring-1 focus:ring-primary focus:border-primary transition-all" placeholder="Tìm kiếm đơn hàng..." type="text"/>
</div>
<div class="flex items-center gap-2">
<button class="material-symbols-outlined text-primary p-2 hover:bg-surface-container-high rounded-full transition-all">shopping_basket</button>
<button class="material-symbols-outlined text-primary p-2 hover:bg-surface-container-high rounded-full transition-all">notifications</button>
<button class="material-symbols-outlined text-primary p-2 hover:bg-surface-container-high rounded-full transition-all">account_circle</button>
</div>
</div>
</div>
</header>
<!-- Dashboard Content -->
<div class="p-margin max-w-[1440px] mx-auto w-full space-y-lg">
<!-- Filter Tabs -->
<div class="flex items-center gap-xs bg-surface-container-low p-1 rounded-xl w-fit border border-outline-variant">
<button class="px-5 py-1.5 rounded-lg bg-primary text-on-primary font-button text-sm shadow-sm transition-all">Ngày</button>
<button class="px-5 py-1.5 rounded-lg text-on-surface-variant font-button text-sm hover:bg-surface-container-high transition-colors">Tháng</button>
<button class="px-5 py-1.5 rounded-lg text-on-surface-variant font-button text-sm hover:bg-surface-container-high transition-colors">Năm</button>
</div>
<!-- Bento Grid Metrics -->
<section class="grid grid-cols-1 md:grid-cols-3 gap-gutter">
<!-- Sales Card -->
<div class="bg-surface-container-lowest border border-outline-variant p-md rounded-xl shadow-sm hover:shadow-md transition-shadow group">
<div class="flex justify-between items-start">
<span class="text-label-caps font-label-caps text-on-surface-variant">DOANH THU</span>
<span class="material-symbols-outlined text-primary group-hover:scale-110 transition-transform">payments</span>
</div>
<div class="flex flex-col mt-2">
<h2 class="font-h1 text-h1 text-on-surface">$2,450</h2>
<div class="flex items-center gap-1 text-secondary mt-1">
<span class="material-symbols-outlined text-sm">trending_up</span>
<span class="text-body-sm">+12.5% so với kỳ trước</span>
</div>
</div>
</div>
<!-- Orders Card -->
<div class="bg-surface-container-lowest border border-outline-variant p-md rounded-xl shadow-sm hover:shadow-md transition-shadow group">
<div class="flex justify-between items-start">
<span class="text-label-caps font-label-caps text-on-surface-variant">ĐƠN HÀNG MỚI</span>
<span class="material-symbols-outlined text-primary group-hover:scale-110 transition-transform">bakery_dining</span>
</div>
<div class="flex flex-col mt-2">
<h2 class="font-h1 text-h1 text-on-surface">32</h2>
<div class="flex items-center gap-1 text-on-surface-variant mt-1">
<span class="material-symbols-outlined text-sm">schedule</span>
<span class="text-body-sm">8 đơn đang nướng</span>
</div>
</div>
</div>
<!-- AOV Card -->
<div class="bg-surface-container-lowest border border-outline-variant p-md rounded-xl shadow-sm hover:shadow-md transition-shadow group">
<div class="flex justify-between items-start">
<span class="text-label-caps font-label-caps text-on-surface-variant">GIÁ TRỊ ĐƠN TRUNG BÌNH</span>
<span class="material-symbols-outlined text-primary group-hover:scale-110 transition-transform">analytics</span>
</div>
<div class="flex flex-col mt-2">
<h2 class="font-h1 text-h1 text-on-surface">$76</h2>
<div class="flex items-center gap-1 text-secondary mt-1">
<span class="material-symbols-outlined text-sm">star</span>
<span class="text-body-sm">Hiệu suất đỉnh điểm buổi sáng</span>
</div>
</div>
</div>
</section>
<!-- Main Dashboard Layout -->
<div class="grid grid-cols-1 lg:grid-cols-12 gap-gutter items-start">
<!-- Batch Progress -->
<section class="lg:col-span-12 bg-surface-container-lowest border border-outline-variant p-md rounded-xl shadow-sm">
<div class="flex justify-between items-center mb-md">
<h3 class="font-h3 text-h3 text-primary">Chu trình Sản xuất Sourdough</h3>
<span class="bg-secondary-container text-on-secondary-container text-label-caps px-4 py-1.5 rounded-full font-bold">Đang nướng</span>
</div>
<div class="relative w-full h-3 bg-surface-container rounded-full overflow-hidden">
<div class="absolute left-0 top-0 h-full bg-primary rounded-full" style="width: 65%;"></div>
</div>
<div class="flex justify-between mt-base text-body-sm text-on-surface-variant font-medium">
<span>Ủ bột</span>
<span class="text-primary font-bold">Đang nướng (65%)</span>
<span>Để nguội</span>
</div>
</section>
<!-- Top Products -->
<section class="lg:col-span-8 bg-surface-container-lowest border border-outline-variant rounded-xl shadow-sm overflow-hidden">
<div class="p-md border-b border-outline-variant flex justify-between items-center">
<h3 class="font-h3 text-h3 text-on-surface">Sản phẩm bán chạy</h3>
<button class="text-primary font-button hover:underline decoration-primary">Xem Kho hàng</button>
</div>
<div class="divide-y divide-outline-variant">
<!-- Product Item -->
<div class="p-md flex items-center justify-between hover:bg-surface-container-low transition-colors group">
<div class="flex items-center gap-md">
<img alt="Sourdough Loaf" class="w-16 h-16 rounded-lg object-cover border border-outline-variant shadow-sm group-hover:scale-105 transition-transform" src="https://lh3.googleusercontent.com/aida-public/AB6AXuD_2oWFg2We5hWN-zshzph_4svZztjjmAJQI4STxiZ1sI0XniCNUl-ZsF4sfYhWyasJmdWuCluNqBC3ezNg28Dz81D_I0mSIXftV2PsmD4kkah04EqwJx1_huXRFl3QxzHMd3qqCZFfOzVw0zk-fSN7HmLFNXFrv5hPxLzFfLeWWkJAtR5OdSbYvetEXva1habla_6Miu2DtgBt9tsNyyYGTX79PE5-OtpSEddtF9ne30LUevcLqYJTBEyRKpukd5A5ekRzCCksqQ"/>
<div>
<h4 class="font-body-lg text-body-lg font-bold text-on-surface">Bánh mì Sourdough</h4>
<p class="font-body-sm text-body-sm text-on-surface-variant">Hỗn hợp Lúa mì Di sản</p>
</div>
</div>
<div class="text-right">
<div class="font-h3 text-h3 text-on-surface">142</div>
<div class="text-label-caps text-on-surface-variant">ĐÃ BÁN HÔM NAY</div>
</div>
</div>
<!-- Product Item -->
<div class="p-md flex items-center justify-between hover:bg-surface-container-low transition-colors group">
<div class="flex items-center gap-md">
<img alt="Cinnamon Rolls" class="w-16 h-16 rounded-lg object-cover border border-outline-variant shadow-sm group-hover:scale-105 transition-transform" src="https://lh3.googleusercontent.com/aida-public/AB6AXuCMG_7T-k8c0E5nbbBaXuFI-ipND0hHW7_kOqP815bgW4XLl9iSACOQd4-38YAHF1N8GbTKmRw8mV6BbSArClQNNp1WMXd9t41lEX_6YI3I1ex2S2k5xccMdXOQ-MPj_EyqxtWu8FkoJO-TqZ2kZpxMlqCph8U19yA5OUrdGxNoo81b3oyFVTa0RIKrbMxBbSIP1dpg3OgdK47lZyZJGf2jxOT1RGYSSudXFOj19l51xFF1hwF4--dRgUTE18qmr8kJ1fnVZCtomQ"/>
<div>
<h4 class="font-body-lg text-body-lg font-bold text-on-surface">Bánh cuộn Quế</h4>
<p class="font-body-sm text-body-sm text-on-surface-variant">Lớp phủ Kem phô mai Đặc trưng</p>
</div>
</div>
<div class="text-right">
<div class="font-h3 text-h3 text-on-surface">98</div>
<div class="text-label-caps text-on-surface-variant">ĐÃ BÁN HÔM NAY</div>
</div>
</div>
<!-- Product Item -->
<div class="p-md flex items-center justify-between hover:bg-surface-container-low transition-colors group">
<div class="flex items-center gap-md">
<img alt="Butter Croissant" class="w-16 h-16 rounded-lg object-cover border border-outline-variant shadow-sm group-hover:scale-105 transition-transform" src="https://lh3.googleusercontent.com/aida-public/AB6AXuBVfdpsisB6mfwajSg8PyjSUNtT67DcYeyb2cT9LChQCxmPGMO1TpcrWOxEi65IXFmZ7f5rC2py2mxfUAa9Ur1Gi4VwnkX-YGZ0Nj0J5v8aVdFKIoCi_Y__hXhOp6UQlqrk2VaoOllzRfqMMRJMlaAbEIBSgc4QSnG4ArGUmSU2x7o8flkdcIkM1w64164jTMIFi1UT0yIBJEL3wfI1Odc_wXdjh4y6XyBjKzV5LELKPD27GiD7Bc_79d21jMpzGS-H1ML5xV_GSQ"/>
<div>
<h4 class="font-body-lg text-body-lg font-bold text-on-surface">Bánh Croissant Bơ</h4>
<p class="font-body-sm text-body-sm text-on-surface-variant">Bơ AOP Isigny</p>
</div>
</div>
<div class="text-right">
<div class="font-h3 text-h3 text-on-surface">86</div>
<div class="text-label-caps text-on-surface-variant">ĐÃ BÁN HÔM NAY</div>
</div>
</div>
</div>
</section>
<!-- Recent Activity Feed -->
<section class="lg:col-span-4 bg-surface-container-lowest border border-outline-variant rounded-xl shadow-sm flex flex-col">
<div class="p-md border-b border-outline-variant">
<h3 class="font-h3 text-h3 text-on-surface">Hoạt động gần đây</h3>
</div>
<div class="p-md space-y-lg flex-grow">
<!-- Activity Item -->
<div class="flex gap-sm relative">
<div class="flex flex-col items-center shrink-0">
<div class="w-2.5 h-2.5 rounded-full bg-primary z-10 border-2 border-surface"></div>
<div class="w-[1px] h-full bg-outline-variant absolute top-1"></div>
</div>
<div class="pb-base">
<p class="font-body-sm text-on-surface leading-tight"><span class="font-bold">Đơn hàng sỉ mới</span> từ 'The Green Bean Café'</p>
<span class="text-label-caps text-[10px] text-on-surface-variant mt-1 block">12 PHÚT TRƯỚC</span>
</div>
</div>
<!-- Activity Item -->
<div class="flex gap-sm relative">
<div class="flex flex-col items-center shrink-0">
<div class="w-2.5 h-2.5 rounded-full bg-secondary z-10 border-2 border-surface"></div>
<div class="w-[1px] h-full bg-outline-variant absolute top-1"></div>
</div>
<div class="pb-base">
<p class="font-body-sm text-on-surface leading-tight"><span class="font-bold">Lô hàng #402 sẵn sàng</span>: Đế bánh đã chuyển sang Để nguội</p>
<span class="text-label-caps text-[10px] text-on-surface-variant mt-1 block">45 PHÚT TRƯỚC</span>
</div>
</div>
<!-- Activity Item -->
<div class="flex gap-sm relative">
<div class="flex flex-col items-center shrink-0">
<div class="w-2.5 h-2.5 rounded-full bg-primary z-10 border-2 border-surface"></div>
<div class="w-[1px] h-full bg-outline-variant absolute top-1"></div>
</div>
<div class="pb-base">
<p class="font-body-sm text-on-surface leading-tight"><span class="font-bold">Cảnh báo kho hàng</span>: Bột lúa mạch đen sắp hết</p>
<span class="text-label-caps text-[10px] text-on-surface-variant mt-1 block">1 GIỜ TRƯỚC</span>
</div>
</div>
<!-- Activity Item -->
<div class="flex gap-sm relative">
<div class="flex flex-col items-center shrink-0">
<div class="w-2.5 h-2.5 rounded-full bg-secondary z-10 border-2 border-surface"></div>
</div>
<div class="">
<p class="font-body-sm text-on-surface leading-tight"><span class="font-bold">Đơn hàng hoàn tất</span>: #8892 (Lấy hàng trực tiếp)</p>
<span class="text-label-caps text-[10px] text-on-surface-variant mt-1 block">2 GIỜ TRƯỚC</span>
</div>
</div>
</div>
<div class="p-md pt-0">
<button class="w-full py-2.5 border border-outline-variant rounded-lg font-button text-on-surface-variant hover:bg-surface-container-high transition-all text-sm">Xem tất cả hoạt động</button>
</div>
</section>
</div>
</div>
<!-- Footer -->
<footer class="mt-auto bg-surface-container-highest border-t border-outline-variant">
<div class="flex flex-col md:flex-row justify-between items-center w-full px-margin py-lg max-w-[1440px] mx-auto gap-md">
<div class="flex flex-col gap-1 text-center md:text-left">
<h2 class="font-h3 text-h3 font-bold text-primary">Hearth &amp; Harvest</h2>
<p class="font-body-sm text-body-sm text-on-surface-variant">© 2024 Tiệm bánh Hearth &amp; Harvest. Chất lượng thủ công, tươi ngon mỗi ngày.</p>
</div>
<div class="flex flex-wrap justify-center gap-x-lg gap-y-2">
<a class="font-body-sm text-body-sm text-on-surface-variant hover:text-primary transition-colors hover:underline decoration-primary" href="#">Câu chuyện</a>
<a class="font-body-sm text-body-sm text-on-surface-variant hover:text-primary transition-colors hover:underline decoration-primary" href="#">Địa điểm</a>
<a class="font-body-sm text-body-sm text-on-surface-variant hover:text-primary transition-colors hover:underline decoration-primary" href="#">Bán sỉ</a>
<a class="font-body-sm text-body-sm text-on-surface-variant hover:text-primary transition-colors hover:underline decoration-primary" href="#">Liên hệ</a>
</div>
</div>
</footer>
</main>

  </div>
</template>

<script setup>
</script>

<style scoped>

        .material-symbols-outlined {
            font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
        }
        body {
            font-family: 'Inter', sans-serif;
            background-color: #fcf9f8;
        }
        .font-h1 { font-family: 'Noto Serif', serif; }
        .font-h2 { font-family: 'Noto Serif', serif; }
        .font-h3 { font-family: 'Noto Serif', serif; }
    
</style>
