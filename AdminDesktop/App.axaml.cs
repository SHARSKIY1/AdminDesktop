using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AdminDesktop.Controls.General;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;

namespace AdminDesktop;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            desktop.MainWindow.Opened += async (_, _) =>
            {
                bool connected = await Program.CheckDatabaseConnectionAsync();

                if (!connected)
                {
                    await MessageBoxService.ShowErrorAsync(
                        "Не удалось подключиться к БД. Проверьте подключение к PostgreSQL.");
                }
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}