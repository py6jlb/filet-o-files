import { defineStore } from 'pinia'
import axios from '../utils/api'

export const useTagsStore = defineStore('tags', {
  state: () => ({
    tags: null,
    loading: false,
    error: null,
  }),
  actions: {
    async fetchTags() {
      this.loading = true
      this.error = null
      try {
        const res = await axios.get('/tags')
        this.tags = res.data
      } catch (e) {
        this.error = e.response?.data?.message || 'Ошибка при получении тегов'
        this.tags = null
      } finally {
        this.loading = false
      }
    },
  },
})
