using CvQuestionGenerator.API.Models.JobDescription;
using CvQuestionGenerator.API.Repositories;

namespace CvQuestionGenerator.API.Services;

/// <summary>
/// Service for job description management operations.
/// </summary>
public class JobDescService : IJobDescService
{
    private readonly IJobDescRepository _repository;
    private readonly IAIService _aiService;
    private readonly ILogger<JobDescService> _logger;

    public JobDescService(IJobDescRepository repository, IAIService aiService, ILogger<JobDescService> logger)
    {
        _repository = repository;
        _aiService = aiService;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task SubmitJobDescriptionAsync(string jobDescText, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(jobDescText))
        {
            throw new ArgumentException("Job description text is required and cannot be empty", nameof(jobDescText));
        }

        _logger.LogInformation("Processing job description submission");

        // Extract structured data using AI
        var extractedData = await _aiService.ExtractJobDescriptionDataAsync(jobDescText, cancellationToken);

        // Create and store job description data
        var jobDescData = new JobDescriptionData
        {
            RawText = jobDescText,
            ExtractedData = extractedData,
            CreatedAt = DateTime.UtcNow
        };

        _repository.Set(jobDescData);

        _logger.LogInformation("Job description processed and stored successfully. Extracted {SkillCount} required skills", 
            extractedData.RequiredSkills.Count);
    }

    /// <inheritdoc/>
    public JobDescriptionData? GetJobDescription()
    {
        return _repository.Get();
    }
}
