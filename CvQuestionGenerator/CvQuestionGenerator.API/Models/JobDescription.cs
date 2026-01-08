namespace CvQuestionGenerator.API.Models;

/// <summary>
/// Represents a job description with AI-extracted requirements.
/// </summary>
public sealed record JobDescription
{
    /// <summary>Unique identifier for this job description.</summary>
    public required Guid Id { get; init; }
    
    /// <summary>Original unformatted job description text.</summary>
    public required string OriginalText { get; init; }
    
    /// <summary>Extracted required skills.</summary>
    public required IReadOnlyList<Skill> RequiredSkills { get; init; }
    
    /// <summary>Key competencies to assess.</summary>
    public required IReadOnlyList<string> Competencies { get; init; }
    
    /// <summary>Experience requirements.</summary>
    public required IReadOnlyList<ExperienceRequirement> ExperienceRequirements { get; init; }
    
    /// <summary>When the job description was processed and data extracted.</summary>
    public required DateTimeOffset ExtractedAt { get; init; }
}
