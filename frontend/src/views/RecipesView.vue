<template>
  <div>
    <!-- Панель поиска -->
    <v-toolbar border floating class="w-100 mb-3" rounded="lg" density="comfortable">
      <div class="d-flex w-100 px-2 py-1 align-center flex-wrap ga-2">
        <!-- Поиск по названию -->
        <v-text-field
          v-model="searchQuery"
          density="compact"
          placeholder="Поиск..."
          variant="solo"
          clearable
          flat
          hide-details
          single-line
          class="flex-grow-1"
          style="min-width: 200px"
          @keyup.enter="doSearch"
        />

        <!-- Кнопка поиска -->
        <v-btn icon="mdi-magnify" color="primary" @click="doSearch"></v-btn>

        <!-- Кнопка фильтра по тегам -->
        <v-btn
          icon="mdi-filter-variant"
          :color="selectedTagIds.length > 0 ? 'primary' : 'default'"
          @click="openFilterDialog"
        >
          <v-badge
            v-if="selectedTagIds.length > 0"
            dot
            color="error"
            location="top right"
            :offset-x="-5"
            :offset-y="-5"
          >
            <v-icon>mdi-filter-variant</v-icon></v-badge
          >
          <v-icon v-else>mdi-filter-variant</v-icon>
        </v-btn>
      </div>

      <template v-slot:prepend>
        <v-btn icon="mdi-plus" to="/add_recipe" color="primary"></v-btn>
      </template>
    </v-toolbar>

    <!-- Выбранные теги -->
    <div v-if="selectedTagIds.length > 0" class="mb-4 d-flex align-center flex-wrap ga-2">
      <v-chip
        v-for="tag in selectedTagsChips"
        :key="tag.id"
        closable
        size="small"
        :style="{
          backgroundColor: tag.color || '#757575',
          color: getContrastColor(tag.color),
        }"
        @click:close="removeTag(tag.id)"
      >
        {{ tag.name }}
      </v-chip>
    </div>

    <div>
      <div v-if="loading" class="text-center py-8">
        <v-progress-circular indeterminate color="primary"></v-progress-circular>
        <div class="mt-2">Загрузка рецептов...</div>
      </div>
      <div v-else-if="error" class="text-center py-8 text-error">
        {{ error }}
      </div>
      <template v-else>
        <div v-if="!recipes?.items?.length" class="text-center py-8 text-grey">
          Рецепты не найдены
        </div>
        <div v-else>
          <v-row>
            <v-col v-for="recipe in recipes.items" :key="recipe.id" cols="12" sm="6" md="4">
              <v-card
                variant="outlined"
                class="h-100 cursor-pointer"
                @click="goToRecipe(recipe.id)"
              >
                <v-img
                  v-if="recipe.files?.length > 0"
                  :src="getFileUrl(recipe.files[0].id)"
                  height="150"
                  cover
                  class="bg-grey-lighten-2"
                ></v-img>
                <v-img
                  v-else
                  height="150"
                  cover
                  class="bg-grey-lighten-2 d-flex align-center justify-center"
                >
                  <v-icon size="48" color="grey">mdi-food</v-icon>
                </v-img>
                <v-card-title>{{ recipe.title }}</v-card-title>
                <v-card-text v-if="recipe.descriptions">
                  {{ recipe.descriptions }}
                </v-card-text>
                <v-card-text v-if="recipe.tags?.length">
                  <v-chip
                    v-for="tag in recipe.tags"
                    :key="tag.id"
                    size="x-small"
                    class="mr-1 mb-1"
                    :style="{
                      backgroundColor: tag.color || '#757575',
                      color: getContrastColor(tag.color),
                    }"
                  >
                    {{ tag.name }}
                  </v-chip>
                </v-card-text>
              </v-card>
            </v-col>
          </v-row>

          <!-- Пагинация -->
          <div v-if="recipes?.totalCount > recipes?.pageSize" class="d-flex justify-center mt-4">
            <v-pagination
              v-model="currentPage"
              :length="totalPages"
              :total-visible="5"
              @update:model-value="onPageChange"
            ></v-pagination>
          </div>
        </div>
      </template>
    </div>

    <!-- Диалог фильтра по тегам -->
    <FilterDialog
      v-model="filterDialog"
      :selected-tags="selectedTagIds"
      @update="onTagsFilterUpdate"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { storeToRefs } from 'pinia'
import { useRecipesStore } from '../stores/recipes.store'
import { useTagsStore } from '../stores/tags.store'
import FilterDialog from '../components/FilterDialog.vue'
import { getContrastColor } from '../utils/colors'

const router = useRouter()
const recipesStore = useRecipesStore()
const tagsStore = useTagsStore()
const { recipes, loading, error } = storeToRefs(recipesStore)

// Состояние поиска
const searchQuery = ref('')
const selectedTagIds = ref([])
const selectedTagsChips = ref([]) // Для отображения чипов
const filterDialog = ref(false)
const currentPage = ref(1)

// Вычисляемое
const totalPages = computed(() => {
  if (!recipes.value?.totalCount || !recipes.value?.pageSize) return 1
  return Math.ceil(recipes.value.totalCount / recipes.value.pageSize)
})

// Открытие диалога фильтра
const openFilterDialog = () => {
  filterDialog.value = true
}

// Обновление тегов после фильтрации
const onTagsFilterUpdate = async (tagIds) => {
  selectedTagIds.value = tagIds

  // Загружаем информацию о выбранных тегах для отображения чипов
  if (tagIds.length > 0) {
    try {
      const result = await tagsStore.searchTags('')
      const allTags = result.items || result || []
      selectedTagsChips.value = allTags.filter((t) => tagIds.includes(t.id))
    } catch (err) {
      console.error('Ошибка загрузки тегов:', err)
    }
  } else {
    selectedTagsChips.value = []
  }
}

// Удаление тега из фильтра
const removeTag = (tagId) => {
  selectedTagIds.value = selectedTagIds.value.filter((id) => id !== tagId)
  selectedTagsChips.value = selectedTagsChips.value.filter((t) => t.id !== tagId)
}

// Поиск
const doSearch = () => {
  currentPage.value = 1
  recipesStore.fetchRecipes(searchQuery.value, selectedTagIds.value, 1)
}

// Смена страницы
const onPageChange = (page) => {
  recipesStore.fetchRecipes(searchQuery.value, selectedTagIds.value, page)
}

// Получение URL файла
const getFileUrl = (fileId) => {
  return `${import.meta.env.VITE_API_BASE_URL}/files/${fileId}`
}

const goToRecipe = (id) => {
  router.push(`/recipe/${id}`)
}

onMounted(() => {
  recipesStore.fetchRecipes()
})
</script>

<style scoped>
.cursor-pointer {
  cursor: pointer;
}
</style>
