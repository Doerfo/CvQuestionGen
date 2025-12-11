using CvQuestionGenerator.API.Models;
using CvQuestionGenerator.API.Models.Responses;
using CvQuestionGenerator.API.Storage;

namespace CvQuestionGenerator.API.Services;

/// <summary>
/// Service for job description management operations.
/// </summary>
public sealed class JobService(
    IDataStore dataStore,
    IAiExtractionService aiExtractionService,
    ILogger<JobService> logger) : IJobService
{
    /// <inheritdoc/>
    public async Task<JobUploadResponse> UploadJobAsync(string jobText, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Processing job description upload, text length: {Length}", jobText.Length);

        var extraction = await aiExtractionService.ExtractJobDataAsync(jobText, cancellationToken);

        var job = new JobDescription
        {
            Id = Guid.NewGuid(),
            OriginalText = jobText,
            RequiredSkills = extraction.RequiredSkills,
            Competencies = extraction.Competencies,
            ExperienceRequirements = extraction.ExperienceRequirements,
            ExtractedAt = DateTimeOffset.UtcNow
        };

        dataStore.StoreJob(job);
        logger.LogInformation("Job description stored with ID: {JobId}", job.Id);

        return new JobUploadResponse
        {
            Id = job.Id,
            Message = "Job description successfully processed",
            Extraction = extraction
        };
    }

    /// <inheritdoc/>
    public JobDescription? GetJob()
    {
        var job = dataStore.GetJob();
        if (job is null)
        {
            logger.LogDebug("No job description found in storage");
        }
        return job;
    }
}
