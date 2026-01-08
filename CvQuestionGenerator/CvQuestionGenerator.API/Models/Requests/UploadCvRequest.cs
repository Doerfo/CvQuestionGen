using System.ComponentModel.DataAnnotations;

namespace CvQuestionGenerator.API.Models.Requests;

/// <summary>
/// Request to upload and process CV text.
/// </summary>
public sealed record UploadCvRequest
{
    /// <summary>
    /// Unformatted CV text to process.
    /// </summary>
    [Required]
    [StringLength(50000, MinimumLength = 1, ErrorMessage = "Text must be between 1 and 50,000 characters.")]
    public required string Text { get; init; }
}
