<template>
  <v-dialog v-model="dialogVisible" max-width="500" @update:model-value="$emit('update:modelValue', $event)">
    <v-card>
      <v-card-title>Выбор тега</v-card-title>
      <v-card-text>
        <v-text-field
          v-model="searchQuery"
          label="Поиск тега"
          variant="outlined"
          density="compact"
          prepend-inner-icon="mdi-magnify"
          clearable
          @update:model-value="onSearch"
        ></v-text-field>

        <v-list v-if="searchResults.length > 0" class="mt-2" max-height="300" style="overflow-y: auto">
          <v-list-item
            v-for="tag in searchResults"
            :key="tag.id"
            @click="selectTag(tag)"
          >
            <template v-slot:prepend>
              <v-chip
                size="small"
                :style="{
                  backgroundColor: tag.color || '#757575',
                  color: getContrastColor(tag.color),
                }"
              >
                {{ tag.name }}
              </v-chip>
            </template>
          </v-list-item>
        </v-list>
        <div v-else-if="searchQuery && !loading" class="text-center mt-4">
          <v-btn
            color="primary"
            variant="tonal"
            @click="createTag"
          >
            <v-icon left>mdi-plus</v-icon>
            Создать "{{ searchQuery }}"
          </v-btn>
        </div>
        <div v-else-if="!searchQuery" class="text-center mt-4 text-grey">
          Введите название для поиска
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
const searchQuery = ref('')
const searchResults = ref([])
const loading = ref(false)

watch(() => props.modelValue, (val) => {
  dialogVisible.value = val
  if (val) {
    searchQuery.value = ''
    searchResults.value = []
    loadAllTags()
  }
})

const searchTagsDebounced = debounce(async (query) => {
  if (!query || query.length < 1) {
    searchResults.value = []
    return
  }

  loading.value = true
  try {
    const result = await tagStore.searchTags(query)
    searchResults.value = result.items || result || []
  } catch (error) {
    console.error('Ошибка поиска тегов:', error)
    searchResults.value = []
  } finally {
    loading.value = false
  }
}, 300)

const onSearch = (value) => {
  searchQuery.value = value || ''
  searchTagsDebounced(value || '')
}

const loadAllTags = async () => {
  loading.value = true
  try {
    const result = await tagStore.searchTags('')
    searchResults.value = result.items || result || []
  } catch (error) {
    console.error('Ошибка загрузки тегов:', error)
  } finally {
    loading.value = false
  }
}

const selectTag = (tag) => {
  emit('select', tag)
  close()
}

const createTag = async () => {
  if (!searchQuery.value) return

  try {
    const newTag = await tagStore.createTag({ name: searchQuery.value })
    selectTag(newTag)
  } catch (error) {
    console.error('Ошибка создания тега:', error)
  }
}

const close = () => {
  dialogVisible.value = false
  emit('update:modelValue', false)
}
</script>
