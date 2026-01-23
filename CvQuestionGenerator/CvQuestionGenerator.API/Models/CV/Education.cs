using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CvQuestionGenerator.API.Models.CV;

/// <summary>
/// Education entry extracted from a CV.
/// </summary>
public class Education
{
    /// <summary>
    /// Educational institution name.
    /// </summary>
    [Required]
    [JsonPropertyName("institution")]
    public required string Institution { get; set; }

    /// <summary>
    /// Degree obtained or pursued.
    /// </summary>
    [JsonPropertyName("degree")]
    public string? Degree { get; set; }

    /// <summary>
    /// Year of graduation or attendance.
    /// </summary>
    [JsonPropertyName("year")]
    public string? Year { get; set; }
}
