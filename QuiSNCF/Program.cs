using QuiSNCF.Database;
using Microsoft.EntityFrameworkCore;
using QuiSNCF.Repository;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();

builder.Services.AddScoped<IStationRepository, StationRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});



var app = builder.Build();

app.UseCors("AllowFrontend");
app.UseCors("AllowLocalhost");


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<GameDbContext>();
        context.Database.Migrate();
        
        if (!context.Stations.Any())
        {
            var sql = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Seeds", "station.sql"));
            Console.WriteLine("Peuplement de la table Station");
            Console.WriteLine(sql);
            await context.Database.ExecuteSqlRawAsync(sql);
        }
        
        
        Console.WriteLine("Database migrations OK.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database migration NOT OK: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    app.MapScalarApiReference(options =>
    {
        options.Title = "UrbanFlow Trips API";
        options.Theme = ScalarTheme.Moon;
    });
}


app.UseHttpsRedirection();


app.UseHttpsRedirection();  
app.UseAuthorization();
app.MapControllers();

app.Run();
