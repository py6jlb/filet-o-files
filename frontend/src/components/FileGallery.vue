<template>
  <div class="d-flex flex-wrap" :class="gapClass">
    <template v-for="file in files" :key="file.id">
      <!-- Изображение -->
      <v-img
        v-if="file.mimeType?.startsWith('image')"
        :src="getFileUrl(file.id)"
        :width="size"
        :height="size"
        cover
        class="rounded-lg cursor-pointer"
        @click="$emit('click', getFileUrl(file.id))"
      ></v-img>
      <!-- PDF с превью -->
      <v-img
        v-else-if="file.previewFileId"
        :src="getFileUrl(file.previewFileId)"
        :width="size"
        :height="size"
        cover
        class="rounded-lg cursor-pointer"
        @click="$emit('click', getFileUrl(file.id))"
      ></v-img>
      <!-- PDF без превью - иконка -->
      <div
        v-else
        class="pdf-preview rounded-lg d-flex align-center justify-center cursor-pointer"
        :style="{ width: size + 'px', height: size + 'px' }"
        @click="$emit('click', getFileUrl(file.id))"
      >
        <v-icon size="32" color="red">mdi-file-pdf-box</v-icon>
      </div>
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

const getFileUrl = (fileId) => {
  return `${import.meta.env.VITE_API_BASE_URL}/files/${fileId}`
}
</script>

<style scoped>
.cursor-pointer {
  cursor: pointer;
}
.pdf-preview {
  background-color: #f5f5f5;
}
</style>
