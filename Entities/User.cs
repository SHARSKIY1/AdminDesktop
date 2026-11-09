
namespace Entities;

public enum UserRole
{
    User = 0,
    Admin = 1
}

public class User
{
    public UserRole Role { get; set; }
    public int UserId { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; } = new DateTime(2026, 9, 1);
    public bool IsOnline { get; set; } = true;
    public string IconPath { get; set; } = string.Empty;
    public List<Payment> Payments { get; set; } = new();
}
