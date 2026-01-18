<script setup>
import { RouterLink, RouterView } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'
import MainMenu from './components/MainMenu.vue'

const authStore = useAuthStore()
function logout() {
  authStore.logout()
}
</script>

<template>
  <div>
    <nav >
      <div >
        <div class="dropdown" v-if="authStore.user">
          <button tabindex="0">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M4 6h16M4 12h8m-8 6h16"
              />
            </svg>
          </button>
          <ul
            tabindex="0"
          >
            <MainMenu />
          </ul>
        </div>
        <RouterLink to="/">Filet-o-files</RouterLink>
      </div>
      <div  v-if="authStore.user">
        <ul >
          <MainMenu />
        </ul>
      </div>
      <div  v-if="authStore.user">
        <div >
          <div tabindex="0" role="button" >
            <div >
              <img alt="avatar" src="@/assets/avatar.svg" />
            </div>
          </div>
          <ul
            tabindex="0"
          >
            <li><RouterLink to="/profile">Профиль</RouterLink></li>
            <!-- <li><a>Настройки</a></li> -->
            <li><a @click="logout">Выход</a></li>
          </ul>
        </div>
      </div>
    </nav>

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
