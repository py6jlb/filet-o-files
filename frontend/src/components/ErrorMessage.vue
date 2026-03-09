<template>
  <div class="text-center py-8">
    <v-alert :type="type" variant="tonal" class="mb-4">
      {{ message }}
    </v-alert>
    <v-btn v-if="buttonText" :color="buttonColor" @click="$emit('button-click')">
      {{ buttonText }}
    </v-btn>
  </div>
</template>

<script setup>
defineProps({
  message: {
    type: String,
    default: 'Произошла ошибка'
  },
  type: {
    type: String,
    default: 'error'
  },
  buttonText: {
    type: String,
    default: ''
  },
  buttonColor: {
    type: String,
    default: 'primary'
  }
})

defineEmits(['button-click'])
</script>
