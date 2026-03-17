using Microsoft.AspNetCore.Mvc;
using QuiSNCF.Models;
using QuiSNCF.Repository;

namespace QuiSNCF.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StationController(IStationRepository repo) : ControllerBase
{
    [HttpGet("todaysStation")]
    public async Task<Station?> GetRandomStation()
    {
        var station = await repo.GetTodayStation();
        if (station == null)
            return null;
        return station;
    }

    [HttpDelete("removeStation/{id}")]
    public void DeleteStation(int id)
    {
        repo.DeleteStation(id);
    }
    

    [HttpPost("createStation")]
    public async Task CreateStation(Station station)
    {
        await repo.CreateStation(station);
    }
}