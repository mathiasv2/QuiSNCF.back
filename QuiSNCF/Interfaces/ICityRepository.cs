using QuiSNCF.DTO;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public interface ICityRepository
{
    Task<City?> GetOrPickRandomCity();
    Task CreateCity(CreateCityDTO city);
    Task<List<City>> GetAllCities();
}