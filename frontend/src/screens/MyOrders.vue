<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 min-h-screen">
    <div class="flex items-center gap-4 mb-8">
      <router-link to="/" class="p-2 rounded-full hover:bg-stone-100 text-stone-500 transition-colors flex items-center justify-center">
        <span class="material-symbols-outlined">arrow_back</span>
      </router-link>
      <h1 class="text-3xl font-bold text-[#b36a3a]">Lịch sử đơn hàng</h1>
    </div>
    
    <div v-if="loading" class="text-center py-10">Đang tải...</div>
    <div v-else-if="orders.length === 0" class="text-center py-10 text-gray-500">
      Bạn chưa có đơn hàng nào.
      <router-link to="/" class="text-[#b36a3a] hover:underline ml-2">Mua sắm ngay</router-link>
    </div>
    
    <div v-else class="space-y-6">
      <div v-for="order in orders" :key="order.orderId" class="bg-white border border-[#e8e0d8] rounded-xl overflow-hidden shadow-sm">
        <div class="bg-[#faf8f5] px-6 py-4 border-b border-[#e8e0d8] flex justify-between items-center flex-wrap gap-4">
          <div>
            <p class="text-sm text-gray-500">Mã đơn hàng</p>
            <p class="font-bold">#ORD-{{ order.orderId }}</p>
          </div>
          <div>
            <p class="text-sm text-gray-500">Ngày đặt</p>
            <p class="font-semibold">{{ new Date(order.createdAt).toLocaleDateString('vi-VN') }}</p>
          </div>
          <div>
            <p class="text-sm text-gray-500">Tổng tiền</p>
            <p class="font-bold text-[#b36a3a]">{{ fmtMoney(order.totalPrice) }}</p>
          </div>
          <div class="flex flex-col items-end gap-2">
            <span :class="getStatusClass(order.orderStatus)" class="px-3 py-1 rounded-full text-xs font-bold uppercase tracking-wider">
              {{ formatStatus(order.orderStatus) }}
            </span>
            <button v-if="order.orderStatus === 'pending'" @click="showPayment(order.orderId)" class="bg-[#b36a3a] text-white px-3 py-1.5 rounded-md text-xs font-bold shadow hover:bg-[#8a7060] transition-colors flex items-center gap-1">
              <span class="material-symbols-outlined text-[14px]">qr_code_scanner</span> Thanh toán QR
            </button>
          </div>
        </div>
        
        <div class="px-6 py-4">
          <h4 class="font-semibold mb-3">Sản phẩm</h4>
          <div class="space-y-3">
            <div v-for="item in order.items" :key="item.orderItemId" class="flex items-center gap-4">
              <img :src="imgUrl(item.variant?.imageUrl || item.variant?.product?.variants?.find(v => v?.imageUrl)?.imageUrl)" class="w-16 h-16 object-cover rounded-lg border border-gray-100" />
              <div class="flex-1">
                <p class="font-medium">{{ item.variant?.product?.name || 'Sản phẩm' }}</p>
                <p class="text-sm text-gray-500">{{ item.variant?.sku }}</p>
              </div>
              <div class="text-right">
                <p>{{ fmtMoney(item.priceAtPurchase) }}</p>
                <p class="text-sm text-gray-500">x{{ item.quantity }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- QR Modal -->
    <div v-if="showQrModal" class="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" @click="closeQrModal"></div>
      <div class="relative bg-white rounded-2xl shadow-xl max-w-sm w-full p-6 flex flex-col items-center animate-slide-up">
        <button @click="closeQrModal" class="absolute top-3 right-3 p-1 rounded-full text-gray-400 hover:bg-gray-100">
          <span class="material-symbols-outlined">close</span>
        </button>
        
        <h3 class="text-xl font-bold text-[#b36a3a] mb-2 flex items-center gap-2">
          <span class="material-symbols-outlined">account_balance</span> Chuyển khoản
        </h3>
        <p class="text-sm text-gray-500 mb-6 text-center">Mã đơn hàng: #ORD-{{ selectedOrderId }}<br>Vui lòng quét mã QR bên dưới để thanh toán.</p>
        
        <div v-if="loadingQr" class="w-48 h-48 flex items-center justify-center bg-gray-50 rounded-xl border border-gray-100">
          <span class="material-symbols-outlined animate-spin text-3xl text-gray-400">refresh</span>
        </div>
        <img v-else-if="qrUrl" :src="qrUrl" class="w-56 h-56 object-contain rounded-xl border border-gray-100 shadow-sm" alt="Payment QR" />
        <div v-else class="w-48 h-48 flex items-center justify-center bg-gray-50 text-red-500 text-sm text-center px-4 rounded-xl border border-gray-100">
          Lỗi tải mã QR. Vui lòng thử lại.
        </div>
        
        <button @click="closeQrModal" class="mt-6 w-full py-2.5 bg-gray-100 text-gray-700 font-semibold rounded-xl hover:bg-gray-200 transition-colors">
          Đóng
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const orders = ref([])
const loading = ref(true)

const showQrModal = ref(false)
const qrUrl = ref(null)
const loadingQr = ref(false)
const selectedOrderId = ref(null)

const fmtMoney = v => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v)
const imgUrl = url => url?.startsWith('http') ? url : (url ? `${import.meta.env.VITE_BASE_URL}\${url}` : 'https://placehold.co/100x100?text=No+Image')

const getStatusClass = (status) => {
  const s = status ? status.toLowerCase() : '';
  switch(s) {
    case 'pending': return 'bg-yellow-100 text-yellow-800'
    case 'processing': return 'bg-blue-100 text-blue-800'
    case 'completed': return 'bg-green-100 text-green-800'
    case 'cancelled': return 'bg-red-100 text-red-800'
    default: return 'bg-gray-100 text-gray-800'
  }
}

const formatStatus = (s) => {
  const map = { pending: 'Chờ xử lý', processing: 'Đang chuẩn bị', completed: 'Hoàn thành', cancelled: 'Đã hủy' };
  return map[s] || s || 'Không xác định';
}

onMounted(async () => {
  try {
    const userStr = localStorage.getItem('user')
    if (!userStr) {
      window.location.href = '/login'
      return
    }
    const token = JSON.parse(userStr).token
    
    const res = await fetch(import.meta.env.VITE_API_BASE_URL + '/order/my-orders', {
      headers: { 'Authorization': `Bearer ${token}` }
    })
    const data = await res.json()
    if(data.success) {
      orders.value = data.data
    }
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
})

async function showPayment(orderId) {
  selectedOrderId.value = orderId;
  showQrModal.value = true;
  loadingQr.value = true;
  qrUrl.value = null;
  
  try {
    const userStr = localStorage.getItem('user');
    if (!userStr) return;
    const token = JSON.parse(userStr).token;
    
    const qrRes = await fetch(`${import.meta.env.VITE_API_BASE_URL}/payment/vietqr/${orderId}`, {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    const qrJson = await qrRes.json();
    if (qrJson.success) {
      qrUrl.value = qrJson.qrUrl;
    }
  } catch (err) {
    console.error("Error fetching QR:", err);
  } finally {
    loadingQr.value = false;
  }
}

function closeQrModal() {
  showQrModal.value = false;
  qrUrl.value = null;
  selectedOrderId.value = null;
}
</script>

<style scoped>
.animate-slide-up { animation: slideUp 0.3s cubic-bezier(0.16, 1, 0.3, 1) forwards; }
@keyframes slideUp { from { opacity: 0; transform: translateY(20px); } to { opacity: 1; transform: translateY(0); } }
</style>
