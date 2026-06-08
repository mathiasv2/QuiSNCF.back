using QuiSNCF.DTO;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public interface IWordRepository
{
    Task<Word?> GetOrPickTodayWord();
    Task<bool> IsInputRight(string input);
    Task CreateWord(CreateWordDTO word);
    Task UpdateWordDefinition(UpdateWordDefinitionDTO wordDto, int wordId);
    Task UpdateWord(UpdateWordDTO wordDto, int wordId);
    Task<List<Word>> GetWords();
    Task<bool> LookUpForWord(string word);
}