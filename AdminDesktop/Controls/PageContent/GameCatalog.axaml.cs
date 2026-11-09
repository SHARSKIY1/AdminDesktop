using AdminDesktop.Controls.General;
using Avalonia.Controls;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
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
            EditDelControl.AddRequested = AddGameAsync;
            EditDelControl.EditRequested = EditGameAsync;
            EditDelControl.DeleteRequested = DeleteGameAsync;
            _ = LoadGamesAsync();
        }

        private async Task LoadGamesAsync()
        {
            try
            {
                using var timeout = new CancellationTokenSource(
                    System.TimeSpan.FromSeconds(Program.DatabaseLoadingTimeoutSeconds));
                await using var db = Program.CreateDbContext();
                var games = await db.Games
                    .AsNoTracking()
                    .ToListAsync(timeout.Token);

                foreach (var game in games)
                    Games.Add(game);
            }
            catch
            {
                Games.Clear();
                await MessageBoxService.ShowErrorAsync(
                    $"Не удалось загрузить каталог игр. Проверьте подключение к PostgreSQL. ");
            }
        }

        private void GameCard_GameSelected(Game game)
        {
            EditDelControl.SetGame(game);
        }

        private async Task AddGameAsync(Game game)
        {
            try
            {
                await using var db = Program.CreateDbContext();

                bool exists = await db.Games.AnyAsync(g => g.Name.ToLower() == game.Name.ToLower());

                if (exists)
                {
                    await MessageBoxService.ShowErrorAsync("Игра с таким названием уже существует.");
                    return;
                }

                await db.Games.AddAsync(game);
                await db.SaveChangesAsync();

                Games.Add(game);
                EditDelControl.ClearFields();
            }
            catch
            {
                await MessageBoxService.ShowErrorAsync("Не удалось добавить игру.");
            }
        }

        private async Task EditGameAsync(Game game)
        {
            try
            {
                await using var db = Program.CreateDbContext();

                var dbGame = await db.Games
                    .FirstOrDefaultAsync(g => g.GameId == game.GameId);

                if (dbGame == null)
                {
                    await MessageBoxService.ShowErrorAsync("Игра не найдена.");
                    return;
                }

                bool exists = await db.Games.AnyAsync(g =>
                    g.GameId != game.GameId &&
                    g.Name.ToLower() == game.Name.ToLower());

                if (exists)
                {
                    await MessageBoxService.ShowErrorAsync("Игра с таким названием уже существует.");
                    return;
                }

                dbGame.Name = game.Name;
                dbGame.Tags = game.Tags;
                dbGame.Price = game.Price;
                dbGame.IconPath = game.IconPath;
                dbGame.ApkPath = game.ApkPath;

                await db.SaveChangesAsync();

                for (int i = 0; i < Games.Count; i++)
                {
                    if (Games[i].GameId == game.GameId)
                    {
                        Games[i] = dbGame;
                        break;
                    }
                }

                EditDelControl.SetGame(dbGame);
            }
            catch
            {
                await MessageBoxService.ShowErrorAsync("Не удалось изменить игру.");
            }
        }

        private async Task DeleteGameAsync(Game game, string reasons, string description)
        {
            try
            {
                await using var db = Program.CreateDbContext();

                var dbGame = await db.Games
                    .FirstOrDefaultAsync(g => g.GameId == game.GameId);

                if (dbGame == null)
                {
                    await MessageBoxService.ShowErrorAsync("Игра не найдена в базе данных.");
                    return;
                }

                string folder = Path.Combine(AppContext.BaseDirectory, "DeleteLogs");
                Directory.CreateDirectory(folder);

                string fileName = $"{game.Name}_{game.GameId}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                foreach (char c in Path.GetInvalidFileNameChars())
                    fileName = fileName.Replace(c, '_');

                string report =
                    $"Game: {game.Name}\n" +
                    $"Game ID: {game.GameId}\n" +
                    $"Deletion date: {DateTime.Now}\n" +
                    $"Reasons: {reasons}\n" +
                    $"Description: {description}";

                await File.WriteAllTextAsync(Path.Combine(folder, fileName), report);

                db.Games.Remove(dbGame);
                await db.SaveChangesAsync();

                var selected = Games.FirstOrDefault(g => g.GameId == game.GameId);

                if (selected != null)
                    Games.Remove(selected);

                EditDelControl.ClearFields();
            }
            catch (Exception ex)
            {
                await MessageBoxService.ShowErrorAsync(
                    $"Не удалось удалить игру: {ex.Message}");
            }
        }
    }
}
