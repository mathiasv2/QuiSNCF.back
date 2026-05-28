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


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>()
            .HasKey(x =>  x.PlayerId);

        modelBuilder.Entity<Station>()
            .HasKey(x => x.StationId);

        modelBuilder.Entity<Word>()
            .HasKey(x => x.WordId);

        modelBuilder.Entity<DailyPlay>()
            .HasOne(dp => dp.Player)
            .WithMany(p => p.DailyPlays)
            .HasForeignKey(dp => dp.PlayerId);
        
        base.OnModelCreating(modelBuilder);
    }
    
}