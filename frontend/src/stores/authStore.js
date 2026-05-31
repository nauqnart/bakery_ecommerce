import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useAuthStore = defineStore('auth', () => {
  const user = ref(JSON.parse(localStorage.getItem('user') || 'null'))
  const token = ref(localStorage.getItem('token') || null)

  function login(userData, jwtToken) {
    user.value = userData
    token.value = jwtToken
    localStorage.setItem('user', JSON.stringify(userData))
    if (jwtToken) {
      localStorage.setItem('token', jwtToken)
    }
  }

  function logout() {
    user.value = null
    token.value = null
    localStorage.removeItem('user')
    localStorage.removeItem('token')
  }

  function getAuthHeaders() {
    if (token.value) {
      return {
        'Authorization': `Bearer ${token.value}`,
        'Content-Type': 'application/json'
      }
    }
    return { 'Content-Type': 'application/json' }
  }

  return { user, token, login, logout, getAuthHeaders }
})
