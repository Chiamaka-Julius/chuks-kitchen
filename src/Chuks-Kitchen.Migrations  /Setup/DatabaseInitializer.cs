using Chuks_Kitchen.Migrations.Data;
using Chuks_Kitchen.Models;
using Microsoft.EntityFrameworkCore;

namespace Chuks_Kitchen.Migrations.Setup;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);

        if (pendingMigrations.Any())
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        else
        {
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        }

        if (!await dbContext.Foods.AnyAsync(cancellationToken))
        {
            dbContext.Foods.AddRange(
                new Food { Id = Guid.NewGuid(), Name = "Jollof Rice", Price = 3500m, IsAvailable = true },
                new Food { Id = Guid.NewGuid(), Name = "Fried Rice", Price = 4000m, IsAvailable = true },
                new Food { Id = Guid.NewGuid(), Name = "Peppered Chicken", Price = 2500m, IsAvailable = true }
            );

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
