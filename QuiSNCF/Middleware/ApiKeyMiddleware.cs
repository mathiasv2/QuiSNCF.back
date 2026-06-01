namespace QuiSNCF.Middleware;

public class ApiKeyMiddleware(RequestDelegate next, IConfiguration config)
{
    private const string ApiKeyHeader = "X-Api-Key";

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();
        Console.WriteLine($"[Middleware] Path reçu : {path}"); 
    
        var protectedRoutes = new[] { "/api/station/createstation",  "/api/word" };

        if (protectedRoutes.Any(r => path?.StartsWith(r.ToLower()) == true))
        {
            if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var key)
                || key != config["ApiKey:Secret"])
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Non autorisé");
                return; 
                
            }
            
            await next(context); 

        }
    }
}