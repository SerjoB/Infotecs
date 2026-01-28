# InfotecsApi - API для обработки CSV данных

WebAPI приложение для работы с timescale данными из CSV файлов с хранением в PostgreSQL.

## Функциональность

- **Импорт CSV**: загрузка и валидация CSV файлов с автоматической обработкой данных
- **Запрос результатов**: фильтрация и получение агрегированных результатов по имени файла, диапазону дат, средним значениям и времени выполнения
- **Запрос значений**: получение последних 10 значений для указанного файла

## Технологический стек

- .NET 9
- Entity Framework Core
- PostgreSQL
- Docker & Docker Compose
- xUnit (тестирование)

## Запуск через Docker

### Предварительные требования
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

1. Клонируйте репозиторий:
```bash
git clone https://github.com/SerjoB/Infotecs.git
cd Infotecs
```

2. Запустите приложение с помощью Docker Compose:
```bash
docker-compose up -d
```

3. Доступ к API:
- **Swagger UI**: http://localhost:5000/swagger

4. Остановка приложения:
```bash
docker-compose down
```

5. Остановка и удаление всех данных:
```bash
docker-compose down -v
```

## Endpoints API

### 1. Импорт CSV
**POST** `/api/import`

Загрузка и обработка CSV файла.

**Запрос:**
- Content-Type: `multipart/form-data`
- Body: `file` (CSV файл)

**Формат CSV:**
```csv
Date;ExecutionTime;Value
2024-01-15T10:00:00.0000Z;5.2;150.5
2024-01-15T10:05:00.0000Z;6.1;175.3
```

**Правила валидации:**
- Date: Между 2000-01-01 и текущей датой
- ExecutionTime: >= 0
- Value: >= 0
- Количество строк: 1-10,000

**Пример с curl:**

bash:
```bash
curl -X POST "http://localhost:5000/api/import" \ -F "file=@Data Samples/example.csv"
```
powershell:
```powershell
curl.exe -X POST "http://localhost:5000/api/import" -F "file=@Data Samples/example.csv"
```

### 2. Получение отфильтрованных результатов
**GET** `/api/results`

Получение агрегированных результатов с опциональными фильтрами.

**Параметры запроса:**
- `fileName` (string): Фильтр по имени файла
- `minDateFrom` (datetime): Фильтр по минимальной дате (начало диапазона)
- `minDateTo` (datetime): Фильтр по минимальной дате (конец диапазона)
- `avgValueFrom` (double): Фильтр по среднему значению (минимум)
- `avgValueTo` (double): Фильтр по среднему значению (максимум)
- `avgExecutionTimeFrom` (double): Фильтр по среднему времени выполнения (минимум)
- `avgExecutionTimeTo` (double): Фильтр по среднему времени выполнения (максимум)

**Пример:**

bash:
```bash
curl "http://localhost:5000/api/results?fileName=example&avgValueFrom=100"
```

powershell:
```powershell
curl.exe "http://localhost:5000/api/results?fileName=example&avgValueFrom=100"
```

### 3. Получение последних 10 значений
**GET** `/api/values/{fileName}/last`

Получение последних 10 значений для указанного файла, отсортированных по дате.

**Пример:**

bash:
```bash
curl "http://localhost:5000/api/values/test/last"
```
powershell:
```powershell
curl.exe "http://localhost:5000/api/values/test/last"
```

## Запуск локально (без Docker)

### Предварительные требования
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL](https://www.postgresql.org/download/)


1. Клонируйте репозиторий:
```bash
git clone https://github.com/SerjoB/Infotecs.git
cd Infotecs
```

2. Обновите строку подключения в `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=название_вашей_БД;Username=имя_пользователя_БД;Password=ваш_пароль"
  }
}
```

3. Убедитесь, что база данных существует и PostgreSQL запущен локально.

4. Примените миграции:
```bash
cd InfotecsApi
dotnet ef database update
```

4. Запустите приложение:
```bash
dotnet run
```

5. Откройте Swagger UI: http://localhost:5131/swagger

## Запуск тестов
```bash
cd InfotecsApiTests
dotnet test
```
