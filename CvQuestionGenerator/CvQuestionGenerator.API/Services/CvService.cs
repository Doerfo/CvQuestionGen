using CvQuestionGenerator.API.Models;
using CvQuestionGenerator.API.Models.Responses;
using CvQuestionGenerator.API.Storage;

namespace CvQuestionGenerator.API.Services;

/// <summary>
/// Service for CV management operations.
/// </summary>
public sealed class CvService(
    IDataStore dataStore,
    IAiExtractionService aiExtractionService,
    ILogger<CvService> logger) : ICvService
{
    /// <inheritdoc/>
    public async Task<CvUploadResponse> UploadCvAsync(string cvText, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Processing CV upload, text length: {Length}", cvText.Length);

        var extraction = await aiExtractionService.ExtractCvDataAsync(cvText, cancellationToken);

        var cv = new Cv
        {
            Id = Guid.NewGuid(),
            OriginalText = cvText,
            PersonalInfo = extraction.PersonalInfo,
            Skills = extraction.Skills,
            Experience = extraction.Experience,
            Education = extraction.Education,
            ExtractedAt = DateTimeOffset.UtcNow
        };

        dataStore.StoreCv(cv);
        logger.LogInformation("CV stored with ID: {CvId}", cv.Id);

        return new CvUploadResponse
        {
            Id = cv.Id,
            Message = "CV successfully processed",
            Extraction = extraction
        };
    }

    /// <inheritdoc/>
    public Cv? GetCv()
    {
        var cv = dataStore.GetCv();
        if (cv is null)
        {
            logger.LogDebug("No CV found in storage");
        }
        return cv;
    }
}
