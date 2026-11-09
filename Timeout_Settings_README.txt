Настройки тайм-аутов PostgreSQL

Все три значения находятся в AdminDesktop/Program.cs:

DatabaseConnectionTimeoutSeconds = 2
  Максимальное ожидание установления соединения с PostgreSQL.

DatabaseCommandTimeoutSeconds = 15
  Максимальное время выполнения одного SQL-запроса.

DatabaseLoadingTimeoutSeconds = 15
  Максимальное общее время асинхронной загрузки данных в UserCatalog, GameCatalog и StatisticsContent.

Поэтому медленный SQL-запрос может выполняться дольше 2 секунд, если соединение с БД было установлено вовремя.
При превышении общего лимита загрузки запрос отменяется, а существующая обработка ошибок показывает MessageBox.
