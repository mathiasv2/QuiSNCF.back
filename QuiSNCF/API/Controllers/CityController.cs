using Microsoft.AspNetCore.Mvc;
using QuiSNCF.Repository;
using QuiSNCF.Service;

namespace QuiSNCF.API.Controllers;

[Route("api/[controller]")]
public class CityController(SNCFApiRequest request): ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
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