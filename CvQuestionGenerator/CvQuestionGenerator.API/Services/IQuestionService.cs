using CvQuestionGenerator.API.Models.Responses;

namespace CvQuestionGenerator.API.Services;

/// <summary>
/// Service for generating interview questions.
/// </summary>
public interface IQuestionService
{
    /// <summary>
    /// Generates interview questions based on the stored CV and job description.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Generated questions response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when CV or job description is not uploaded.</exception>
    Task<QuestionsResponse> GenerateQuestionsAsync(CancellationToken cancellationToken = default);
}
