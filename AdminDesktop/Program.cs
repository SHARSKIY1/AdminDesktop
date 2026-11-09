using Avalonia;
using Entities.DataDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace AdminDesktop;

class Program
{
    public const int DatabaseConnectionTimeoutSeconds = 2;
    public const int DatabaseCommandTimeoutSeconds = 15;
    public const int DatabaseLoadingTimeoutSeconds = 15;
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();

    public static AppDataBase CreateDbContext()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration
            .GetConnectionString("DefaultConnection");

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Timeout = DatabaseConnectionTimeoutSeconds,
            CommandTimeout = DatabaseCommandTimeoutSeconds
        };

        var options = new DbContextOptionsBuilder<AppDataBase>()
            .UseNpgsql(connectionStringBuilder.ConnectionString)
            .Options;

        return new AppDataBase(options);
    }

    public static async Task<bool> CheckDatabaseConnectionAsync()
    {
        try
        {
            await using var db = CreateDbContext();
            return await db.Database.CanConnectAsync();
        }
        catch
        {
            return false;
        }
    }
}
