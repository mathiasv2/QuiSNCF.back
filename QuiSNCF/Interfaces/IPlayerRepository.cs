using QuiSNCF.DTO;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public interface IPlayerRepository
{
    Task<List<Player>> GetBillboard(int season);
    Task<List<PlayerScoreDTO>> GetBillboardByGame(GameType gameType, int season);
    Task<int> SavePlayAsync(CreatePlayerDTO dto, GameType gameType);
    Task<List<GetPlayerScoreDTO>> GetScoreByGameAndPlayer(string playerName, GameType gameType);
    Task<int> GetPlayersCount(GameType? gametype);
}