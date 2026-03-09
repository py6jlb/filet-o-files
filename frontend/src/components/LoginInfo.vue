<script setup>
import { storeToRefs } from 'pinia'
import { useAuthStore } from '../stores/auth.store'

const auth = useAuthStore()
const { user } = storeToRefs(auth)
</script>

<template>
  <div>
    <v-list-item
      :to="'/profile'"
      lines="two"
      prepend-avatar="images/avatar.svg"
      :title="user?.email ?? 'Unknown'"
      :subtitle="user?.role.indexOf('Admin') > 0 ? 'Администратор' : 'Пользователь'"
    ></v-list-item>
  </div>
</template>
