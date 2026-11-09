using AdminDesktop.Controls.General;
using Avalonia.Controls;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AdminDesktop.Controls.PageContent
{
    public partial class GameCatalog : UserControl
    {
        public ObservableCollection<Game> Games { get; set; } = new();

        public GameCatalog()
        {
            InitializeComponent();

            DataContext = this;

            _ = LoadGamesAsync();
        }

        private async Task LoadGamesAsync()
        {
            try
            {
                await using var db = Program.CreateDbContext();
                var games = await db.Games
                    .AsNoTracking()
                    .ToListAsync();

                Games.Clear();
                foreach (var game in games)
                {
                    Games.Add(game);
                }

                if (games.Count == 0)
                {
                    await MessageBoxService.ShowInfoAsync(
                        "Игры не найдены",
                        "Запрос к базе данных выполнен успешно, но каталог игр пуст.");
                }
            }
            catch (Exception exception)
            {
                Games.Clear();
                Debug.WriteLine($"Game data loading failed: {exception}");

                await MessageBoxService.ShowErrorAsync(
                    "Ошибка загрузки игр",
                    "Не удалось загрузить игры для GameCard. Карточки игр не будут отображены.\n\n" +
                    "Проверьте подключение к PostgreSQL и строку подключения.\n\n" +
                    $"Подробности: {exception.Message}");
            }
        }
    }
}
