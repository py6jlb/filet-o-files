<template>
  <v-table v-if="tags.length > 0" density="compact" class="tags-table">
    <tbody>
      <tr v-for="tagInfo in tags" :key="tagInfo.id">
        <td class="tag-cell">
          <v-chip
            size="small"
            :style="{
              backgroundColor: tagInfo.color || '#757575',
              color: getContrastColor(tagInfo.color),
            }"
          >
            {{ tagInfo.name }}
          </v-chip>
        </td>
        <td class="info-cell">
          <v-textarea
            v-model="tagInfo.additionalInfo"
            density="compact"
            variant="outlined"
            placeholder="Доп. информация"
            rows="1"
            auto-grow
            hide-details
          ></v-textarea>
        </td>
        <td class="action-cell">
          <v-btn
            icon="mdi-delete"
            size="small"
            variant="text"
            color="error"
            @click="$emit('remove', tagInfo)"
          ></v-btn>
        </td>
      </tr>
    </tbody>
  </v-table>
</template>

<script setup>
import { getContrastColor } from '../utils/colors'

defineProps({
  tags: {
    type: Array,
    default: () => []
  }
})

defineEmits(['remove'])
</script>

<style scoped>
.tags-table {
  width: 100%;
}

.tags-table .tag-cell {
  width: 120px;
  vertical-align: middle;
}

.tags-table .info-cell {
  vertical-align: middle;
}

.tags-table .action-cell {
  width: 50px;
  vertical-align: middle;
  text-align: center;
}
</style>
