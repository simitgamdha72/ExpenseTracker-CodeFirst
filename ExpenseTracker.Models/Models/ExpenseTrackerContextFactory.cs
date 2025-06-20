using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class ExpenseTrackerContextFactory : IDesignTimeDbContextFactory<ExpenseTrackerContext>
{
    public ExpenseTrackerContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../ExpenseTracker.API");

        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<ExpenseTrackerContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ExpenseTrackerContext(optionsBuilder.Options);
    }
}
