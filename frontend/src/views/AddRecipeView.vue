<template>
  <div>
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

          <!-- Превью изображений -->
          <div v-if="filesPreview.length > 0" class="mt-1 files-preview-container">
            <div class="d-flex flex-wrap gap-2">
              <div v-for="(preview, index) in filesPreview" :key="index" class="file-preview-item">
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
                  @click.stop="removeFile(index)"
                ></v-btn>
              </div>
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
          <v-btn variant="outlined" size="small" prepend-icon="mdi-tag-plus" @click="openTagDialog">
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
      <TagSelectDialog v-model="tagDialog" @select="addTag" />

      <!-- Кнопки действий -->
      <v-row class="mt-6">
        <v-col cols="12" class="text-center">
          <v-btn
            type="submit"
            color="success"
            size="small"
            variant="elevated"
            :disabled="!formValid"
            :loading="submitting"
            class="mr-4"
          >
            <v-icon left>mdi-content-save</v-icon>
            Сохранить рецепт
          </v-btn>

          <v-btn
            type="button"
            color="grey"
            size="small"
            variant="outlined"
            @click="resetForm"
            :disabled="submitting"
          >
            <v-icon left>mdi-refresh</v-icon>
            Очистить
          </v-btn>
        </v-col>
      </v-row>
    </v-form>
    <!-- Сообщения -->
    <SnackbarNotification
      v-model="snackbar.show"
      :text="snackbar.text"
      :color="snackbar.color"
    />
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'

import { useRecipesStore } from '../stores/recipes.store'
import MarkdownEditor from '../components/MarkdownEditor.vue'
import TagSelectDialog from '../components/TagSelectDialog.vue'
import TagsTable from '../components/TagsTable.vue'
import SnackbarNotification from '../components/SnackbarNotification.vue'

const recipesStore = useRecipesStore()

const form = ref()
const formValid = ref(false)
const submitting = ref(false)
const filesPreview = ref([])
const fileInput = ref(null)

// Состояние для тегов
const tagDialog = ref(false)
const recipeTags = ref([]) // Массив { id, name, additionalInfo }

const recipe = reactive({
  title: '',
  description: '',
  files: [],
})

const snackbar = reactive({
  show: false,
  text: '',
  color: 'success',
})

const rules = {
  required: (value) => !!value || 'Обязательное поле',
  positiveNumber: (value) => !value || value > 0 || 'Должно быть больше 0',
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

  // Добавляем новые файлы к существующим
  const newFiles = Array.from(files)

  newFiles.forEach((file) => {
    const isImage = file.type.startsWith('image/')

    if (isImage) {
      const reader = new FileReader()
      reader.onload = (e) => {
        filesPreview.value.push({
          type: 'image',
          url: e.target.result,
          file: file,
        })
      }
      reader.readAsDataURL(file)
    } else {
      filesPreview.value.push({
        type: 'pdf',
        url: null,
        file: file,
      })
    }
  })

  // Добавляем в recipe.files
  recipe.files = [...recipe.files, ...newFiles]

  // Очищаем input для возможности повторного выбора того же файла
  event.target.value = ''
}

const removeFile = (index) => {
  recipe.files.splice(index, 1)
  filesPreview.value.splice(index, 1)
  if (recipe.files.length === 0) {
    recipe.files = []
  }
}

const resetForm = () => {
  Object.assign(recipe, {
    title: '',
    description: '',
    files: [],
  })
  filesPreview.value = []
  recipeTags.value = []
  form.value?.reset()
}

const submitForm = async () => {
  if (!formValid.value) return

  submitting.value = true

  try {
    // 1. Создаём рецепт
    const newRecipe = await recipesStore.createRecipe({
      title: recipe.title,
      descriptions: recipe.description,
    })

    // 2. Загружаем файлы (если есть)
    if (recipe.files && recipe.files.length > 0) {
      for (let i = 0; i < recipe.files.length; i++) {
        const isTitle = i === 0 // Первый файл - главное изображение
        await recipesStore.uploadFile(newRecipe.id, recipe.files[i], isTitle)
      }
    }

    // 3. Обновляем теги с дополнительной информацией
    if (recipeTags.value.length > 0) {
      await recipesStore.updateRecipeTags(newRecipe.id, recipeTags.value)
    }

    showMessage('Рецепт успешно сохранен!', 'success')
    resetForm()
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
</script>

<style scoped>
.gap-2 {
  gap: 8px;
}

.text-center {
  text-align: center;
}

.files-preview-container {
  overflow-x: auto;
  padding-bottom: 4px;
}
</style>
