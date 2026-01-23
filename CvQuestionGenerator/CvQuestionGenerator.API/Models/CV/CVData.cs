using System.Text.Json.Serialization;

namespace CvQuestionGenerator.API.Models.CV;

/// <summary>
/// Complete CV data including raw text and AI-extracted structured data.
/// </summary>
public class CVData
{
    /// <summary>
    /// Original unformatted CV text.
    /// </summary>
    [JsonPropertyName("rawText")]
    public required string RawText { get; set; }

    /// <summary>
    /// AI-extracted structured data from the CV.
    /// </summary>
    [JsonPropertyName("extractedData")]
    public CVExtractedData ExtractedData { get; set; } = new();

    /// <summary>
    /// Timestamp when the CV was submitted.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
