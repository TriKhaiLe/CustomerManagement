using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Server.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Server.Api");

        Console.WriteLine($"[DbContextFactory] Base path resolved to: {basePath}");
        if (!Directory.Exists(basePath))
            Console.WriteLine($"[DbContextFactory] Warning: basePath directory does not exist!");

        var configuration = new ConfigurationBuilder()
        .SetBasePath(basePath) // Install Microsoft.Extensions.Configuration.Json to use
        .AddJsonFile("appsettings.Development.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("[DbContextFactory] Connection string is missing. Check appsettings or environment variables.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}