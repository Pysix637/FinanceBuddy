# Инструкция: GitHub

## Вариант через GitHub Desktop
1. Создай новый репозиторий на GitHub.
2. Открой GitHub Desktop и добавь локальную папку проекта как Local Repository.
3. Сделай первый коммит.
4. Нажми Publish repository и опубликуй код.

## Вариант через командную строку
В папке проекта:
```bash
git init
git add .
git commit -m "init: FinanceBuddy WPF + docs"
git branch -M main
git remote add origin <URL_твоего_репозитория>
git push -u origin main
```
