using Microsoft.AspNetCore.Mvc;
using QuiSNCF.Models;
using QuiSNCF.Repository;
using QuiSNCF.Service;

namespace QuiSNCF.API.Controllers;

[Route("api/[controller]")]
public class CityController(DailyPickRepository picker, SNCFApiRequest request, PlayProofService proofs): ControllerBase
{
    [HttpPost("checkinput/{input}")]
    public async Task<IActionResult> CheckInput(string input, [FromQuery] string? state)
    {
        bool correct = await picker.IsInputRight<City>(input);
        var cityName = correct ? await picker.GetTodaysAnswer<City>() : null;

        int tries = proofs.GetCurrentTries(state, GameType.Display);
        var token = correct
            ? proofs.Issue(GameType.Display, tries, won: true)
            : proofs.Issue(GameType.Display, tries + 1, won: false);

        return Ok(new { correct, cityName, token });
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> Get(string name)
    {
        var result = await request.GetCityId();
        return Ok(result);
    }
    
    [HttpGet("departures/")]
    public async Task<IActionResult> GetDepartures()
    {
        var result = await request.GetNextDepartures();
        return Ok(result);
    }
}