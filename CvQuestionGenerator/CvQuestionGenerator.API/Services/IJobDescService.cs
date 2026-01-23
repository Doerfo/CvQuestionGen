using CvQuestionGenerator.API.Models.JobDescription;

namespace CvQuestionGenerator.API.Services;

/// <summary>
/// Interface for job description management operations.
/// </summary>
public interface IJobDescService
{
    /// <summary>
    /// Submit a job description for processing. Replaces any existing job description.
    /// </summary>
    /// <param name="jobDescText">The raw job description text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SubmitJobDescriptionAsync(string jobDescText, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the currently stored job description with extracted data.
    /// </summary>
    /// <returns>The job description data, or null if none exists.</returns>
    JobDescriptionData? GetJobDescription();
}
