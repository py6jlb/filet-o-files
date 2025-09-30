import { defineStore } from 'pinia'
import axios from '../utils/api'
import router from '../router/index'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: JSON.parse(localStorage.getItem('user')) || null,
    accessToken: localStorage.getItem('accessToken') || null,
    refreshToken: localStorage.getItem('refreshToken') || null,
    refreshTimeout: null,
  }),
  actions: {
    async login(username, password) {
      const res = await axios.post('/auth/login', { email: username, password })
      this.accessToken = res.data.accessToken
      this.refreshToken = res.data.refreshToken
      const payload = JSON.parse(atob(this.accessToken.split('.')[1]))
      this.user = payload

      localStorage.setItem('accessToken', this.accessToken)
      localStorage.setItem('refreshToken', this.refreshToken)
      localStorage.setItem('user', JSON.stringify(this.user))
      this.scheduleRefresh()
    },
    async telegramLogin(user) {
      const res = await axios.post('/auth/tg/callback', user)
      if (res.status === 202) {
        return 'Заявка на регистрацию на рассмотрении'
      } else {
        this.accessToken = res.data.accessToken
        this.refreshToken = res.data.refreshToken
        const payload = JSON.parse(atob(this.accessToken.split('.')[1]))
        this.user = payload

        localStorage.setItem('accessToken', this.accessToken)
        localStorage.setItem('refreshToken', this.refreshToken)
        localStorage.setItem('user', JSON.stringify(this.user))
        this.scheduleRefresh()
        return undefined
      }
    },
    async logout() {
      this.clearRefresh()
      this.accessToken = null
      this.refreshToken = null
      this.user = null
      localStorage.removeItem('accessToken')
      localStorage.removeItem('refreshToken')
      localStorage.removeItem('user')
      router.push('/login')
    },
    async refresh() {
      const res = await axios.post('/auth/refresh', { refreshToken: this.refreshToken })
      this.accessToken = res.data.accessToken
      this.refreshToken = res.data.refreshToken
      localStorage.setItem('accessToken', this.accessToken)
      localStorage.setItem('refreshToken', this.refreshToken)
      this.scheduleRefresh()
    },
    scheduleRefresh() {
      this.clearRefresh()
      const payload = JSON.parse(atob(this.accessToken.split('.')[1]))
      const timeout = payload.exp * 1000 - Date.now() - 60000
      if (timeout > 0) {
        this.refreshTimeout = setTimeout(() => this.refresh(), timeout)
      }
    },
    clearRefresh() {
      if (this.refreshTimeout) clearTimeout(this.refreshTimeout)
      this.refreshTimeout = null
    },
    initializeAuth() {
      this.accessToken = localStorage.getItem('accessToken')
      this.refreshToken = localStorage.getItem('refreshToken')
      this.user = JSON.parse(localStorage.getItem('user'))
      //console.log(this.accessToken, this.refreshToken, this.user)
      if (this.accessToken) this.scheduleRefresh()
    },
  },
})
