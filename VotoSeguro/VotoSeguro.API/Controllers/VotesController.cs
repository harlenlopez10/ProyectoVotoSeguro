using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VotoSeguro.API.DTOs;
using VotoSeguro.API.Services;

namespace VotoSeguro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "votante")]
public class VotesController : ControllerBase
{
    private readonly IVoteService _voteService;

    public VotesController(IVoteService voteService)
    {
        _voteService = voteService;
    }

    [HttpPost]
    public async Task<IActionResult> CastVote([FromBody] CastVoteDto voteDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            // Verificar que no haya votado
            if (await _voteService.HasUserVoted(userId))
            {
                return BadRequest(new { message = "Ya has votado anteriormente" });
            }

            var result = await _voteService.CastVote(userId, voteDto.CandidateId);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al emitir voto", error = ex.Message });
        }
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetVoteStatus()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var user = await _voteService.GetVoteStatus(userId);
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(new
            {
                hasVoted = user.HasVoted,
                votedFor = user.VotedFor,
                votedForName = user.VotedForName,
                voteTimestamp = user.VoteTimestamp
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener estado", error = ex.Message });
        }
    }
}
