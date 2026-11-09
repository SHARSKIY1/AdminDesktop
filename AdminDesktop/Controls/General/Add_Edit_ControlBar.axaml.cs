using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Entities;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Linq;

namespace AdminDesktop.Controls.General
{
    public partial class Add_Edit_ControlBar : UserControl
    {
        private Game? selectedGame;

        public Func<Game, Task>? AddRequested { get; set; }
        public Func<Game, Task>? EditRequested { get; set; }
        public Func<Game, string, string, Task>? DeleteRequested { get; set; }

        public Add_Edit_ControlBar()
        {
            InitializeComponent();
        }

        public void SetGame(Game game)
        {
            selectedGame = game;

            if (System.IO.File.Exists(game.IconPath))
            {
                GameIconImage.Source = new Avalonia.Media.Imaging.Bitmap(game.IconPath);
            }
            else
            {
                GameIconImage.Source = new Avalonia.Media.Imaging.Bitmap(
                    Avalonia.Platform.AssetLoader.Open(
                        new Uri("avares://AdminDesktop/Images/GameIcon.png")));
            }

            NameTextBox.Text = game.Name;
            TagsTextBox.Text = game.TagsText;
            PriceTextBox.Text = game.Price.ToString(CultureInfo.CurrentCulture);
            ImageIconTextBox.Text = game.IconPath;
            ApkFileTextBox.Text = game.ApkPath;
        }

        private Game? GetGameFromFields()
        {
            if (!decimal.TryParse(
                PriceTextBox.Text,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out decimal price))
            {
                return null;
            }

            return new Game
            {
                GameId = selectedGame?.GameId ?? 0,
                Name = NameTextBox.Text?.Trim() ?? string.Empty,
                Tags = (TagsTextBox.Text ?? string.Empty)
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToList(),
                Price = price,
                IconPath = ImageIconTextBox.Text?.Trim() ?? string.Empty,
                ApkPath = ApkFileTextBox.Text?.Trim() ?? string.Empty
            };
        }

        private async void AddGame(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Game? game = GetGameFromFields();

            if (game == null)
            {
                await MessageBoxService.ShowErrorAsync("Некорректная цена игры.");
                return;
            }

            game.GameId = 0;

            if (AddRequested != null)
                await AddRequested.Invoke(game);
        }

        private async void EditGame(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (selectedGame == null)
            {
                await MessageBoxService.ShowErrorAsync("Сначала выберите игру из каталога.");
                return;
            }

            Game? game = GetGameFromFields();

            if (game == null)
            {
                await MessageBoxService.ShowErrorAsync("Некорректная цена игры.");
                return;
            }

            if (EditRequested != null)
                await EditRequested.Invoke(game);
        }

        private void ResetGame(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ClearFields();
        }

        public void ClearFields()
        {
            selectedGame = null;

            NameTextBox.Text = string.Empty;
            TagsTextBox.Text = string.Empty;
            PriceTextBox.Text = string.Empty;
            ImageIconTextBox.Text = string.Empty;
            ApkFileTextBox.Text = string.Empty;
            GameIconImage.Source = new Avalonia.Media.Imaging.Bitmap(Avalonia.Platform.AssetLoader.Open(new Uri("avares://AdminDesktop/Images/GameIcon.png")));

            Technical.IsChecked = false;
            Fraud.IsChecked = false;
            SecurityScam.IsChecked = false;
            Unreabiality.IsChecked = false;
            Vulnerability.IsChecked = false;
            Other.IsChecked = false;

            DelDescription.Text = string.Empty;
        }

        private async void DeleteGame(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (selectedGame == null)
            {
                await MessageBoxService.ShowErrorAsync("Сначала выберите игру.");
                return;
            }

            string reasons = string.Join(", ", new[]
            {
                Technical.IsChecked == true ? "Technical" : null,
                Fraud.IsChecked == true ? "Fraud" : null,
                SecurityScam.IsChecked == true ? "SecurityScam" : null,
                Unreabiality.IsChecked == true ? "Unreliability" : null,
                Vulnerability.IsChecked == true ? "Vulnerability" : null,
                Other.IsChecked == true ? "Other" : null
            }.Where(x => x != null));

            string description = DelDescription.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(reasons))
            {
                await MessageBoxService.ShowErrorAsync("Выберите хотя бы одну причину удаления.");
                return;
            }

            if (DeleteRequested != null)
                await DeleteRequested.Invoke(selectedGame, reasons, description);
        }
    }
}