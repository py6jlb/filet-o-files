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
  <div >
    <div >
      <div v-if="loading" >
        <span ></span>
        <span >Загрузка рецептов...</span>
      </div>
      <div v-else-if="error">
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
