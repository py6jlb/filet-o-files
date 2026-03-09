<template>
  <div>
    <!-- Загрузка -->
    <LoadingIndicator v-if="loading" text="Загрузка рецепта..." />

    <!-- Ошибка -->
    <ErrorMessage
      v-else-if="error"
      message="Ошибка при загрузке"
      button-text="Вернуться к списку"
      @button-click="router.push('/')"
    />

    <!-- Рецепт не найден -->
    <ErrorMessage
      v-else-if="!recipe"
      message="Рецепт не найден"
      type="warning"
      button-text="Вернуться к списку"
      @button-click="router.push('/')"
    />

    <!-- Просмотр рецепта -->
    <div v-else>
      <!-- Кнопки действий -->
      <div class="d-flex justify-end mb-4 ga-2">
        <v-btn
          color="primary"
          variant="outlined"
          :to="`/edit_recipe/${recipe.id}`"
        >
          <v-icon left>mdi-pencil</v-icon>
          Редактировать
        </v-btn>
        <v-btn
          color="error"
          variant="outlined"
          @click="confirmDelete"
        >
          <v-icon left>mdi-delete</v-icon>
          Удалить
        </v-btn>
      </div>

      <!-- Основная информация -->
      <v-card variant="outlined" class="mb-4">
        <!-- Изображение -->
        <v-img
          v-if="recipe.files?.length > 0"
          :src="getFileUrl(recipe.files[0].id)"
          height="300"
          cover
          class="bg-grey-lighten-2"
        ></v-img>
        <v-img
          v-else
          height="200"
          cover
          class="bg-grey-lighten-2 d-flex align-center justify-center"
        >
          <v-icon size="64" color="grey">mdi-food</v-icon>
        </v-img>

        <v-card-title class="text-h4 py-4">
          {{ recipe.title }}
        </v-card-title>

        <!-- Теги -->
        <v-card-text v-if="recipe.tags?.length">
          <div class="d-flex flex-wrap ga-2">
            <v-chip
              v-for="tag in recipe.tags"
              :key="tag.id"
              :style="{
                backgroundColor: tag.color || '#757575',
                color: getContrastColor(tag.color),
              }"
            >
              <v-icon size="small" class="mr-1">mdi-tag</v-icon>
              {{ tag.name }}
            </v-chip>
          </div>
        </v-card-text>

        <!-- Описание (рендерится как Markdown) -->
        <v-card-text v-if="recipe.descriptions">
          <div class="text-subtitle-1 text-grey-darken-1 mb-2">Описание</div>
          <MarkdownRenderer :content="recipe.descriptions" />
        </v-card-text>

        <!-- Дата создания -->
        <v-card-text>
          <div class="text-caption text-grey">
            <v-icon size="small" class="mr-1">mdi-calendar</v-icon>
            Создан: {{ formatDate(recipe.created) }}
          </div>
        </v-card-text>
      </v-card>

      <!-- Галерея изображений -->
      <v-card v-if="recipe.files?.length > 1" variant="outlined" class="mb-4">
        <v-card-title>Галерея</v-card-title>
        <v-card-text>
          <FileGallery :files="recipe.files" :size="120" @click="openImage" />
        </v-card-text>
      </v-card>

      <!-- Кнопка назад -->
      <v-btn
        variant="text"
        color="primary"
        to="/"
        class="mt-4"
      >
        <v-icon left>mdi-arrow-left</v-icon>
        Назад к списку рецептов
      </v-btn>
    </div>

    <!-- Диалог подтверждения удаления -->
    <DeleteConfirmDialog
      v-model="deleteDialog"
      title="Подтверждение удаления"
      :message="`Вы уверены, что хотите удалить рецепт «${recipe?.title}»? Это действие нельзя отменить.`"
      :loading="deleting"
      @confirm="deleteRecipeHandler"
    />

    <!-- Диалог просмотра изображения -->
    <ImageViewerDialog
      v-model="imageDialog"
      :src="selectedImage"
    />

    <!-- Уведомление -->
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
import LoadingIndicator from '../components/LoadingIndicator.vue'
import ErrorMessage from '../components/ErrorMessage.vue'
import MarkdownRenderer from '../components/MarkdownRenderer.vue'
import FileGallery from '../components/FileGallery.vue'
import DeleteConfirmDialog from '../components/DeleteConfirmDialog.vue'
import ImageViewerDialog from '../components/ImageViewerDialog.vue'
import SnackbarNotification from '../components/SnackbarNotification.vue'
import { getContrastColor } from '../utils/colors'

const route = useRoute()
const router = useRouter()
const recipesStore = useRecipesStore()

const recipe = ref(null)
const loading = ref(true)
const error = ref(null)
const deleting = ref(false)

const deleteDialog = ref(false)
const imageDialog = ref(false)
const selectedImage = ref('')

const snackbar = reactive({
  show: false,
  text: '',
  color: 'success',
})

const getFileUrl = (fileId) => {
  return `${import.meta.env.VITE_API_BASE_URL}/files/${fileId}`
}

const formatDate = (dateString) => {
  if (!dateString) return ''
  const date = new Date(dateString)
  return date.toLocaleDateString('ru-RU', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

const openImage = (url) => {
  selectedImage.value = url
  imageDialog.value = true
}

const confirmDelete = () => {
  deleteDialog.value = true
}

const deleteRecipeHandler = async () => {
  if (!recipe.value) return

  deleting.value = true
  try {
    // Удаляем последовательно: файлы -> теги -> рецепт
    await recipesStore.deleteRecipeWithDependencies(recipe.value)
    showMessage('Рецепт успешно удалён', 'success')
    deleteDialog.value = false
    router.push('/')
  } catch (e) {
    console.error(e)
    showMessage('Ошибка при удалении рецепта', 'error')
  } finally {
    deleting.value = false
  }
}

const showMessage = (text, color = 'success') => {
  snackbar.text = text
  snackbar.color = color
  snackbar.show = true
}

const loadRecipe = async () => {
  loading.value = true
  error.value = null

  try {
    const id = route.params.id
    recipe.value = await recipesStore.fetchRecipe(id)
  } catch (e) {
    error.value = e.response?.data?.message || 'Не удалось загрузить рецепт'
    recipe.value = null
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadRecipe()
})
</script>

<style scoped>
/* Стили в MarkdownRenderer.vue */
</style>
