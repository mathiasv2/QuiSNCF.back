using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public interface IWordRepository
{
    Task<Word?> GetOrPickTodayWord();
    Task<bool> IsInputRight(string input);
}