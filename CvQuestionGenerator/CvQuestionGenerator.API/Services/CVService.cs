using CvQuestionGenerator.API.Models.CV;
using CvQuestionGenerator.API.Repositories;

namespace CvQuestionGenerator.API.Services;

/// <summary>
/// Service for CV management operations.
/// </summary>
public class CVService : ICVService
{
    private readonly ICVRepository _repository;
    private readonly IAIService _aiService;
    private readonly ILogger<CVService> _logger;

    public CVService(ICVRepository repository, IAIService aiService, ILogger<CVService> logger)
    {
        _repository = repository;
        _aiService = aiService;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task SubmitCVAsync(string cvText, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cvText))
        {
            throw new ArgumentException("CV text is required and cannot be empty", nameof(cvText));
        }

        _logger.LogInformation("Processing CV submission");

        // Extract structured data using AI
        var extractedData = await _aiService.ExtractCVDataAsync(cvText, cancellationToken);

        // Create and store CV data
        var cvData = new CVData
        {
            RawText = cvText,
            ExtractedData = extractedData,
            CreatedAt = DateTime.UtcNow
        };

        _repository.Set(cvData);

        _logger.LogInformation("CV processed and stored successfully. Extracted {SkillCount} skills", 
            extractedData.Skills.Count);
    }

    /// <inheritdoc/>
    public CVData? GetCV()
    {
        return _repository.Get();
    }
}
