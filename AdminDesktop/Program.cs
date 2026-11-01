using Avalonia;
using System;
using Entities.DataDB;
using Microsoft.EntityFrameworkCore;

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
        var connectionString =
            "Host=localhost;Port=5432;Database=AdminDesktopDB;Username=postgres;Password=322";

        var options = new DbContextOptionsBuilder<AppDataBase>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDataBase(options);
    }
}
