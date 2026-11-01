using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Entities;
using System.IO;

namespace AdminDesktop.Controls.General
{
    public partial class GameCard : UserControl
    {
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
    }
}