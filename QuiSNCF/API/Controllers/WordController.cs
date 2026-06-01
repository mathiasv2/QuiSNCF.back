using Microsoft.AspNetCore.Mvc;
using QuiSNCF.Repository;

namespace QuiSNCF.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WordController(IWordRepository repo): ControllerBase
{
    [HttpGet("todaysWord")]
    public async Task<IActionResult> GetRandomWord()
    {
        var station = await repo.GetOrPickTodayWord();
        if (station == null)
            return null;
        return Ok(station);
    }
}