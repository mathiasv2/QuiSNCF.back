using QuiSNCF.Database;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public class CityRepository(GameDbContext db, ILogger<StationRepository> logger, DailyPickRepository picker)
{
    public async Task<City?> GetOrPickRandomCity() => await picker.GetOrPickToday<City>();
    
    

}