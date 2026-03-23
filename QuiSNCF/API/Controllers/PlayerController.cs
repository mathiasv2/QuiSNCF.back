using Microsoft.AspNetCore.Mvc;
using QuiSNCF.DTO;
using QuiSNCF.Models;
using QuiSNCF.Repository;

namespace QuiSNCF.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayerController(IPlayerRepository repo) : ControllerBase
{
    [HttpPost("createPlayer")]
    public async Task CreatePlayer([FromBody] CreatePlayerDTO player)
    {
        await repo.CreatePlayerAsync(player);
    }

    [HttpGet("getPlayer")]
    public async Task<List<Player>> GetAll()
    {
        return await repo.GetPlayers();
    }

    [HttpGet("getTodayTries")]
    public async Task<int> GetTodayTries()
    {
        return await repo.AllTriesToday();
    }
    
    [HttpGet("getTodayBillboard")]
    public async Task<List<Player>> GetTodayBillboard()
    {
        return await repo.GetTodaysBillboard();
    }
    
    
}