using CvQuestionGenerator.API.Models.CV;

namespace CvQuestionGenerator.API.Services;

/// <summary>
/// Interface for CV management operations.
/// </summary>
public interface ICVService
{
    /// <summary>
    /// Submit a CV for processing. Replaces any existing CV.
    /// </summary>
    /// <param name="cvText">The raw CV text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SubmitCVAsync(string cvText, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the currently stored CV with extracted data.
    /// </summary>
    /// <returns>The CV data, or null if none exists.</returns>
    CVData? GetCV();
}
