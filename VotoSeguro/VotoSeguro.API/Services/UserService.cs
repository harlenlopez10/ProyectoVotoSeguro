using Google.Cloud.Firestore;
using VotoSeguro.API.DTOs;
using VotoSeguro.API.Models;

namespace VotoSeguro.API.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllVoters();
    Task<UserDto?> GetUserById(string userId);
}

public class UserService : IUserService
{
    private readonly FirebaseService _firebaseService;

    public UserService(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    public async Task<List<UserDto>> GetAllVoters()
    {
        var snapshot = await _firebaseService.UsersCollection.GetSnapshotAsync();
        var users = new List<UserDto>();

        foreach (var doc in snapshot.Documents)
        {
            var user = doc.ConvertTo<User>();
            users.Add(MapToDto(user));
        }

        return users.OrderBy(u => u.FullName).ToList();
    }

    public async Task<UserDto?> GetUserById(string userId)
    {
        var doc = await _firebaseService.UsersCollection.Document(userId).GetSnapshotAsync();
        if (!doc.Exists)
        {
            return null;
        }

        var user = doc.ConvertTo<User>();
        return MapToDto(user);
    }

    private UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            HasVoted = user.HasVoted,
            VotedFor = user.VotedFor,
            VotedForName = user.VotedForName,
            VoteTimestamp = user.VoteTimestamp,
            CreatedAt = user.CreatedAt
        };
    }
}
