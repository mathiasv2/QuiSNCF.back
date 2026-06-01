using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using QuiSNCF.DTO;
using QuiSNCF.Repository;

namespace QuiSNCF.API.Controllers;

[EnableRateLimiting("fixed")]
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

    [HttpPost]
    public async Task<IActionResult> CreateWord(CreateWordDTO word)
    {
        await repo.CreateWord(word);
        return Ok( new 
        {
            message = "Word created successfully"
        });
    }
}