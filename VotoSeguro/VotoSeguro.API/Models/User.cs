using Google.Cloud.Firestore;

namespace VotoSeguro.API.Models;

[FirestoreData]
public class User
{
    [FirestoreProperty("id")]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty("email")]
    public string Email { get; set; } = string.Empty;

    [FirestoreProperty("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [FirestoreProperty("fullName")]
    public string FullName { get; set; } = string.Empty;

    [FirestoreProperty("role")]
    public string Role { get; set; } = "votante"; // "administrador" o "votante"

    [FirestoreProperty("hasVoted")]
    public bool HasVoted { get; set; } = false;

    [FirestoreProperty("votedFor")]
    public string? VotedFor { get; set; } // ID del candidato

    [FirestoreProperty("votedForName")]
    public string? VotedForName { get; set; } // Nombre del candidato

    [FirestoreProperty("voteTimestamp")]
    public DateTime? VoteTimestamp { get; set; }

    [FirestoreProperty("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
