using Microsoft.AspNetCore.Mvc;
using QuiSNCF.Models;
using QuiSNCF.Repository;
using QuiSNCF.Service;

namespace QuiSNCF.API.Controllers;

[Route("api/[controller]")]
public class CityController(DailyPickRepository picker): ControllerBase
{
    [HttpPost("checkinput/{input}")]
    public async Task<IActionResult> CheckInput(string input)
    {
        bool correct = await picker.IsInputRight<City>(input);
        var cityName = correct ? await picker.GetTodaysAnswer<City>() : null;
        return Ok(new { correct, cityName });
    }
}