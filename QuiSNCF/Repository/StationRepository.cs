using Microsoft.EntityFrameworkCore;
using QuiSNCF.Database;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public class StationRepository(GameDbContext db) : IStationRepository
{
    /* TODO : Finalement supprimer le background service et faire en sorte que le
     premier joueur de la journée fasse le tirage de la journée pour tout le monde, plus simple et plus safe
     */
    
    public async Task<Station?> GetRandomStation()
    {
        Random rdn = new Random();
        
        var today = DateOnly.FromDateTime(DateTime.Today);

        var availableStations = db.Stations
            .Where(s => s.LastTimePlayed < today || s.LastTimePlayed > today)
            .ToList();

        if (!availableStations.Any())
            return null;

        int index = rdn.Next(availableStations.Count);
        
        await UpdateStationLastTimePlayed(index);

        return availableStations[index];
    }
    
    public async Task<Station?> GetTodayStation()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        Station? todaysStation = await db.Stations.Where(x => x.LastTimePlayed == today).FirstOrDefaultAsync();
        if (todaysStation != null)
            return todaysStation;

        return await GetRandomStation();
    }

    private async Task UpdateStationLastTimePlayed(int id)
    {
        var station = await db.Stations.FindAsync(id);
        if (station == null)
            return;
        station.LastTimePlayed = DateOnly.FromDateTime(DateTime.Today);
        await db.SaveChangesAsync();
    }
    public async Task CreateStation(Station station)
    {
        await db.Stations.AddAsync(station);
        await db.SaveChangesAsync();
    }
    
    public void DeleteStation(int id)
    {
        var station = db.Stations.FirstOrDefault(x => x.StationId == id);
        db.Stations.Remove(station);
        db.SaveChanges();
    }
}