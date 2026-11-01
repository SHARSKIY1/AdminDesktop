using Avalonia;
using Entities.DataDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace AdminDesktop;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        using (var db = CreateDbContext())
        {
            bool connected = db.Database.CanConnect();

            Console.WriteLine($"Подключение: {connected}");
        }

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

        var options = new DbContextOptionsBuilder<AppDataBase>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDataBase(options);
    }
}
