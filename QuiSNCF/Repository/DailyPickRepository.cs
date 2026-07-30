using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using QuiSNCF.Database;

namespace QuiSNCF.Repository;

public class DailyPickRepository(GameDbContext db, ILogger<DailyPickRepository> logger)
{
    private const int CooldownDays = 40;

    public async Task<T?> GetOrPickToday<T>(Action<T>? onNewPick = null) where T : class, IPlayable
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var todaysPick = await db.Set<T>()
            .FirstOrDefaultAsync(x => x.LastTimePlayed == today);
        
        
        if (todaysPick != null)
            return todaysPick;

        return await PickRandom(today, onNewPick);
    }

    private async Task<T?> PickRandom<T>(DateOnly today, Action<T>? onNewPick) where T : class, IPlayable
    {
        var cutoff = today.AddDays(-CooldownDays);

        var available = await db.Set<T>()
            .Where(x => x.LastTimePlayed == null || x.LastTimePlayed < cutoff)
            .ToListAsync();

        if (available.Count == 0)
            return null;

        var pick = available[Random.Shared.Next(available.Count)];
        pick.LastTimePlayed = today;
        onNewPick?.Invoke(pick);

        await db.SaveChangesAsync();

        logger.Success($"Sélection du jour : {pick.DisplayName}");
        return pick;
    }
    
    public async Task<string?> GetTodaysAnswer<T>() where T : class, IPlayable
    {
        var pick = await GetOrPickToday<T>();
        return pick?.DisplayName;
    }

    public async Task<bool> IsInputRight<T>(string input) where T : class, IPlayable
    {
        var answer = await GetTodaysAnswer<T>();
        return answer != null && Normalize(input) == Normalize(answer);
    }

    private static string Normalize(string value)
    {
        var formD = value.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in formD)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC)
            .ToLowerInvariant().Replace('-', ' ').Trim();
    }
}