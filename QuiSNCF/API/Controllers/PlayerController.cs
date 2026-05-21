using System.Net.Sockets;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using QuiSNCF.DTO;
using QuiSNCF.Models;
using QuiSNCF.Repository;

namespace QuiSNCF.API.Controllers;

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
            

        if (!await repo.DoesPlayerExist(player.Name))
            await repo.CreatePlayerAsync(player);
        else
            await repo.UpdatePlayerScore(player.Name, player.Tries);

        return Ok();
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

    [HttpGet("getBillboard")]
    public async Task<List<Player>> GetBillboard()
    {
        return await repo.GetBillboard();
    }


}