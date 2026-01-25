<template>
  <v-row align="center" justify="center">
    <v-col cols="12" sm="8" md="4">
      <v-card class="elevation-12">
        <v-card-text>
          <div v-if="loginError">
            {{ loginError }}
          </div>
          <v-form @submit.prevent="onSubmit">
            <v-text-field
              v-model="email"
              label="Email"
              name="email"
              prepend-icon="mdi-account"
              type="text"
              :error-messages="emailError"
              required
            ></v-text-field>

            <v-text-field
              v-model="password"
              id="password"
              label="Пароль"
              name="password"
              prepend-icon="mdi-lock"
              type="password"
              :error-messages="passwordError"
              required
            ></v-text-field>
            <v-btn type="submit" color="primary" :loading="isLoading" block> Войти </v-btn>
          </v-form>
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
</template>

<script setup>
import { useForm, useField } from 'vee-validate'
import * as yup from 'yup'
import { useAuthStore } from '../stores/auth.store'
import { ref } from 'vue'
import router from '../router/index'

const schema = yup.object({
  email: yup.string().required('Введите e-mail').email('Введите корректный e-mail'),
  password: yup.string().required('Введите пароль').min(6, 'Минимум 6 символов'),
})

const { handleSubmit } = useForm({
  validationSchema: schema,
})

const { value: email, errorMessage: emailError } = useField('email')
const { value: password, errorMessage: passwordError } = useField('password')

const auth = useAuthStore()
const loginError = ref('')

const isLoading = ref(false)

const onSubmit = handleSubmit(async (values) => {
  isLoading.value = true
  loginError.value = ''
  try {
    await auth.login(values.email, values.password)
    router.push('/')
  } catch (err) {
    loginError.value = err.response?.data?.message || 'Ошибка при входе'
    isLoading.value = false
  }
})
</script>
