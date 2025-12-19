using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VotoSeguro.API.DTOs;
using VotoSeguro.API.Services;

namespace VotoSeguro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CandidatesController : ControllerBase
{
    private readonly ICandidateService _candidateService;

    public CandidatesController(ICandidateService candidateService)
    {
        _candidateService = candidateService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var candidates = await _candidateService.GetAllCandidates();
            return Ok(candidates);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener candidatos", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var candidate = await _candidateService.GetCandidateById(id);
            if (candidate == null)
            {
                return NotFound(new { message = "Candidato no encontrado" });
            }
            return Ok(candidate);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener candidato", error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] CreateCandidateDto createDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var candidate = await _candidateService.CreateCandidate(createDto, userId);
            return CreatedAtAction(nameof(GetById), new { id = candidate.Id }, candidate);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al crear candidato", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateCandidateDto updateDto)
    {
        try
        {
            var candidate = await _candidateService.UpdateCandidate(id, updateDto);
            return Ok(candidate);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al actualizar candidato", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _candidateService.DeleteCandidate(id);
            return Ok(new { message = "Candidato eliminado exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al eliminar candidato", error = ex.Message });
        }
    }
}
