using QuiSNCF.DTO;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public interface IPlayerRepository
{
    Task<List<Player>> GetBillboard(int season);
    Task<List<PlayerScoreDTO>> GetBillboardByGame(GameType gameType, int season);
    Task<int> SavePlayAsync(CreatePlayerDTO dto, GameType gameType);
    Task<List<GetPlayerScoreDTO>> GetScoreByGameAndPlayer(string playerName, GameType gameType);
    int GetPlayersCount(GameType? gametype);
    PlayerScoreDTO GetBestPlayerBySeason(int season);
    Task<int> GetTotalScoreByPlayerAndGameType(string playerName, GameType? gameType);
}