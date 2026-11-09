Изменения обработки ошибок и тайм-аутов БД

- Установление соединения с PostgreSQL ограничено 2 секундами.
- Выполнение отдельной SQL-команды ограничено 15 секундами.
- Общая асинхронная загрузка данных в UserCatalog, GameCatalog и StatisticsContent ограничена 15 секундами.
- Настройки вынесены в константы Program.cs: DatabaseConnectionTimeoutSeconds,
  DatabaseCommandTimeoutSeconds и DatabaseLoadingTimeoutSeconds.
- При ошибке или истечении времени сохраняется упрощённая обработка с MessageBox.
- При ошибке загрузки статистика продолжает отображаться на демонстрационных данных.
- NuGet MessageBox.Avalonia 12.0.0 остаётся в AdminDesktop.csproj; новый пакет устанавливать не требуется.

Установка: распакуйте архив поверх проекта, сохранив .git, затем выполните:
  dotnet restore
  dotnet build AdminDesktop.slnx
