namespace CvQuestionGenerator.API.Models;

/// <summary>
/// Experience requirement from a job description.
/// </summary>
public sealed record ExperienceRequirement
{
    /// <summary>Area of expertise (e.g., "Java", "Cloud Architecture").</summary>
    public required string Area { get; init; }
    
    /// <summary>Minimum proficiency level required.</summary>
    public required ProficiencyLevel MinimumLevel { get; init; }
    
    /// <summary>Number of years required, if specified.</summary>
    public int? YearsRequired { get; init; }
}
