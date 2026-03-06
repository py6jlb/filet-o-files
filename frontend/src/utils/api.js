import axios from 'axios'
import tokenService from './token.service'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
})

let isRefreshing = false
let failedQueue = []

const processQueue = (error, token = null) => {
  failedQueue.forEach((prom) => {
    if (error) prom.reject(error)
    else prom.resolve(token)
  })
  failedQueue = []
}

api.interceptors.request.use((config) => {
  if (tokenService.accessToken) {
    config.headers.Authorization = `Bearer ${tokenService.accessToken}`
  }
  return config
})

api.interceptors.response.use(
  (response) => response,
  async (err) => {
    const originalRequest = err.config

    if (err.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject })
        }).then((token) => {
          originalRequest.headers.Authorization = `Bearer ${token}`
          return api(originalRequest)
        })
      }

      originalRequest._retry = true
      isRefreshing = true

      try {
        const { useAuthStore } = await import('../stores/auth.store')
        const { default: router } = await import('../router/index')
        const auth = useAuthStore()

        const success = await auth.refresh()

        if (success) {
          processQueue(null, tokenService.accessToken)
          originalRequest.headers.Authorization = `Bearer ${tokenService.accessToken}`
          return api(originalRequest)
        } else {
          processQueue(new Error('Refresh failed'), null)
          router.push('/login')
          return Promise.reject(err)
        }
      } catch (refreshError) {
        const { default: router } = await import('../router/index')
        processQueue(refreshError, null)
        router.push('/login')
        return Promise.reject(refreshError)
      } finally {
        isRefreshing = false
      }
    }

    return Promise.reject(err)
  },
)

export default api
