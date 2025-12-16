using Google.Cloud.Firestore;

namespace VotoSeguro.API.Models;

[FirestoreData]
public class Candidate
{
    [FirestoreProperty("id")]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty("name")]
    public string Name { get; set; } = string.Empty;

    [FirestoreProperty("party")]
    public string Party { get; set; } = string.Empty;

    [FirestoreProperty("photoUrl")]
    public string PhotoUrl { get; set; } = string.Empty;

    [FirestoreProperty("logoUrl")]
    public string LogoUrl { get; set; } = string.Empty;

    [FirestoreProperty("proposals")]
    public List<string> Proposals { get; set; } = new();

    [FirestoreProperty("voteCount")]
    public int VoteCount { get; set; } = 0;

    [FirestoreProperty("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [FirestoreProperty("createdBy")]
    public string CreatedBy { get; set; } = string.Empty; // ID del admin que lo creó
}
