<template>
  <div>
    <v-toolbar border floating class="w-100" rounded="lg">
      <div class=" d-flex w-100 px-2 py-1 align-center">
        <v-text-field density="comfortable" placeholder="Поиск" variant="solo" clearable flat hide-details single-line />
        <v-btn size="large" class="mx-2">Искать</v-btn>
      </div>

      <template v-slot:prepend>
        <v-btn icon="mdi-plus" to="/add_recipe"></v-btn>
        <v-divider class="mx-1" vertical></v-divider>
      </template>
    </v-toolbar>
    <div>
      <div v-if="loading">
        <span></span>
        <span>Загрузка рецептов...</span>
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
