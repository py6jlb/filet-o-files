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

    async uploadFile(recipeId, file, isTitle = false, preview = null) {
      const formData = new FormData()
      formData.append('RecipeId', recipeId)
      formData.append('File', file)
      formData.append('IsTitle', isTitle.toString())

      if (preview) {
        formData.append('Preview', preview)
      }

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

    async fetchRecipe(id) {
      this.loading = true
      this.error = null
      try {
        const res = await axios.get(`/recipes/${id}`)
        return res.data
      } catch (e) {
        this.error = e.response?.data?.message || 'Ошибка при получении рецепта'
        throw e
      } finally {
        this.loading = false
      }
    },

    async deleteRecipe(id) {
      this.loading = true
      this.error = null
      try {
        await axios.delete(`/recipes/${id}`)
        return true
      } catch (e) {
        this.error = e.response?.data?.message || 'Ошибка при удалении рецепта'
        throw e
      } finally {
        this.loading = false
      }
    },

    async updateRecipe(id, recipeData) {
      this.loading = true
      this.error = null
      try {
        const res = await axios.post(`/recipes/${id}`, recipeData)
        return res.data
      } catch (e) {
        this.error = e.response?.data?.message || 'Ошибка при обновлении рецепта'
        throw e
      } finally {
        this.loading = false
      }
    },

    async deleteFile(fileId) {
      try {
        await axios.delete(`/files/${fileId}`)
        return true
      } catch (e) {
        this.error = e.response?.data?.message || 'Ошибка при удалении файла'
        throw e
      }
    },

    async removeRecipeTag(recipeId, tagId) {
      try {
        await axios.delete(`/recipes/${recipeId}/tags/${tagId}`)
        return true
      } catch (e) {
        this.error = e.response?.data?.message || 'Ошибка при удалении тега'
        throw e
      }
    },

    async deleteRecipeWithDependencies(recipe) {
      this.loading = true
      this.error = null

      try {
        // 1. Удаляем все файлы рецепта
        if (recipe.files && recipe.files.length > 0) {
          for (const file of recipe.files) {
            await this.deleteFile(file.id)
          }
        }

        // 2. Удаляем связи с тегами
        if (recipe.tags && recipe.tags.length > 0) {
          for (const tag of recipe.tags) {
            await this.removeRecipeTag(recipe.id, tag.id)
          }
        }

        // 3. Удаляем сам рецепт
        await this.deleteRecipe(recipe.id)

        return true
      } catch (e) {
        this.error = e.response?.data?.message || 'Ошибка при удалении рецепта'
        throw e
      } finally {
        this.loading = false
      }
    },
  },
})
