/**
 * Вычисляет контрастный цвет текста (белый или чёрный) для заданного фона
 * @param {string} hexColor - цвет в формате HEX (например, "#E53935")
 * @returns {string} "black" или "white"
 */
export function getContrastColor(hexColor) {
  if (!hexColor) return 'white'
  const hex = hexColor.replace('#', '')
  const r = parseInt(hex.substr(0, 2), 16)
  const g = parseInt(hex.substr(2, 2), 16)
  const b = parseInt(hex.substr(4, 2), 16)
  const brightness = (r * 299 + g * 587 + b * 114) / 1000
  return brightness > 128 ? 'black' : 'white'
}
