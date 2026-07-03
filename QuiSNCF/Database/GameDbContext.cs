using Microsoft.EntityFrameworkCore;
using QuiSNCF.Models;

namespace QuiSNCF.Database;

public class GameDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options): base(options){}

    public DbSet<Player> Players { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<Word>  Words { get; set; }
    public DbSet<DailyPlay> DailyPlays { get; set; } 
    public DbSet<City> Cities { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>()
            .HasKey(x => x.PlayerId);
        
        modelBuilder.Entity<Player>()
            .HasIndex(p => p.Name)
            .HasDatabaseName("IX_Players_Name");
        
        modelBuilder.Entity<Station>()
            .HasKey(x => x.StationId);

        modelBuilder.Entity<Word>()
            .HasKey(x => x.WordId);

        modelBuilder.Entity<DailyPlay>()
            .HasOne(dp => dp.Player)
            .WithMany(p => p.DailyPlays)
            .HasForeignKey(dp => dp.PlayerId);
        
        modelBuilder.Entity<DailyPlay>()
            .HasIndex(dp => new { dp.PlayerId, dp.GameType, dp.PlayedDate })
            .HasDatabaseName("IX_DailyPlays_PlayerId_GameType_PlayedDate");
        
        modelBuilder.Entity<City>()
            .HasKey(x => x.CityId);
        
        base.OnModelCreating(modelBuilder);
    }
    
}