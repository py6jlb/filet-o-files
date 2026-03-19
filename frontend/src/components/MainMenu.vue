<template>
  <v-navigation-drawer v-model="modelValue" absolute temporary>
    <div class="d-flex">
      <v-spacer /><v-btn
        block
        @click="handleClick"
        variant="plain"
        size="small"
        prepend-icon="mdi-chevron-triple-left"
        slim
        rounded="0"
      >
        Свернуть</v-btn
      >
    </div>
    <v-list>
      <v-list-item :to="'/'"> <v-icon>mdi-chef-hat</v-icon> Рецепты </v-list-item>
      <v-list-item :to="'/scheduler'">
        <v-icon>mdi-calendar-month</v-icon> Планирование
      </v-list-item>
    </v-list>
  </v-navigation-drawer>
</template>

<script setup>

const modelValue = defineModel()

const handleClick = () => {
  modelValue.value = false
}
</script>

<style scoped>
.slim-item {
  min-height: 24px !important;
  padding: 0 8px !important;
}
</style>
