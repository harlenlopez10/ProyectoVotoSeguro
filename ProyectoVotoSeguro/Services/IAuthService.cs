using ProyectoVotoSeguro.DTOs;
using ProyectoVotoSeguro.Models;

namespace ProyectoVotoSeguro.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Register(RegisterDto registerdto);
        Task<AuthResponseDto> Login(LoginDto logindto);
        Task<User?> GetUserById(string userId);
        Task<User?> GetUserByEmail(string email);
        string GenerateJwtToken(User user);
    }
}

