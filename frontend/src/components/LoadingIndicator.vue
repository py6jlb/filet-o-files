<template>
  <div class="text-center py-8">
    <v-progress-circular indeterminate :color="color" :size="size"></v-progress-circular>
    <div v-if="text" class="mt-2">{{ text }}</div>
  </div>
</template>

<script setup>
defineProps({
  text: {
    type: String,
    default: 'Загрузка...'
  },
  color: {
    type: String,
    default: 'primary'
  },
  size: {
    type: [Number, String],
    default: 40
  }
})
</script>
