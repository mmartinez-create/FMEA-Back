using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FmeaManager.Infrastructure.Persistence.DesignTime;

public sealed class FmeaManagerDbContextFactory
    : IDesignTimeDbContextFactory<FmeaManagerDbContext>
{
    public FmeaManagerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FmeaManagerDbContext>();

        var connectionString = Environment.GetEnvironmentVariable(
            "FMEA_CONNECTION_STRING")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=FmeaManagerDb;Trusted_Connection=True;TrustServerCertificate=True;";

        optionsBuilder.UseSqlServer(connectionString);

        return new FmeaManagerDbContext(optionsBuilder.Options);
    }
}
