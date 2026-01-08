namespace CvQuestionGenerator.API.Models;

/// <summary>
/// Education entry from a CV.
/// </summary>
public sealed record Education
{
    /// <summary>Degree or certification name.</summary>
    public required string Degree { get; init; }
    
    /// <summary>Institution name.</summary>
    public required string Institution { get; init; }
    
    /// <summary>Year of graduation or completion.</summary>
    public string? Year { get; init; }
}
