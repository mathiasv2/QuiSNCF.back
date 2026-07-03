using Microsoft.EntityFrameworkCore;
using QuiSNCF.Database;
using QuiSNCF.DTO;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public class StationRepository(GameDbContext db, ILogger<StationRepository> logger, DailyPickRepository picker) : IStationRepository
{
    public async Task<Station?> GetRandomStation()
    {
        logger.LogInformation($"Sélection de la gare du jour à {TimeOnly.FromDateTime(DateTime.Now)}");
        
        var station = await  picker.GetOrPickToday<Station>();
        
        logger.Success($"Station du jour sélectionnée : " +
                       $"Ville : {station.City}," +
                       $"Dernière fois qu'elle a été joué : {station.LastTimePlayed};" +
                       $"Nom de la gare : {station.Name}");
        
        await UpdateStationLastTimePlayed(station);
        
        return station;
    }

    public async Task<List<Station>> GetStations()
    {
        return await db.Stations.ToListAsync();
    }

    public async Task<Station?> GetOrPickTodayStation() => await  picker.GetOrPickToday<Station>(s =>
    {
        var (x, y) = TodayRandomZoom();
        s.RandomX = x;
        s.RandomY = y;
    });

    private async Task<string> GetTodayStationsCity()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var station = await db.Stations.Where(x =>  x.LastTimePlayed == today).FirstOrDefaultAsync();
        if (station != null)
            return station.City;
        return null;
    }

    public async Task UpdateStationLastTimePlayed(Station station)
    {
        var positions = TodayRandomZoom();
        station.LastTimePlayed = DateOnly.FromDateTime(DateTime.Today);
        station.RandomX = positions.x;
        station.RandomY = positions.y;
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
        station.Hint = updateStation.Hint;
        db.SaveChanges();
    }

    private (double x, double y) TodayRandomZoom()
    {
        Random rdn = new Random();
        double x = rdn.NextDouble();
        double y = rdn.NextDouble();
        
        logger.Success($"Zoom du jour : {x},{y}");
        return (x, y);
    }

    public async Task<bool> IsInputRight(string input)
    {
        string todaysCity = await GetTodayStationsCity();
        return input.ToLower() == todaysCity.ToLower();
    }
}