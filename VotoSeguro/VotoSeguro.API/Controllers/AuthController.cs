using Microsoft.AspNetCore.Mvc;
using VotoSeguro.API.DTOs;
using VotoSeguro.API.Services;

namespace VotoSeguro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(registerDto.Email) ||
                string.IsNullOrWhiteSpace(registerDto.Password) ||
                string.IsNullOrWhiteSpace(registerDto.FullName))
            {
                return BadRequest(new { message = "Todos los campos son requeridos" });
            }

            var response = await _authService.Register(registerDto);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al registrar usuario", error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest(new { message = "Email y contraseña son requeridos" });
            }

            var response = await _authService.Login(loginDto);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al iniciar sesión", error = ex.Message });
        }
    }
}
