<template>
  <div class="files-preview-container">
    <div class="d-flex flex-wrap" :class="gapClass">
      <div
        v-for="(preview, index) in previews"
        :key="index"
        class="file-preview-item"
      >
        <v-img
          v-if="preview.type === 'image'"
          :src="preview.url"
          :width="size"
          :height="size"
          cover
          class="rounded-lg elevation-1"
        ></v-img>
        <div
          v-else
          class="pdf-preview rounded-lg elevation-1 d-flex align-center justify-center"
          :style="{ width: size + 'px', height: size + 'px' }"
        >
          <v-icon :size="size / 2.5" color="red">mdi-file-pdf-box</v-icon>
        </div>
        <v-btn
          v-if="removable"
          icon="mdi-close"
          size="x-small"
          color="error"
          class="remove-file-btn"
          @click.stop="$emit('remove', index)"
        ></v-btn>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  previews: {
    type: Array,
    default: () => []
  },
  size: {
    type: Number,
    default: 80
  },
  removable: {
    type: Boolean,
    default: true
  },
  gap: {
    type: String,
    default: 'gap-2'
  }
})

defineEmits(['remove'])

const gapClass = computed(() => props.gap)
</script>

<style scoped>
.file-preview-item {
  position: relative;
  flex-shrink: 0;
}

.files-preview-container {
  overflow-x: auto;
  padding-bottom: 4px;
}

.remove-file-btn {
  position: absolute;
  top: -8px;
  left: auto;
  right: -8px;
}
</style>
