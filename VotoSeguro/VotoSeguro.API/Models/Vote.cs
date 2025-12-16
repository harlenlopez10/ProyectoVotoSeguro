using Google.Cloud.Firestore;

namespace VotoSeguro.API.Models;

[FirestoreData]
public class Vote
{
    [FirestoreProperty("id")]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty("userId")]
    public string UserId { get; set; } = string.Empty;

    [FirestoreProperty("userName")]
    public string UserName { get; set; } = string.Empty;

    [FirestoreProperty("candidateId")]
    public string CandidateId { get; set; } = string.Empty;

    [FirestoreProperty("candidateName")]
    public string CandidateName { get; set; } = string.Empty;

    [FirestoreProperty("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
