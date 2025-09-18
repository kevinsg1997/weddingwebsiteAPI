using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WeddingMerchantApi.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                               ?? "Host=yamabiko.proxy.rlwy.net;Port=53637;Username=postgres;Password=yngKHbrXNUUPEknEsIkvMVxsuaKzIljz;Database=railway;Pooling=true;SSL Mode=Require;Trust Server Certificate=true;";

        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}