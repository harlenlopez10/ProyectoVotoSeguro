using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;
using VotoSeguro.API.DTOs;
using VotoSeguro.API.Models;
using BCrypt.Net;

namespace VotoSeguro.API.Services;

public interface IAuthService
{
    Task<AuthResponseDto> Register(RegisterDto registerDto);
    Task<AuthResponseDto> Login(LoginDto loginDto);
    Task<User?> GetUserById(string userId);
}

public class AuthService : IAuthService
{
    private readonly FirebaseService _firebaseService;
    private readonly IConfiguration _configuration;

    public AuthService(FirebaseService firebaseService, IConfiguration configuration)
    {
        _firebaseService = firebaseService;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> Register(RegisterDto registerDto)
    {
        // Verificar si el email ya existe
        var existingUsers = await _firebaseService.UsersCollection
            .WhereEqualTo("email", registerDto.Email)
            .GetSnapshotAsync();

        if (existingUsers.Count > 0)
        {
            throw new InvalidOperationException("El email ya está registrado");
        }

        // Crear el usuario
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            FullName = registerDto.FullName,
            Role = "voter", // Por defecto es voter
            HasVoted = false,
            CreatedAt = DateTime.UtcNow
        };

        // Guardar en Firestore
        await _firebaseService.UsersCollection.Document(user.Id).SetAsync(user);

        // Generar token
        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            HasVoted = user.HasVoted
        };
    }

    public async Task<AuthResponseDto> Login(LoginDto loginDto)
    {
        // Buscar usuario por email
        var snapshot = await _firebaseService.UsersCollection
            .WhereEqualTo("email", loginDto.Email)
            .GetSnapshotAsync();

        if (snapshot.Count == 0)
        {
            throw new InvalidOperationException("Credenciales inválidas");
        }

        var userDoc = snapshot.Documents[0];
        var user = userDoc.ConvertTo<User>();

        // Normalizar roles para compatibilidad
        if (user.Role == "administrador") user.Role = "admin";
        if (user.Role == "votante") user.Role = "voter";

        // Verificar contraseña
        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new InvalidOperationException("Credenciales inválidas");
        }

        // Generar token
        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            HasVoted = user.HasVoted,
            VotedFor = user.VotedFor,
            VotedForName = user.VotedForName,
            VoteTimestamp = user.VoteTimestamp
        };
    }

    public async Task<User?> GetUserById(string userId)
    {
        var doc = await _firebaseService.UsersCollection.Document(userId).GetSnapshotAsync();
        if (!doc.Exists)
        {
            return null;
        }
        return doc.ConvertTo<User>();
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Key no configurada");
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("role", user.Role), // Duplicate for easier frontend decoding
            new Claim("hasVoted", user.HasVoted.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
