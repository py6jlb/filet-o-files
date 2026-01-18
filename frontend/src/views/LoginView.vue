<template>
  <div >
    <div v-if="loginError">
      {{ loginError }}
    </div>

    <div v-if="loginInfo">
      {{ loginInfo }}
    </div>
    <div >
      <Form :validation-schema="schema" @submit="onSubmit">
        <h2 >Вход</h2>
        <div >
          <label for="email" >
            <span >E-mail</span>
          </label>
          <Field
            name="email"
            type="email"
            id="email"
            placeholder="you@example.com"

          />
          <ErrorMessage name="email"  />
        </div>

        <div >
          <label for="password" >
            <span >Пароль</span>
          </label>
          <Field
            name="password"
            type="password"
            id="password"
            placeholder="••••••••"

          />
          <ErrorMessage name="password" />
        </div>

        <button type="submit">Войти</button>
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
// const botName = ref(import.meta.env.VITE_TELEGRAM_BOT_NAME)
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

</script>
