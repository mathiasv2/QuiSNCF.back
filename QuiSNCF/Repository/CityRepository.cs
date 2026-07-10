using Microsoft.EntityFrameworkCore;
using QuiSNCF.Database;
using QuiSNCF.DTO;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public class CityRepository(GameDbContext db, ILogger<StationRepository> logger, DailyPickRepository picker) : ICityRepository
{
    public async Task<City?> GetOrPickRandomCity() => await picker.GetOrPickToday<City>();

    public async Task CreateCity(CreateCityDTO city)
    {
        City cityToCreate = new City()
        {
            CityName = city.CityName,
            LastTimePlayed = new DateOnly(2000, 01, 01)
        };
        
        db.Cities.Add(cityToCreate);
        await db.SaveChangesAsync();
    }

    public async Task<List<City>> GetAllCities() => await db.Cities.ToListAsync();

}