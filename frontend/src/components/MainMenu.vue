<script setup>
//import { useAuthStore } from '@/stores/auth.store'
</script>

<template>
  <v-list>
    <v-list-item :to="'/'"> <v-icon>mdi-chef-hat</v-icon> Рецепты </v-list-item>
    <v-list-item :to="'/scheduler'"> <v-icon>mdi-calendar-month</v-icon> Планирование </v-list-item>
  </v-list>
</template>
