<template>
  <div class="flex justify-center items-center min-h-screen bg-base-200">
    <div v-if="loginError" class="alert alert-error mb-4">
      {{ loginError }}
    </div>

    <div v-if="loginInfo" class="alert alert-info mb-4">
      {{ loginInfo }}
    </div>
    <div class="w-full max-w-md p-6 bg-base-100 rounded-xl shadow">
      <Form :validation-schema="schema" @submit="onSubmit">
        <h2 class="text-2xl font-bold mb-6 text-center">Вход</h2>
        <div class="form-control mb-4">
          <label for="email" class="label">
            <span class="label-text">E-mail</span>
          </label>
          <Field
            name="email"
            type="email"
            id="email"
            placeholder="you@example.com"
            class="input input-bordered w-full"
          />
          <ErrorMessage name="email" class="text-error text-sm mt-1" />
        </div>

        <div class="form-control mb-6">
          <label for="password" class="label">
            <span class="label-text">Пароль</span>
          </label>
          <Field
            name="password"
            type="password"
            id="password"
            placeholder="••••••••"
            class="input input-bordered w-full"
          />
          <ErrorMessage name="password" class="text-error text-sm mt-1" />
        </div>

        <button class="btn btn-primary w-full" type="submit">Войти</button>
      </Form>
    </div>
    <div class="flex flex-col items-center gap-3">
      <div v-if="!isLoaded" class="flex flex-col items-center justify-center">
        <span class="loading loading-spinner loading-lg text-primary"></span>
      </div>
      <telegram-login-temp
        v-else
        mode="callback"
        :telegram-login="botName"
        :size="'medium'"
        :userpic="false"
        :radius="12"
        @loaded="onWidgetLoaded"
        @callback="onTelegramLogin"
        class="btn btn-outline btn-primary"
      />
    </div>
  </div>
</template>

<script setup>
import { Form, Field, ErrorMessage } from 'vee-validate'
import * as yup from 'yup'
import { useAuthStore } from '../stores/auth.store'
import { ref } from 'vue'
import router from '../router/index'
import { telegramLoginTemp } from 'vue3-telegram-login'

const schema = yup.object({
  email: yup.string().required('Введите e-mail').email('Введите корректный e-mail'),
  password: yup.string().required('Введите пароль').min(6, 'Минимум 6 символов'),
})
const botName = ref(import.meta.env.VITE_TELEGRAM_BOT_NAME)
const auth = useAuthStore()
const loginError = ref('')
const loginInfo = ref('')

async function onSubmit(values) {
  loginError.value = ''
  try {
    await auth.login(values.email, values.password)
    router.push('/')
  } catch (err) {
    loginError.value = err.response?.data?.message || 'Ошибка при входе'
  }
}

const isLoaded = ref(false)
function onWidgetLoaded() {
  isLoaded.value = true
}
async function onTelegramLogin(user) {
  try {
    var result = await auth.telegramLogin(user)
    if (result) {
      loginInfo.value = result
    } else {
      router.push('/')
    }
  } catch (err) {
    loginError.value = err.response?.data?.message || 'Ошибка при входе'
  }
}
</script>
