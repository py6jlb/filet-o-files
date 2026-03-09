<template>
  <div class="markdown-content" v-html="renderedContent"></div>
</template>

<script setup>
import { computed } from 'vue'
import { marked } from 'marked'

const props = defineProps({
  content: {
    type: String,
    default: ''
  },
  maxLength: {
    type: Number,
    default: 0
  }
})

// Настройка marked
marked.setOptions({ breaks: true, gfm: true })

const renderedContent = computed(() => {
  if (!props.content) return ''
  
  let text = props.content
  
  // Если указана максимальная длина - обрезаем
  if (props.maxLength > 0 && text.length > props.maxLength) {
    const html = marked.parse(text)
    const plainText = html.replace(/<[^>]*>/g, '')
    text = plainText.substring(0, props.maxLength) + '...'
  }
  
  return marked.parse(text)
})
</script>

<style scoped>
/* Стили для markdown контента */
:deep(.markdown-content h1) {
  font-size: 2rem;
  font-weight: 600;
  margin-bottom: 1rem;
  border-bottom: 1px solid #e0e0e0;
  padding-bottom: 0.5rem;
}

:deep(.markdown-content h2) {
  font-size: 1.5rem;
  font-weight: 600;
  margin-top: 1.5rem;
  margin-bottom: 0.75rem;
  border-bottom: 1px solid #e0e0e0;
  padding-bottom: 0.25rem;
}

:deep(.markdown-content h3) {
  font-size: 1.25rem;
  font-weight: 600;
  margin-top: 1rem;
  margin-bottom: 0.5rem;
}

:deep(.markdown-content p) {
  margin-bottom: 1rem;
  line-height: 1.6;
}

:deep(.markdown-content ul),
:deep(.markdown-content ol) {
  margin-bottom: 1rem;
  padding-left: 1.5rem;
}

:deep(.markdown-content li) {
  margin-bottom: 0.25rem;
}

:deep(.markdown-content code) {
  background: #f0f0f0;
  padding: 0.125rem 0.375rem;
  border-radius: 4px;
  font-family: 'Consolas', 'Monaco', monospace;
  font-size: 0.9em;
}

:deep(.markdown-content pre) {
  background: #2d2d2d;
  color: #f8f8f2;
  padding: 1rem;
  border-radius: 6px;
  overflow-x: auto;
  margin-bottom: 1rem;
}

:deep(.markdown-content pre code) {
  background: transparent;
  padding: 0;
  color: inherit;
}

:deep(.markdown-content blockquote) {
  border-left: 4px solid #e0e0e0;
  padding-left: 1rem;
  margin-left: 0;
  color: #666;
  font-style: italic;
}

:deep(.markdown-content a) {
  color: #1976d2;
  text-decoration: none;
}

:deep(.markdown-content a:hover) {
  text-decoration: underline;
}

:deep(.markdown-content img) {
  max-width: 100%;
  border-radius: 4px;
}

:deep(.markdown-content table) {
  border-collapse: collapse;
  width: 100%;
  margin-bottom: 1rem;
}

:deep(.markdown-content th),
:deep(.markdown-content td) {
  border: 1px solid #e0e0e0;
  padding: 0.5rem;
  text-align: left;
}

:deep(.markdown-content th) {
  background: #f5f5f5;
}

:deep(.markdown-content hr) {
  border: none;
  border-top: 1px solid #e0e0e0;
  margin: 1.5rem 0;
}
</style>
