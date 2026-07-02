using System.Net.Sockets;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using QuiSNCF.DTO;
using QuiSNCF.Models;
using QuiSNCF.Repository;

namespace QuiSNCF.API.Controllers;

[EnableRateLimiting("fixed")]
[Route("api/[controller]")]
[ApiController]
public class PlayerController(IPlayerRepository repo, ILogger<IPlayerRepository> logger) : ControllerBase
{
    [HttpPost("createPlayer")]
    public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerDTO player)
    {
        var regex = new Regex(@"^[a-zA-Z0-9_\-éèêëàâùûüîïôçœæ ]+$");
        if (string.IsNullOrWhiteSpace(player.Name) || player.Name.Length > 20 || !regex.IsMatch(player.Name))
        {
            logger.LogError($"Pseudo bizarre: {player.Name}");
            return BadRequest("Pseudo invalide");
        }

        if (player.Tries < 0 || player.Tries > 50)
        {
            logger.ErrorColored($"Il a essayé de me couiller lui : {player.Name}, essai : {player.Tries} :");
            return BadRequest("Nombre d'essais invalide");
        }
            
        int finalScore = await repo.SavePlayAsync(player, player.GameType);

        return Ok(finalScore);
    }

    /*
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
    */

    [HttpGet("getTodayBillboard/{gameType}/{season}")]
    public async Task<List<PlayerScoreDTO>> GetTodayBillboard(GameType gameType, int season)
    {
        return await repo.GetBillboardByGame(gameType, season);
    }

    [HttpGet("getBillboard/{season}")]
    public async Task<List<Player>> GetBillboard(int season)
    {
        return await repo.GetBillboard(season);
    }

    [HttpGet("getByName/{name}/{gameType}")]
    public async Task<List<GetPlayerScoreDTO>> GetBillboardByName(string name, GameType gameType)
    {
        return await repo.GetScoreByGameAndPlayer(name, gameType);
    }
    [HttpGet("count")]
    public async Task<int> GetPlayersCount([FromQuery] GameType? gameType)
    {
        return repo.GetPlayersCount(gameType);
    }

    [HttpGet("best/{season}")]
    public IActionResult GetBestPlayerBySeason(int season)
    {
        return Ok(repo.GetBestPlayerBySeason(season));
    }

    [HttpGet("total/{name}")]
    public async Task<int> GetTotalScoreByPlayerAndGameType(string name, [FromQuery] GameType? gameType)
    {
        return await repo.GetTotalScoreByPlayerAndGameType(name, gameType);
    }
    

}