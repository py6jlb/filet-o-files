import { createRouter, createWebHistory } from 'vue-router'
import RecipesView from '@/views/RecipesView.vue'
import ProfileView from '@/views/ProfileView.vue'
import SchedulerView from '@/views/SchedulerView.vue'
import LoginView from '@/views/LoginView.vue'
import { useAuthStore } from '../stores/auth.store'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'recipes',
      component: RecipesView,
      meta: { requiresAuth: true, routeName: "Рецепты" },
    },
    {
      path: '/scheduler',
      name: 'scheduler',
      component: SchedulerView,
      meta: { requiresAuth: true, routeName: "Планирование" },
    },
    {
      path: '/profile',
      name: 'profile',
      component: ProfileView,
      meta: { requiresAuth: true, routeName: "Профиль" },
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView,
      meta: { requiresAuth: false, routeName: "Вход" },
    },
  ],
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  if (to.meta.requiresAuth && !authStore.user) {
    next({ name: 'login' })
  } else {
    next()
  }
})

export default router
