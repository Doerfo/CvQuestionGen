using CvQuestionGenerator.API.Models.Responses;

namespace CvQuestionGenerator.API.Services;

/// <summary>
/// Service for AI-powered extraction of structured data from text.
/// </summary>
public interface IAiExtractionService
{
    /// <summary>
    /// Extracts structured CV data from unformatted text.
    /// </summary>
    /// <param name="cvText">The raw CV text to process.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Extracted CV data.</returns>
    Task<CvExtractionResult> ExtractCvDataAsync(string cvText, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts structured job description data from unformatted text.
    /// </summary>
    /// <param name="jobText">The raw job description text to process.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Extracted job data.</returns>
    Task<JobExtractionResult> ExtractJobDataAsync(string jobText, CancellationToken cancellationToken = default);
}
