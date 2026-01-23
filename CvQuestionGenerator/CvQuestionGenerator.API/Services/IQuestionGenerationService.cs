using CvQuestionGenerator.API.Models.Questions;

namespace CvQuestionGenerator.API.Services;

/// <summary>
/// Interface for question generation operations.
/// </summary>
public interface IQuestionGenerationService
{
    /// <summary>
    /// Generate interview questions based on the currently stored CV and job description.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated question set.</returns>
    /// <exception cref="InvalidOperationException">Thrown when CV or job description is missing.</exception>
    Task<QuestionSet> GenerateQuestionsAsync(CancellationToken cancellationToken = default);
}
