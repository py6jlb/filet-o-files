<template>
  <div class="flex justify-center items-center min-h-screen bg-base-200">
    <div class="w-full max-w-md p-6 bg-base-100 rounded-xl shadow">
      <Form :validation-schema="schema" @submit="onSubmit">
        <h2 class="text-2xl font-bold mb-6 text-center">Вход</h2>

        <div v-if="loginError" class="alert alert-error mb-4">
          {{ loginError }}
        </div>

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
  </div>
</template>

<script setup>
import { Form, Field, ErrorMessage } from 'vee-validate'
import * as yup from 'yup'
import { useAuthStore } from '../stores/auth.store'
import { ref } from 'vue'
import router from '../router/index'

const schema = yup.object({
  email: yup.string().required('Введите e-mail').email('Введите корректный e-mail'),
  password: yup.string().required('Введите пароль').min(6, 'Минимум 6 символов'),
})

const auth = useAuthStore()
const loginError = ref('')

async function onSubmit(values) {
  loginError.value = ''
  try {
    await auth.login(values.email, values.password)
    router.push('/')
  } catch (err) {
    loginError.value = err.response?.data?.message || 'Ошибка при входе'
  }
}
</script>
