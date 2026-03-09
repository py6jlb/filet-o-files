<template>
  <v-dialog v-model="dialogVisible" max-width="500" @update:model-value="$emit('update:modelValue', $event)">
    <v-card>
      <v-card-title>Выбор тега</v-card-title>
      <v-card-text>
        <v-autocomplete
          v-model="selectedTagInput"
          :items="tagSearchResults"
          item-title="name"
          item-value="id"
          label="Поиск тега"
          variant="outlined"
          density="compact"
          prepend-inner-icon="mdi-magnify"
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

        <div v-if="selectedTagInput.length > 0" class="text-center mt-2">
          <v-btn color="primary" @click="applySelected">
            Добавить выбранные
          </v-btn>
        </div>
        <div v-else-if="searchQuery && !tagsLoading" class="text-center mt-2">
          <v-btn color="primary" variant="tonal" @click="createTag">
            <v-icon left>mdi-plus</v-icon>
            Создать "{{ searchQuery }}"
          </v-btn>
        </div>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="grey" variant="text" @click="close">Закрыть</v-btn>
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
  }
})

const emit = defineEmits(['update:modelValue', 'select'])

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

    // Загружаем теги при открытии
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
  if (tagSearchResults.value.length === 0) {
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

const applySelected = () => {
  // Добавляем по одному тегу
  selectedTagInput.value.forEach(tag => {
    emit('select', tag)
  })
  close()
}

const createTag = async () => {
  if (!tagSearchQuery.value) return

  try {
    const newTag = await tagStore.createTag({ name: tagSearchQuery.value })
    emit('select', newTag)
    close()
  } catch (error) {
    console.error('Ошибка создания тега:', error)
  }
}

const close = () => {
  dialogVisible.value = false
  emit('update:modelValue', false)
}
</script>
