import { defineStore } from 'pinia'
import axios from '../utils/api'

export const useRecipesStore = defineStore('recipes', {
  state: () => ({
    recipes: null,
    loading: false,
    error: null,
  }),
  actions: {
    async fetchRecipes() {
      this.loading = true
      this.error = null
      try {
        const res = await axios.get('/recipes?page=1&pageSize=10')
        this.recipes = res.data
      } catch (e) {
        this.error = e.response?.data?.message || 'Ошибка при получении рецептов'
        this.recipes = null
      } finally {
        this.loading = false
      }
    },
  },
})
