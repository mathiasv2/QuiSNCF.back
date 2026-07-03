using Microsoft.EntityFrameworkCore;
using QuiSNCF.Database;
using QuiSNCF.DTO;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public class WordRepository(GameDbContext db, ILogger<WordRepository> logger, DailyPickRepository picker) : IWordRepository
{
    public async Task<List<Word>> GetWords()
    {
        return await db.Words.ToListAsync();
    }
    
    public async Task<Word?> GetOrPickTodayWord() => await  picker.GetOrPickToday<Word>();



    public async Task CreateWord(CreateWordDTO word)
    {
        Word newWord = new Word()
        {
            LastTimePlayed = word.LastTimePlayed,
            WordName = word.WordName,
            Definition = word.Definition,
        };
        
        await db.Words.AddAsync(newWord);
        await db.SaveChangesAsync();
    }

    private async Task<Word?> GetWordByID(int wordId)
    {
        Word? word =  await db.Words.FindAsync(wordId);
        return word ?? throw new KeyNotFoundException($"Word {wordId} not found");
    }

    public async Task UpdateWordDefinition(UpdateWordDefinitionDTO wordDto, int wordId)
    {
        var word = await GetWordByID(wordId);
        word.Definition = wordDto.Definition;
        await db.SaveChangesAsync();
    }

    public async Task UpdateWord(UpdateWordDTO wordDto, int wordId)
    {
        var word = await GetWordByID(wordId);
        word.WordName = wordDto.Word;
        await db.SaveChangesAsync();
    }
    
    public async Task DeleteWord(int wordId)
    {
        var word = await GetWordByID(wordId);

        db.Words.Remove(word);
    }

    public async Task<bool> LookUpForWord(string word)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "mots.txt");
        var lines = await File.ReadAllLinesAsync(path);
        return lines.Any(l => l.Trim().Equals(word.Trim(), StringComparison.OrdinalIgnoreCase));
    }
    
    
}