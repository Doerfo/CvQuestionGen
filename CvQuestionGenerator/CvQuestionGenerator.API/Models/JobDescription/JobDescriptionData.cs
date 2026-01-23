using System.Text.Json.Serialization;

namespace CvQuestionGenerator.API.Models.JobDescription;

/// <summary>
/// Complete job description data including raw text and AI-extracted structured data.
/// </summary>
public class JobDescriptionData
{
    /// <summary>
    /// Original unformatted job description text.
    /// </summary>
    [JsonPropertyName("rawText")]
    public required string RawText { get; set; }

    /// <summary>
    /// AI-extracted structured data from the job description.
    /// </summary>
    [JsonPropertyName("extractedData")]
    public JobDescExtractedData ExtractedData { get; set; } = new();

    /// <summary>
    /// Timestamp when the job description was submitted.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
