using Microsoft.AspNetCore.Mvc;
using QuiSNCF.Repository;

namespace QuiSNCF.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WordController(IWordRepository repo): ControllerBase
{
    [HttpGet("todaysStation")]
    public async Task<IActionResult> GetRandomStation()
    {
        var station = await repo.GetOrPickTodayWord();
        if (station == null)
            return null;
        return Ok(station);
    }
}