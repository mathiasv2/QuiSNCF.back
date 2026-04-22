using QuiSNCF.DTO;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public interface IPlayerRepository
{
    Task<List<Player>> GetPlayers();
    Task CreatePlayerAsync(CreatePlayerDTO player);
    Task<List<Player>> GetTodaysBillboard();
    Task<int> AllTriesToday();
    Task<bool> DoesPlayerExist(string name);
    Task UpdatePlayerScore(string playersName, int tries);
}