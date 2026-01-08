using CvQuestionGenerator.API.Models;

namespace CvQuestionGenerator.API.Storage;

/// <summary>
/// Interface for in-memory storage of CV and Job Description data.
/// </summary>
public interface IDataStore
{
    /// <summary>
    /// Stores a CV, replacing any existing CV.
    /// </summary>
    /// <param name="cv">The CV to store.</param>
    void StoreCv(Cv cv);

    /// <summary>
    /// Gets the currently stored CV.
    /// </summary>
    /// <returns>The stored CV, or null if none exists.</returns>
    Cv? GetCv();

    /// <summary>
    /// Stores a job description, replacing any existing job description.
    /// </summary>
    /// <param name="job">The job description to store.</param>
    void StoreJob(JobDescription job);

    /// <summary>
    /// Gets the currently stored job description.
    /// </summary>
    /// <returns>The stored job description, or null if none exists.</returns>
    JobDescription? GetJob();

    /// <summary>
    /// Clears all stored data.
    /// </summary>
    void Clear();
}
