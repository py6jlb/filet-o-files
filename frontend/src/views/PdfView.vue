<template>
  <div class="pdf-view">
    <!-- PDF в iframe -->
    <div class="pdf-iframe-container">
      <iframe :src="pdfUrl" class="pdf-iframe" title="PDF Viewer"></iframe>
    </div>
  </div>
</template>

<script setup>
import { useRoute } from 'vue-router'

const route = useRoute()

const fileId = route.params.id
const fileName = route.params.name
const apiBaseUrl = import.meta.env.VITE_API_BASE_URL
const pdfUrl = apiBaseUrl + '/files/' + fileId + '/' + fileName
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

.pdf-iframe-container {
  flex: 1;
  overflow: hidden;
}

.pdf-iframe {
  width: 100%;
  height: 100%;
  border: none;
  background: white;
}
</style>
