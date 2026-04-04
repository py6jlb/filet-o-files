import { createRouter, createWebHistory } from 'vue-router'
import RecipesView from '@/views/RecipesView.vue'
import RecipeView from '@/views/RecipeView.vue'
import ProfileView from '@/views/ProfileView.vue'
import SchedulerView from '@/views/SchedulerView.vue'
import LoginView from '@/views/LoginView.vue'
import LogoutView from '@/views/LogoutView.vue'
import AddRecipeView from '@/views/AddRecipeView.vue'
import EditRecipeView from '@/views/EditRecipeView.vue'
import PdfView from '@/views/PdfView.vue'
import { useAuthStore } from '../stores/auth.store'
import tokenService from '../utils/token.service'

// Функция проверки валидности токена
const isTokenValid = () => {
  const token = tokenService.accessToken
  if (!token) return false

  try {
    const payload = JSON.parse(atob(token.split('.')[1]))
    // Проверяем, что токен не истёк (с запасом 30 секунд)
    return payload.exp * 1000 > Date.now() + 30000
  } catch {
    return false
  }
}

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'recipes',
      component: RecipesView,
      meta: { requiresAuth: true, routeName: 'Рецепты' },
    },
    {
      path: '/recipe/:id',
      name: 'recipe',
      component: RecipeView,
      meta: { requiresAuth: true, routeName: 'Просмотр рецепта' },
    },
    {
      path: '/add_recipe',
      name: 'add_recipe',
      component: AddRecipeView,
      meta: { requiresAuth: true, routeName: 'Новый рецепт' },
    },
    {
      path: '/edit_recipe/:id',
      name: 'edit_recipe',
      component: EditRecipeView,
      meta: { requiresAuth: true, routeName: 'Редактирование рецепта' },
    },
    {
      path: '/scheduler',
      name: 'scheduler',
      component: SchedulerView,
      meta: { requiresAuth: true, routeName: 'Планирование' },
    },
    {
      path: '/profile',
      name: 'profile',
      component: ProfileView,
      meta: { requiresAuth: true, routeName: 'Профиль' },
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView,
      meta: { requiresAuth: false, routeName: 'Вход' },
    },
    {
      path: '/logout',
      name: 'logout',
      component: LogoutView,
      meta: { requiresAuth: false, routeName: 'Выход' },
    },
    {
      path: '/pdf/:id',
      name: 'pdf',
      component: PdfView,
      meta: { requiresAuth: true, routeName: 'Просмотр PDF' },
    },
  ],
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  // Проверяем, инициализирован ли auth store
  if (!authStore.isInitialized) {
    // Ждём инициализации - показываем текущую страницу
    next()
    return
  }

  // Проверяем: нужен ли auth и валиден ли токен
  if (to.meta.requiresAuth) {
    if (!authStore.user || !isTokenValid()) {
      // Токен недействителен или пользователь не авторизован
      next({ name: 'login' })
      return
    }
  }

  next()
})

// Обновление заголовка страницы при каждом переходе
router.afterEach((to) => {
  const appName = 'Поваренная книга'
  const routeName = to.meta.routeName || 'Страница'
  document.title = `${appName} - ${routeName}`
})

export default router
