using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Entities.DataDB;

public class AppDataBaseFactory : IDesignTimeDbContextFactory<AppDataBase>
{
    public AppDataBase CreateDbContext(string[] args)
    {
        var connectionString =
            "Host=localhost;Port=5432;Database=AdminDesktopDB;Username=postgres;Password=322";

        var options = new DbContextOptionsBuilder<AppDataBase>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDataBase(options);
    }
}