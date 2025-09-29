<script setup>
import { onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useUserStore } from '../stores/user.store'

const userStore = useUserStore()
const { user, loading, error } = storeToRefs(userStore)

onMounted(() => {
  userStore.fetchProfile()
})
</script>

<template>
  <div class="flex justify-center items-center min-h-[60vh]">
    <div class="card shadow-xl bg-base-100 max-w-sm w-full">
      <div class="card-body items-center text-center">
        <div v-if="loading" class="flex flex-col items-center py-6">
          <span class="loading loading-spinner loading-lg text-primary mb-3"></span>
          <span class="text-base-content/70">Загрузка профиля...</span>
        </div>
        <div v-else-if="error" class="alert alert-error mb-3">
          {{ error }}
        </div>
        <template v-else>
          <div class="avatar mb-4">
            <div class="w-20 rounded-full bg-base-200 flex items-center justify-center">
              <img alt="avatar" src="@/assets/avatar.svg" />
            </div>
          </div>
          <h2 class="card-title mb-2">{{ user?.name || 'Гость' }}</h2>
          <div class="text-sm text-base-content/70 mb-1">E-mail:</div>
          <div class="font-mono text-base-content">{{ user?.email || '-' }}</div>
        </template>
      </div>
    </div>
  </div>
</template>
