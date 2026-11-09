using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Entities;
using System.IO;
using Avalonia.Input;
using System;

namespace AdminDesktop.Controls.General
{
    public partial class GameCard : UserControl
    {
        public event Action<Game>? GameSelected;

        public GameCard()
        {
            InitializeComponent();

            DataContextChanged += (_, _) =>
            {
                if (DataContext is Game game &&
                    File.Exists(game.IconPath))
                {
                    IconImage.Source = new Bitmap(game.IconPath);
                }
                else
                {
                    IconImage.Source = null;
                }
            };
        }

        private void SelectGame(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (DataContext is Game game)
            {
                GameSelected?.Invoke(game);
            }
        }
    }
}