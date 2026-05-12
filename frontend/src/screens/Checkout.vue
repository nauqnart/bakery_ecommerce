<template>
  <div class="checkout-page">
    <header class="co-header">
      <router-link to="/" class="brand">← Hearth &amp; Harvest</router-link>
      <h1>Thanh toán</h1>
    </header>

    <!-- Success screen -->
    <div v-if="orderDone" class="success-screen">
      <div class="success-card">
        <span class="material-symbols-outlined success-icon">check_circle</span>
        <h2>Đặt hàng thành công!</h2>
        <p>Mã đơn hàng: <strong>#ORD-{{ orderId }}</strong></p>
        <p class="success-sub">Chúng tôi sẽ liên hệ xác nhận và giao hàng sớm nhất có thể.</p>
        <router-link to="/" class="btn-primary">Tiếp tục mua sắm</router-link>
      </div>
    </div>

    <div v-else class="co-body">
      <!-- Left: Form -->
      <section class="co-form">
        <div class="form-section">
          <h2 class="section-title">Thông tin giao hàng</h2>
          <div class="field-row">
            <div class="field">
              <label>Họ tên *</label>
              <input v-model="form.fullName" required placeholder="Nguyễn Văn A" />
            </div>
            <div class="field">
              <label>Số điện thoại *</label>
              <input v-model="form.phone" required placeholder="0901234567" />
            </div>
          </div>
          <div class="field">
            <label>Email</label>
            <input v-model="form.email" type="email" placeholder="example@email.com" />
          </div>
          <div class="field">
            <label>Địa chỉ giao hàng *</label>
            <input v-model="form.address" required placeholder="Số nhà, đường, phường/xã, quận/huyện, tỉnh/thành" />
          </div>
          <div class="field">
            <label>Ghi chú cho đơn hàng</label>
            <textarea v-model="form.note" rows="3" placeholder="VD: Giao trước 18h, gọi trước khi giao..."></textarea>
          </div>
        </div>

        <div class="form-section">
          <h2 class="section-title">Phương thức thanh toán</h2>
          <div class="pay-methods">
            <label class="pay-method" :class="form.payMethod === 'cod' && 'pay-method--active'">
              <input type="radio" v-model="form.payMethod" value="cod" />
              <span class="material-symbols-outlined">payments</span>
              <div><strong>Thanh toán khi nhận hàng (COD)</strong><small>Trả tiền mặt khi nhận</small></div>
            </label>
            <label class="pay-method" :class="form.payMethod === 'bank' && 'pay-method--active'">
              <input type="radio" v-model="form.payMethod" value="bank" />
              <span class="material-symbols-outlined">account_balance</span>
              <div><strong>Chuyển khoản ngân hàng</strong><small>Chuyển khoản trước khi giao</small></div>
            </label>
          </div>
        </div>

        <p v-if="submitErr" class="err-msg">{{ submitErr }}</p>

        <button class="btn-order" :disabled="placing || cart.items.length === 0" @click="placeOrder">
          <span v-if="placing" class="material-symbols-outlined spin">refresh</span>
          {{ placing ? 'Đang đặt hàng...' : `Đặt hàng — ${fmtMoney(cart.totalPrice)}` }}
        </button>
      </section>

      <!-- Right: Order summary -->
      <aside class="co-summary">
        <h2 class="section-title">Đơn hàng của bạn ({{ cart.totalItems }} sản phẩm)</h2>

        <div v-if="cart.items.length === 0" class="empty-cart">
          Giỏ hàng trống. <router-link to="/">Mua sắm ngay →</router-link>
        </div>

        <div v-else class="summary-items">
          <div v-for="item in cart.items" :key="item.variantId" class="sum-item">
            <div class="sum-img-wrap">
              <img v-if="item.image" :src="imgUrl(item.image)" :alt="item.name" class="sum-img" />
              <div v-else class="sum-img-ph"><span class="material-symbols-outlined">bakery_dining</span></div>
              <span class="sum-qty">{{ item.quantity }}</span>
            </div>
            <div class="sum-info">
              <p class="sum-name">{{ item.name }}</p>
              <p class="sum-sku">{{ item.sku }}</p>
            </div>
            <div class="sum-price">{{ fmtMoney(item.price * item.quantity) }}</div>
          </div>
        </div>

        <div class="sum-totals">
          <div class="sum-row"><span>Tạm tính</span><span>{{ fmtMoney(cart.totalPrice) }}</span></div>
          <div class="sum-row"><span>Thuế (10%)</span><span>{{ fmtMoney(cart.totalPrice * 0.1) }}</span></div>
          <div class="sum-row sum-row--total">
            <span>Tổng thanh toán</span>
            <span>{{ fmtMoney(cart.totalPrice * 1.1) }}</span>
          </div>
        </div>
      </aside>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useCartStore } from '../stores/cartStore'

const cart = useCartStore()
const API  = 'http://localhost:5072/api/order'

const form = ref({
  fullName: '', phone: '', email: '',
  address: '', note: '', payMethod: 'cod'
})
const placing   = ref(false)
const submitErr = ref('')
const orderDone = ref(false)
const orderId   = ref(null)

const fmtMoney = v => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v)
const imgUrl   = url => url?.startsWith('http') ? url : `http://localhost:5072${url}`

async function placeOrder() {
  if (!form.value.fullName || !form.value.phone || !form.value.address) {
    submitErr.value = 'Vui lòng điền đầy đủ thông tin họ tên, SĐT và địa chỉ.'; return
  }

  placing.value = true; submitErr.value = ''
  try {
    // Lấy userId từ localStorage (nếu đã đăng nhập)
    const userStr = localStorage.getItem('user')
    const userId  = userStr ? JSON.parse(userStr).userId : 1 // default guest

    const payload = {
      userId,
      shippingAddress: `${form.value.fullName} | ${form.value.phone} | ${form.value.address}`,
      note: form.value.note,
      items: cart.items.map(i => ({
        variantId: i.variantId,
        quantity:  i.quantity
      }))
    }

    const res  = await fetch(API, {
      method:  'POST',
      headers: { 'Content-Type': 'application/json' },
      body:    JSON.stringify(payload)
    })
    const json = await res.json()

    if (!json.success) throw new Error(json.message || 'Đặt hàng thất bại')

    orderId.value = json.data.orderId
    cart.clearCart()
    orderDone.value = true
  } catch (e) {
    submitErr.value = e.message
  } finally {
    placing.value = false
  }
}
</script>

<style scoped>
.material-symbols-outlined { font-family: 'Material Symbols Outlined'; font-size: 20px; line-height: 1; }
.spin { display: inline-block; animation: spin 1s linear infinite; }
@keyframes spin { to { transform: rotate(360deg); } }

/* Layout */
.checkout-page { min-height: 100vh; background: #faf8f5; font-family: 'Inter', sans-serif; }
.co-header { background: #fff; border-bottom: 1px solid #e8e0d8; padding: 1rem 2rem; display: flex; align-items: center; gap: 1.5rem; }
.brand { text-decoration: none; color: #8a7060; font-size: .9rem; }
.brand:hover { color: #b36a3a; }
.co-header h1 { font-size: 1.1rem; font-weight: 700; color: #b36a3a; margin: 0; }
.co-body { display: grid; grid-template-columns: 1fr 420px; gap: 1.5rem; max-width: 1100px; margin: 2rem auto; padding: 0 1.5rem 3rem; }
@media(max-width: 800px) { .co-body { grid-template-columns: 1fr; } }

/* Form */
.co-form { display: flex; flex-direction: column; gap: 1.25rem; }
.form-section { background: #fff; border: 1px solid #e8e0d8; border-radius: 16px; padding: 1.5rem; }
.section-title { font-size: .95rem; font-weight: 700; color: #b36a3a; margin: 0 0 1.1rem; }
.field-row { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; }
.field { display: flex; flex-direction: column; gap: .35rem; margin-bottom: .75rem; }
.field label { font-size: .8rem; font-weight: 600; color: #5a4a3a; }
.field input, .field textarea { padding: .65rem .9rem; border: 1px solid #e0d8d0; border-radius: 8px; font-size: .875rem; background: #faf8f5; outline: none; font-family: inherit; resize: vertical; }
.field input:focus, .field textarea:focus { border-color: #b36a3a; box-shadow: 0 0 0 3px rgba(179,106,58,.1); }

/* Payment methods */
.pay-methods { display: flex; flex-direction: column; gap: .6rem; }
.pay-method { display: flex; align-items: center; gap: .9rem; padding: 1rem; border: 2px solid #e0d8d0; border-radius: 10px; cursor: pointer; transition: all .15s; }
.pay-method input[type=radio] { display: none; }
.pay-method--active { border-color: #b36a3a; background: #fff8f3; }
.pay-method strong { display: block; font-size: .875rem; color: #2c2018; }
.pay-method small   { font-size: .75rem; color: #8a7060; }

/* Buttons */
.btn-order { width: 100%; padding: 1rem; background: #b36a3a; color: #fff; border: none; border-radius: 12px; font-size: 1rem; font-weight: 700; cursor: pointer; display: flex; align-items: center; justify-content: center; gap: .5rem; transition: opacity .15s; }
.btn-order:hover { opacity: .9; }
.btn-order:disabled { opacity: .6; cursor: not-allowed; }
.err-msg { color: #c0392b; font-size: .85rem; }

/* Summary */
.co-summary { background: #fff; border: 1px solid #e8e0d8; border-radius: 16px; padding: 1.5rem; height: fit-content; position: sticky; top: 1rem; }
.empty-cart { text-align: center; color: #8a7060; font-size: .9rem; padding: 1rem 0; }
.empty-cart a { color: #b36a3a; text-decoration: none; }
.summary-items { display: flex; flex-direction: column; gap: .75rem; margin-bottom: 1rem; }
.sum-item { display: flex; align-items: center; gap: .75rem; }
.sum-img-wrap { position: relative; width: 48px; height: 48px; flex-shrink: 0; }
.sum-img { width: 100%; height: 100%; object-fit: cover; border-radius: 8px; }
.sum-img-ph { width: 100%; height: 100%; background: #f5ede6; border-radius: 8px; display: flex; align-items: center; justify-content: center; color: #b36a3a; }
.sum-qty { position: absolute; top: -6px; right: -6px; background: #b36a3a; color: #fff; width: 18px; height: 18px; border-radius: 50%; font-size: .65rem; font-weight: 700; display: flex; align-items: center; justify-content: center; }
.sum-info { flex: 1; min-width: 0; }
.sum-name { font-size: .82rem; font-weight: 600; color: #2c2018; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
.sum-sku  { font-size: .72rem; color: #8a7060; }
.sum-price { font-weight: 700; color: #b36a3a; font-size: .85rem; white-space: nowrap; }
.sum-totals { border-top: 1px solid #f0e8e0; padding-top: .75rem; display: flex; flex-direction: column; gap: .5rem; }
.sum-row { display: flex; justify-content: space-between; font-size: .85rem; color: #6b5c50; }
.sum-row--total { font-size: 1rem; font-weight: 700; color: #2c2018; padding-top: .5rem; border-top: 1px solid #e8e0d8; }

/* Success */
.success-screen { display: flex; align-items: center; justify-content: center; min-height: 70vh; }
.success-card { background: #fff; border: 1px solid #e8e0d8; border-radius: 20px; padding: 3rem; text-align: center; max-width: 420px; width: 100%; }
.success-icon { font-family: 'Material Symbols Outlined'; font-size: 64px; color: #5b7e6b; display: block; margin-bottom: 1rem; }
.success-card h2 { font-size: 1.4rem; font-weight: 700; color: #2c2018; margin: 0 0 .75rem; }
.success-card p { color: #6b5c50; margin: .3rem 0; }
.success-sub { font-size: .85rem; color: #8a7060; margin-top: .5rem; }
.btn-primary { display: inline-block; margin-top: 1.5rem; padding: .75rem 2rem; background: #b36a3a; color: #fff; border-radius: 10px; text-decoration: none; font-weight: 700; }
</style>
