using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using QuiSNCF.DTO;
using QuiSNCF.Middleware;
using QuiSNCF.Repository;

namespace QuiSNCF.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WordController(IWordRepository repo, DailyPickRepository picker): ControllerBase
{
    [HttpGet("todaysWord")]
    public async Task<IActionResult> GetRandomWord()
    {
        var word = await picker.GetOrPickToday<Word>();
        if (word == null)
            return null;
        return Ok(word);
    }

    [ApiKey]
    [HttpPost("createWord")]
    public async Task<IActionResult> CreateWord(CreateWordDTO word)
    {
        await repo.CreateWord(word);
        return Ok( new 
        {
            message = "Word created successfully"
        });
    }

    [ApiKey]
    [HttpPut("updateworddefinition/{id}")]
    public async Task<IActionResult> UpdateWordDefinition(UpdateWordDefinitionDTO wordDto, int id)
    {
        await repo.UpdateWordDefinition(wordDto, id);
        return Ok(new
        {
            message = "Word definition updated successfully"
        });
    }
    
    [ApiKey]
    [HttpPut("updateword/{id}")]
    public async Task<IActionResult> UpdateWord(UpdateWordDTO wordDto, int id)
    {
        await repo.UpdateWord(wordDto, id);
        return Ok(new
        {
            message = "Word updated successfully"
        });
    }
    
    [ApiKey]
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