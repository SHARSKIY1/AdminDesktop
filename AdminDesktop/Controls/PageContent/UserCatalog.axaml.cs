using AdminDesktop.Controls.General;
using Avalonia.Controls;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace AdminDesktop.Controls.PageContent;

public partial class UserCatalog : UserControl
{
    public ObservableCollection<User> Users { get; set; } = new();

    public UserCatalog()
    {
        InitializeComponent();
        DataContext = this;
        _ = LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        try
        {
            using var timeout = new CancellationTokenSource(
                System.TimeSpan.FromSeconds(Program.DatabaseLoadingTimeoutSeconds));
            await using var db = Program.CreateDbContext();
            var users = await db.Users
                .AsNoTracking()
                .ToListAsync(timeout.Token);

            foreach (var user in users)
                Users.Add(user);
        }
        catch
        {
            Users.Clear();
            await MessageBoxService.ShowErrorAsync(
                $"Не удалось загрузить пользователей. Проверьте подключение к PostgreSQL.");
        }
    }
}
