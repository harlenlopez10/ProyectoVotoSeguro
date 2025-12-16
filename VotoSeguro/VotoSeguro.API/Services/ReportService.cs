using Google.Cloud.Firestore;
using VotoSeguro.API.Models;

namespace VotoSeguro.API.Services;

public interface IReportService
{
    Task<VoteStatistics> GetStatistics();
}

public class ReportService : IReportService
{
    private readonly FirebaseService _firebaseService;

    public ReportService(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    public async Task<VoteStatistics> GetStatistics()
    {
        // Obtener todos los usuarios
        var usersSnapshot = await _firebaseService.UsersCollection.GetSnapshotAsync();
        var users = usersSnapshot.Documents.Select(d => d.ConvertTo<User>()).ToList();

        // Obtener todos los candidatos
        var candidatesSnapshot = await _firebaseService.CandidatesCollection.GetSnapshotAsync();
        var candidates = candidatesSnapshot.Documents.Select(d => d.ConvertTo<Candidate>()).ToList();

        // Obtener todos los votos para tendencias
        var votesSnapshot = await _firebaseService.VotesCollection.OrderBy("timestamp").GetSnapshotAsync();
        var votes = votesSnapshot.Documents.Select(d => d.ConvertTo<Vote>()).ToList();

        var totalVoters = users.Count(u => u.Role == "votante");
        var totalVotes = users.Count(u => u.HasVoted);
        var participationPercentage = totalVoters > 0 ? (double)totalVotes / totalVoters * 100 : 0;

        // Resultados por candidato
        var candidateResults = candidates.Select(c => new CandidateResult
        {
            Id = c.Id,
            Name = c.Name,
            Party = c.Party,
            PhotoUrl = c.PhotoUrl,
            LogoUrl = c.LogoUrl,
            Votes = c.VoteCount,
            Percentage = totalVotes > 0 ? (double)c.VoteCount / totalVotes * 100 : 0
        }).OrderByDescending(c => c.Votes).ToList();

        // Calcular tendencias temporales
        var trends = CalculateTrends(votes, candidates);

        return new VoteStatistics
        {
            TotalVoters = totalVoters,
            TotalVotes = totalVotes,
            ParticipationPercentage = Math.Round(participationPercentage, 2),
            CandidateResults = candidateResults,
            Trends = trends
        };
    }

    private List<VoteTrend> CalculateTrends(List<Vote> votes, List<Candidate> candidates)
    {
        var trends = new List<VoteTrend>();

        // Agrupar votos por candidato
        var votesByCandida = votes.GroupBy(v => v.CandidateId);

        foreach (var group in votesByCandida)
        {
            var candidate = candidates.FirstOrDefault(c => c.Id == group.Key);
            if (candidate == null) continue;

            var orderedVotes = group.OrderBy(v => v.Timestamp).ToList();
            int cumulativeCount = 0;

            foreach (var vote in orderedVotes)
            {
                cumulativeCount++;
                trends.Add(new VoteTrend
                {
                    Timestamp = vote.Timestamp,
                    CandidateName = candidate.Name,
                    CumulativeVotes = cumulativeCount
                });
            }
        }

        return trends.OrderBy(t => t.Timestamp).ToList();
    }
}
