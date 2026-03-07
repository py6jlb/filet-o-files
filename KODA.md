# KODA.md — Инструкции для работы с проектом

## Обзор проекта

**Название:** filet-o-files (Поваренная книга)  
**Тип:** Полноценное веб-приложение (SPA + REST API)  
**Описание:** Веб-приложение для управления кулинарными рецептами. Позволяет создавать, просматривать рецепты, управлять профилем пользователя и планировать меню.

---

## Технологический стек

### Backend
- **Платформа:** ASP.NET Core (.NET 9)
- **База данных:** SQLite (Entity Framework Core)
- **Аутентификация:** ASP.NET Core Identity
- **Валидация:** FluentValidation
- **API документация:** Scalar (Swagger-подобный интерфейс)
- **Архитектура:** Feature-based (папки Features, DTOs, Domain, Services, Infrastructure)

### Frontend
- **Фреймворк:** Vue 3 (Composition API)
- **Сборщик:** Vite 7
- **UI-фреймворк:** Vuetify 3 (Material Design)
- **State management:** Pinia
- **Роутинг:** Vue Router
- **Валидация форм:** VeeValidate + Yup
- **HTTP-клиент:** Axios
- **Линтинг:** ESLint 9
- **Форматирование:** Prettier 3

---

## Структура проекта

```
filet-o-files/
├── .devcontainer/          # Docker-конфигурация для разработки
│   ├── backend.Dockerfile
│   ├── frontend.Dockerfile
│   ├── docker-compose.yml
│   ├── devcontainer.json
│   └── scripts/
├── .config/                # Конфигурация инструментов
├── backend/                # ASP.NET Core API
│   ├── FiletOFiles.Api/    # Основной проект
│   │   ├── DTOs/           # Data Transfer Objects
│   │   ├── Domain/         # Доменные модели
│   │   ├── Features/       # API endpoints
│   │   ├── Services/       # Бизнес-логика
│   │   ├── Infrastructure/ # Инфраструктура (БД)
│   │   ├── Middleware/     # Промежуточное ПО
│   │   ├── Migrations/     # Миграции EF Core
│   │   └── Program.cs      # Точка входа
│   └── FiletOFiles.sln     # Solution файл
└── frontend/               # Vue 3 приложение
    ├── src/
    │   ├── components/     # Vue компоненты
    │   ├── views/          # Страницы (Views)
    │   ├── stores/         # Pinia stores
    │   ├── router/         # Vue Router конфиг
    │   ├── utils/          # Утилиты (API, токены)
    │   └── styles/         # Стили
    ├── package.json
    └── vite.config.js
```

---

## Сборка и запуск

### Предварительные требования

- **Backend:** .NET 9 SDK, Node.js 22 (для frontend в docker)
- **Frontend:** Node.js 20.19+ или 22.12+
- **Docker:** Docker и Docker Compose (опционально)

### Вариант 1: Запуск через Dev Container (рекомендуется)

1. Откройте проект в VS Code
2. Установите расширение "Dev Containers"
3. Нажмите `F1` → "Dev Containers: Reopen in Container"

Будут запущены:
- Backend API: http://localhost:5000
- API Documentation (Scalar): http://localhost:5000/scalar
- Frontend Dev Server: http://localhost:5173

### Вариант 2: Ручной запуск

#### Backend

```bash
cd backend/FiletOFiles.Api
dotnet restore
dotnet ef database update  # Применить миграции (если нужно)
dotnet run
```

API будет доступно по адресу: http://localhost:5000

#### Frontend

```bash
cd frontend
npm install
npm run dev
```

Приложение будет доступно по адресу: http://localhost:5173

---

## Команды

### Frontend

| Команда | Описание |
|---------|----------|
| `npm run dev` | Запуск dev-сервера |
| `npm run build` | Сборка для продакшена |
| `npm run preview` | Превью продакшен-сборки |
| `npm run lint` | Линтинг с автоисправлением |
| `npm run format` | Форматирование кода через Prettier |

### Backend

| Команда | Описание |
|---------|----------|
| `dotnet restore` | Восстановление пакетов |
| `dotnet build` | Сборка проекта |
| `dotnet run` | Запуск приложения |
| `dotnet ef migrations add <name>` | Создание миграции |
| `dotnet ef database update` | Применение миграций |
| `dotnet ef database drop` | Удаление БД |

---

## Основные сущности

### Frontend Views (страницы)
- **LoginView** — Страница входа
- **RecipesView** — Список рецептов
- **AddRecipeView** — Добавление нового рецепта
- **ProfileView** — Профиль пользователя
- **SchedulerView** — Планировщик меню

### Frontend Stores (Pinia)
- `auth.store.js` — Аутентификация и авторизация
- `recipes.store.js` — Управление рецептами
- `tags.store.js` — Теги/категории рецептов
- `user.store.js` — Данные пользователя

---

## Правила разработки

### Frontend
- Используется Composition API (`<script setup>`)
- Стилизация через Vuetify компоненты и SCSS
- Линтинг: ESLint с конфигурацией `@vue/eslint-config-prettier`
- Форматирование: Prettier (3.6.2)
- Импорты через alias `@/` (корневая папка src)

### Backend
- Code style: CSharpier (форматирование)
- Архитектура: Feature-based (каждый endpoint в своей папке Features)
- Валидация: FluentValidation
- Работа с БД: Entity Framework Core
- Аутентификация: ASP.NET Core Identity с JWT

### Общие
- IDE: VS Code с расширениями (см. .devcontainer/devcontainer.json)
- Docker-окружение для единообразия разработки

---

## Порты

| Сервис | Порт |
|--------|------|
| Backend API | 5000 |
| Backend HTTPS | 5001 |
| Frontend Dev | 5173 |
| Scalar API Docs | 5000/scalar |

---

## Полезные ссылки

- **Frontend API Base URL:** http://localhost:5000
- **Scalar (документация API):** http://localhost:5000/scalar

---

## TODO

- [ ] Уточнить конкретные API endpoints
- [ ] Проверить наличие тестов
- [ ] Добавить информацию о CI/CD (если есть)
