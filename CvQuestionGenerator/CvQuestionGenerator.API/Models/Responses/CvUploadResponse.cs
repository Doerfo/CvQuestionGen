namespace CvQuestionGenerator.API.Models.Responses;

/// <summary>
/// Response from uploading and processing a CV.
/// </summary>
public sealed record CvUploadResponse
{
    /// <summary>Unique identifier for the stored CV.</summary>
    public required Guid Id { get; init; }
    
    /// <summary>Confirmation message.</summary>
    public required string Message { get; init; }
    
    /// <summary>Result of AI extraction.</summary>
    public required CvExtractionResult Extraction { get; init; }
}
