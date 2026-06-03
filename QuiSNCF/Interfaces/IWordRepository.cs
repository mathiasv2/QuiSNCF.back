using QuiSNCF.DTO;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public interface IWordRepository
{
    Task<Word?> GetOrPickTodayWord();
    Task<bool> IsInputRight(string input);
    Task CreateWord(CreateWordDTO word);
    Task UpdateWord(UpdateWordDTO wordDto, int wordId);
}