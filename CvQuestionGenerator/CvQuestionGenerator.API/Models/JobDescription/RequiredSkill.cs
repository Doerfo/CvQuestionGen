using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CvQuestionGenerator.API.Models.JobDescription;

/// <summary>
/// A required skill extracted from a job description.
/// </summary>
public class RequiredSkill
{
    /// <summary>
    /// Name of the required skill.
    /// </summary>
    [Required]
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Required proficiency level for this skill.
    /// </summary>
    [JsonPropertyName("requiredLevel")]
    public ProficiencyLevel RequiredLevel { get; set; }
}
