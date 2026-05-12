// stores/cartStore.js — Pinia store cho giỏ hàng
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useCartStore = defineStore('cart', () => {
  // Mỗi item: { variantId, productId, name, sku, price, image, quantity }
  const items = ref(JSON.parse(localStorage.getItem('cart') || '[]'))

  function _save() {
    localStorage.setItem('cart', JSON.stringify(items.value))
  }

  function addItem(product, variant) {
    const existing = items.value.find(i => i.variantId === variant.variantId)
    if (existing) {
      existing.quantity++
    } else {
      items.value.push({
        variantId: variant.variantId,
        productId: product.productId,
        name: product.name,
        sku: variant.sku,
        price: variant.price,
        image: variant.imageUrl,
        quantity: 1
      })
    }
    _save()
  }

  function removeItem(variantId) {
    items.value = items.value.filter(i => i.variantId !== variantId)
    _save()
  }

  function increaseQty(variantId) {
    const item = items.value.find(i => i.variantId === variantId)
    if (item) { item.quantity++; _save() }
  }

  function decreaseQty(variantId) {
    const item = items.value.find(i => i.variantId === variantId)
    if (item) {
      if (item.quantity <= 1) removeItem(variantId)
      else { item.quantity--; _save() }
    }
  }

  function clearCart() {
    items.value = []
    _save()
  }

  const totalItems   = computed(() => items.value.reduce((s, i) => s + i.quantity, 0))
  const totalPrice   = computed(() => items.value.reduce((s, i) => s + i.price * i.quantity, 0))

  return { items, addItem, removeItem, increaseQty, decreaseQty, clearCart, totalItems, totalPrice }
})
