<template>
  <div class="markdown-editor">
    <!-- Вкладки -->
    <v-tabs v-model="activeTab" color="primary" density="compact" class="mb-1">
      <v-tab value="write" size="small">
        <v-icon size="small" class="mr-1">mdi-pencil</v-icon>
        Редакт.
      </v-tab>
      <v-tab value="preview" size="small">
        <v-icon size="small" class="mr-1">mdi-eye</v-icon>
        Просмотр
      </v-tab>
    </v-tabs>

    <!-- Панель инструментов -->
    <div class="toolbar mb-1">
      <v-btn-group variant="text" density="compact">
        <v-btn size="small" @click="toggleBold" title="Жирный (Ctrl+B)">
          <v-icon>mdi-format-bold</v-icon>
        </v-btn>
        <v-btn size="small" @click="toggleItalic" title="Курсив (Ctrl+I)">
          <v-icon>mdi-format-italic</v-icon>
        </v-btn>
        <v-btn size="small" @click="toggleStrike" title="Зачёркнутый">
          <v-icon>mdi-format-strikethrough</v-icon>
        </v-btn>
        <v-divider vertical class="mx-1"></v-divider>
        <v-btn size="small" @click="toggleHeading(1)" title="Заголовок 1">
          <span class="text-body-2 font-weight-bold">H1</span>
        </v-btn>
        <v-btn size="small" @click="toggleHeading(2)" title="Заголовок 2">
          <span class="text-body-2 font-weight-bold">H2</span>
        </v-btn>
        <v-btn size="small" @click="toggleHeading(3)" title="Заголовок 3">
          <span class="text-body-2 font-weight-bold">H3</span>
        </v-btn>
        <v-divider vertical class="mx-1"></v-divider>
        <v-btn size="small" @click="toggleBulletList" title="Маркированный список">
          <v-icon>mdi-format-list-bulleted</v-icon>
        </v-btn>
        <v-btn size="small" @click="toggleNumberedList" title="Нумерованный список">
          <v-icon>mdi-format-list-numbered</v-icon>
        </v-btn>
        <v-btn size="small" @click="toggleTaskList" title="Чеклист">
          <v-icon>mdi-checkbox-marked-outline</v-icon>
        </v-btn>
        <v-divider vertical class="mx-1"></v-divider>
        <v-btn size="small" @click="toggleCode" title="Код">
          <v-icon>mdi-code-tags</v-icon>
        </v-btn>
        <v-btn size="small" @click="toggleCodeBlock" title="Блок кода">
          <v-icon>mdi-code-braces</v-icon>
        </v-btn>
        <v-divider vertical class="mx-1"></v-divider>
        <v-btn size="small" @click="toggleLink" title="Ссылка (Ctrl+K)">
          <v-icon>mdi-link</v-icon>
        </v-btn>
        <v-btn size="small" @click="toggleImage" title="Изображение">
          <v-icon>mdi-image</v-icon>
        </v-btn>
        <v-btn size="small" @click="toggleQuote" title="Цитата">
          <v-icon>mdi-format-quote-close</v-icon>
        </v-btn>
        <v-btn size="small" @click="insertHorizontalRule" title="Горизонтальная линия">
          <v-icon>mdi-minus</v-icon>
        </v-btn>
      </v-btn-group>
    </div>

    <!-- Область редактирования / просмотра -->
    <div class="editor-content">
      <div v-show="activeTab === 'write'" class="codemirror-wrapper">
        <Codemirror
          v-model="content"
          :style="{ height: editorHeight }"
          :extensions="extensions"
          :autofocus="false"
          @ready="handleReady"
        />
      </div>

      <div
        v-show="activeTab === 'preview'"
        class="markdown-preview pa-2 rounded border"
        v-html="renderedContent"
      ></div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, shallowRef } from 'vue'
import { Codemirror } from 'vue-codemirror'
import { markdown } from '@codemirror/lang-markdown'
import { EditorView, keymap, lineNumbers, highlightActiveLine } from '@codemirror/view'
import { defaultKeymap, history, historyKeymap, indentWithTab } from '@codemirror/commands'
import { languages } from '@codemirror/language-data'
import { marked } from 'marked'

const props = defineProps({
  modelValue: {
    type: String,
    default: ''
  },
  label: {
    type: String,
    default: 'Описание'
  },
  placeholder: {
    type: String,
    default: 'Введите описание в формате Markdown...'
  },
  height: {
    type: String,
    default: '250px'
  }
})

const emit = defineEmits(['update:modelValue'])

const activeTab = ref('write')
const content = ref(props.modelValue)
const editorHeight = ref(props.height)
const view = shallowRef(null)

// Настройка CodeMirror
const extensions = [
  lineNumbers(),
  highlightActiveLine(),
  history(),
  markdown({ codeLanguages: languages }),
  keymap.of([
    ...defaultKeymap,
    ...historyKeymap,
    indentWithTab,
    // Ctrl+B - жирный
    { key: 'Mod-b', run: () => { toggleBold(); return true; } },
    // Ctrl+I - курсив
    { key: 'Mod-i', run: () => { toggleItalic(); return true; } },
    // Ctrl+K - ссылка
    { key: 'Mod-k', run: () => { toggleLink(); return true; } }
  ]),
  EditorView.lineWrapping,
  EditorView.theme({
    '&': { height: '100%', fontSize: '14px' },
    '.cm-scroller': { overflow: 'auto', fontFamily: "'Consolas', 'Monaco', monospace" },
    '.cm-content': { padding: '8px 0' },
    '.cm-line': { padding: '0 8px' },
    '&.cm-focused': { outline: 'none' }
  })
]

// Рендеринг markdown
marked.setOptions({ breaks: true, gfm: true })

const renderedContent = computed(() => {
  if (!content.value) return '<p class="text-grey">Нет содержимого</p>'
  return marked.parse(content.value)
})

watch(() => props.modelValue, (newVal) => {
  if (newVal !== content.value) content.value = newVal
})

watch(content, (newVal) => emit('update:modelValue', newVal))

const handleReady = (payload) => { view.value = payload.view }

// Получение выделения
const getSelection = () => {
  if (!view.value) return { text: '', from: 0, to: 0 }
  const { from, to } = view.value.state.selection.main
  return { text: view.value.state.sliceDoc(from, to), from, to }
}

// Получить все строки в выделении
const getLinesInSelection = () => {
  if (!view.value) return []
  const { from, to } = view.value.state.selection.main
  const doc = view.value.state.doc
  const startLine = doc.lineAt(from)
  const endLine = doc.lineAt(to)
  const lines = []
  for (let i = startLine.number; i <= endLine.number; i++) {
    lines.push({ number: i, text: doc.line(i).text, from: doc.line(i).from, to: doc.line(i).to })
  }
  return lines
}

// Проверить префикс строки
const getLinePrefix = (lineText, prefix) => {
  const trimmed = lineText.trimStart()
  const offset = lineText.length - trimmed.length
  return { has: trimmed.startsWith(prefix), offset }
}

// Применить массив изменений
const applyChanges = (changes) => {
  if (!view.value || !changes.length) return
  changes.sort((a, b) => b.from - a.from)
  let offset = 0
  for (const change of changes) {
    const aFrom = change.from - offset
    const aTo = change.to - offset
    view.value.dispatch({ changes: { from: aFrom, to: aTo, insert: change.insert } })
    offset += (change.to - change.from) - change.insert.length
  }
}

const toggleBold = () => {
  const sel = getSelection()
  if (!view.value) return
  if (sel.text.includes('\n')) {
    const lines = getLinesInSelection()
    const changes = []
    for (const line of lines) {
      const prefix = getLinePrefix(line.text, '**')
      if (prefix.has) {
        changes.push({ from: line.from + prefix.offset, to: line.from + prefix.offset + 2, insert: '' })
        if (line.text.slice(-2) === '**') changes.push({ from: line.to - 2, to: line.to, insert: '' })
      } else {
        changes.push({ from: line.from + prefix.offset, to: line.from + prefix.offset, insert: '**' + line.text.substring(prefix.offset) + '**' })
      }
    }
    applyChanges(changes)
  } else {
    const p = sel.text.startsWith('**') ? '**' : ''
    const s = sel.text.endsWith('**') ? '**' : ''
    view.value.dispatch({ changes: { from: sel.from, to: sel.to, insert: p + sel.text + s } })
  }
}

const toggleItalic = () => {
  const sel = getSelection()
  if (!view.value) return
  if (sel.text.includes('\n')) {
    const lines = getLinesInSelection()
    const changes = []
    for (const line of lines) {
      const prefix = getLinePrefix(line.text, '*')
      if (prefix.has && !line.text.substring(prefix.offset, prefix.offset + 2).startsWith('**')) {
        changes.push({ from: line.from + prefix.offset, to: line.from + prefix.offset + 1, insert: '' })
      } else {
        changes.push({ from: line.from + prefix.offset, to: line.from + prefix.offset, insert: '*' + line.text.substring(prefix.offset) })
      }
    }
    applyChanges(changes)
  } else {
    const p = sel.text.startsWith('*') && !sel.text.startsWith('**') ? '*' : ''
    const s = sel.text.endsWith('*') && !sel.text.endsWith('**') ? '*' : ''
    view.value.dispatch({ changes: { from: sel.from, to: sel.to, insert: p + sel.text + s } })
  }
}

const toggleStrike = () => {
  const sel = getSelection()
  if (!view.value) return
  if (sel.text.includes('\n')) {
    const lines = getLinesInSelection()
    const changes = []
    for (const line of lines) {
      const prefix = getLinePrefix(line.text, '~~')
      if (prefix.has) {
        changes.push({ from: line.from + prefix.offset, to: line.from + prefix.offset + 2, insert: '' })
        if (line.text.slice(-2) === '~~') changes.push({ from: line.to - 2, to: line.to, insert: '' })
      } else {
        changes.push({ from: line.from + prefix.offset, to: line.from + prefix.offset, insert: '~~' + line.text.substring(prefix.offset) + '~~' })
      }
    }
    applyChanges(changes)
  } else {
    const p = sel.text.startsWith('~~') ? '~~' : ''
    const s = sel.text.endsWith('~~') ? '~~' : ''
    view.value.dispatch({ changes: { from: sel.from, to: sel.to, insert: p + sel.text + s } })
  }
}

const toggleHeading = (level) => {
  if (!view.value) return
  const prefix = '#'.repeat(level) + ' '
  const lines = getLinesInSelection()
  const changes = []
  for (const line of lines) {
    let currentLevel = 0
    const match = line.text.match(/^(#{1,6})\s?/)
    if (match) currentLevel = match[1].length
    if (currentLevel === level) {
      changes.push({ from: line.from, to: line.from + match[0].length, insert: '' })
    } else if (currentLevel > 0) {
      changes.push({ from: line.from, to: line.from + match[0].length, insert: prefix })
    } else {
      changes.push({ from: line.from, to: line.from, insert: prefix })
    }
  }
  applyChanges(changes)
}

const toggleBulletList = () => {
  if (!view.value) return
  const lines = getLinesInSelection()
  const changes = []
  for (const line of lines) {
    const prefix = getLinePrefix(line.text, '- ')
    const altPrefix = getLinePrefix(line.text, '* ')
    if (prefix.has || altPrefix.has) {
      const pos = line.from + (prefix.has ? prefix.offset : altPrefix.offset)
      changes.push({ from: pos, to: pos + 2, insert: '' })
    } else {
      changes.push({ from: line.from, to: line.from, insert: '- ' })
    }
  }
  applyChanges(changes)
}

const toggleNumberedList = () => {
  const lines = getLinesInSelection()
  if (!view.value || !lines.length) return
  let hasNumbering = true
  for (let i = 0; i < lines.length; i++) {
    const m = lines[i].text.match(/^(\d+)\.\s/)
    if (!m || parseInt(m[1]) !== i + 1) { hasNumbering = false; break }
  }
  const changes = []
  if (hasNumbering) {
    for (const line of lines) {
      const m = line.text.match(/^(\d+)\.\s/)
      if (m) changes.push({ from: line.from, to: line.from + m[0].length, insert: '' })
    }
  } else {
    for (let i = 0; i < lines.length; i++) {
      const m = lines[i].text.match(/^(\d+)\.\s/)
      if (m) changes.push({ from: lines[i].from, to: lines[i].from + m[1].length, insert: String(i + 1) })
      else changes.push({ from: lines[i].from, to: lines[i].from, insert: `${i + 1}. ` })
    }
  }
  applyChanges(changes)
}

const toggleTaskList = () => {
  const lines = getLinesInSelection()
  if (!view.value || !lines.length) return
  const changes = []
  for (const line of lines) {
    const unchecked = getLinePrefix(line.text, '- [ ] ')
    const checked = getLinePrefix(line.text, '- [x] ')
    const checkedUp = getLinePrefix(line.text, '- [X] ')
    if (checked.has || checkedUp.has) {
      changes.push({ from: line.from, to: line.from + 7, insert: '- ' })
    } else if (unchecked.has) {
      changes.push({ from: line.from + 3, to: line.from + 5, insert: 'x]' })
    } else {
      changes.push({ from: line.from, to: line.from, insert: '- [ ] ' })
    }
  }
  applyChanges(changes)
}

const toggleCode = () => {
  const sel = getSelection()
  if (!view.value) return
  const p = sel.text.startsWith('`') ? '' : '`'
  const s = sel.text.endsWith('`') ? '' : '`'
  view.value.dispatch({ changes: { from: sel.from, to: sel.to, insert: p + sel.text + s } })
}

const toggleCodeBlock = () => {
  const sel = getSelection()
  if (!view.value) return
  const hasBlock = sel.text.startsWith('```') && sel.text.endsWith('```')
  if (hasBlock) {
    const lines = sel.text.split('\n')
    const first = lines[0], last = lines[lines.length - 1]
    const from = sel.from + first.length + 1
    const to = sel.to - (last.length + 1)
    view.value.dispatch({ changes: { from, to, insert: sel.text.slice(first.length + 1, -(last.length + 1)) } })
  } else {
    const code = sel.text || 'код'
    view.value.dispatch({ changes: { from: sel.from, to: sel.to, insert: `\n\`\`\`\n${code}\n\`\`\`` } })
  }
}

const toggleLink = () => {
  const sel = getSelection()
  if (!view.value) return
  const match = sel.text.match(/^\[.+\]\(.+\)$/s)
  if (match) {
    const tm = sel.text.match(/\[(.+)\]/)
    if (tm) view.value.dispatch({ changes: { from: sel.from, to: sel.to, insert: tm[1] } })
  } else {
    const text = sel.text || 'текст ссылки'
    view.value.dispatch({ changes: { from: sel.from, to: sel.to, insert: `[${text}](url)` } })
  }
}

const toggleImage = () => {
  const sel = getSelection()
  if (!view.value) return
  const text = sel.text || 'alt текст'
  view.value.dispatch({ changes: { from: sel.from, to: sel.to, insert: `![${text}](url)` } })
}

const toggleQuote = () => {
  const lines = getLinesInSelection()
  if (!view.value || !lines.length) return
  const changes = []
  for (const line of lines) {
    const prefix = getLinePrefix(line.text, '> ')
    if (prefix.has) changes.push({ from: line.from, to: line.from + 2, insert: '' })
    else changes.push({ from: line.from, to: line.from, insert: '> ' })
  }
  applyChanges(changes)
}

const insertHorizontalRule = () => {
  const sel = getSelection()
  if (!view.value) return
  const text = sel.text ? '\n\n---\n\n' + sel.text : '\n\n---\n'
  view.value.dispatch({ changes: { from: sel.from, to: sel.to, insert: text } })
}
</script>

<style scoped>
.markdown-editor { display: flex; flex-direction: column; }
.toolbar { border-bottom: 1px solid rgba(0,0,0,0.12); padding-bottom: 4px; }
.editor-content { flex: 1; }
.codemirror-wrapper { border: 1px solid rgba(0,0,0,0.38); border-radius: 4px; overflow: hidden; }
.codemirror-wrapper :deep(.cm-editor) { height: 100%; }
.markdown-preview { background: #fafafa; }

:deep(.markdown-preview h1) { font-size: 2rem; font-weight: 600; margin-bottom: 1rem; border-bottom: 1px solid #e0e0e0; padding-bottom: 0.5rem; }
:deep(.markdown-preview h2) { font-size: 1.5rem; font-weight: 600; margin-top: 1.5rem; margin-bottom: 0.75rem; border-bottom: 1px solid #e0e0e0; padding-bottom: 0.25rem; }
:deep(.markdown-preview h3) { font-size: 1.25rem; font-weight: 600; margin-top: 1rem; margin-bottom: 0.5rem; }
:deep(.markdown-preview p) { margin-bottom: 1rem; line-height: 1.6; }
:deep(.markdown-preview ul), :deep(.markdown-preview ol) { margin-bottom: 1rem; padding-left: 1.5rem; }
:deep(.markdown-preview li) { margin-bottom: 0.25rem; }
:deep(.markdown-preview code) { background: #f0f0f0; padding: 0.125rem 0.375rem; border-radius: 4px; font-family: 'Consolas', monospace; font-size: 0.9em; }
:deep(.markdown-preview pre) { background: #2d2d2d; color: #f8f8f2; padding: 1rem; border-radius: 6px; overflow-x: auto; margin-bottom: 1rem; }
:deep(.markdown-preview pre code) { background: transparent; padding: 0; color: inherit; }
:deep(.markdown-preview blockquote) { border-left: 4px solid #e0e0e0; padding-left: 1rem; margin-left: 0; color: #666; font-style: italic; }
:deep(.markdown-preview a) { color: #1976d2; text-decoration: none; }
:deep(.markdown-preview a:hover) { text-decoration: underline; }
:deep(.markdown-preview img) { max-width: 100%; border-radius: 4px; }
:deep(.markdown-preview table) { border-collapse: collapse; width: 100%; margin-bottom: 1rem; }
:deep(.markdown-preview th), :deep(.markdown-preview td) { border: 1px solid #e0e0e0; padding: 0.5rem; text-align: left; }
:deep(.markdown-preview th) { background: #f5f5f5; }
:deep(.markdown-preview hr) { border: none; border-top: 1px solid #e0e0e0; margin: 1.5rem 0; }
:deep(.markdown-preview input[type="checkbox"]) { margin-right: 0.5rem; }
</style>
