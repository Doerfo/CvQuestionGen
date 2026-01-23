using CvQuestionGenerator.API.Models.Questions;
using CvQuestionGenerator.API.Repositories;

namespace CvQuestionGenerator.API.Services;

/// <summary>
/// Service for generating interview questions based on CV and job description.
/// </summary>
public class QuestionGenerationService : IQuestionGenerationService
{
    private readonly ICVRepository _cvRepository;
    private readonly IJobDescRepository _jobDescRepository;
    private readonly IAIService _aiService;
    private readonly ILogger<QuestionGenerationService> _logger;

    public QuestionGenerationService(
        ICVRepository cvRepository,
        IJobDescRepository jobDescRepository,
        IAIService aiService,
        ILogger<QuestionGenerationService> logger)
    {
        _cvRepository = cvRepository;
        _jobDescRepository = jobDescRepository;
        _aiService = aiService;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<QuestionSet> GenerateQuestionsAsync(CancellationToken cancellationToken = default)
    {
        // Validate that both CV and job description exist
        if (!_cvRepository.Exists())
        {
            throw new InvalidOperationException("CV must be uploaded before generating questions");
        }

        if (!_jobDescRepository.Exists())
        {
            throw new InvalidOperationException("Job description must be uploaded before generating questions");
        }

        var cvData = _cvRepository.Get()!;
        var jobDescData = _jobDescRepository.Get()!;

        _logger.LogInformation("Generating questions for CV with {SkillCount} skills and job with {RequiredSkillCount} required skills",
            cvData.ExtractedData.Skills.Count,
            jobDescData.ExtractedData.RequiredSkills.Count);

        // Generate questions using AI
        var questionSet = await _aiService.GenerateQuestionsAsync(
            cvData.ExtractedData,
            jobDescData.ExtractedData,
            cancellationToken);

        _logger.LogInformation("Generated {TopicCount} topic groups with questions",
            questionSet.TopicGroups.Count);

        return questionSet;
    }
}
