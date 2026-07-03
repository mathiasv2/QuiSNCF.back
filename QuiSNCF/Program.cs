using System.Threading.RateLimiting;
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
builder.Services.AddScoped<DailyPickRepository>();

builder.Services.AddEndpointsApiExplorer();

// RATE LIMITER
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 30;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
    
    options.RejectionStatusCode = 429;
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
        
        if (!context.Cities.Any())
        {
            var sql = await File.ReadAllTextAsync(
                Path.Combine(AppContext.BaseDirectory, "Seeds", "city.sql"));
            Console.WriteLine("Peuplement de la table Cities");
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

app.UseRateLimiter();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();