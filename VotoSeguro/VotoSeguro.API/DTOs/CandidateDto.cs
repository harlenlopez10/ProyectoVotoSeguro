namespace VotoSeguro.API.DTOs;

public class CreateCandidateDto
{
    public string Name { get; set; } = string.Empty;
    public string Party { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? PhotoBase64 { get; set; }
    public string? LogoBase64 { get; set; }
    public string? ImageUrl { get; set; } // Map frontend imageUrl if provided directly
    public List<string> Proposals { get; set; } = new();
}

public class UpdateCandidateDto
{
    public string Name { get; set; } = string.Empty;
    public string Party { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? PhotoBase64 { get; set; }
    public string? LogoBase64 { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> Proposals { get; set; } = new();
}

public class CandidateDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Party { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public List<string> Proposals { get; set; } = new();
    public int VoteCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
