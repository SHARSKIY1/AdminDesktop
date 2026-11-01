using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public bool IsOnline { get; set; } = true;
}