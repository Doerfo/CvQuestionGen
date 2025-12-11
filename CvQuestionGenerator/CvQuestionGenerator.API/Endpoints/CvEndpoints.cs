using CvQuestionGenerator.API.Models.Requests;
using CvQuestionGenerator.API.Models.Responses;
using CvQuestionGenerator.API.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CvQuestionGenerator.API.Endpoints;

/// <summary>
/// Endpoints for CV management operations.
/// </summary>
public static class CvEndpoints
{
    /// <summary>
    /// Maps CV endpoints to the application.
    /// </summary>
    public static void MapCvEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/cvs")
            .WithTags("CVs");

        group.MapPost("/", UploadCv)
            .WithName("UploadCv")
            .WithSummary("Upload and process CV")
            .WithDescription("Upload unformatted CV text for AI extraction. Extracts skills, experience, and education.");

        group.MapGet("/", GetCv)
            .WithName("GetCv")
            .WithSummary("Retrieve stored CV")
            .WithDescription("Retrieve the currently stored CV including original text and AI-extracted structured data.");
    }

    /// <summary>
    /// Upload and process a CV.
    /// </summary>
    private static async Task<Results<Created<CvUploadResponse>, ValidationProblem, ProblemHttpResult>> UploadCv(
        [FromBody] UploadCvRequest request,
        ICvService cvService,
        ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        // Validate request
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["text"] = ["The Text field is required."]
            });
        }

        if (request.Text.Length > AppConstants.MaxTextLength)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["text"] = [$"Text must not exceed {AppConstants.MaxTextLength} characters."]
            });
        }

        try
        {
            var response = await cvService.UploadCvAsync(request.Text, cancellationToken);
            return TypedResults.Created($"/api/cvs", response);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "AI service error during CV upload");
            return TypedResults.Problem(
                title: "AI Service Unavailable",
                detail: "The AI service is temporarily unavailable. Please retry later.",
                statusCode: StatusCodes.Status503ServiceUnavailable,
                type: "https://api.cvquestiongen.com/errors/ai-unavailable"
            );
        }
    }

    /// <summary>
    /// Get the stored CV.
    /// </summary>
    private static Results<Ok<CvResponse>, ProblemHttpResult> GetCv(ICvService cvService)
    {
        var cv = cvService.GetCv();
        
        if (cv is null)
        {
            return TypedResults.Problem(
                title: "CV Not Found",
                detail: "No CV has been uploaded yet.",
                statusCode: StatusCodes.Status404NotFound,
                type: "https://api.cvquestiongen.com/errors/cv-not-found"
            );
        }

        var response = new CvResponse
        {
            Id = cv.Id,
            OriginalText = cv.OriginalText,
            Extraction = new CvExtractionResult
            {
                PersonalInfo = cv.PersonalInfo,
                Skills = cv.Skills,
                Experience = cv.Experience,
                Education = cv.Education
            },
            ExtractedAt = cv.ExtractedAt
        };

        return TypedResults.Ok(response);
    }
}
