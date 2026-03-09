<template>
  <v-dialog v-model="dialogVisible" max-width="500" @update:model-value="$emit('update:modelValue', $event)">
    <v-card>
      <v-card-title>Фильтр</v-card-title>
      <v-card-text>
        <v-autocomplete
          v-model="selectedTagInput"
          :items="tagSearchResults"
          item-title="name"
          item-value="id"
          label="Тег"
          variant="outlined"
          density="compact"
          prepend-inner-icon="mdi-magnify"
          no-data-text="Данных нет"
          chips
          closable-chips
          multiple
          return-object
          :loading="tagsLoading"
          @update:search="onTagSearch"
          @update:model-value="onTagsSelected"
          @update:opened="onMenuOpen"
          cache-items
        >
          <template v-slot:chip="{ props, item }">
            <v-chip
              v-bind="props"
              closable
              @click:close="removeTag(item.raw)"
              size="small"
              :style="{
                backgroundColor: item.raw.color || '#757575',
                color: getContrastColor(item.raw.color),
              }"
            >
              {{ item.raw.name }}
            </v-chip>
          </template>
        </v-autocomplete>
      </v-card-text>
      <v-card-actions>
        <v-btn color="error" variant="text" @click="clearAll">Очистить</v-btn>
        <v-spacer></v-spacer>
        <v-btn color="grey" variant="text" @click="close">Отмена</v-btn>
        <v-btn color="primary" variant="elevated" @click="apply">Применить</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup>
import { ref, watch } from 'vue'
import { useTagsStore } from '../stores/tags.store'
import { debounce } from '../utils/debounce'
import { getContrastColor } from '../utils/colors'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  selectedTags: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['update:modelValue', 'update'])

const tagStore = useTagsStore()

const dialogVisible = ref(props.modelValue)
const tagSearchQuery = ref('')
const tagSearchResults = ref([])
const tagsLoading = ref(false)
const selectedTagInput = ref([])

watch(() => props.modelValue, async (val) => {
  dialogVisible.value = val
  if (val) {
    tagSearchQuery.value = ''
    selectedTagInput.value = []

    // Загружаем теги для отображения в выпадающем списке
    tagsLoading.value = true
    try {
      const result = await tagStore.searchTags('')
      tagSearchResults.value = result.items || result || []
    } catch (error) {
      console.error('Ошибка загрузки тегов:', error)
    } finally {
      tagsLoading.value = false
    }

    // Загружаем выбранные теги для отображения в чипах
    if (props.selectedTags.length > 0) {
      const allTags = tagSearchResults.value
      selectedTagInput.value = allTags.filter(t => props.selectedTags.includes(t.id))
    }
  }
})

const searchTagsDebounced = debounce(async (query) => {
  if (!query || query.length < 1) {
    tagSearchResults.value = []
    return
  }

  tagsLoading.value = true
  try {
    const result = await tagStore.searchTags(query)
    tagSearchResults.value = result.items || result || []
  } catch (error) {
    console.error('Ошибка поиска тегов:', error)
    tagSearchResults.value = []
  } finally {
    tagsLoading.value = false
  }
}, 300)

const onTagSearch = (value) => {
  tagSearchQuery.value = value || ''
  searchTagsDebounced(value || '')
}

const onMenuOpen = async () => {
  // Всегда загружаем теги при открытии меню
  if (tagSearchResults.value.length === 0 || tagSearchQuery.value) {
    tagsLoading.value = true
    try {
      const result = await tagStore.searchTags('')
      tagSearchResults.value = result.items || result || []
    } catch (error) {
      console.error('Ошибка загрузки тегов:', error)
    } finally {
      tagsLoading.value = false
    }
  }
}



const onTagsSelected = (selected) => {
  selectedTagInput.value = selected
}

const removeTag = (tag) => {
  selectedTagInput.value = selectedTagInput.value.filter((t) => t.id !== tag.id)
}

const clearAll = () => {
  selectedTagInput.value = []
  tagSearchQuery.value = ''
  emit('update', [])
  close()
}

const apply = () => {
  const tagIds = selectedTagInput.value.map(t => t.id)
  emit('update', tagIds)
  close()
}

const close = () => {
  dialogVisible.value = false
  emit('update:modelValue', false)
}
</script>
