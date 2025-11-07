using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace Bcp.Exchange.FunctionApp.Middleware;

public class AzureAdAuthenticationMiddleware : IFunctionsWorkerMiddleware
{
    private readonly ILogger<AzureAdAuthenticationMiddleware> _logger;

    public AzureAdAuthenticationMiddleware(ILogger<AzureAdAuthenticationMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var requestData = await context.GetHttpRequestDataAsync();

        if (requestData != null)
        {
            if (requestData.Headers.TryGetValues("Authorization", out var authHeaderValues))
            {
                var authHeader = authHeaderValues.FirstOrDefault();
                if (
                    !string.IsNullOrEmpty(authHeader)
                    && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                )
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();

                    try
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(token);

                        var claims = jwtToken.Claims.ToList();
                        var identity = new ClaimsIdentity(claims, "AzureAd");
                        var principal = new ClaimsPrincipal(identity);

                        context.Items["User"] = principal;

                        var email = principal.FindFirst("preferred_username")?.Value ?? "Unknown";
                        var roles = principal.FindAll("roles").Select(c => c.Value).ToList();

                        _logger.LogInformation(
                            "User authenticated: {Email}, Roles: {Roles}",
                            email,
                            string.Join(", ", roles)
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to process JWT token");
                    }
                }
            }
        }

        await next(context);
    }
}
