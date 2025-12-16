using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotoSeguro.API.Services;

namespace VotoSeguro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "administrador")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVoters()
    {
        try
        {
            var voters = await _userService.GetAllVoters();
            return Ok(voters);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener votantes", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener usuario", error = ex.Message });
        }
    }
}
