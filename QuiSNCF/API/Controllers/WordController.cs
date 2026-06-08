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

    [HttpPost("createWord")]
    public async Task<IActionResult> CreateWord(CreateWordDTO word)
    {
        await repo.CreateWord(word);
        return Ok( new 
        {
            message = "Word created successfully"
        });
    }

    [HttpPut("updateworddefinition/{id}")]
    public async Task<IActionResult> UpdateWordDefinition(UpdateWordDefinitionDTO wordDto, int id)
    {
        await repo.UpdateWordDefinition(wordDto, id);
        return Ok(new
        {
            message = "Word definition updated successfully"
        });
    }
    
    [HttpPut("updateword/{id}")]
    public async Task<IActionResult> UpdateWord(UpdateWordDTO wordDto, int id)
    {
        await repo.UpdateWord(wordDto, id);
        return Ok(new
        {
            message = "Word updated successfully"
        });
    }
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var words = await repo.GetWords();
        return Ok(words);
    }

    [HttpGet("{word}")]
    public async Task<bool> WordExists(string word)
    {
        return await repo.LookUpForWord(word);
    }
}