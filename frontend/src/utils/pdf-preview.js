import * as pdfjsLib from 'pdfjs-dist'

// Настройка worker для pdf.js
pdfjsLib.GlobalWorkerOptions.workerSrc = new URL(
  'pdfjs-dist/build/pdf.worker.min.mjs',
  import.meta.url
).toString()

/**
 * Генерирует превью первой страницы PDF файла
 * @param {File} pdfFile - PDF файл
 * @param {number} width - желаемая ширина превью
 * @returns {Promise<string|null>} - base64 data URL изображения или null при ошибке
 */
export async function generatePdfPreview(pdfFile, width = 200) {
  try {
    const arrayBuffer = await pdfFile.arrayBuffer()
    const pdf = await pdfjsLib.getDocument({ data: arrayBuffer }).promise
    
    if (pdf.numPages < 1) {
      return null
    }

    const page = await pdf.getPage(1)
    const viewport = page.getViewport({ scale: 1 })
    
    // Вычисляем scale для нужной ширины
    const scale = width / viewport.width
    const scaledViewport = page.getViewport({ scale })

    // Создаём canvas
    const canvas = document.createElement('canvas')
    canvas.width = scaledViewport.width
    canvas.height = scaledViewport.height
    
    const context = canvas.getContext('2d')
    
    await page.render({
      canvasContext: context,
      viewport: scaledViewport,
    }).promise

    // Конвертируем в data URL
    return canvas.toDataURL('image/jpeg', 0.8)
  } catch (error) {
    console.error('Ошибка генерации PDF превью:', error)
    return null
  }
}

/**
 * Проверяет, является ли файл PDF
 * @param {File} file 
 * @returns {boolean}
 */
export function isPdfFile(file) {
  return file.type === 'application/pdf' || 
         file.name.toLowerCase().endsWith('.pdf')
}

/**
 * Конвертирует data URL в Blob
 * @param {string} dataUrl 
 * @returns {Blob}
 */
export function dataUrlToBlob(dataUrl) {
  const parts = dataUrl.split(',')
  const mime = parts[0].match(/:(.*?);/)?.[1] || 'image/jpeg'
  const bstr = atob(parts[1])
  let n = bstr.length
  const u8arr = new Uint8Array(n)
  while (n--) {
    u8arr[n] = bstr.charCodeAt(n)
  }
  return new Blob([u8arr], { type: mime })
}
