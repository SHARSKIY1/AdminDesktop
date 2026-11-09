Statistics dashboard changes

Copy the AdminDesktop folder from this archive into the root folder of your local solution/repository and allow Windows to replace the existing files when prompted.
Do not delete or replace the repository's .git folder.

Changed existing files:
- AdminDesktop/Pages/MainWindow.axaml
- AdminDesktop/Pages/MainWindow.axaml.cs
- AdminDesktop/AdminDesktop.csproj

New files:
- AdminDesktop/Controls/PageContent/StatisticsContent.axaml
- AdminDesktop/Controls/PageContent/StatisticsContent.axaml.cs

Data notes:
- UserStatistic reads Users.RegistrationDate from PostgreSQL and calculates monthly new users and cumulative users for the current year.
- GameTags reads Game.Tags from PostgreSQL and counts tag occurrences.
- PaymentMethod and Profit currently contain clearly marked demonstration values because the current database model has no payment or transaction entities.
- If the database connection fails while the page loads, UserStatistic uses demonstration values and displays a warning.

After copying, run:
  dotnet restore
  dotnet build AdminDesktop.slnx

The archive intentionally does not contain .git, bin, obj, .vs, appsettings.json or database credentials.
