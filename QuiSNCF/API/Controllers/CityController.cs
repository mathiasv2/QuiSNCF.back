using Microsoft.AspNetCore.Mvc;
using QuiSNCF.Models;
using QuiSNCF.Repository;
using QuiSNCF.Service;

namespace QuiSNCF.API.Controllers;

[Route("api/[controller]")]
public class CityController(DailyPickRepository picker, SNCFApiRequest request): ControllerBase
{
    [HttpPost("checkinput/{input}")]
    public async Task<IActionResult> CheckInput(string input)
    {
        bool correct = await picker.IsInputRight<City>(input);
        var cityName = correct ? await picker.GetTodaysAnswer<City>() : null;
        return Ok(new { correct, cityName });
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