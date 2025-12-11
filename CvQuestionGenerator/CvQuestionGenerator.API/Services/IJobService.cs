using CvQuestionGenerator.API.Models;
using CvQuestionGenerator.API.Models.Responses;

namespace CvQuestionGenerator.API.Services;

/// <summary>
/// Service for job description management operations.
/// </summary>
public interface IJobService
{
    /// <summary>
    /// Uploads and processes a job description, storing the result.
    /// </summary>
    /// <param name="jobText">The raw job description text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The upload response with extraction results.</returns>
    Task<JobUploadResponse> UploadJobAsync(string jobText, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the currently stored job description.
    /// </summary>
    /// <returns>The stored job description, or null if none exists.</returns>
    JobDescription? GetJob();
}
