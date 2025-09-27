<script setup>
import { RouterView } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'

const authStore = useAuthStore()
function logout() {
  authStore.logout()
}
</script>

<template>
  <div class="flex flex-col min-h-screen bg-base-200">
    <nav class="navbar bg-base-100 shadow-sm">
      <div class="navbar-start">
        <div class="dropdown" v-if="authStore.user">
          <button tabindex="0" class="btn btn-ghost lg:hidden">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-5 w-5"
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
            class="menu menu-sm dropdown-content mt-3 z-[1] p-2 shadow bg-base-100 rounded-box w-52"
          >
            <li><a href="#">Главная</a></li>
            <li><a href="#">Планировщик</a></li>
          </ul>
        </div>
        <a class="btn btn-ghost text-xl" href="#">Filet-o-files</a>
      </div>
      <div class="navbar-center hidden lg:flex" v-if="authStore.user">
        <ul class="menu menu-horizontal px-1">
          <li><a href="#">Главная</a></li>
          <li><a href="#">Планировщик</a></li>
        </ul>
      </div>
      <div class="navbar-end" v-if="authStore.user">
        <div class="dropdown dropdown-end">
          <div tabindex="0" role="button" class="btn btn-ghost btn-circle avatar">
            <div class="w-10 rounded-full">
              <img
                alt="Tailwind CSS Navbar component"
                src="@/assets/avatar.svg"
              />
            </div>
          </div>
          <ul
            tabindex="0"
            class="menu dropdown-content bg-base-100 rounded-box z-1 mt-3 w-52 p-2 shadow"
          >
            <li><a>Профиль</a></li>
            <!-- <li><a>Настройки</a></li> -->
            <li><a @click="logout">Выход</a></li>
          </ul>
        </div>
      </div>
    </nav>

    <main class="flex-1 container mx-auto py-6 px-4">
      <RouterView />
    </main>

    <footer class="footer bg-base-100 p-4 text-base-content">
      <div class="container mx-auto flex flex-col md:flex-row justify-between items-center">
        <div>Filet-o-files</div>
      </div>
    </footer>
  </div>
</template>

<style scoped></style>
