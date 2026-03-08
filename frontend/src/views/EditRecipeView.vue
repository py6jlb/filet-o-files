<template>
  <div>
    <!-- Загрузка -->
    <div v-if="loading" class="text-center py-8">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
      <div class="mt-2">Загрузка рецепта...</div>
    </div>

    <!-- Ошибка -->
    <div v-else-if="error" class="text-center py-8">
      <v-alert type="error" variant="tonal" class="mb-4">
        {{ error }}
      </v-alert>
      <v-btn color="primary" to="/">Вернуться к списку рецептов</v-btn>
    </div>

    <!-- Форма редактирования -->
    <div v-else>
      <v-form ref="form" v-model="formValid" @submit.prevent="submitForm">
        <v-row>
          <v-col cols="12">
            <v-text-field
              v-model="recipe.title"
              label="Название рецепта *"
              :rules="[rules.required]"
              variant="outlined"
              prepend-inner-icon="mdi-food"
              required
            ></v-text-field>
          </v-col>
        </v-row>

        <!-- Кнопка добавления файлов -->
        <v-row>
          <v-col cols="12">
            <input
              ref="fileInput"
              type="file"
              multiple
              accept="image/*,.pdf"
              style="display: none"
              @change="handleFileSelect"
            />
            <v-btn variant="outlined" prepend-icon="mdi-paperclip" @click="triggerFileSelect">
              Добавить файлы
            </v-btn>

            <!-- Превью новых изображений -->
            <div v-if="newFilesPreview.length > 0" class="mt-3 files-preview-container">
              <div class="text-subtitle-1 mb-2">Новые файлы</div>
              <div class="d-flex flex-wrap gap-2">
                <div
                  v-for="(preview, index) in newFilesPreview"
                  :key="index"
                  class="file-preview-item"
                >
                  <v-img
                    v-if="preview.type === 'image'"
                    :src="preview.url"
                    width="80"
                    height="80"
                    cover
                    class="rounded-lg elevation-1"
                  ></v-img>
                  <div
                    v-else
                    class="pdf-preview rounded-lg elevation-1 d-flex align-center justify-center"
                  >
                    <v-icon size="32" color="red">mdi-file-pdf-box</v-icon>
                  </div>
                  <v-btn
                    icon="mdi-close"
                    size="x-small"
                    color="error"
                    class="remove-file-btn"
                    @click.stop="removeNewFile(index)"
                  ></v-btn>
                </div>
              </div>
            </div>
          </v-col>
        </v-row>

        <!-- Существующие файлы -->
        <v-row v-if="existingFiles.length > 0">
          <v-col cols="12">
            <div class="text-subtitle-1 mb-2">Текущие файлы</div>
            <div class="d-flex flex-wrap ga-2 files-preview-container">
              <div
                v-for="file in existingFiles"
                :key="file.id"
                class="file-preview-item position-relative"
              >
                <v-img
                  v-if="file.mimeType?.startsWith('image')"
                  :src="getFileUrl(file.id)"
                  width="80"
                  height="80"
                  cover
                  class="rounded-lg elevation-1"
                ></v-img>
                <div
                  v-else
                  class="pdf-preview rounded-lg elevation-1 d-flex align-center justify-center"
                  style="width: 80px; height: 80px"
                >
                  <v-icon size="32" color="red">mdi-file-pdf-box</v-icon>
                </div>
                <v-btn
                  icon="mdi-close"
                  size="x-small"
                  color="error"
                  class="remove-file-btn"
                  @click.stop="removeExistingFile(file.id)"
                ></v-btn>
              </div>
            </div>
          </v-col>
        </v-row>

        <v-row>
          <v-col cols="12">
            <MarkdownEditor
              v-model="recipe.description"
              label="Инструкции приготовления"
              placeholder="Опишите процесс приготовления в формате Markdown..."
            ></MarkdownEditor>
          </v-col>
        </v-row>

        <!-- Блок выбора тегов -->
        <v-row>
          <v-col cols="12">
            <v-autocomplete
              v-model="selectedTagInput"
              :items="tagSearchResults"
              item-title="name"
              item-value="id"
              label="Теги"
              variant="outlined"
              prepend-inner-icon="mdi-tag-multiple"
              chips
              closable-chips
              multiple
              return-object
              density="compact"
              :loading="tagsLoading"
              @update:search="onTagSearch"
              @update:model-value="onTagsSelected"
              @update:opened="onMenuOpen"
              cache-items
            >
              <template v-slot:chip="{ props, item }">
                <v-chip
                  v-bind="props"
                  closable
                  @click:close="removeTag(item.raw)"
                  size="small"
                  :style="{
                    backgroundColor: item.raw.color || '#757575',
                    color: getContrastColor(item.raw.color),
                  }"
                >
                  {{ item.raw.name }}
                </v-chip>
              </template>
              <template v-slot:no-data>
                <v-list-item>
                  <template v-slot:append>
                    <v-btn
                      color="primary"
                      variant="text"
                      size="small"
                      @click="createNewTag"
                      :disabled="!tagSearchQuery"
                    >
                      <v-icon left size="small">mdi-plus</v-icon>
                      Создать "{{ tagSearchQuery }}"
                    </v-btn>
                  </template>
                </v-list-item>
              </template>
            </v-autocomplete>
          </v-col>
        </v-row>

        <!-- Дополнительная информация для выбранных тегов -->
        <v-row v-if="recipeTags.length > 0" class="mt-0">
          <v-col cols="12">
            <div class="d-flex flex-wrap align-center ga-2">
              <div v-for="tagInfo in recipeTags" :key="tagInfo.id" class="d-flex align-center">
                <v-chip
                  size="small"
                  :style="{
                    backgroundColor: tagInfo.color || '#757575',
                    color: getContrastColor(tagInfo.color),
                  }"
                >
                  {{ tagInfo.name }}
                </v-chip>
                <v-text-field
                  v-model="tagInfo.additionalInfo"
                  density="compact"
                  variant="outlined"
                  placeholder="Инфо"
                  class="ml-2 tag-info-input"
                  hide-details
                ></v-text-field>
              </div>
            </div>
          </v-col>
        </v-row>

        <!-- Кнопки действий -->
        <v-row class="mt-6">
          <v-col cols="12" class="d-flex justify-center ga-2">
            <v-btn
              type="submit"
              color="success"
              size="large"
              variant="elevated"
              :disabled="!formValid"
              :loading="submitting"
            >
              <v-icon left>mdi-content-save</v-icon>
              Сохранить изменения
            </v-btn>

            <v-btn
              type="button"
              color="grey"
              size="large"
              variant="outlined"
              @click="cancel"
              :loading="submitting"
            >
              <v-icon left>mdi-arrow-left</v-icon>
              Отмена
            </v-btn>
          </v-col>
        </v-row>
      </v-form>
    </div>

    <!-- Сообщения -->
    <v-snackbar v-model="snackbar.show" :color="snackbar.color" :timeout="3000" top>
      {{ snackbar.text }}
      <template v-slot:actions>
        <v-btn color="white" variant="text" @click="snackbar.show = false"> Закрыть </v-btn>
      </template>
    </v-snackbar>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'

import { useTagsStore } from '../stores/tags.store'
import { useRecipesStore } from '../stores/recipes.store'
import MarkdownEditor from '../components/MarkdownEditor.vue'
import api from '../utils/api'
import { debounce } from '../utils/debounce'
import { getContrastColor } from '../utils/colors'

const route = useRoute()
const router = useRouter()

const tagStore = useTagsStore()
const recipesStore = useRecipesStore()

const form = ref()
const formValid = ref(false)
const submitting = ref(false)
const loading = ref(true)
const error = ref(null)
const newFilesPreview = ref([])
const tagsLoading = ref(false)
const fileInput = ref(null)

// Состояние для тегов
const selectedTagInput = ref([])
const tagSearchQuery = ref('')
const recipeTags = ref([]) // Массив { id, name, additionalInfo }
const tagSearchResults = ref([]) // Результаты поиска с сервера

const recipe = reactive({
  title: '',
  description: '',
})

const newFiles = ref([])
const existingFiles = ref([])
const deletedFileIds = ref([])

const snackbar = reactive({
  show: false,
  text: '',
  color: 'success',
})

const rules = {
  required: (value) => !!value || 'Обязательное поле',
  positiveNumber: (value) => !value || value > 0 || 'Должно быть больше 0',
}

const getFileUrl = (fileId) => {
  return `${import.meta.env.VITE_API_BASE_URL}/files/${fileId}`
}

// Загрузка рецепта
const loadRecipe = async () => {
  loading.value = true
  error.value = null

  try {
    const id = route.params.id
    const data = await recipesStore.fetchRecipe(id)

    recipe.title = data.title || ''
    recipe.description = data.descriptions || ''

    // Загружаем файлы
    existingFiles.value = data.files || []

    // Загружаем теги
    if (data.tags && data.tags.length > 0) {
      selectedTagInput.value = data.tags.map((t) => ({
        id: t.id,
        name: t.name,
        color: t.color,
      }))
      recipeTags.value = data.tags.map((t) => ({
        id: t.id,
        name: t.name,
        color: t.color,
        additionalInfo: t.additionalInfo || '',
      }))
    }

    // Загружаем все теги для автодополнения
    const tagsResult = await tagStore.searchTags('')
    tagSearchResults.value = tagsResult.items || tagsResult || []
  } catch (e) {
    error.value = e.response?.data?.message || 'Не удалось загрузить рецепт'
  } finally {
    loading.value = false
  }
}

// Debounced поиск тегов (задержка 300мс)
const searchTagsDebounced = debounce(async (query) => {
  if (!query || query.length < 1) {
    tagSearchResults.value = []
    return
  }

  tagsLoading.value = true
  try {
    const result = await tagStore.searchTags(query)
    // API возвращает { items: [...], totalCount, page, pageSize }
    tagSearchResults.value = result.items || result || []
  } catch (error) {
    console.error('Ошибка поиска тегов:', error)
    tagSearchResults.value = []
  } finally {
    tagsLoading.value = false
  }
}, 300)

// Обработчик поиска тегов - вызывается при вводе текста
const onTagSearch = (value) => {
  tagSearchQuery.value = value || ''
  searchTagsDebounced(value || '')
}

// Обработчик открытия меню - загружаем популярные теги или пустой список
const onMenuOpen = async () => {
  // Если есть выбранные теги, показываем только их
  if (selectedTagInput.value.length > 0) {
    tagSearchResults.value = selectedTagInput.value
  } else if (tagSearchResults.value.length === 0 && !tagSearchQuery.value) {
    // При открытии без запроса - загружаем все теги (первые 20)
    tagsLoading.value = true
    try {
      const result = await tagStore.searchTags('')
      tagSearchResults.value = result.items || result || []
    } catch (error) {
      console.error('Ошибка загрузки тегов:', error)
    } finally {
      tagsLoading.value = false
    }
  }
}

// Обработчик выбора тегов
const onTagsSelected = (selected) => {
  // Синхронизируем recipeTags с выбранными тегами
  const existingIds = recipeTags.value.map((t) => t.id)

  // Добавляем новые теги
  selected.forEach((tag) => {
    if (!existingIds.includes(tag.id)) {
      recipeTags.value.push({
        id: tag.id,
        name: tag.name,
        color: tag.color,
        additionalInfo: '',
      })
    }
  })
  // Удаляем теги, которые были убраны
  const selectedIds = selected.map((t) => t.id)
  recipeTags.value = recipeTags.value.filter((t) => selectedIds.includes(t.id))
}

// Удаление тега
const removeTag = (tag) => {
  selectedTagInput.value = selectedTagInput.value.filter((t) => t.id !== tag.id)
  recipeTags.value = recipeTags.value.filter((t) => t.id !== tag.id)
}

// Создание нового тега
const createNewTag = async () => {
  if (!tagSearchQuery.value) return

  try {
    const newTag = await tagStore.createTag({ name: tagSearchQuery.value })

    // Добавляем новый тег в список выбранных
    selectedTagInput.value = [...selectedTagInput.value, newTag]
    recipeTags.value.push({
      id: newTag.id,
      name: newTag.name,
      color: newTag.color,
      additionalInfo: '',
    })

    // Добавляем в результаты поиска
    tagSearchResults.value = [...tagSearchResults.value, newTag]

    tagSearchQuery.value = ''
    showMessage('Тег успешно создан!', 'success')
  } catch (error) {
    console.error('Ошибка создания тега:', error)
    showMessage('Ошибка при создании тега', 'error')
  }
}

const triggerFileSelect = () => {
  fileInput.value?.click()
}

const handleFileSelect = (event) => {
  const files = event.target.files
  if (!files || files.length === 0) return

  const newFilesArr = Array.from(files)

  newFilesArr.forEach((file) => {
    const isImage = file.type.startsWith('image/')

    if (isImage) {
      const reader = new FileReader()
      reader.onload = (e) => {
        newFilesPreview.value.push({
          type: 'image',
          url: e.target.result,
          file: file,
        })
      }
      reader.readAsDataURL(file)
    } else {
      newFilesPreview.value.push({
        type: 'pdf',
        url: null,
        file: file,
      })
    }
  })

  newFiles.value = [...newFiles.value, ...newFilesArr]
  event.target.value = ''
}

const removeNewFile = (index) => {
  newFiles.value.splice(index, 1)
  newFilesPreview.value.splice(index, 1)
  if (newFiles.value.length === 0) {
    newFiles.value = []
  }
}

const removeExistingFile = (fileId) => {
  existingFiles.value = existingFiles.value.filter((f) => f.id !== fileId)
  deletedFileIds.value.push(fileId)
}

const cancel = () => {
  router.push(`/recipe/${route.params.id}`)
}

const submitForm = async () => {
  if (!formValid.value) return

  submitting.value = true

  try {
    const recipeId = route.params.id

    // 1. Обновляем рецепт
    await recipesStore.updateRecipe(recipeId, {
      title: recipe.title,
      descriptions: recipe.description,
    })

    // 2. Удаляем помеченные файлы
    for (const fileId of deletedFileIds.value) {
      try {
        await api.delete(`/files/${fileId}`)
      } catch (e) {
        console.error('Ошибка удаления файла:', e)
      }
    }

    // 3. Загружаем новые файлы (если есть)
    if (newFiles.value && newFiles.value.length > 0) {
      for (let i = 0; i < newFiles.value.length; i++) {
        const isTitle = existingFiles.value.length === 0 && i === 0 // Первый файл - главное изображение
        await recipesStore.uploadFile(recipeId, newFiles.value[i], isTitle)
      }
    }

    // 4. Обновляем теги с дополнительной информацией
    if (recipeTags.value.length > 0) {
      await recipesStore.updateRecipeTags(recipeId, recipeTags.value)
    }

    showMessage('Рецепт успешно обновлен!', 'success')
    router.push(`/recipe/${recipeId}`)
  } catch (error) {
    console.error('Ошибка сохранения рецепта:', error)
    showMessage('Ошибка при сохранении рецепта', 'error')
  } finally {
    submitting.value = false
  }
}

const showMessage = (text, color = 'success') => {
  snackbar.text = text
  snackbar.color = color
  snackbar.show = true
}

onMounted(() => {
  loadRecipe()
})
</script>

<style scoped>
.gap-2 {
  gap: 8px;
}

.text-center {
  text-align: center;
}

.mx-auto {
  margin-left: auto;
  margin-right: auto;
}

.tag-info-input {
  min-width: 200px;
}

.tag-info-input :deep(.v-field) {
  font-size: 12px;
}

.file-preview-item {
  position: relative;
  flex-shrink: 0;
}

.files-preview-container {
  overflow-x: auto;
  padding-bottom: 4px;
}

.remove-file-btn {
  top: -15px;
  left: 64px;
}
</style>
