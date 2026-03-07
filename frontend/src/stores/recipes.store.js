import { defineStore } from 'pinia'
import axios from '../utils/api'

export const useRecipesStore = defineStore('recipes', {
  state: () => ({
    recipes: null,
    loading: false,
    error: null,
  }),
  actions: {
    async fetchRecipes(searchQuery = '', tags = [], page = 1, pageSize = 10) {
      this.loading = true
      this.error = null
      try {
        const params = new URLSearchParams()
        params.append('page', page)
        params.append('pageSize', pageSize)

        if (searchQuery) {
          params.append('q', searchQuery)
        }

        if (tags && tags.length > 0) {
          params.append('tags', tags.join(','))
        }

        const res = await axios.get(`/recipes?${params.toString()}`)
        this.recipes = res.data
      } catch (e) {
        this.error = e.response?.data?.message || 'Ошибка при получении рецептов'
        this.recipes = null
      } finally {
        this.loading = false
      }
    },

    async createRecipe(recipeData) {
      this.loading = true
      this.error = null
      try {
        const res = await axios.post('/recipes', recipeData)
        return res.data
      } catch (e) {
        this.error = e.response?.data?.message || 'Ошибка при создании рецепта'
        throw e
      } finally {
        this.loading = false
      }
    },

    async uploadFile(recipeId, file, isTitle = false) {
      const formData = new FormData()
      formData.append('RecipeId', recipeId)
      formData.append('File', file)
      formData.append('IsTitle', isTitle.toString())

      try {
        await axios.post('/files', formData, {
          headers: {
            'Content-Type': 'multipart/form-data',
          },
        })
      } catch (e) {
        this.error = e.response?.data?.message || 'Ошибка при загрузке файла'
        throw e
      }
    },

    async updateRecipeTags(recipeId, tags) {
      // Теги обновляются по одному
      for (const tag of tags) {
        try {
          await axios.put(`/recipes/${recipeId}/tags`, {
            TagId: tag.id,
            AdditionalData: tag.additionalInfo,
          })
        } catch (e) {
          this.error = e.response?.data?.message || 'Ошибка при обновлении тегов'
          throw e
        }
      }
    },
  },
})
