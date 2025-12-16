namespace VotoSeguro.API.DTOs;

public class CastVoteDto
{
    public string CandidateId { get; set; } = string.Empty;
}

public class VoteResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? CandidateName { get; set; }
    public DateTime? Timestamp { get; set; }
}
