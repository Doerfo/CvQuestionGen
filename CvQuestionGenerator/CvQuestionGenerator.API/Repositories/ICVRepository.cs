using CvQuestionGenerator.API.Models.CV;

namespace CvQuestionGenerator.API.Repositories;

/// <summary>
/// Interface for CV storage operations.
/// </summary>
public interface ICVRepository
{
    /// <summary>
    /// Store a CV, replacing any existing CV.
    /// </summary>
    /// <param name="cvData">The CV data to store.</param>
    void Set(CVData cvData);

    /// <summary>
    /// Get the currently stored CV.
    /// </summary>
    /// <returns>The stored CV data, or null if none exists.</returns>
    CVData? Get();

    /// <summary>
    /// Check if a CV has been stored.
    /// </summary>
    /// <returns>True if a CV exists, false otherwise.</returns>
    bool Exists();
}
