namespace CvQuestionGenerator.API.Models;

/// <summary>
/// Represents a candidate's CV with AI-extracted structured data.
/// </summary>
public sealed record Cv
{
    /// <summary>Unique identifier for this CV.</summary>
    public required Guid Id { get; init; }
    
    /// <summary>Original unformatted CV text.</summary>
    public required string OriginalText { get; init; }
    
    /// <summary>Extracted personal information.</summary>
    public required PersonalInfo PersonalInfo { get; init; }
    
    /// <summary>Extracted skills with proficiency levels.</summary>
    public required IReadOnlyList<Skill> Skills { get; init; }
    
    /// <summary>Extracted work experience.</summary>
    public required IReadOnlyList<WorkExperience> Experience { get; init; }
    
    /// <summary>Extracted education history.</summary>
    public required IReadOnlyList<Education> Education { get; init; }
    
    /// <summary>When the CV was processed and data extracted.</summary>
    public required DateTimeOffset ExtractedAt { get; init; }
}
