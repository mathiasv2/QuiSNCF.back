using Microsoft.AspNetCore.Mvc;
using QuiSNCF.Repository;
using QuiSNCF.Service;

namespace QuiSNCF.API.Controllers;

[Route("api/[controller]")]
public class CityController(SNCFApiRequest request): ControllerBase
{
    [HttpGet("{name}")]
    public async Task<IActionResult> Get(string name)
    {
        var result = await request.GetCityId(name);
        return Ok(result);
    }
    
    [HttpGet("departures/{name}")]
    public async Task<IActionResult> GetDepartures(string name)
    {
        var result = await request.GetNextDepartures(name);
        return Ok(result);
    }
}