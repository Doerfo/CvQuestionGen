namespace CvQuestionGenerator.API.Models.Responses;

/// <summary>
/// Response from uploading and processing a job description.
/// </summary>
public sealed record JobUploadResponse
{
    /// <summary>Unique identifier for the stored job description.</summary>
    public required Guid Id { get; init; }
    
    /// <summary>Confirmation message.</summary>
    public required string Message { get; init; }
    
    /// <summary>Result of AI extraction.</summary>
    public required JobExtractionResult Extraction { get; init; }
}
