using Bcp.Exchange.Application;
using Bcp.Exchange.FunctionApp.Authorization;
using Bcp.Exchange.FunctionApp.Middleware;
using Bcp.Exchange.Infrastructure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(worker =>
    {
        worker.UseMiddleware<CorsMiddleware>();
        worker.UseMiddleware<AzureAdAuthenticationMiddleware>();
        worker.UseMiddleware<RoleAuthorizationMiddleware>();
        worker.UseMiddleware<ExceptionHandlingMiddleware>();
    })
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddApplication();
    })
    .ConfigureServices(
        (context, services) =>
        {
            services.AddInfrastructure(context.Configuration);
        }
    )
    .Build();

await host.RunAsync();
