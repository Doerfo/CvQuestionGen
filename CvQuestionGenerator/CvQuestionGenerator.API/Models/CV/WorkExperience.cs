using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CvQuestionGenerator.API.Models.CV;

/// <summary>
/// Work experience entry extracted from a CV.
/// </summary>
public class WorkExperience
{
    /// <summary>
    /// Company name.
    /// </summary>
    [Required]
    [JsonPropertyName("company")]
    public required string Company { get; set; }

    /// <summary>
    /// Job position/title.
    /// </summary>
    [Required]
    [JsonPropertyName("position")]
    public required string Position { get; set; }

    /// <summary>
    /// Duration of employment (e.g., "2 years", "Jan 2020 - Dec 2022").
    /// </summary>
    [JsonPropertyName("duration")]
    public string? Duration { get; set; }

    /// <summary>
    /// Description of responsibilities and achievements.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
