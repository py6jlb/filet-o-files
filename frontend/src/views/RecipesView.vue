<template>
  <div>
    <v-toolbar border floating class="w-100 mb-4" rounded="lg">
      <div class="d-flex w-100 px-2 py-1 align-center flex-wrap ga-2">
        <v-text-field
          v-model="searchQuery"
          density="comfortable"
          placeholder="Поиск по названию"
          variant="solo"
          clearable
          flat
          hide-details
          single-line
          class="flex-grow-1"
          style="min-width: 200px"
        />

        <v-autocomplete
          v-model="selectedTags"
          :items="tagSearchResults"
          item-title="name"
          item-value="id"
          label="Теги"
          variant="solo"
          density="comfortable"
          chips
          closable-chips
          multiple
          return-object
          :loading="tagsLoading"
          hide-details
          class="flex-grow-1"
          style="min-width: 200px"
          @update:search="onTagSearch"
          @update:opened="onMenuOpen"
          cache-items
        >
          <template v-slot:chip="{ props, item }">
            <v-chip v-bind="props" size="small">
              {{ item.raw.name }}
            </v-chip>
          </template>
        </v-autocomplete>

        <v-btn size="large" color="primary" @click="doSearch">
          <v-icon left>mdi-magnify</v-icon>
          Искать
        </v-btn>
      </div>

      <template v-slot:prepend>
        <v-btn icon="mdi-plus" to="/add_recipe"></v-btn>
        <v-divider class="mx-1" vertical></v-divider>
      </template>
    </v-toolbar>

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
              <v-card variant="outlined" class="h-100">
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
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useRecipesStore } from '../stores/recipes.store'
import { useTagsStore } from '../stores/tags.store'
import { debounce } from '../utils/debounce'
import { getContrastColor } from '../utils/colors'

const recipesStore = useRecipesStore()
const tagsStore = useTagsStore()
const { recipes, loading, error } = storeToRefs(recipesStore)

// Состояние поиска
const searchQuery = ref('')
const selectedTags = ref([])
const tagSearchResults = ref([])
const tagsLoading = ref(false)
const currentPage = ref(1)

// Вычисляемое
const totalPages = computed(() => {
  if (!recipes.value?.totalCount || !recipes.value?.pageSize) return 1
  return Math.ceil(recipes.value.totalCount / recipes.value.pageSize)
})

// Debounced поиск тегов
const searchTagsDebounced = debounce(async (query) => {
  if (!query || query.length < 1) {
    tagSearchResults.value = []
    return
  }

  tagsLoading.value = true
  try {
    const result = await tagsStore.searchTags(query)
    tagSearchResults.value = result.items || result || []
  } catch (err) {
    console.error('Ошибка поиска тегов:', err)
    tagSearchResults.value = []
  } finally {
    tagsLoading.value = false
  }
}, 300)

const onTagSearch = (value) => {
  searchTagsDebounced(value || '')
}

const onMenuOpen = async () => {
  if (tagSearchResults.value.length === 0) {
    tagsLoading.value = true
    try {
      const result = await tagsStore.searchTags('')
      tagSearchResults.value = result.items || result || []
    } catch (err) {
      console.error('Ошибка загрузки тегов:', err)
    } finally {
      tagsLoading.value = false
    }
  }
}

// Поиск
const doSearch = () => {
  currentPage.value = 1
  const tags = selectedTags.value.map((t) => t.id)
  recipesStore.fetchRecipes(searchQuery.value, tags, 1)
}

// Смена страницы
const onPageChange = (page) => {
  const tags = selectedTags.value.map((t) => t.id)
  recipesStore.fetchRecipes(searchQuery.value, tags, page)
}

// Получение URL файла
const getFileUrl = (fileId) => {
  const url = `${import.meta.env.VITE_API_BASE_URL}/files/${fileId}`
  console.log(url)
  return url
}

onMounted(() => {
  recipesStore.fetchRecipes()
})
</script>
