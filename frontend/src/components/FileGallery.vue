<template>
  <div class="d-flex flex-wrap" :class="gapClass">
    <v-img
      v-for="file in files"
      :key="file.id"
      :src="getFileUrl(file.id)"
      :width="size"
      :height="size"
      cover
      class="rounded-lg cursor-pointer"
      @click="$emit('click', getFileUrl(file.id))"
    ></v-img>
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
</style>
