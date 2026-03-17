using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public interface IStationRepository
{
    Task<Station?> GetRandomStation();
    Task<Station?> GetTodayStation();
    Task CreateStation(Station station);
    void DeleteStation(int id);
}