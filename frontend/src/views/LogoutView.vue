<template>
  <div class="logout-page">
    <p>Выход из аккаунта...</p>
  </div>
</template>


<script setup>
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth.store'

const router = useRouter()
const authStore = useAuthStore()

onMounted(async () => {
  await authStore.logout()
  router.replace({ name: 'login' })
})
</script>
