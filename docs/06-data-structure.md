# Структура данных

## Логическая модель

### Transaction, операция
- `Id` (Guid) — идентификатор.
- `Date` (DateTime) — дата операции.
- `Type` (TransactionType) — доход/расход.
- `Category` (string) — категория.
- `Description` (string) — описание.
- `Amount` (decimal) — сумма.

### Category, категория
На уровне UI категория — строка с названием. В БД категория хранится в отдельной таблице.

## Физическая модель (SQLite + EF Core)

База данных создаётся автоматически при запуске приложения.
Файл базы: `%LOCALAPPDATA%\FinanceBuddy\financebuddy.db`.

### Таблица `Categories`
- `Name` TEXT — primary key (название категории).

### Таблица `Transactions`
- `Id` TEXT — primary key (Guid).
- `Date` TEXT/NUMERIC — дата.
- `Type` INTEGER — значение enum `TransactionType`.
- `CategoryName` TEXT — внешний ключ на `Categories(Name)`.
- `Description` TEXT — описание.
- `Amount` NUMERIC — сумма.

## Тестовые данные
При первом запуске выполняется seed:
- создаются стандартные категории;
- добавляется несколько операций (доход/расход) для проверки UI.
