using Microsoft.EntityFrameworkCore;
using QuiSNCF.Database;
using QuiSNCF.DTO;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public class PlayerRepository(GameDbContext db) : IPlayerRepository
{
    public async Task<List<Player>> GetPlayers()
    {
        return await db.Players.ToListAsync();
    }
    
    private async Task<Player?> GetPlayerByName(string name)
    {
        return await db.Players.FirstOrDefaultAsync(p => p.Name.Trim().ToLower() == name.Trim().ToLower());
    }

    public async Task<bool> DoesPlayerExist(string name)
    {
        name = name.ToLower().Trim();
        return await db.Players.AnyAsync(p => p.Name.Trim().ToLower() == name.Trim().ToLower());
    }

    public async Task<List<Player>> GetTodaysBillboard()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var players = await db.Players.Where(x => x.ScoreDate == today).OrderBy(x => x.Score).ToListAsync();
        return players;
    }

    public async Task<int> AllTriesToday()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var tries = await db.Players.Where(x => x.ScoreDate == today).SumAsync(x => x.Score);
        return tries;
        
    }

    private int CalculateScore(int multiplier)
    {
        int score = 5000;
        int lostPoints = 323 * multiplier;
        score -= lostPoints;
        return score;
    }

    public async Task CreatePlayerAsync(CreatePlayerDTO player)
    {

        Player newPlayer = new Player()
        {
            Tries = player.Tries,
            Name = player.Name,
            Score = CalculateScore(player.Multiplier),
            ScoreDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };
        
        await db.Players.AddAsync(newPlayer);
        await db.SaveChangesAsync();
    }

    public async Task UpdatePlayerScore(string playersName, int tries)
    { 
        var player = await GetPlayerByName(playersName);
        player.Score += CalculateScore(tries);
        player.Tries += tries;
        player.ScoreDate = DateOnly.FromDateTime(DateTime.UtcNow);
        await db.SaveChangesAsync();
    }
    
}