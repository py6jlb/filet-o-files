<template>
  <v-snackbar v-model="modelValue" :color="color" :timeout="timeout" :location="location" top>
    {{ text }}
    <template v-slot:actions>
      <v-btn color="white" variant="text" @click="close">
        {{ closeText }}
      </v-btn>
    </template>
  </v-snackbar>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  text: {
    type: String,
    default: ''
  },
  color: {
    type: String,
    default: 'success'
  },
  timeout: {
    type: Number,
    default: 3000
  },
  closeText: {
    type: String,
    default: 'Закрыть'
  },
  location: {
    type: String,
    default: 'top'
  }
})

const emit = defineEmits(['update:modelValue', 'close'])

const modelValue = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const close = () => {
  emit('update:modelValue', false)
  emit('close')
}
</script>
