<script setup>
import { onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useRecipesStore } from '../stores/recipes.store'

const recipesStore = useRecipesStore()
const { recipes, loading, error } = storeToRefs(recipesStore)

onMounted(() => {
  recipesStore.fetchRecipes()
})
</script>

<template>
  <div class="card lg:card-side shadow-sm bg-base-100">
    <div class="card-body items-center text-center">
      <div v-if="loading" class="flex flex-col items-center py-6">
        <span class="loading loading-spinner loading-lg text-primary mb-3"></span>
        <span class="text-base-content/70">Загрузка рецептов...</span>
      </div>
      <div v-else-if="error" class="alert alert-error mb-3">
        {{ error }}
      </div>
      <template v-else>
        <div v-for="recipe in recipes?.items" :key="recipe.id">
          {{ recipe.title }}
        </div>
      </template>
    </div>
  </div>
</template>
