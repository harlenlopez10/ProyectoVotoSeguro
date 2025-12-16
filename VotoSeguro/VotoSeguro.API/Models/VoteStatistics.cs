namespace VotoSeguro.API.Models;

public class VoteStatistics
{
    public int TotalVoters { get; set; }
    public int TotalVotes { get; set; }
    public double ParticipationPercentage { get; set; }
    public List<CandidateResult> CandidateResults { get; set; } = new();
    public List<VoteTrend> Trends { get; set; } = new();
}

public class CandidateResult
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Party { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public int Votes { get; set; }
    public double Percentage { get; set; }
}

public class VoteTrend
{
    public DateTime Timestamp { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public int CumulativeVotes { get; set; }
}
