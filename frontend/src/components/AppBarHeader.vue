<template>
  <div class="d-flex align-center">
    <v-img src="/images/logo_1.png" alt="Logo" width="40" height="40" class="mr-2"></v-img>
    {{ routeNameReactive }}
  </div>
  <v-spacer></v-spacer>
  <LoginInfo />
</template>

<script setup>
import { useRoute } from 'vue-router'
import { computed } from 'vue'
import LoginInfo from './LoginInfo.vue'

const route = useRoute()
const routeNameReactive = computed(() => {
  return route?.meta?.routeName != null && route?.meta?.routeName !== '' ? route.meta.routeName : ''
})
</script>
