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
    

    private async Task<bool> HasPlayerPlayedToday(string playerName, GameType gameType)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return await db.DailyPlays.AnyAsync(dp =>
            dp.Player.Name.Trim().ToLower() == playerName.Trim().ToLower()
            && dp.GameType == gameType
            && dp.PlayedDate == today
        );
    }
    
    public async Task<List<PlayerScoreDTO>> GetBillboardByGame(GameType gameType)
    {
        return await db.DailyPlays
            .Where(dp => dp.GameType == gameType)
            .GroupBy(dp => dp.Player.Name)
            .Select(g => new PlayerScoreDTO
            {
                Name = g.Key,
                Score = g.Sum(dp => dp.Score),
            })
            .OrderByDescending(p => p.Score)
            .ToListAsync();
    }


    public async Task<List<GetPlayerScoreDTO>> GetScoreByGameAndPlayer(string playerName, GameType gameType)
    {
        return await db.DailyPlays
            .Include(dp => dp.Player)
            .Where(dp => dp.Player.Name == playerName && dp.GameType == gameType)
            .OrderBy(dp => dp.PlayedDate)
            .Select(dp => new GetPlayerScoreDTO()
            {
                Tries = dp.Tries,
                Name = dp.Player.Name,
                Score = dp.Score,
                PlayedDate = dp.PlayedDate
            })
            .ToListAsync();
    }
    
   
    
    
    public async Task<bool> DoesPlayerExist(string name)
    {
        name = name.ToLower().Trim();
        return await db.Players.AnyAsync(p => p.Name.Trim().ToLower() == name.Trim().ToLower());
    }
    

    public async Task<List<Player>> GetBillboard()
    {
        var players = await db.Players.OrderByDescending(x => x.Score).ToListAsync();
        return players;
    }

    /*
    public async Task<int> AllTriesToday(GameType gameType)
       {
           var today = DateOnly.FromDateTime(DateTime.Today);

           return await db.DailyPlays
               .Where(dp => dp.PlayedDate == today && dp.GameType == gameType)
               .SumAsync(dp => dp.Tries);
       }
    */

    private int CalculateScore(int multiplier)
    {
        int score = 5000 - 323 * (multiplier - 1);
        return Math.Max(500, score);
    }

    public async Task CreatePlayerAsync(CreatePlayerDTO player)
    {

        Player newPlayer = new Player()
        {
            Tries = 0,
            Name = player.Name,
            Score = 0,
        };
        
        await db.Players.AddAsync(newPlayer);
        await db.SaveChangesAsync();
        
        logger.Success($"Joueur {player.Name} a joué pour la première fois avec {player.Tries} essais");
    }
    
    public async Task SavePlayAsync(CreatePlayerDTO dto, GameType gameType)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var player = await GetPlayerByName(dto.Name);

        if (player == null)
        {
            await CreatePlayerAsync(dto);
            player = await GetPlayerByName(dto.Name);
        }

        bool alreadyPlayed = await HasPlayerPlayedToday(dto.Name, gameType);

        
        if (alreadyPlayed)
        {
            logger.LogWarning("[TRICHEUR] {PlayerName} a déjà joué en mode {GameType} aujourd'hui",
                dto.Name, gameType);
            return;
        }

        var dailyPlay = new DailyPlay
        {
            PlayerId = player.PlayerId,
            GameType = gameType,
            Score = CalculateScore(dto.Tries),
            Tries = dto.Tries,
            PlayedDate = today
        };

        await db.DailyPlays.AddAsync(dailyPlay);
        await db.SaveChangesAsync();
        
        await UpdatePlayerScore(dto.Name, dto.Tries);

        logger.Success($"{dto.Name} a joué en mode {gameType} avec {dto.Tries} essais → {dailyPlay.Score} pts");
    }

    public async Task UpdatePlayerScore(string playersName, int tries)
    { 
        var player = await GetPlayerByName(playersName);
        
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        
        player.Score += CalculateScore(tries);
        player.Tries += tries;
        await db.SaveChangesAsync();
    }
    
}