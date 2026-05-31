<template>
  <div class="fixed inset-0 z-50 flex justify-end">
    <!-- Backdrop -->
    <div class="absolute inset-0 bg-black/60 backdrop-blur-sm" @click="closeCart"></div>

    <!-- Drawer -->
    <div class="relative w-[460px] max-w-[95vw] bg-white h-full shadow-2xl flex flex-col animate-slide-in">

      <!-- Header -->
      <div class="flex items-center justify-between px-5 py-4 border-b border-stone-200">
        <h2 class="font-bold text-lg text-stone-800 flex items-center gap-2">
          <span class="material-symbols-outlined text-terracotta">shopping_basket</span>
          Giỏ hàng
          <span v-if="cart.totalItems > 0" class="bg-terracotta text-white text-xs font-bold px-2 py-0.5 rounded-full">{{ cart.totalItems }}</span>
        </h2>
        <button @click="closeCart" class="p-1 rounded-full hover:bg-stone-100 text-stone-500">
          <span class="material-symbols-outlined">close</span>
        </button>
      </div>

      <!-- Empty -->
      <div v-if="cart.items.length === 0" class="flex-1 flex flex-col items-center justify-center gap-3 text-stone-400 p-8">
        <span class="material-symbols-outlined text-6xl">shopping_basket</span>
        <p class="font-medium">Giỏ hàng đang trống</p>
        <button @click="closeCart" class="mt-2 px-5 py-2 bg-terracotta text-white rounded-lg text-sm font-semibold">Mua sắm ngay</button>
      </div>

      <!-- Items -->
      <div v-else class="flex-1 overflow-y-auto p-4 space-y-3">
        <div v-for="item in cart.items" :key="item.variantId" class="flex gap-3 bg-stone-50 rounded-xl p-3 border border-stone-100">
          <!-- Image -->
          <div class="w-18 h-18 rounded-lg overflow-hidden bg-stone-200 flex-shrink-0" style="width:72px;height:72px;">
            <img v-if="item.image" :src="imgUrl(item.image)" :alt="item.name" class="w-full h-full object-cover" />
            <div v-else class="w-full h-full flex items-center justify-center text-stone-400">
              <span class="material-symbols-outlined text-3xl">bakery_dining</span>
            </div>
          </div>

          <!-- Info -->
          <div class="flex-1 flex flex-col justify-between min-w-0">
            <div class="flex justify-between items-start gap-1">
              <div>
                <p class="font-semibold text-stone-800 text-sm leading-tight">{{ item.name }}</p>
                <p class="text-xs text-stone-400">SKU: {{ item.sku }}</p>
              </div>
              <button @click="cart.removeItem(item.variantId)" class="text-stone-300 hover:text-red-400 transition-colors flex-shrink-0">
                <span class="material-symbols-outlined text-[18px]">delete</span>
              </button>
            </div>

            <div class="flex items-center justify-between mt-2">
              <!-- Qty control -->
              <div class="flex items-center border border-stone-200 rounded-lg overflow-hidden">
                <button @click="cart.decreaseQty(item.variantId)" class="w-7 h-7 flex items-center justify-center text-stone-500 hover:bg-stone-100">
                  <span class="material-symbols-outlined text-[16px]">remove</span>
                </button>
                <span class="w-7 text-center text-sm font-bold text-stone-700">{{ item.quantity }}</span>
                <button @click="cart.increaseQty(item.variantId)" class="w-7 h-7 flex items-center justify-center text-stone-500 hover:bg-stone-100">
                  <span class="material-symbols-outlined text-[16px]">add</span>
                </button>
              </div>
              <span class="font-bold text-terracotta text-sm">{{ fmtMoney(item.price * item.quantity) }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Footer -->
      <div v-if="cart.items.length > 0" class="p-5 border-t border-stone-200 bg-stone-50 space-y-3">
        <div class="flex justify-between text-sm text-stone-500"><span>Tạm tính</span><span class="font-semibold text-stone-800">{{ fmtMoney(cart.totalPrice) }}</span></div>
        <div class="flex justify-between text-sm text-stone-500"><span>Phí vận chuyển</span><span class="italic">Tính khi thanh toán</span></div>
        <div class="flex justify-between items-center pt-3 border-t border-stone-200">
          <span class="font-bold text-stone-800">Tổng cộng</span>
          <span class="font-bold text-xl text-terracotta">{{ fmtMoney(cart.totalPrice) }}</span>
        </div>

        <button @click="goCheckout" class="w-full bg-terracotta text-white py-3 rounded-xl font-bold text-sm flex items-center justify-center gap-2 hover:opacity-90 transition shadow-md">
          Tiến hành thanh toán <span class="material-symbols-outlined text-[18px]">arrow_forward</span>
        </button>
        <button @click="closeCart" class="w-full text-stone-500 text-sm text-center py-1 hover:text-terracotta transition">← Tiếp tục mua sắm</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { useCartStore } from '../stores/cartStore'

const emit  = defineEmits(['close'])
const cart  = useCartStore()
const router = useRouter()

const fmtMoney = v => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v)
const imgUrl   = url => url?.startsWith('http') ? url : `${import.meta.env.VITE_BASE_URL}\${url}`

function closeCart() { emit('close') }

function goCheckout() {
  emit('close')
  router.push('/checkout')
}
</script>

<style scoped>
.material-symbols-outlined { font-family: 'Material Symbols Outlined'; font-size: 20px; line-height: 1; }
.text-terracotta { color: #b36a3a; }
.bg-terracotta   { background-color: #b36a3a; }

@keyframes slideIn { from { transform: translateX(100%) } to { transform: translateX(0) } }
.animate-slide-in { animation: slideIn .3s cubic-bezier(.4,0,.2,1) forwards; }
</style>
