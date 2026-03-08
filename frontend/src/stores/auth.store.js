import { defineStore } from 'pinia'
import axios from '../utils/api'
import tokenService from '../utils/token.service'
import router from '../router/index'

function parseJwt(token) {
  try {
    const base64Url = token.split('.')[1]
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join(''),
    )
    return JSON.parse(jsonPayload)
  } catch {
    return null
  }
}

function getTokenExpiryTime(accessToken) {
  const payload = parseJwt(accessToken)
  if (!payload?.exp) return 0
  return payload.exp * 1000 - Date.now()
}

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    refreshTimeout: null,
    isInitialized: false,
  }),
  getters: {
    isAuthenticated: (state) => !!state.user && !!tokenService.accessToken,
  },
  actions: {
    async login(username, password) {
      try {
        const res = await axios.post('/auth/login', { email: username, password })
        this.setTokens(res.data.accessToken, res.data.refreshToken)
        this.scheduleRefresh()
      } catch (error) {
        this.clearAuth()
        throw error
      }
    },

    async logout() {
      try {
        this.clearAuth()
      } catch {
        // Игнорируем ошибки при logout
      } finally {
        router.push('/login')
      }
    },

    async refresh() {
      try {
        const res = await axios.post('/auth/refresh', {
          refreshToken: tokenService.refreshToken,
        })
        this.setTokens(res.data.accessToken, res.data.refreshToken)
        this.scheduleRefresh()
        return true
      } catch (error) {
        console.error('Token refresh failed:', error)
        this.clearAuth()
        router.push('/login')
        return false
      }
    },

    setTokens(accessToken, refreshToken) {
      // Синхронизируем с tokenService
      tokenService.accessToken = accessToken
      tokenService.refreshToken = refreshToken

      this.user = parseJwt(accessToken)

      localStorage.setItem(
        'auth',
        JSON.stringify({
          accessToken,
          refreshToken,
          user: this.user,
        }),
      )
    },

    scheduleRefresh() {
      this.clearRefresh()

      const timeUntilExpiry = getTokenExpiryTime(tokenService.accessToken)
      // Обновляем за 30 секунд до истечения
      const timeout = timeUntilExpiry - 30000

      if (timeout > 0) {
        this.refreshTimeout = setTimeout(() => this.refresh(), timeout)
      } else {
        // Токен уже истёк или истекает очень скоро
        this.refresh()
      }
    },

    clearRefresh() {
      if (this.refreshTimeout) {
        clearTimeout(this.refreshTimeout)
        this.refreshTimeout = null
      }
    },

    clearAuth() {
      this.clearRefresh()
      tokenService.clear()
      this.user = null
      localStorage.removeItem('auth')
    },

    initializeAuth() {
      tokenService.setFromStorage()
      const accessToken = tokenService.accessToken
      const refreshToken = tokenService.refreshToken

      if (!accessToken || !refreshToken) {
        this.isInitialized = true
        this.clearAuth()
        router.push('/login')
        return
      }

      try {
        const expiryTime = getTokenExpiryTime(accessToken)

        if (expiryTime <= 0) {
          // Токен истёк - пробуем обновить
          this.user = parseJwt(accessToken)
          // Вызываем refresh и ждём результат
          this.refresh().then((success) => {
            if (!success) {
              // Не удалось обновить токен - перенаправляем на login
              this.clearAuth()
              router.push('/login')
            }
          }).catch(() => {
            this.clearAuth()
            router.push('/login')
          }).finally(() => {
            this.isInitialized = true
          })
          return
        }

        this.setTokens(accessToken, refreshToken)
        this.scheduleRefresh()
      } catch (error) {
        console.error('Failed to initialize auth:', error)
        this.clearAuth()
        router.push('/login')
      } finally {
        this.isInitialized = true
      }
    },
  },
})
