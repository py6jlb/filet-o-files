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

      <v-row>
        <v-col cols="12">
          <v-textarea
            v-model="recipe.description"
            label="Описание"
            variant="outlined"
            prepend-inner-icon="mdi-text"
            rows="3"
          ></v-textarea>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12">
          <v-textarea
            v-model="recipe.instructions"
            label="Инструкции приготовления *"
            :rules="[rules.required]"
            variant="outlined"
            prepend-inner-icon="mdi-chef-hat"
            rows="8"
            hint="Опишите пошагово процесс приготовления"
            persistent-hint
          ></v-textarea>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12">
          <v-file-input
            v-model="recipe.image"
            label="Изображение рецепта"
            variant="outlined"
            prepend-icon="mdi-camera"
            accept="image/*"
            show-size
            @change="handleImagePreview"
          >
            <template v-slot:selection="{ fileNames }">
              <template v-for="fileName in fileNames" :key="fileName">
                <v-chip size="small" label class="mr-2">
                  {{ fileName }}
                </v-chip>
              </template>
            </template>
          </v-file-input>

          <div v-if="imagePreview" class="text-center mt-3">
            <v-img
              :src="imagePreview"
              max-width="200"
              max-height="200"
              class="mx-auto rounded-lg elevation-2"
            ></v-img>
          </div>
        </v-col>
      </v-row>

      <!-- Кнопки действий -->
      <v-row class="mt-6">
        <v-col cols="12" class="text-center">
          <v-btn
            type="submit"
            color="success"
            size="large"
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
            size="large"
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

// import { storeToRefs } from 'pinia'
import { useTagsStore } from '../stores/tags.store'

const tagStore = useTagsStore()
// const { tags, loading: tagsLoading, error: tagsError } = storeToRefs(tagStore)

const form = ref()
const formValid = ref(false)
const submitting = ref(false)
const imagePreview = ref('')

const recipe = reactive({
  title: '',
  description: '',
  image: null,
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

const handleImagePreview = (event) => {
  const file = event.target?.files?.[0]
  if (file) {
    const reader = new FileReader()
    reader.onload = (e) => {
      imagePreview.value = e.target.result
    }
    reader.readAsDataURL(file)
  } else {
    imagePreview.value = ''
  }
}

const resetForm = () => {
  Object.assign(recipe, {
    title: '',
    description: '',
    image: null,
  })
  imagePreview.value = ''
  form.value?.reset()
}

const submitForm = async () => {
  if (!formValid.value) return

  submitting.value = true

  try {
    const formData = new FormData()

    // Добавляем основные поля
    Object.keys(recipe).forEach((key) => {
      if (key !== 'image' && recipe[key] !== null && recipe[key] !== '') {
        formData.append(key, recipe[key])
      }
    })

    // Добавляем файл
    if (recipe.image) {
      formData.append('image', recipe.image)
    }

   // await apiService.createRecipe(formData)

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

// Инициализация
onMounted(() => {
  tagStore.fetchTags()
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
</style>
