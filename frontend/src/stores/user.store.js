import { defineStore } from 'pinia'
import axios from '../utils/api'

export const useUserStore = defineStore('user', {
  state: () => ({
    user: null,
    loading: false,
    error: null,
  }),
  actions: {
    async fetchProfile() {
      this.loading = true
      this.error = null
      try {
        const res = await axios.get('/users/me')
        this.user = res.data
      } catch (e) {
        this.error = e.response?.data?.message || 'Ошибка при получении профиля'
        this.user = null
      } finally {
        this.loading = false
      }
    },
    clearProfile() {
      this.user = null
      this.error = null
    }
  }
})
