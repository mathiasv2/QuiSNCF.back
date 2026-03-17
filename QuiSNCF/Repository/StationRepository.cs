using QuiSNCF.Database;
using QuiSNCF.Models;

namespace QuiSNCF.Repository;

public class StationRepository(GameDbContext db)
{
    public Station GetRandomStation()
    {
        Random rdn = new Random();
        int rendomId = rdn.Next(1, db.Stations.Count());
        
        return db.Stations.ElementAt(rendomId);
    }

    public int HasStationBeenAlreadyPlayed(Station station)
    {
        var diff = station.LastTimePlayed.CompareTo(DateOnly.FromDateTime(DateTime.Today));
        return diff;
    }

    public async Task CreateStation(Station station)
    {
        await db.Stations.AddAsync(station);
        await db.SaveChangesAsync();
    }
}