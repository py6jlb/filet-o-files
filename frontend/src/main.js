import './styles/style.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'

import { useAuthStore } from "./stores/auth.store";

const app = createApp(App)

app.use(createPinia())
app.use(router)

const auth = useAuthStore();
auth.initializeAuth();

app.mount('#app')
