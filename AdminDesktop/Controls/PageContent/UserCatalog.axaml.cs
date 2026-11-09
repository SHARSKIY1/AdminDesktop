using AdminDesktop.Controls.General;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdminDesktop.Controls.PageContent;

public partial class UserCatalog : UserControl
{
    private User? selectedUser;
    private List<User> allUsers = new();
    public ObservableCollection<User> Users { get; set; } = new();

    public UserCatalog()
    {
        InitializeComponent();
        DataContext = this;
        _ = LoadUsersAsync();
        SearchBar.SearchTextChanged += SearchUsers;
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

            allUsers = users;

            Users.Clear();

            foreach (var user in allUsers)
                Users.Add(user);
        }
        catch
        {
            Users.Clear();
            await MessageBoxService.ShowErrorAsync(
                $"Не удалось загрузить пользователей. Проверьте подключение к PostgreSQL.");
        }
    }

    private void UserCard_UserSelected(User user)
    {
        selectedUser = user;

        NickNameTextBox.Text = user.Login;
        IconPathTextBox.Text = user.IconPath;

        if (!string.IsNullOrWhiteSpace(user.IconPath) &&
            File.Exists(user.IconPath))
        {
            UserIconImage.Source = new Bitmap(user.IconPath);
        }
        else
        {
            UserIconImage.Source = null;
        }
    }

    private async void Edit(object? sender, RoutedEventArgs e)
    {
        if (selectedUser == null)
        {
            await MessageBoxService.ShowErrorAsync("Сначала выберите пользователя.");
            return;
        }

        try
        {
            using var timeout = new CancellationTokenSource(
                System.TimeSpan.FromSeconds(Program.DatabaseLoadingTimeoutSeconds));

            await using var db = Program.CreateDbContext();

            var user = await db.Users.FirstOrDefaultAsync(
                u => u.UserId == selectedUser.UserId, timeout.Token);

            if (user == null)
            {
                await MessageBoxService.ShowErrorAsync("Пользователь не найден.");
                return;
            }

            user.Login = NickNameTextBox.Text ?? string.Empty;
            user.IconPath = IconPathTextBox.Text ?? string.Empty;

            await db.SaveChangesAsync(timeout.Token);

            var index = Users.IndexOf(selectedUser);
            if (index >= 0)
                Users[index] = user;

            selectedUser = user;

            var allIndex = allUsers.FindIndex(u => u.UserId == user.UserId);

            if (allIndex >= 0)
                allUsers[allIndex] = user;
        }
        catch
        {
            await MessageBoxService.ShowErrorAsync(
                "Не удалось сохранить изменения пользователя.");
        }
    }

    private void Reset(object? sender, RoutedEventArgs e)
    {
        selectedUser = null;

        NickNameTextBox.Text = string.Empty;
        IconPathTextBox.Text = string.Empty;
    }

    private async void Delete(object? sender, RoutedEventArgs e)
    {
        if (selectedUser == null)
        {
            await MessageBoxService.ShowErrorAsync("Сначала выберите пользователя.");
            return;
        }

        string? cause = null;

        if (CheatingRadioButton.IsChecked == true)
            cause = "Cheating";
        else if (BehaviorRadioButton.IsChecked == true)
            cause = "Unacceptable behavior";
        else if (OtherRadioButton.IsChecked == true)
            cause = "Other";

        if (cause == null)
        {
            await MessageBoxService.ShowErrorAsync("Выберите причину удаления.");
            return;
        }

        var description = CauseDescriptionTextBox.Text ?? string.Empty;
        var deletedUser = selectedUser;
        var deletionDate = DateTime.Now;

        try
        {
            var directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "AdminDesktop",
                "DeletedUsers");

            Directory.CreateDirectory(directory);

            var filePath = Path.Combine(
                directory,
                $"User_{deletedUser.UserId}_{deletionDate:yyyyMMdd_HHmmss}.txt");

            var document = $"""
            Информация об удалении пользователя

            ID: {deletedUser.UserId}
            Логин: {deletedUser.Login}
            Дата удаления: {deletionDate:dd.MM.yyyy HH:mm:ss}
            Причина: {cause}
            Описание: {description}
            """;

            using var timeout = new CancellationTokenSource(
                TimeSpan.FromSeconds(Program.DatabaseLoadingTimeoutSeconds));

            await using var db = Program.CreateDbContext();

            var user = await db.Users.FirstOrDefaultAsync(
                u => u.UserId == deletedUser.UserId, timeout.Token);

            if (user == null)
            {
                await MessageBoxService.ShowErrorAsync("Пользователь не найден.");
                return;
            }

            db.Users.Remove(user);
            await db.SaveChangesAsync(timeout.Token);

            await File.WriteAllTextAsync(filePath, document, Encoding.UTF8);

            Users.Remove(deletedUser);

            allUsers.RemoveAll(u => u.UserId == deletedUser.UserId);

            selectedUser = null;

            NickNameTextBox.Text = string.Empty;
            IconPathTextBox.Text = string.Empty;
            CauseDescriptionTextBox.Text = string.Empty;

            CheatingRadioButton.IsChecked = false;
            BehaviorRadioButton.IsChecked = false;
            OtherRadioButton.IsChecked = false;
        }
        catch
        {
            await MessageBoxService.ShowErrorAsync(
                "Не удалось удалить пользователя. Проверьте подключение к БД и доступ к файлам.");
        }
    }

    private void SearchUsers(string searchText)
    {
        Users.Clear();

        var filteredUsers = allUsers.Where(user =>
            user.Login.Contains(searchText, StringComparison.OrdinalIgnoreCase));

        foreach (var user in filteredUsers)
            Users.Add(user);
    }
}
