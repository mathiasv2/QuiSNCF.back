using Microsoft.EntityFrameworkCore;
using QuiSNCF.Database;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public class WordRepository(GameDbContext db, ILogger<WordRepository> logger) : IWordRepository
{
    public async Task<Word?> GetOrPickTodayWord()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var todaysWord = await db.Words.Where(w => w.LastTimePlayed == today).FirstOrDefaultAsync();

        if (todaysWord != null)
            return todaysWord;

        return await GetRandomWord();
    }

    private async Task<Word?> GetRandomWord()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var available = await db.Words
            .Where(w => w.LastTimePlayed < today.AddDays(-10))
            .ToListAsync();

        if (!available.Any())
            return null;

        var word = available[new Random().Next(available.Count)];
        word.LastTimePlayed = today;
        await db.SaveChangesAsync();

        logger.Success($"Mot du jour sélectionné : {word.WordName}");
        return word;
    }

    public async Task<bool> IsInputRight(string input)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var word = await db.Words.Where(w => w.LastTimePlayed == today).FirstOrDefaultAsync();

        if (word == null) return false;
        return string.Equals(input.Trim(), word.WordName.Trim(), StringComparison.OrdinalIgnoreCase);
    }
}