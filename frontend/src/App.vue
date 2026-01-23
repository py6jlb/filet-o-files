<script setup>
import { RouterView, useRoute } from 'vue-router'
import { computed } from 'vue'
import MainMenu from './components/MainMenu.vue'
import { ref } from 'vue'
const route = useRoute()
const drawer = ref(false)
const reactiveRouteName = computed(() =>
  route.name != null && route.name !== '' ? ` / ${route.name}` : '',
)
</script>

<template>
  <v-app>
    <v-navigation-drawer v-model="drawer">
      <MainMenu />
      <template v-slot:append>
        <div class="pa-2">
          <v-list-item
            lines="two"
            prepend-avatar="https://randomuser.me/api/portraits/women/81.jpg"
            title="Jane Doe"
            subtitle="jane.doe@example.com"
          ></v-list-item>
        </div>
      </template>
    </v-navigation-drawer>
    <v-app-bar>
      <v-app-bar-nav-icon @click="drawer = !drawer"></v-app-bar-nav-icon>
      <div class="d-flex align-center">
        <v-img src="/images/logo_1.png" alt="Logo" width="40" height="40" class="mr-2"></v-img>
        Filet-o-Files{{ reactiveRouteName }}
      </div>
    </v-app-bar>

    <v-main>
      <v-container>
        <RouterView />
      </v-container>
    </v-main>
  </v-app>
</template>

<script>
export default {
  data: () => ({ drawer: false }),
}
</script>

<style scoped></style>
