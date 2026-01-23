using System.Text.Json;
using CvQuestionGenerator.API.Models.CV;
using CvQuestionGenerator.API.Models.JobDescription;
using CvQuestionGenerator.API.Models.Questions;
using Microsoft.Extensions.AI;

namespace CvQuestionGenerator.API.Services;

/// <summary>
/// AI service implementation using Aspire Azure OpenAI chat client.
/// </summary>
public class AIService : IAIService
{
    private readonly IChatClient _chatClient;
    private readonly ILogger<AIService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public AIService(IChatClient chatClient, ILogger<AIService> logger)
    {
        _chatClient = chatClient;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<CVExtractedData> ExtractCVDataAsync(string cvText, CancellationToken cancellationToken = default)
    {
        var userPrompt = AppConstants.Prompts.CvExtractionUser.Replace("{cvText}", cvText);

        var response = await ExecuteWithRetryAsync(
            AppConstants.Prompts.CvExtractionSystem,
            userPrompt,
            cancellationToken);

        return ParseJsonResponse<CVExtractedData>(response, "CV extraction");
    }

    /// <inheritdoc/>
    public async Task<JobDescExtractedData> ExtractJobDescriptionDataAsync(string jobDescText, CancellationToken cancellationToken = default)
    {
        var userPrompt = AppConstants.Prompts.JobDescExtractionUser.Replace("{jobDescText}", jobDescText);

        var response = await ExecuteWithRetryAsync(
            AppConstants.Prompts.JobDescExtractionSystem,
            userPrompt,
            cancellationToken);

        return ParseJsonResponse<JobDescExtractedData>(response, "Job description extraction");
    }

    /// <inheritdoc/>
    public async Task<QuestionSet> GenerateQuestionsAsync(CVExtractedData cvData, JobDescExtractedData jobDescData, CancellationToken cancellationToken = default)
    {
        var cvSkillsJson = JsonSerializer.Serialize(cvData.Skills, JsonOptions);
        var jobRequirementsJson = JsonSerializer.Serialize(new
        {
            requiredSkills = jobDescData.RequiredSkills,
            experienceLevel = jobDescData.ExperienceLevel,
            keyCompetencies = jobDescData.KeyCompetencies
        }, JsonOptions);

        var userPrompt = AppConstants.Prompts.QuestionGenerationUser
            .Replace("{cvSkills}", cvSkillsJson)
            .Replace("{jobRequirements}", jobRequirementsJson);

        var response = await ExecuteWithRetryAsync(
            AppConstants.Prompts.QuestionGenerationSystem,
            userPrompt,
            cancellationToken);

        var result = ParseJsonResponse<QuestionSet>(response, "Question generation");
        result.GeneratedAt = DateTime.UtcNow;
        return result;
    }

    private async Task<string> ExecuteWithRetryAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken)
    {
        const int maxRetries = 3;
        var delay = TimeSpan.FromSeconds(1);

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var messages = new List<ChatMessage>
                {
                    new(ChatRole.System, systemPrompt),
                    new(ChatRole.User, userPrompt)
                };

                var response = await _chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
                
                return response.Text ?? throw new InvalidOperationException("AI returned empty response");
            }
            catch (Exception ex) when (attempt < maxRetries && IsTransientError(ex))
            {
                _logger.LogWarning(ex, "AI service call failed (attempt {Attempt}/{MaxRetries}). Retrying in {Delay}...", 
                    attempt, maxRetries, delay);
                
                await Task.Delay(delay, cancellationToken);
                delay *= 2; // Exponential backoff
            }
        }

        throw new InvalidOperationException("AI service call failed after maximum retries");
    }

    private static bool IsTransientError(Exception ex)
    {
        // Check for transient errors that warrant retry
        return ex is HttpRequestException or TaskCanceledException or TimeoutException;
    }

    private T ParseJsonResponse<T>(string response, string operationName) where T : new()
    {
        try
        {
            // Clean up response - remove markdown code blocks if present
            var jsonText = response.Trim();
            if (jsonText.StartsWith("```json"))
            {
                jsonText = jsonText[7..];
            }
            else if (jsonText.StartsWith("```"))
            {
                jsonText = jsonText[3..];
            }
            if (jsonText.EndsWith("```"))
            {
                jsonText = jsonText[..^3];
            }
            jsonText = jsonText.Trim();

            var result = JsonSerializer.Deserialize<T>(jsonText, JsonOptions);
            
            if (result is null)
            {
                _logger.LogWarning("{OperationName}: JSON deserialization returned null, using default", operationName);
                return new T();
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "{OperationName}: Failed to parse AI response as JSON. Response: {Response}", 
                operationName, response);
            
            // Return a default instance as fallback
            return new T();
        }
    }
}
