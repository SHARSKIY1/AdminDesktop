using AdminDesktop.Controls.General;
using Avalonia.Controls;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            await using var db = Program.CreateDbContext();
            var users = await db.Users
                .AsNoTracking()
                .ToListAsync();

            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }

            if (users.Count == 0)
            {
                await MessageBoxService.ShowInfoAsync(
                    "Пользователи не найдены",
                    "Запрос к базе данных выполнен успешно, но список пользователей пуст.");
            }
        }
        catch (Exception exception)
        {
            Users.Clear();
            Debug.WriteLine($"User data loading failed: {exception}");

            await MessageBoxService.ShowErrorAsync(
                "Ошибка загрузки пользователей",
                "Не удалось загрузить пользователей для UserCard. Карточки пользователей не будут отображены.\n\n" +
                "Проверьте подключение к PostgreSQL и строку подключения.\n\n" +
                $"Подробности: {exception.Message}");
        }
    }
}
