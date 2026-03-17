using Microsoft.AspNetCore.Mvc;
using QuiSNCF.Models;
using QuiSNCF.Repository;

namespace QuiSNCF.API.Controllers;

[Route("api/[controller]")]
public class StationController(StationRepository repo) : Controller
{
    [HttpGet("GetStation")]
    public Station GetRandomStation()
    {
        var station = repo.GetRandomStation();
        return station;
    }

    [HttpGet("testInt")]
    public int TestInt()
    {
        var station = repo.GetRandomStation();
        return repo.HasStationBeenAlreadyPlayed(station);
    }

    [HttpPost("CreateStation")]
    public async Task CreateStation(Station station)
    {
        await repo.CreateStation(station);
    }
}