using Microsoft.EntityFrameworkCore;
using QuiSNCF.Database;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public class StationRepository(GameDbContext db) : IStationRepository
{
    public async Task<Station?> GetRandomStation()
    {
        Random rdn = new Random();
        
        var today = DateOnly.FromDateTime(DateTime.Today);

        var availableStations = db.Stations
            .Where(s => s.LastTimePlayed < today)
            .ToList();

        if (!availableStations.Any())
            return null;

        int index = rdn.Next(1, availableStations.Count);
        
        await UpdateStationLastTimePlayed(availableStations[index]);
        Console.WriteLine(index);

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

    private async Task UpdateStationLastTimePlayed(Station station)
    {
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