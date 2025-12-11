using CvQuestionGenerator.API.Models.Requests;
using CvQuestionGenerator.API.Models.Responses;
using CvQuestionGenerator.API.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CvQuestionGenerator.API.Endpoints;

/// <summary>
/// Endpoints for job description management operations.
/// </summary>
public static class JobEndpoints
{
    /// <summary>
    /// Maps job description endpoints to the application.
    /// </summary>
    public static void MapJobEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/jobs")
            .WithTags("Jobs");

        group.MapPost("/", UploadJob)
            .WithName("UploadJob")
            .WithSummary("Upload and process job description")
            .WithDescription("Upload unformatted job description text for AI extraction. Extracts required skills, competencies, and experience requirements.");

        group.MapGet("/", GetJob)
            .WithName("GetJob")
            .WithSummary("Retrieve stored job description")
            .WithDescription("Retrieve the currently stored job description including original text and AI-extracted structured data.");
    }

    /// <summary>
    /// Upload and process a job description.
    /// </summary>
    private static async Task<Results<Created<JobUploadResponse>, ValidationProblem, ProblemHttpResult>> UploadJob(
        [FromBody] UploadJobRequest request,
        IJobService jobService,
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
            var response = await jobService.UploadJobAsync(request.Text, cancellationToken);
            return TypedResults.Created($"/api/jobs", response);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "AI service error during job upload");
            return TypedResults.Problem(
                title: "AI Service Unavailable",
                detail: "The AI service is temporarily unavailable. Please retry later.",
                statusCode: StatusCodes.Status503ServiceUnavailable,
                type: "https://api.cvquestiongen.com/errors/ai-unavailable"
            );
        }
    }

    /// <summary>
    /// Get the stored job description.
    /// </summary>
    private static Results<Ok<JobResponse>, ProblemHttpResult> GetJob(IJobService jobService)
    {
        var job = jobService.GetJob();
        
        if (job is null)
        {
            return TypedResults.Problem(
                title: "Job Description Not Found",
                detail: "No job description has been uploaded yet.",
                statusCode: StatusCodes.Status404NotFound,
                type: "https://api.cvquestiongen.com/errors/job-not-found"
            );
        }

        var response = new JobResponse
        {
            Id = job.Id,
            OriginalText = job.OriginalText,
            Extraction = new JobExtractionResult
            {
                RequiredSkills = job.RequiredSkills,
                Competencies = job.Competencies,
                ExperienceRequirements = job.ExperienceRequirements
            },
            ExtractedAt = job.ExtractedAt
        };

        return TypedResults.Ok(response);
    }
}
