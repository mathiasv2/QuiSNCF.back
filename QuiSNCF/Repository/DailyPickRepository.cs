using Microsoft.EntityFrameworkCore;
using QuiSNCF.Database;

namespace QuiSNCF.Repository;

public class DailyPickRepository(GameDbContext db, ILogger logger)
{
    private const int CooldownDays = 40;

    public async Task<T?> GetOrPickToday<T>() where T : class, IPlayable
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var todaysPick = await db.Set<T>()
            .FirstOrDefaultAsync(x => x.LastTimePlayed == today);

        if (todaysPick != null)
            return todaysPick;

        return await PickRandom<T>(today);
    }

    private async Task<T?> PickRandom<T>(DateOnly today) where T : class, IPlayable
    {
        var cutoff = today.AddDays(-CooldownDays);

        var available = await db.Set<T>()
            .Where(x => x.LastTimePlayed == null || x.LastTimePlayed < cutoff)
            .ToListAsync();

        if (available.Count == 0)
            return null;

        var pick = available[Random.Shared.Next(available.Count)];
        pick.LastTimePlayed = today;
        await db.SaveChangesAsync();

        logger.Success($"Sélection du jour : {pick.DisplayName}");
        return pick;
    }
}