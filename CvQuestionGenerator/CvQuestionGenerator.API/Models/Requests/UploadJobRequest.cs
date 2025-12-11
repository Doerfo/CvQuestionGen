using System.ComponentModel.DataAnnotations;

namespace CvQuestionGenerator.API.Models.Requests;

/// <summary>
/// Request to upload and process job description text.
/// </summary>
public sealed record UploadJobRequest
{
    /// <summary>
    /// Unformatted job description text to process.
    /// </summary>
    [Required]
    [StringLength(50000, MinimumLength = 1, ErrorMessage = "Text must be between 1 and 50,000 characters.")]
    public required string Text { get; init; }
}
