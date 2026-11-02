
using Avalonia.Controls;
using Entities;
using System.Collections.ObjectModel;
using System.Linq;

namespace AdminDesktop.Controls.PageContent;

public partial class UserCatalog : UserControl
{
    public ObservableCollection<User> Users { get; set; } = new();

    public UserCatalog()
    {
        InitializeComponent();

        DataContext = this;

        LoadUsers();
    }

    private void LoadUsers()
    {
        using var db = Program.CreateDbContext();

        var users = db.Users.ToList();

        foreach (var user in users)
        {
            Users.Add(user);
        }
    }
}
