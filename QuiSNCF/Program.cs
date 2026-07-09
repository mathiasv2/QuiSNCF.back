using System.Threading.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using QuiSNCF.Database;
using Microsoft.EntityFrameworkCore;
using QuiSNCF.Middleware;
using QuiSNCF.Repository;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// DB CONTEXT
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();

// DEPENDANCY INJECTION
builder.Services.AddScoped<IStationRepository, StationRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IWordRepository, WordRepository>();

builder.Services.AddEndpointsApiExplorer();

// RATE LIMITER
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(ip, _ =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = 30,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });
});

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseCors("AllowFrontend");

var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor
};
// Vider les listes par défaut pour accepter le proxy de l'ingress
forwardedOptions.KnownNetworks.Clear();
forwardedOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedOptions);

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<GameDbContext>();
        context.Database.Migrate();

        if (!context.Stations.Any())
        {
            var sql = await File.ReadAllTextAsync(
                Path.Combine(AppContext.BaseDirectory, "Seeds", "station.sql"));
            Console.WriteLine("Peuplement de la table Station");
            await context.Database.ExecuteSqlRawAsync(sql);
        }

        if (!context.Words.Any())
        {
            var sql = await File.ReadAllTextAsync(
                Path.Combine(AppContext.BaseDirectory, "Seeds", "word.sql"));
            Console.WriteLine("Peuplement de la table Words");
            await context.Database.ExecuteSqlRawAsync(sql);
        }
        
        

        Console.WriteLine("Database migrations OK.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database migration NOT OK: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "UrbanFlow Trips API";
        options.Theme = ScalarTheme.Moon;
    });

    app.UseHttpsRedirection();
}

app.UseExceptionHandler("/error");
app.UseRateLimiter();
app.UseAuthorization();
app.MapControllers();

app.Run();