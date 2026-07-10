using QuiSNCF.DTO;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public interface IStationRepository
{
    Task<Station?> GetOrPickTodayStation();
    Task CreateStation(CreateStationDTO station);
    void DeleteStation(int id);
    public void UpdateStation(UpdateStationDTO updateStation, int id);
    Task<List<Station>> GetStations();
    void UpdateStationName(UpdateStationNameDTO name, int id);

}