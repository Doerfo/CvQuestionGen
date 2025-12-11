namespace CvQuestionGenerator.API.Models.Responses;

/// <summary>
/// Result of AI extraction from a CV.
/// </summary>
public sealed record CvExtractionResult
{
    /// <summary>Extracted personal information.</summary>
    public required PersonalInfo PersonalInfo { get; init; }
    
    /// <summary>Extracted skills with proficiency levels.</summary>
    public required IReadOnlyList<Skill> Skills { get; init; }
    
    /// <summary>Extracted work experience.</summary>
    public required IReadOnlyList<WorkExperience> Experience { get; init; }
    
    /// <summary>Extracted education history.</summary>
    public required IReadOnlyList<Education> Education { get; init; }
    
    /// <summary>Any warnings from extraction (e.g., no skills found).</summary>
    public IReadOnlyList<string>? Warnings { get; init; }
}
