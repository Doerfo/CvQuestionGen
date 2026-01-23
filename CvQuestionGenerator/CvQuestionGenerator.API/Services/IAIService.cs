using CvQuestionGenerator.API.Models.CV;
using CvQuestionGenerator.API.Models.JobDescription;
using CvQuestionGenerator.API.Models.Questions;

namespace CvQuestionGenerator.API.Services;

/// <summary>
/// Interface for AI-powered data extraction and question generation.
/// </summary>
public interface IAIService
{
    /// <summary>
    /// Extract structured data from raw CV text.
    /// </summary>
    /// <param name="cvText">The raw CV text to parse.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Extracted CV data.</returns>
    Task<CVExtractedData> ExtractCVDataAsync(string cvText, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extract structured data from raw job description text.
    /// </summary>
    /// <param name="jobDescText">The raw job description text to parse.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Extracted job description data.</returns>
    Task<JobDescExtractedData> ExtractJobDescriptionDataAsync(string jobDescText, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate interview questions based on CV and job description data.
    /// </summary>
    /// <param name="cvData">The extracted CV data.</param>
    /// <param name="jobDescData">The extracted job description data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Generated question set.</returns>
    Task<QuestionSet> GenerateQuestionsAsync(CVExtractedData cvData, JobDescExtractedData jobDescData, CancellationToken cancellationToken = default);
}
