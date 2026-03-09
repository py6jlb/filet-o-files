<template>
  <div>
    <LoadingIndicator v-if="loading" text="Загрузка рецептов..." />
    <ErrorMessage v-else-if="error" :message="error" />
    <template v-else>
      <div v-for="recipe in recipes?.items" :key="recipe.id">
        {{ recipe.title }}
      </div>
    </template>
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useRecipesStore } from '../stores/recipes.store'
import LoadingIndicator from '../components/LoadingIndicator.vue'
import ErrorMessage from '../components/ErrorMessage.vue'

const recipesStore = useRecipesStore()
const { recipes, loading, error } = storeToRefs(recipesStore)

onMounted(() => {
  recipesStore.fetchRecipes()
})
</script>
