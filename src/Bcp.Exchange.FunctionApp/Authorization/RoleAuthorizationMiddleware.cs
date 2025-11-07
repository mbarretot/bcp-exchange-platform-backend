using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace Bcp.Exchange.FunctionApp.Authorization;

public class RoleAuthorizationMiddleware : IFunctionsWorkerMiddleware
{
    private readonly ILogger<RoleAuthorizationMiddleware> _logger;

    public RoleAuthorizationMiddleware(ILogger<RoleAuthorizationMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var httpContext = context.GetHttpContext();
        if (httpContext != null)
        {
            var targetMethod = GetTargetFunctionMethod(context);
            if (targetMethod != null)
            {
                var roleAttribute = targetMethod.GetCustomAttribute<RequireRoleAttribute>();
                if (roleAttribute != null)
                {
                    if (!ValidateRole(context, roleAttribute.Role))
                    {
                        _logger.LogWarning(
                            "Access denied for user. Required role: {RequiredRole}",
                            roleAttribute.Role
                        );

                        httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await httpContext.Response.WriteAsJsonAsync(
                            new
                            {
                                error = "Forbidden",
                                message = $"Se requiere el rol '{roleAttribute.Role}' para realizar esta operación",
                            }
                        );
                        return;
                    }

                    _logger.LogInformation(
                        "Role validation passed. Required role: {RequiredRole}",
                        roleAttribute.Role
                    );
                }
            }
        }

        await next(context);
    }

    private bool ValidateRole(FunctionContext context, string requiredRole)
    {
        if (context.Items.TryGetValue("User", out var userObj) && userObj is ClaimsPrincipal user)
        {
            var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            if (!roles.Any())
            {
                var rolesClaim = user.FindAll("roles").Select(c => c.Value);
                roles.AddRange(rolesClaim);
            }

            return roles.Contains(requiredRole, StringComparer.OrdinalIgnoreCase);
        }

        return false;
    }

    private MethodInfo? GetTargetFunctionMethod(FunctionContext context)
    {
        try
        {
            var entryPoint = context.FunctionDefinition.EntryPoint;
            var assemblyPath = context.FunctionDefinition.PathToAssembly;

            var assembly = Assembly.LoadFrom(assemblyPath);
            var typeName = entryPoint.Substring(0, entryPoint.LastIndexOf('.'));
            var methodName = entryPoint.Substring(entryPoint.LastIndexOf('.') + 1);

            var type = assembly.GetType(typeName);
            return type?.GetMethod(methodName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get target function method");
            return null;
        }
    }
}
