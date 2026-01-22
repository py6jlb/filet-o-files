<script setup>
import { RouterView } from 'vue-router'
import MainMenu from './components/MainMenu.vue'
</script>

<template>
  <v-app>
    <v-app-bar>
      <MainMenu />
    </v-app-bar>
    <v-main class="d-flex justify-center">
      <v-container>
        <RouterView />
      </v-container>
    </v-main>
    <v-footer
      style="max-height: 48px"
      class="d-flex align-center justify-center ga-2 flex-wrap flex-grow-1"
      color="surface-light"
    >
      <div class="text-center">{{ new Date().getFullYear() }} — <strong>Filet-o-Files</strong></div>
    </v-footer>
  </v-app>
</template>

<style scoped>
.app-bar-logo-link {
  display: flex;
  align-items: center;
  text-decoration: none;
  color: inherit;
  cursor: pointer;
  margin-left: 8px;
}

.logo_link_text {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
</style>
