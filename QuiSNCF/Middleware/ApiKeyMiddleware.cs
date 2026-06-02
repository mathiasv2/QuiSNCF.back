namespace QuiSNCF.Middleware;

public class ApiKeyMiddleware(RequestDelegate next, IConfiguration config)
{
    private const string ApiKeyHeader = "X-Api-Key";

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();

        var protectedRoutes = new[]
        {
            "/api/station/createstation",
            "/api/word/createword",
            "updateStation"
        };

        if (protectedRoutes.Any(r => path?.StartsWith(r.ToLower()) == true))
        {
            if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var key)
                || key != config["ApiKey:Secret"])
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Non autorisé");
                return;
            }
        }

        await next(context);
    }
}