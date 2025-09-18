using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WeddingMerchantApi.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        var rawDatabaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

        var databaseUri = new Uri(rawDatabaseUrl);
        var userInfo = databaseUri.UserInfo.Split(':');

        var builderDb = new Npgsql.NpgsqlConnectionStringBuilder
        {
            Host = databaseUri.Host,
            Port = databaseUri.Port,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = databaseUri.AbsolutePath.TrimStart('/'),
            SslMode = Npgsql.SslMode.Require
        };

        var connectionString = builderDb.ToString();

        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}