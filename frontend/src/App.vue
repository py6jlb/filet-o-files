<script setup>
import { RouterView } from 'vue-router'
import MainMenu from './components/MainMenu.vue'
import { ref, onMounted } from 'vue'
import AppBarHeader from './components/AppBarHeader.vue'
import { useAuthStore } from './stores/auth.store'

const drawer = ref(false)
const authStore = useAuthStore()

// Экран загрузки пока инициализируется auth
const loading = ref(true)

onMounted(() => {
  // Ждём когда auth инициализируется
  const checkAuth = setInterval(() => {
    if (authStore.isInitialized) {
      loading.value = false
      clearInterval(checkAuth)
    }
  }, 100)

  // Таймаут на случай если что-то пойдёт не так
  setTimeout(() => {
    loading.value = false
    clearInterval(checkAuth)
  }, 5000)
})
</script>

<template>
  <v-app>
    <!-- Экран загрузки при инициализации -->
    <v-overlay :model-value="loading" class="d-flex align-center justify-center">
      <div class="text-center">
        <v-progress-circular indeterminate size="64" color="primary"></v-progress-circular>
        <div class="mt-4 text-h6">Загрузка...</div>
      </div>
    </v-overlay>

    <MainMenu v-model="drawer" />
    <v-app-bar>
      <v-app-bar-nav-icon @click="drawer = !drawer"></v-app-bar-nav-icon>
      <AppBarHeader />
    </v-app-bar>
    <v-main>
      <v-container>
        <RouterView />
      </v-container>
    </v-main>
  </v-app>
</template>

<style scoped></style>
