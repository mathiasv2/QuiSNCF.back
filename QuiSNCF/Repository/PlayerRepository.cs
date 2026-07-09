using Microsoft.EntityFrameworkCore;
using QuiSNCF.Database;
using QuiSNCF.DTO;
using QuiSNCF.Exceptions;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public class PlayerRepository(GameDbContext db, ILogger<PlayerRepository> logger, IConfiguration config) : IPlayerRepository
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
    
    

    private async Task<bool> HasPlayedBeforeSpecialWeek(string playerName, GameType gameType)
    {
        DateOnly firstDaySpecialWeek = new(2026, 6, 22);

        return await db.DailyPlays.AnyAsync(dp =>
            dp.GameType == gameType &&
            dp.Player.Name == playerName &&
            dp.PlayedDate < firstDaySpecialWeek);
    }

    private async Task<float> GetStreakMultiplier(string playerName, GameType gameType)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var yesterday = today.AddDays(-1);

        var recentDates = await db.DailyPlays
            .Where(dp => dp.GameType == gameType 
                         && dp.Player.Name.Trim().ToLower() == playerName.Trim().ToLower())
            .Select(dp => dp.PlayedDate)
            .Distinct()
            .OrderByDescending(d => d)
            .Take(5)
            .ToListAsync();

        int streak = 1; 
        var expected = yesterday;

        foreach (var date in recentDates)
        {
            if (date == expected)
            {
                streak++;
                expected = expected.AddDays(-1);
            }
            else break;
        }

        return streak switch
        {
            >= 5 => 1.5f,
            4    => 1.4f,
            3    => 1.3f,
            2    => 1.2f,
            _    => 1f
        };
    }

    public async Task<int> GetTotalScoreByPlayerAndGameType(string playerName, GameType gameType)
    {
        return await db.DailyPlays
            .Where(x => x.Player.Name.Trim().ToLower() == playerName.Trim().ToLower() && x.GameType == gameType)
            .SumAsync(x=> x.Score);
    }
    
    public async Task<List<PlayerScoreDTO>> GetBillboardByGame(GameType gameType, int season)
    {
        return await db.DailyPlays
            .Where(dp => dp.GameType == gameType && dp.Season == season)
            .GroupBy(dp => dp.Player.Name)
            .Select(g => new PlayerScoreDTO
            {
                Name = g.Key,
                Score = g.Sum(dp => dp.Score),
            })
            .OrderByDescending(p => p.Score)
            .ToListAsync();
    }

    public PlayerScoreDTO GetBestPlayerBySeason(int season)
    {
        return db.Players.OrderByDescending(p => p.Score)
            .Where(x =>  x.Season == season)
            .Select(x => new PlayerScoreDTO()
            {
                Name = x.Name,
                Score = x.Score,
            })
            .First();
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
    

    public async Task<List<Player>> GetBillboard(int season)
    {
        var players = await db.Players.Where(x => x.Season == season).OrderByDescending(x => x.Score).ToListAsync();
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
            Season = 0
        };
        
        await db.Players.AddAsync(newPlayer);
        await db.SaveChangesAsync();
        
        logger.Success($"Joueur {player.Name} a joué pour la première fois avec {player.Tries} essais");
    }
    
    public async Task<int> SavePlayAsync(CreatePlayerDTO dto, GameType gameType)
    {
        var season = config.GetValue<int>("Season");
        
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
            logger.WarningColored($"[TRICHEUR] {dto.Name} a déjà joué en mode {dto.GameType} aujourd'hui");
            throw new AlreadyPlayedException(dto.Name);

        }
        
        var baseScore = CalculateScore(dto.Tries);
        var streakMultiplier = await GetStreakMultiplier(dto.Name, gameType);
        var isNewPlayer = !await HasPlayedBeforeSpecialWeek(dto.Name, gameType);
        var finalScore = (int)(baseScore * (isNewPlayer ? 1.5 : 1.0) * streakMultiplier);


        var dailyPlay = new DailyPlay
        {
            PlayerId = player.PlayerId,
            GameType = gameType,
            Score = finalScore,
            Tries = dto.Tries,
            PlayedDate = today,
            Season = season
        };
        
        

        await db.DailyPlays.AddAsync(dailyPlay);
        await db.SaveChangesAsync();
        
        await UpdatePlayerScore(dto.Name, dailyPlay.Score, dailyPlay.Tries);

        logger.Success($"{dto.Name} a joué en mode {gameType} avec {dto.Tries} essais → {dailyPlay.Score} pts");
        
        return finalScore;
    }

    private async Task UpdatePlayerScore(string playersName, int score, int tries)
    { 
        var season = config.GetValue<int>("Season");

        var player = await GetPlayerByName(playersName);
        
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        player.Score += score;
        player.Tries += tries;
        player.Season = season;
        await db.SaveChangesAsync();
    }

    public int GetPlayersCount(GameType? gametype)
    {
        return gametype == null 
            ? db.DailyPlays.GroupBy(x => x.PlayerId).Count()  
            : db.DailyPlays.Where(x => x.GameType == gametype.Value).GroupBy(x => x.PlayerId).Count();
    }
    
}