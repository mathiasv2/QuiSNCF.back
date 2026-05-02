using Microsoft.AspNetCore.Mvc;
using QuiSNCF.DTO;
using QuiSNCF.Models;
using QuiSNCF.Repository;

namespace QuiSNCF.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StationController(IStationRepository repo) : ControllerBase
{
    [HttpGet("todaysStation")]
    public async Task<IActionResult> GetRandomStation()
    {
        var station = await repo.GetOrPickTodayStation();
        if (station == null)
            return null;
        return Ok(station);
    }

    [HttpDelete("removeStation/{id}")]
    public IActionResult DeleteStation(int id)
    {
        repo.DeleteStation(id);
        return Ok("Station deleted");
    }
    

    [HttpPost("createStation")]
    public async Task<IActionResult> CreateStation(CreateStationDTO station)
    {
        await repo.CreateStation(station);
        return Ok("Station created");
    }

    [HttpPost("checkinput/{input}")]
    public async Task<IActionResult> CheckInput(string input)
    {
        bool result = await repo.IsInputRight(input);
        return Ok(result);
    }

    [HttpPut("updateStation/{id}")]
    public async Task<IActionResult> UpdateStation(UpdateStationDTO station, int id)
    {
        repo.UpdateStation(station, id);
        return Ok("Station updated");   
    }
}