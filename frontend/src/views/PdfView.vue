<template>
  <div class="pdf-view">
    <!-- Верхняя панель -->
    <div class="pdf-toolbar">
      <v-btn icon="mdi-arrow-left" variant="text" @click="goBack" title="Назад"></v-btn>

      <v-spacer></v-spacer>

      <v-btn
        icon="mdi-minus"
        variant="text"
        size="small"
        :disabled="scale <= 0.5"
        @click="zoomOut"
        title="Уменьшить"
      ></v-btn>

      <span class="scale-label">{{ Math.round(scale * 100) }}%</span>

      <v-btn
        icon="mdi-plus"
        variant="text"
        size="small"
        :disabled="scale >= 3"
        @click="zoomIn"
        title="Увеличить"
      ></v-btn>

      <v-divider vertical class="mx-2"></v-divider>

      <v-btn
        icon="mdi-chevron-left"
        variant="text"
        :disabled="currentPage <= 1"
        @click="prevPage"
        title="Предыдущая страница"
      ></v-btn>

      <span class="page-label">{{ currentPage }} / {{ totalPages }}</span>

      <v-btn
        icon="mdi-chevron-right"
        variant="text"
        :disabled="currentPage >= totalPages"
        @click="nextPage"
        title="Следующая страница"
      ></v-btn>

      <v-divider vertical class="mx-2"></v-divider>

      <v-btn icon="mdi-download" variant="text" @click="downloadPdf" title="Скачать"></v-btn>
    </div>

    <!-- Контейнер PDF -->
    <div ref="pdfContainer" class="pdf-container" @wheel="onWheel">
      <div v-if="loading" class="pdf-loading">
        <v-progress-circular indeterminate color="primary" size="64"></v-progress-circular>
        <div class="mt-4">Загрузка PDF...</div>
      </div>

      <div v-else-if="error" class="pdf-error">
        <v-icon size="64" color="error">mdi-alert-circle</v-icon>
        <div class="mt-4 text-h6">Ошибка загрузки PDF</div>
        <div class="text-body-2 text-grey">{{ error }}</div>
        <v-btn class="mt-4" color="primary" @click="goBack">Назад к рецепту</v-btn>
      </div>

      <div
        v-else
        class="pdf-pages"
        :style="{ transform: `scale(${scale})`, transformOrigin: 'top center' }"
      >
        <canvas
          v-for="pageNum in totalPages"
          :key="pageNum"
          :ref="(el) => setCanvasRef(el, pageNum)"
          class="pdf-page"
          :class="{ active: pageNum === currentPage }"
        ></canvas>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import * as pdfjsLib from 'pdfjs-dist'

// Настройка worker для pdf.js
pdfjsLib.GlobalWorkerOptions.workerSrc = new URL(
  'pdfjs-dist/build/pdf.worker.min.mjs',
  import.meta.url,
).toString()

const route = useRoute()
const router = useRouter()

const loading = ref(true)
const error = ref(null)
const pdfDoc = ref(null)
const totalPages = ref(0)
const currentPage = ref(1)
const scale = ref(1)
const pdfContainer = ref(null)
const canvasRefs = ref({})

const fileId = route.params.id
const apiBaseUrl = import.meta.env.VITE_API_BASE_URL
const pdfUrl = `${apiBaseUrl}/files/${fileId}`

const setCanvasRef = (el, pageNum) => {
  if (el) {
    canvasRefs.value[pageNum] = el
  }
}

onMounted(async () => {
  await loadPdf()
})

onUnmounted(() => {
  if (pdfDoc.value) {
    pdfDoc.value.destroy()
  }
})

const loadPdf = async () => {
  loading.value = true
  error.value = null
  pdfDoc.value = null
  totalPages.value = 0
  currentPage.value = 1
  canvasRefs.value = {}

  try {
    const loadingTask = pdfjsLib.getDocument(pdfUrl)
    pdfDoc.value = await loadingTask.promise
    totalPages.value = pdfDoc.value.numPages

    await renderAllPages()
  } catch (e) {
    console.error('Ошибка загрузки PDF:', e)
    error.value = e.message || 'Не удалось загрузить PDF'
  } finally {
    loading.value = false
  }
}

const renderAllPages = async () => {
  if (!pdfDoc.value) return

  for (let i = 1; i <= totalPages.value; i++) {
    await renderPage(i)
  }
}

const renderPage = async (pageNum) => {
  if (!pdfDoc.value || !canvasRefs.value[pageNum]) return

  try {
    const page = await pdfDoc.value.getPage(pageNum)
    const canvas = canvasRefs.value[pageNum]
    const context = canvas.getContext('2d')

    // Базовая ширина 600px
    const baseWidth = 600
    const viewport = page.getViewport({ scale: 1 })
    const pageScale = baseWidth / viewport.width
    const scaledViewport = page.getViewport({ scale: pageScale })

    canvas.height = scaledViewport.height
    canvas.width = scaledViewport.width

    await page.render({
      canvasContext: context,
      viewport: scaledViewport,
    }).promise
  } catch (e) {
    console.error('Ошибка рендеринга страницы:', e)
  }
}

const prevPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--
    scrollToPage(currentPage.value)
  }
}

const nextPage = () => {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
    scrollToPage(currentPage.value)
  }
}

const scrollToPage = (pageNum) => {
  const canvas = canvasRefs.value[pageNum]
  if (canvas && pdfContainer.value) {
    canvas.scrollIntoView({ behavior: 'smooth', block: 'start' })
  }
}

const zoomIn = () => {
  if (scale.value < 3) {
    scale.value = Math.min(3, scale.value + 0.25)
  }
}

const zoomOut = () => {
  if (scale.value > 0.5) {
    scale.value = Math.max(0.5, scale.value - 0.25)
  }
}

const onWheel = (e) => {
  if (e.ctrlKey) {
    e.preventDefault()
    if (e.deltaY < 0) {
      zoomIn()
    } else {
      zoomOut()
    }
  }
}

const downloadPdf = () => {
  const link = document.createElement('a')
  link.href = pdfUrl
  link.target = '_blank'
  link.download = ''
  link.click()
}

const goBack = () => {
  router.back()
}
</script>

<style scoped>
.pdf-view {
  display: flex;
  flex-direction: column;
  height: calc(100vh - 64px);
  background: #525252;
}

.pdf-toolbar {
  display: flex;
  align-items: center;
  padding: 8px 16px;
  background: #303030;
  color: white;
  gap: 8px;
}

.scale-label,
.page-label {
  min-width: 60px;
  text-align: center;
}

.pdf-container {
  flex: 1;
  overflow: auto;
  padding: 20px;
}

.pdf-loading,
.pdf-error {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
  color: white;
}

.pdf-pages {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 20px;
  transition: transform 0.2s ease;
}

.pdf-page {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.4);
  background: white;
  display: block;
}

.pdf-page:not(.active) {
  opacity: 0.3;
}

.pdf-page.active {
  opacity: 1;
}
</style>
