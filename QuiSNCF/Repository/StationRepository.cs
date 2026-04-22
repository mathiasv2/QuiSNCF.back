using Microsoft.EntityFrameworkCore;
using QuiSNCF.Database;
using QuiSNCF.DTO;
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
    
    public async Task<Station?> GetOrPickTodayStation()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        Station? todaysStation = await db.Stations.Where(x => x.LastTimePlayed == today).FirstOrDefaultAsync();
        if (todaysStation != null)
            return todaysStation;

        return await GetRandomStation();
    }

    private async Task<string> GetTodayStationsCity()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var station = await db.Stations.Where(x =>  x.LastTimePlayed == today).FirstOrDefaultAsync();
        if (station != null)
            return station.City;
        return null;
    }

    private async Task UpdateStationLastTimePlayed(Station station)
    {
        station.LastTimePlayed = DateOnly.FromDateTime(DateTime.Today);
        await db.SaveChangesAsync();
    }
    public async Task CreateStation(CreateStationDTO station)
    {
        Station newStation = new Station()
        {
            Name = station.Name,
            City = station.City,
            PictureUrl = station.PictureUrl,
            LastTimePlayed = station.LastTimePlayed,
            Hint = station.Hint,
        };
        await db.Stations.AddAsync(newStation);
        await db.SaveChangesAsync();
    }
    
    public void DeleteStation(int id)
    {
        Station? station = db.Stations.FirstOrDefault(x => x.StationId == id);
        if (station == null)
            return;
        db.Stations.Remove(station);
        db.SaveChanges();
    }

    public void UpdateStation(UpdateStationDTO updateStation, int id)
    {
        var station = db.Stations.FirstOrDefault(x => x.StationId == id);
        
        station.Name = updateStation.Name;
        station.City = updateStation.City;
        station.PictureUrl = updateStation.PictureUrl;
        station.Hint = updateStation.Hint;
        db.SaveChanges();
        
    }

    public async Task<bool> IsInputRight(string input)
    {
        string todaysCity = await GetTodayStationsCity();
        return input.ToLower() == todaysCity.ToLower();
    }
}