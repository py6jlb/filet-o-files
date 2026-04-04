<template>
  <div class="d-flex flex-wrap" :class="gapClass">
    <template v-for="file in displayableFiles" :key="file.id">
      <!-- Изображение -->
      <v-img
        v-if="file.mimeType?.startsWith('image')"
        :src="getFileUrl(file.id)"
        :width="size"
        :height="size"
        cover
        class="rounded-lg cursor-pointer"
        @click="$emit('click', getFileUrl(file.id), file.mimeType)"
      ></v-img>
      <!-- PDF с превью -->
      <v-img
        v-else-if="file.previewFileId"
        :src="getFileUrl(file.previewFileId)"
        :width="size"
        :height="size"
        cover
        class="rounded-lg cursor-pointer"
        @click="$emit('click', getFileUrl(file.id), file.mimeType)"
      ></v-img>
    </template>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  files: {
    type: Array,
    default: () => []
  },
  size: {
    type: Number,
    default: 120
  },
  gap: {
    type: String,
    default: 'ga-2'
  }
})

defineEmits(['click'])

const gapClass = computed(() => props.gap)

// Фильтруем файлы: только изображения или PDF с превью
const displayableFiles = computed(() => {
  return props.files.filter(file => {
    // Показываем все изображения
    if (file.mimeType?.startsWith('image')) {
      return true
    }
    // Показываем PDF только если есть превью
    if (file.previewFileId) {
      return true
    }
    return false
  })
})

const getFileUrl = (fileId) => {
  return `${import.meta.env.VITE_API_BASE_URL}/files/${fileId}`
}
</script>

<style scoped>
.cursor-pointer {
  cursor: pointer;
}
</style>
