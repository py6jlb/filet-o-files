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
      <v-btn color="primary" size="small" to="/">Вернуться к списку рецептов</v-btn>
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
        <v-row class="mt-0">
          <v-col cols="12">
            <input
              ref="fileInput"
              type="file"
              multiple
              accept="image/*,.pdf"
              style="display: none"
              @change="handleFileSelect"
            />
            <v-btn variant="outlined" size="small" prepend-icon="mdi-paperclip" @click="triggerFileSelect">
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
        <v-row v-if="existingFiles.length > 0" class="mt-0">
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

        <v-row class="mt-0">
          <v-col cols="12">
            <MarkdownEditor
              v-model="recipe.description"
              label="Инструкции приготовления"
              placeholder="Опишите процесс приготовления в формате Markdown..."
            ></MarkdownEditor>
          </v-col>
        </v-row>

        <!-- Блок выбора тегов -->
        <v-row class="mt-0">
          <v-col cols="12">
            <v-btn
              variant="outlined"
              size="small"
              prepend-icon="mdi-tag-plus"
              @click="openTagDialog"
            >
              Добавить тег
            </v-btn>
          </v-col>
        </v-row>

        <!-- Таблица тегов -->
        <v-row v-if="recipeTags.length > 0" class="mt-0">
          <v-col cols="12">
            <TagsTable :tags="recipeTags" @remove="removeTag" />
          </v-col>
        </v-row>

        <!-- Диалог выбора тега -->
        <TagSelectDialog
          v-model="tagDialog"
          @select="addTag"
        />

        <!-- Кнопки действий -->
        <v-row class="mt-6">
          <v-col cols="12" class="d-flex justify-center ga-2">
            <v-btn
              type="submit"
              color="success"
              size="small"
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
              size="small"
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
    <SnackbarNotification
      v-model="snackbar.show"
      :text="snackbar.text"
      :color="snackbar.color"
    />
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'

import { useRecipesStore } from '../stores/recipes.store'
import MarkdownEditor from '../components/MarkdownEditor.vue'
import TagSelectDialog from '../components/TagSelectDialog.vue'
import TagsTable from '../components/TagsTable.vue'
import SnackbarNotification from '../components/SnackbarNotification.vue'
import api from '../utils/api'

const route = useRoute()
const router = useRouter()

const recipesStore = useRecipesStore()

const form = ref()
const formValid = ref(false)
const submitting = ref(false)
const loading = ref(true)
const error = ref(null)
const newFilesPreview = ref([])
const fileInput = ref(null)

// Состояние для тегов
const tagDialog = ref(false)
const recipeTags = ref([]) // Массив { id, name, additionalInfo }

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
      recipeTags.value = data.tags.map((t) => ({
        id: t.id,
        name: t.name,
        color: t.color,
        additionalInfo: t.additionalInfo || '',
      }))
    }
  } catch (e) {
    error.value = e.response?.data?.message || 'Не удалось загрузить рецепт'
  } finally {
    loading.value = false
  }
}

// Открытие диалога
const openTagDialog = () => {
  tagDialog.value = true
}

// Добавление тега
const addTag = (tag) => {
  const existingIds = recipeTags.value.map((t) => t.id)
  if (!existingIds.includes(tag.id)) {
    recipeTags.value.push({
      id: tag.id,
      name: tag.name,
      color: tag.color,
      additionalInfo: '',
    })
  }
}

// Удаление тега
const removeTag = (tag) => {
  recipeTags.value = recipeTags.value.filter((t) => t.id !== tag.id)
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

.files-preview-container {
  overflow-x: auto;
  padding-bottom: 4px;
}
</style>
