using Microsoft.EntityFrameworkCore;
using QuiSNCF.Database;
using QuiSNCF.DTO;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public class PlayerRepository(GameDbContext db, ILogger<PlayerRepository> logger) : IPlayerRepository
{
    public async Task<List<Player>> GetPlayers()
    {
        return await db.Players.ToListAsync();
    }
    
    private async Task<Player?> GetPlayerByName(string name)
    {
        return await db.Players.FirstOrDefaultAsync(p => p.Name.Trim().ToLower() == name.Trim().ToLower());
    }
    
    

    public async Task<bool> HasPlayerPlayedToday(string playerName)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return await db.DailyPlays.AnyAsync(dp =>
            dp.Player.Name.Trim().ToLower() == playerName.Trim().ToLower()
            && dp.GameType == gameType
            && dp.PlayedDate == today
        );
    }
    
    public async Task<bool> DoesPlayerExist(string name)
    {
        name = name.ToLower().Trim();
        return await db.Players.AnyAsync(p => p.Name.Trim().ToLower() == name.Trim().ToLower());
    }

    public async Task<List<Player>> GetTodaysBillboard()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var players = await db.Players.Where(x => x.ScoreDate == today).OrderByDescending(x => x.Score).ToListAsync();
        return players;
    }

    public async Task<List<Player>> GetBillboard()
    {
        var players = await db.Players.OrderByDescending(x => x.Score).ToListAsync();
        return players;
    }

    public async Task<int> AllTriesToday()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var tries = await db.Players.Where(x => x.ScoreDate == today).SumAsync(x => x.Tries);
        return tries;
        
    }

    private int CalculateScore(int multiplier)
    {
        int score = 5000 - (323 * multiplier);
        return Math.Max(500, score);
    }

    public async Task CreatePlayerAsync(CreatePlayerDTO player)
    {

        Player newPlayer = new Player()
        {
            Tries = player.Tries,
            Name = player.Name,
            Score = CalculateScore(player.Tries),
            ScoreDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };
        
        await db.Players.AddAsync(newPlayer);
        await db.SaveChangesAsync();
        
        logger.Success($"Joueur {player.Name} a joué pour la première fois avec {player.Tries} essais");
    }

    public async Task UpdatePlayerScore(string playersName, int tries)
    { 
        var player = await GetPlayerByName(playersName);
        
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (player.ScoreDate == today)
        {
            logger.LogWarning("[TRICHEUR] Le joueur {PlayerName} a déjà joué aujourd'hui",
                playersName
            );

            return;
        }
        
        player.Score += CalculateScore(tries);
        player.Tries += tries;
        player.ScoreDate = DateOnly.FromDateTime(DateTime.UtcNow);
        await db.SaveChangesAsync();
        
        logger.Success($"Joueur déjà existant {playersName} a joué aujourd'hui et a fait {player.Score} points");
        
    }
    
}