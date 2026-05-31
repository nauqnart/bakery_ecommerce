// stores/cartStore.js — Pinia store cho giỏ hàng
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useCartStore = defineStore('cart', () => {
  const items = ref(JSON.parse(localStorage.getItem('cart') || '[]'))
  const isSyncing = ref(false)
  
  function getHeaders() {
    const token = localStorage.getItem('token')
    return token ? { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' } : null
  }

  async function fetchCart() {
    const headers = getHeaders()
    if (!headers) return
    
    isSyncing.value = true
    try {
      const res = await fetch(import.meta.env.VITE_API_BASE_URL + '/cart', { headers })
      if (res.status === 401) {
        localStorage.removeItem('token')
        localStorage.removeItem('user')
        window.location.reload()
        return
      }
      const data = await res.json()
      if (data.success && data.data) {
        items.value = data.data.items.map(i => ({
          cartItemId: i.cartItemId,
          variantId: i.variantId,
          productId: 0,
          name: i.productName,
          sku: i.sku,
          price: i.price,
          image: i.imageUrl,
          quantity: i.quantity
        }))
        _saveLocal()
      }
    } catch (err) {
      console.error('Failed to fetch cart', err)
    } finally {
      isSyncing.value = false
    }
  }

  async function syncLocalCart() {
    const headers = getHeaders()
    if (!headers || items.value.length === 0) return
    
    // Add each local item to backend sequentially
    const localItems = [...items.value]
    for (const item of localItems) {
      try {
        await fetch(import.meta.env.VITE_API_BASE_URL + '/cart/items', {
          method: 'POST',
          headers,
          body: JSON.stringify({
            variantId: item.variantId,
            quantity: item.quantity
          })
        })
      } catch (e) {
        console.error('Failed to sync item', e)
      }
    }
  }

  function _saveLocal() {
    localStorage.setItem('cart', JSON.stringify(items.value))
  }

  async function addItem(product, variant) {
    const headers = getHeaders()
    if (headers) {
      // API call
      try {
        const res = await fetch(import.meta.env.VITE_API_BASE_URL + '/cart/items', {
          method: 'POST',
          headers,
          body: JSON.stringify({ variantId: variant.variantId, quantity: 1 })
        })
        if (res.status === 401) {
          localStorage.removeItem('token')
          localStorage.removeItem('user')
          window.location.reload()
          return
        }
        if (!res.ok) {
           const errData = await res.json()
           console.error('Lỗi khi thêm vào giỏ:', errData)
        }
        await fetchCart()
      } catch (err) { console.error(err) }
    } else {
        // Local
        const existing = items.value.find(i => i.variantId === variant.variantId)
        if (existing) existing.quantity++
        else {
          items.value.push({
            variantId: variant.variantId,
            productId: product.productId,
            name: product.name,
            sku: variant.sku,
            price: variant.price,
            image: variant.imageUrl || (product.variants && product.variants.find(v => v?.imageUrl)?.imageUrl) || '',
            quantity: 1
          })
        }
        _saveLocal()
    }
  }

  async function updateQty(variantId, quantity, cartItemId) {
    const headers = getHeaders()
    if (headers && cartItemId) {
      try {
        await fetch(`${import.meta.env.VITE_API_BASE_URL}/cart/items/${cartItemId}`, {
          method: 'PUT',
          headers,
          body: JSON.stringify({ quantity })
        })
        await fetchCart()
      } catch (err) { console.error(err) }
    } else {
      const item = items.value.find(i => i.variantId === variantId)
      if (item) {
        item.quantity = quantity
        if (item.quantity <= 0) items.value = items.value.filter(i => i.variantId !== variantId)
        _saveLocal()
      }
    }
  }

  function increaseQty(variantId) {
    const item = items.value.find(i => i.variantId === variantId)
    if (item) updateQty(variantId, item.quantity + 1, item.cartItemId)
  }

  function decreaseQty(variantId) {
    const item = items.value.find(i => i.variantId === variantId)
    if (item) updateQty(variantId, item.quantity - 1, item.cartItemId)
  }

  async function removeItem(variantId) {
    const item = items.value.find(i => i.variantId === variantId)
    const headers = getHeaders()
    if (headers && item?.cartItemId) {
      try {
        await fetch(`${import.meta.env.VITE_API_BASE_URL}/cart/items/${item.cartItemId}`, {
          method: 'DELETE',
          headers
        })
        await fetchCart()
      } catch (err) { console.error(err) }
    } else {
      items.value = items.value.filter(i => i.variantId !== variantId)
      _saveLocal()
    }
  }

  async function clearCart() {
    const headers = getHeaders()
    if (headers) {
      try {
        await fetch(import.meta.env.VITE_API_BASE_URL + '/cart', { method: 'DELETE', headers })
      } catch (err) {}
    }
    items.value = []
    _saveLocal()
  }

  const totalItems   = computed(() => items.value.reduce((s, i) => s + i.quantity, 0))
  const totalPrice   = computed(() => items.value.reduce((s, i) => s + i.price * i.quantity, 0))

  return { items, isSyncing, fetchCart, syncLocalCart, addItem, removeItem, increaseQty, decreaseQty, clearCart, totalItems, totalPrice }
})
