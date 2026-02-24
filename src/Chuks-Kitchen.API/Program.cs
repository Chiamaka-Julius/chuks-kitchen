using Chuks_Kitchen.Migrations.Data;
using Chuks_Kitchen.Migrations.Setup;
using Chuks_Kitchen.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddApplicationServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await DatabaseInitializer.InitializeAsync(dbContext);
    }
    catch (PostgresException ex) when (ex.SqlState == "28P01")
    {
        throw new InvalidOperationException(
            "PostgreSQL authentication failed (28P01). Update ConnectionStrings:DefaultConnection " +
            "in appsettings/launchSettings with the correct username/password, then run again.", ex);
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Ok(new { service = "Chuks Kitchen API", status = "running", architecture = "EF Core + CQRS" }));

app.MapControllers();

app.Run();
