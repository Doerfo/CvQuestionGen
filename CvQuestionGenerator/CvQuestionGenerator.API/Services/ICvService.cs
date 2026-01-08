using CvQuestionGenerator.API.Models;
using CvQuestionGenerator.API.Models.Responses;

namespace CvQuestionGenerator.API.Services;

/// <summary>
/// Service for CV management operations.
/// </summary>
public interface ICvService
{
    /// <summary>
    /// Uploads and processes a CV, storing the result.
    /// </summary>
    /// <param name="cvText">The raw CV text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The upload response with extraction results.</returns>
    Task<CvUploadResponse> UploadCvAsync(string cvText, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the currently stored CV.
    /// </summary>
    /// <returns>The stored CV, or null if none exists.</returns>
    Cv? GetCv();
}
