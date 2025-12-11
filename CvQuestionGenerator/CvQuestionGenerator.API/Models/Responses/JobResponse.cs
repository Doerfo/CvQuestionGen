namespace CvQuestionGenerator.API.Models.Responses;

/// <summary>
/// Response when retrieving a stored job description.
/// </summary>
public sealed record JobResponse
{
    /// <summary>Unique identifier for the job description.</summary>
    public required Guid Id { get; init; }
    
    /// <summary>Original unformatted job description text.</summary>
    public required string OriginalText { get; init; }
    
    /// <summary>Result of AI extraction.</summary>
    public required JobExtractionResult Extraction { get; init; }
    
    /// <summary>When the job description was processed.</summary>
    public required DateTimeOffset ExtractedAt { get; init; }
}
