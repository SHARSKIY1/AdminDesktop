
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Entities.DataDB;

public class AppDataBase : DbContext
{
    public AppDataBase(DbContextOptions<AppDataBase> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentItem> PaymentItems { get; set; }
}
