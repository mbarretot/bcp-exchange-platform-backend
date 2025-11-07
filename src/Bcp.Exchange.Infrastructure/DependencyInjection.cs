using Bcp.Exchange.Core.Configuration.Interfaces;
using Bcp.Exchange.Core.ExchangeRates.Interfaces;
using Bcp.Exchange.Core.Shared.Interfaces;
using Bcp.Exchange.Infrastructure.Persistence;
using Bcp.Exchange.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bcp.Exchange.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<ExchangeDbContext>(options =>
            options.UseSqlServer(configuration["SqlConnectionString"])
        );

        services.AddScoped<IParameterRepository, ParameterRepository>();
        services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
