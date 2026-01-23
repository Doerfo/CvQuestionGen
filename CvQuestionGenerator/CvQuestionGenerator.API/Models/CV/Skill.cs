using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CvQuestionGenerator.API.Models.CV;

/// <summary>
/// A skill extracted from a CV with proficiency assessment.
/// </summary>
public class Skill
{
    /// <summary>
    /// Name of the skill.
    /// </summary>
    [Required]
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Proficiency level based on experience duration and context.
    /// </summary>
    [JsonPropertyName("proficiencyLevel")]
    public ProficiencyLevel ProficiencyLevel { get; set; }

    /// <summary>
    /// Context for the experience (e.g., "3 years", "touched briefly").
    /// </summary>
    [JsonPropertyName("experienceContext")]
    public string? ExperienceContext { get; set; }
}
