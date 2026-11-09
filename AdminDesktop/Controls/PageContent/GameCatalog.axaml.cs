using AdminDesktop.Controls.General;
using Avalonia.Controls;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
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
    }
}
