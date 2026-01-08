namespace CvQuestionGenerator.API.Models.Responses;

/// <summary>
/// Response when retrieving a stored CV.
/// </summary>
public sealed record CvResponse
{
    /// <summary>Unique identifier for the CV.</summary>
    public required Guid Id { get; init; }
    
    /// <summary>Original unformatted CV text.</summary>
    public required string OriginalText { get; init; }
    
    /// <summary>Result of AI extraction.</summary>
    public required CvExtractionResult Extraction { get; init; }
    
    /// <summary>When the CV was processed.</summary>
    public required DateTimeOffset ExtractedAt { get; init; }
}
