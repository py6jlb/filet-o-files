<template>
  <v-dialog v-model="dialogVisible" max-width="90vw" @update:model-value="$emit('update:modelValue', $event)">
    <v-img
      :src="src"
      :max-height="maxHeight"
      contain
      @click="$emit('update:modelValue', false)"
    ></v-img>
  </v-dialog>
</template>

<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  src: {
    type: String,
    default: ''
  },
  maxHeight: {
    type: String,
    default: '80vh'
  }
})

defineEmits(['update:modelValue'])

const dialogVisible = ref(props.modelValue)

watch(() => props.modelValue, (val) => {
  dialogVisible.value = val
})
</script>
