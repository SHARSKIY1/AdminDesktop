using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Entities;
using System.Collections.ObjectModel;
using System.Linq;

namespace AdminDesktop.Controls.PageContent
{
    public partial class GameCatalog : UserControl
    {
        public ObservableCollection<Game> Games { get; set; } = new();

        public GameCatalog()
        {
            InitializeComponent();

            DataContext = this;

            LoadGames();
        }

        private void LoadGames()
        {
            using var db = Program.CreateDbContext();

            var games = db.Games.ToList();

            foreach (var game in games)
            {
                Games.Add(game);
            }
        }
    }
}