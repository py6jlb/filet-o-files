<script setup>
//import { useAuthStore } from '@/stores/auth.store'
</script>

<template>
  <v-menu>
    <template v-slot:activator="{ on }">
      <v-app-bar-title v-on="on" class="app-bar-logo-link">
        <v-img src="/images/logo_1.png" alt="Logo" width="40" height="40" class="ml-2 mr-2"></v-img>
        <span class="logo_link_text">File-o-Files</span>
      </v-app-bar-title>
    </template>
    <v-list>
      <v-list-item :to="'/'"> Рецепты </v-list-item>
      <v-list-item :to="'/scheduler'"> Планирование </v-list-item>
    </v-list>
  </v-menu>
</template>
