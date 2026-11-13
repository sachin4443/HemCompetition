using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HemCompetition.API.Data;

/// <summary>Used only by `dotnet ef` so migrations can be generated without a running database.</summary>
public class DesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var cs = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
                 ?? "Host=localhost;Database=hemcompetition_design;Username=postgres";
        return new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(cs).Options);
    }
}
