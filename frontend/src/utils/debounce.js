/**
 * Создает debounced версию функции
 * @param {Function} func - функция для debounce
 * @param {number} wait - время ожидания в мс
 * @returns {Function}
 */
export function debounce(func, wait = 300) {
  let timeout
  return function executedFunction(...args) {
    const later = () => {
      clearTimeout(timeout)
      func(...args)
    }
    clearTimeout(timeout)
    timeout = setTimeout(later, wait)
  }
}
