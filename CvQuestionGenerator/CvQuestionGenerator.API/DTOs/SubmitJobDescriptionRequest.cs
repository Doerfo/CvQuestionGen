using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CvQuestionGenerator.API.DTOs;

/// <summary>
/// Request DTO for submitting a job description.
/// </summary>
public class SubmitJobDescriptionRequest
{
    /// <summary>
    /// The raw job description text to process.
    /// </summary>
    [Required(ErrorMessage = "Job description text is required")]
    [MinLength(1, ErrorMessage = "Job description text cannot be empty")]
    [JsonPropertyName("jobDescriptionText")]
    public required string JobDescriptionText { get; set; }
}
