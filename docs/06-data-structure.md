# Структура данных

## Логическая модель

### Transaction, операция
- Id, Guid
- Date, DateTime
- Type, TransactionType
- Category, string
- Description, string
- Amount, decimal

### Category
На первом этапе я использую строку с названием категории.

## План для SQLite
### Таблица Transactions
- Id, TEXT, primary key
- Date, TEXT в формате ISO
- Type, INTEGER
- Category, TEXT
- Description, TEXT
- Amount, NUMERIC

## План формата JSON
- массив объектов Transaction
