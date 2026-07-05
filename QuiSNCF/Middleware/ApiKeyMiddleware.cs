using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuiSNCF.Middleware;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ApiKeyAttribute : Attribute, IAsyncActionFilter
{
    private const string ApiKeyHeader = "X-Api-Key";

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var config = context.HttpContext.RequestServices
            .GetRequiredService<IConfiguration>();

        var expected = config["ApiKey:Secret"];

        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeader, out var provided)
            || string.IsNullOrEmpty(expected)
            || !CryptographicOperations.FixedTimeEquals(
                System.Text.Encoding.UTF8.GetBytes(provided.ToString()),
                System.Text.Encoding.UTF8.GetBytes(expected)))
        {
            context.Result = new UnauthorizedObjectResult("Non autorisé");
            return;
        }

        await next();
    }
}