<template>
  <v-row align="center" justify="center">
    <v-col cols="12" sm="8" md="4">
      <v-card>
        <v-card-text>
          <LoadingIndicator v-if="loading" text="Загрузка профиля..." />
          <ErrorMessage v-else-if="error" :message="error" />
          <template v-else>
            <v-img
              src="images/avatar.svg"
              alt="avatar"
              width="120"
              height="120"
              class="mx-auto"
            ></v-img>
            <dl>
              <div>
                <dt><b>Имя пользователя: </b></dt>
                <dd>{{ user?.name ?? 'Гость' }}</dd>
              </div>

              <div>
                <dt><b>E-mail: </b></dt>
                <dd>{{ user?.email ?? '-' }}</dd>
              </div>
            </dl>
          </template>
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
</template>

<script setup>
import { onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useUserStore } from '../stores/user.store'
import LoadingIndicator from '../components/LoadingIndicator.vue'
import ErrorMessage from '../components/ErrorMessage.vue'

const userStore = useUserStore()
const { user, loading, error } = storeToRefs(userStore)

onMounted(() => {
  userStore.fetchProfile()
})
</script>

<style scoped>
dt,
dd {
  display: inline;
}
</style>
