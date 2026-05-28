using QuiSNCF.DTO;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public interface IPlayerRepository
{
    Task<List<Player>> GetPlayers();
    Task CreatePlayerAsync(CreatePlayerDTO player);
    Task<bool> DoesPlayerExist(string name);
    Task UpdatePlayerScore(string playersName, int tries);
    Task<List<Player>> GetBillboard();
    Task<List<PlayerScoreDTO>> GetBillboardByGame(GameType gameType);
    Task SavePlayAsync(CreatePlayerDTO dto, GameType gameType);

}