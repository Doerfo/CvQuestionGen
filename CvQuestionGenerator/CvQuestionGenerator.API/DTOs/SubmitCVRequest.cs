using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CvQuestionGenerator.API.DTOs;

/// <summary>
/// Request DTO for submitting a CV.
/// </summary>
public class SubmitCVRequest
{
    /// <summary>
    /// The raw CV text to process.
    /// </summary>
    [Required(ErrorMessage = "CV text is required")]
    [MinLength(1, ErrorMessage = "CV text cannot be empty")]
    [JsonPropertyName("cvText")]
    public required string CvText { get; set; }
}
