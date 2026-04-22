using QuiSNCF.DTO;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public interface IStationRepository
{
    Task<Station?> GetRandomStation();
    Task<Station?> GetOrPickTodayStation();
    Task CreateStation(CreateStationDTO station);
    void DeleteStation(int id);
    Task<bool> IsInputRight(string input);
    public void UpdateStation(UpdateStationDTO updateStation, int id);

}