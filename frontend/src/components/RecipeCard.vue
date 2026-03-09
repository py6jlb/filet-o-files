<template>
  <v-card
    variant="outlined"
    class="cursor-pointer recipe-card"
    @click="$emit('click', recipe.id)"
  >
    <div class="d-flex">
      <!-- Превью картинки 128x128 -->
      <div v-if="hasFiles" class="recipe-image-container flex-shrink-0">
        <!-- Размытый фон -->
        <v-img
          :src="getFileUrl(recipe.files[0].id)"
          width="128"
          height="128"
          cover
          class="recipe-blur-bg"
        ></v-img>
        <!-- Основное изображение по центру -->
        <v-img
          :src="getFileUrl(recipe.files[0].id)"
          width="128"
          height="128"
          contain
          class="recipe-main-img"
        ></v-img>
      </div>
      <div
        v-else
        class="flex-shrink-0 bg-grey-lighten-2 d-flex align-center justify-center"
        style="width: 128px; height: 128px"
      >
        <v-icon size="48" color="grey">mdi-food</v-icon>
      </div>

      <!-- Контент -->
      <div class="pa-3 flex-grow-1 d-flex flex-column">
        <div class="text-subtitle-1 font-weight-medium mb-1">{{ recipe.title }}</div>

        <div v-if="recipe.tags?.length" class="mb-1">
          <v-chip
            v-for="tag in recipe.tags"
            :key="tag.id"
            size="x-small"
            class="mr-1"
            :style="{
              backgroundColor: tag.color || '#757575',
              color: getContrastColor(tag.color),
            }"
          >
            {{ tag.name }}
          </v-chip>
        </div>

        <div v-if="recipe.descriptions" class="text-body-2 text-grey-darken-1 recipe-description">
          {{ truncateDescription(recipe.descriptions) }}
        </div>
      </div>
    </div>
  </v-card>
</template>

<script setup>
import { computed } from 'vue'
import { marked } from 'marked'
import { getContrastColor } from '../utils/colors'

const props = defineProps({
  recipe: {
    type: Object,
    required: true
  }
})

defineEmits(['click'])

marked.setOptions({ breaks: true, gfm: true })

const hasFiles = computed(() => props.recipe.files?.length > 0)

const getFileUrl = (fileId) => {
  return `${import.meta.env.VITE_API_BASE_URL}/files/${fileId}`
}

const truncateDescription = (text) => {
  if (!text) return ''
  const html = marked.parse(text)
  const plainText = html.replace(/<[^>]*>/g, '')
  if (plainText.length > 180) {
    return plainText.substring(0, 180) + '...'
  }
  return plainText
}
</script>

<style scoped>
.cursor-pointer {
  cursor: pointer;
}

.recipe-card {
  margin-bottom: 12px;
}

.recipe-card:hover {
  background-color: rgb(var(--v-theme-surface));
  border-color: rgb(var(--v-theme-primary));
}

.recipe-description {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.recipe-image-container {
  position: relative;
  width: 128px;
  height: 128px;
  overflow: hidden;
  flex-shrink: 0;
}

.recipe-blur-bg {
  position: absolute;
  top: 0;
  left: 0;
  filter: blur(20px);
  transform: scale(1.2);
  opacity: 0.5;
}

.recipe-main-img {
  position: relative;
  z-index: 1;
}
</style>
