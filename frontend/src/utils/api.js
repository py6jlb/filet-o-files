import axios from 'axios'
import { useAuthStore } from '../stores/auth.store'
import { storeToRefs } from 'pinia'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
})

api.interceptors.request.use((config) => {
  const auth = useAuthStore()
  const { accessToken } = storeToRefs(auth)
  console.log(accessToken.value, "1")
  if (accessToken.value) {
    config.headers.Authorization = `Bearer ${accessToken.value}`
  }
  return config
})

api.interceptors.response.use(
  (res) => res,
  async (err) => {
    const auth = useAuthStore()
    const { accessToken, refreshToken } = storeToRefs(auth)
    const originalRequest = err.config
    if (err.response?.status === 401 && !originalRequest._retry && refreshToken.value) {
      originalRequest._retry = true
      await auth.refresh()
      originalRequest.headers.Authorization = `Bearer ${accessToken.value}`
      console.log(accessToken.value, "2")
      return api(originalRequest)
    }

    return Promise.reject(err)
  },
)

export default api
