using System.Text.Json.Serialization;

namespace VotoSeguro.API.Models;

public class VoteStatistics
{
    [JsonPropertyName("totalVoters")]
    public int TotalVoters { get; set; }

    [JsonPropertyName("totalVotes")]
    public int TotalVotes { get; set; }

    [JsonPropertyName("participationPercentage")]
    public double ParticipationPercentage { get; set; }

    [JsonPropertyName("candidateResults")]
    public List<CandidateResult> CandidateResults { get; set; } = new();

    [JsonPropertyName("trends")]
    public List<VoteTrend> Trends { get; set; } = new();
}

public class CandidateResult
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("party")]
    public string Party { get; set; } = string.Empty;

    [JsonPropertyName("photoUrl")]
    public string PhotoUrl { get; set; } = string.Empty;

    [JsonPropertyName("logoUrl")]
    public string LogoUrl { get; set; } = string.Empty;

    [JsonPropertyName("votes")]
    public int Votes { get; set; }

    [JsonPropertyName("percentage")]
    public double Percentage { get; set; }
}

public class VoteTrend
{
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("candidateName")]
    public string CandidateName { get; set; } = string.Empty;

    [JsonPropertyName("cumulativeVotes")]
    public int CumulativeVotes { get; set; }
}
