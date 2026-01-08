namespace CvQuestionGenerator.API.Models;

/// <summary>
/// Work experience entry from a CV.
/// </summary>
public sealed record WorkExperience
{
    /// <summary>Job title.</summary>
    public required string JobTitle { get; init; }
    
    /// <summary>Company name.</summary>
    public required string Company { get; init; }
    
    /// <summary>Duration of employment (e.g., "2020-Present").</summary>
    public string? Duration { get; init; }
    
    /// <summary>List of responsibilities and achievements.</summary>
    public IReadOnlyList<string> Responsibilities { get; init; } = [];
}
