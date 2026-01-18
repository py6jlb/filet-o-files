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
  <div >
    <div >
      <div >
        <div v-if="loading" >
          <span ></span>
          <span >Загрузка профиля...</span>
        </div>
        <div v-else-if="error" >
          {{ error }}
        </div>
        <template v-else>
          <div >
            <div >
              <img alt="avatar" src="@/assets/avatar.svg" />
            </div>
          </div>
          <h2 >{{ user?.name || 'Гость' }}</h2>
          <div >E-mail:</div>
          <div>{{ user?.email || '-' }}</div>
        </template>
      </div>
    </div>
  </div>
</template>
