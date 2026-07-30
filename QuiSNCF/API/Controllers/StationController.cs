using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using QuiSNCF.DTO;
using QuiSNCF.Middleware;
using QuiSNCF.Models;
using QuiSNCF.Repository;
using QuiSNCF.Service;

namespace QuiSNCF.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StationController(IStationRepository repo, DailyPickRepository picker, PlayProofService proofs) : ControllerBase
{
    [HttpGet("todaysStation")]
    public async Task<IActionResult> GetRandomStation()
    {
        var station = await repo.GetOrPickTodayStation();
        if (station == null)
            return null;
        return Ok(station);
    }


    [ApiKey]
    [HttpPost("createStation")]
    public async Task<IActionResult> CreateStation(CreateStationDTO station)
    {
        await repo.CreateStation(station);
        return Ok("Station created");
    }
    
    [ApiKey]
    [HttpPut("updateStation/{id}")]
    public async Task<IActionResult> UpdateStation(UpdateStationDTO station, int id)
    {
        repo.UpdateStation(station, id);
        return Ok("Station updated");   
    }
    
    [ApiKey]
    [HttpPut("updateStationName/{id}")]
    public async Task<IActionResult> UpdateStation([FromBody] UpdateStationNameDTO name, int id)
    {
        repo.UpdateStationName(name, id);
        return Ok("Station updated");   
    }

    [ApiKey]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var stations = await repo.GetStations();
        return Ok(stations);
    }
    
    [HttpPost("checkinput/{input}")]
    public async Task<IActionResult> CheckInput(string input, [FromQuery] string? state)
    {
        bool correct = await picker.IsInputRight<Station>(input);
        var cityName = correct ? await picker.GetTodaysAnswer<Station>() : null;

        int tries = proofs.GetCurrentTries(state, GameType.Station);
        var token = correct
            ? proofs.Issue(GameType.Station, tries, won: true)
            : proofs.Issue(GameType.Station, tries + 1, won: false);

        return Ok(new { correct, cityName, token });
    }
    
    

    /*
    

    [HttpDelete("removeStation/{id}")]
    public IActionResult DeleteStation(int id)
    {
        repo.DeleteStation(id);
        return Ok("Station deleted");
    }
    


    [HttpPost("checkinput/{input}")]
    public async Task<IActionResult> CheckInput(string input)
    {
        bool result = await repo.IsInputRight(input);
        return Ok(result);
    }

*/
    
    
}