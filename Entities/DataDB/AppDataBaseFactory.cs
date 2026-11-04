using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Entities.DataDB;

public class AppDataBaseFactory : IDesignTimeDbContextFactory<AppDataBase>
{
    public AppDataBase CreateDbContext(string[] args)
    {
        var connectionString =
            "Host=100.94.83.43;Port=5432;Database=AdminDesktopDB;Username=teammate;Password=Korol322!";

        var options = new DbContextOptionsBuilder<AppDataBase>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDataBase(options);
    }
}