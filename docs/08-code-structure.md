# Структура кода

## Архитектура
Проект построен по MVVM + DI (Host).  
ViewModel не создаёт окна напрямую — для этого используется сервис.

## Папки проекта
- `Models` — доменные модели (Transaction, FinanceSummary, TransactionType).
- `ViewModels` — логика главного окна (`MainViewModel`).
- `ViewModels/Dialogs` — логика окон редактирования (TransactionEditViewModel, CategoryManageViewModel).
- `Services` — интерфейсы и реализации сервисов (репозиторий, расчёт итогов, парсер суммы, сервис окон).
- `Data` — EF Core DbContext + инициализация тестовых данных.
- `Data/Entities` — сущности EF Core.
- `Windows` — окна WPF для редактирования данных.

## Точка входа
`App.xaml.cs` поднимает Host, регистрирует зависимости, инициализирует БД, затем показывает `MainWindow`.
