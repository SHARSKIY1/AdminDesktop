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
    public partial class UserCard : UserControl
    {
        public event Action<User>? UserSelected;

        public UserCard()
        {
            InitializeComponent();

            DataContextChanged += (_, _) =>
            {
                if (DataContext is User user)
                    FillCard(user);
            };

            PointerPressed += (_, _) =>
            {
                if (DataContext is User user)
                    UserSelected?.Invoke(user);
            };
        }

        private void FillCard(User user)
        {
            if (!string.IsNullOrWhiteSpace(user.IconPath) &&
                File.Exists(user.IconPath))
            {
                UserAvatar.Source = new Bitmap(user.IconPath);
            }
            else
            {
                UserAvatar.Source = null;
            }
        }
    }
}