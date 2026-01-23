using System.Text.Json.Serialization;

namespace CvQuestionGenerator.API.Models.JobDescription;

/// <summary>
/// Structured data extracted from a job description by AI.
/// </summary>
public class JobDescExtractedData
{
    /// <summary>
    /// Skills required for the position with proficiency levels.
    /// </summary>
    [JsonPropertyName("requiredSkills")]
    public List<RequiredSkill> RequiredSkills { get; set; } = [];

    /// <summary>
    /// Overall experience level requirement (e.g., "3-5 years").
    /// </summary>
    [JsonPropertyName("experienceLevel")]
    public string? ExperienceLevel { get; set; }

    /// <summary>
    /// Key competencies to assess during interviews.
    /// </summary>
    [JsonPropertyName("keyCompetencies")]
    public List<string> KeyCompetencies { get; set; } = [];
}
