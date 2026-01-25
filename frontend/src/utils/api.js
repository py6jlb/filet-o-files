import axios from "axios";
import { useAuthStore } from "../stores/auth.store";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL
});

api.interceptors.request.use(config => {
  const auth = useAuthStore();
  if (auth.accessToken) {
    config.headers.Authorization = `Bearer ${auth.accessToken}`;
  }
  return config;
});


api.interceptors.response.use(
  res => res,
  async err => {
    const auth = useAuthStore();
    const originalRequest = err.config;

    if (err.response?.status === 401 && !originalRequest._retry && auth.refreshToken) {
      originalRequest._retry = true;
      await auth.refresh();
      originalRequest.headers.Authorization = `Bearer ${auth.accessToken}`;
      return api(originalRequest);
    }

    return Promise.reject(err);
  }
);

export default api;
