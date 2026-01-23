using System.Text.Json.Serialization;

namespace CvQuestionGenerator.API.DTOs;

/// <summary>
/// Standard error response DTO.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Error message describing what went wrong.
    /// </summary>
    [JsonPropertyName("error")]
    public required string Error { get; set; }
}
