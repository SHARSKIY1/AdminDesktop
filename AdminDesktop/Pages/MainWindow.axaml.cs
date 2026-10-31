using AdminDesktop.Controls.General;
using Avalonia;
using Avalonia.Controls;


namespace AdminDesktop;

public partial class MainWindow : Window
{
    public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .WithInterFont();

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Show_Online(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ContentArea.Content = new OnlineContent();
    }

    private void Show_Users(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ContentArea.Content = new SearchBar();
    }
}