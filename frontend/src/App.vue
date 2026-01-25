<script setup>
import { RouterView } from 'vue-router'
import MainMenu from './components/MainMenu.vue'
import { ref } from 'vue'
import AppBarHeader from './components/AppBarHeader.vue'

const drawer = ref(false)
</script>

<template>
  <v-app>
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
