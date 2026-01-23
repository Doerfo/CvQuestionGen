using CvQuestionGenerator.API.Models.JobDescription;

namespace CvQuestionGenerator.API.Repositories;

/// <summary>
/// Interface for job description storage operations.
/// </summary>
public interface IJobDescRepository
{
    /// <summary>
    /// Store a job description, replacing any existing one.
    /// </summary>
    /// <param name="jobDescData">The job description data to store.</param>
    void Set(JobDescriptionData jobDescData);

    /// <summary>
    /// Get the currently stored job description.
    /// </summary>
    /// <returns>The stored job description data, or null if none exists.</returns>
    JobDescriptionData? Get();

    /// <summary>
    /// Check if a job description has been stored.
    /// </summary>
    /// <returns>True if a job description exists, false otherwise.</returns>
    bool Exists();
}
