namespace CvQuestionGenerator.API.Models.Responses;

/// <summary>
/// Result of AI extraction from a job description.
/// </summary>
public sealed record JobExtractionResult
{
    /// <summary>Extracted required skills.</summary>
    public required IReadOnlyList<Skill> RequiredSkills { get; init; }
    
    /// <summary>Key competencies to assess.</summary>
    public required IReadOnlyList<string> Competencies { get; init; }
    
    /// <summary>Experience requirements.</summary>
    public required IReadOnlyList<ExperienceRequirement> ExperienceRequirements { get; init; }
    
    /// <summary>Any warnings from extraction.</summary>
    public IReadOnlyList<string>? Warnings { get; init; }
}
