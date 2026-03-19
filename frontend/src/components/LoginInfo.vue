<template>
  <v-menu v-model="showMenu" v-if="user">
    <template v-slot:activator="{ props }">
      <v-btn v-bind="props" class="d-flex align-center">
        <v-avatar size="small" class="mr-2">
          <img src="/images/avatar.svg" />
        </v-avatar>
        <span>{{ user?.email ?? 'Unknown' }}</span>
      </v-btn>
    </template>
    <v-list>
      <v-list-item to="/profile">
        <v-list-item-title>Профиль</v-list-item-title>
      </v-list-item>
      <v-list-item to="/logout">
        <v-list-item-title>Выход</v-list-item-title>
      </v-list-item>
    </v-list>
  </v-menu>
</template>

<script setup>
import { storeToRefs } from 'pinia'
import { useAuthStore } from '../stores/auth.store'
import { ref } from 'vue'

const showMenu = ref(false)
const auth = useAuthStore()
const { user } = storeToRefs(auth)
</script>
