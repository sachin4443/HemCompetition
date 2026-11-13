using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HemCompetition.API.Data;

/// <summary>
/// Used only by the EF tools (`dotnet ef`, Package Manager Console). Reads the same connection
/// string the application uses: environment variable first, then appsettings.Development.json,
/// then appsettings.json.
///
/// Note: on a database that already contains the original hand-made tables, do not run
/// Update-Database. Start the application instead - DatabaseInitializer adopts the existing
/// tables and then applies the new migration.
/// </summary>
public class DesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        var cs = config.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(cs)) cs = "Host=localhost;Database=hemcompetition_design;Username=postgres";
        return new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(cs).Options);
    }
}
