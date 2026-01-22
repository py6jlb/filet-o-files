import { createRouter, createWebHistory } from 'vue-router'
import RecipesView from '../views/RecipesView.vue'
import ProfileView from '../views/ProfileView.vue'
import SchedulerView from '@/views/SchedulerView.vue'
import { useAuthStore } from '../stores/auth.store'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'recipes',
      component: RecipesView,
      meta: { requiresAuth: true },
    },
    {
      path: '/scheduler',
      name: 'scheduler',
      component: SchedulerView,
      meta: { requiresAuth: true },
    },
    {
      path: '/profile',
      name: 'profile',
      component: ProfileView,
      meta: { requiresAuth: true },
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/LoginView.vue'),
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
