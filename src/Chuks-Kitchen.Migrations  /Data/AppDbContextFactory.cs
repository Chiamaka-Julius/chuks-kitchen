using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Chuks_Kitchen.Migrations.Data;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Priority: env var for local override, then sane default.
        var connectionString =
            Environment.GetEnvironmentVariable("CHUKS_KITCHEN_CONNECTION")
            ?? "Host=localhost;Port=5433;Database=chuks_kitchen_db;Username=postgres;Password=0000";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
