<script setup>
import { RouterView } from 'vue-router'

import MainMenu from './components/MainMenu.vue'

</script>

<template>
  <div class="l-site">
    <MainMenu />

    <main >
      <RouterView />
    </main>

    <footer>
      <div>
        <div>Filet-o-files</div>
      </div>
    </footer>
  </div>
</template>

<style scoped></style>
