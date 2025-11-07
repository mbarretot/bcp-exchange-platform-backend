using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Bcp.Exchange.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ExchangeDbContext>
{
    public ExchangeDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ExchangeDbContext>();

        var functionAppPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..",
            "Bcp.Exchange.FunctionApp",
            "local.settings.json"
        );

        string connectionString;

        if (File.Exists(functionAppPath))
        {
            var json = File.ReadAllText(functionAppPath);
            var localSettings = JsonDocument.Parse(json);
            connectionString =
                localSettings
                    .RootElement.GetProperty("Values")
                    .GetProperty("SqlConnectionString")
                    .GetString()
                ?? throw new InvalidOperationException(
                    "SqlConnectionString not found in local.settings.json"
                );
        }
        else
        {
            throw new FileNotFoundException($"local.settings.json not found at {functionAppPath}");
        }

        optionsBuilder.UseSqlServer(connectionString);

        return new ExchangeDbContext(optionsBuilder.Options);
    }
}
