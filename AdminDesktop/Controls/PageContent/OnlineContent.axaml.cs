using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AdminDesktop
{
    public partial class OnlineContent : UserControl
    {
        public OnlineContent()
        {
            InitializeComponent();
            _ = LoadUsersCountAsync();
        }

        private async Task LoadUsersCountAsync()
        {
            await using var db = Program.CreateDbContext();

            var totalUsers = await db.Users.CountAsync();
            var activeUsers = await db.Users.CountAsync(user => user.IsOnline);

            TotalUsers.Text = totalUsers.ToString();
            ActiveUsers.Text = activeUsers.ToString();
        }
    }
}