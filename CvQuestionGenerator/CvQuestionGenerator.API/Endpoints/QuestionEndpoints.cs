using CvQuestionGenerator.API.Models.Responses;
using CvQuestionGenerator.API.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CvQuestionGenerator.API.Endpoints;

/// <summary>
/// Endpoints for interview question generation.
/// </summary>
public static class QuestionEndpoints
{
    /// <summary>
    /// Maps question generation endpoints to the application.
    /// </summary>
    public static void MapQuestionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/questions")
            .WithTags("Questions");

        group.MapGet("/", GenerateQuestions)
            .WithName("GenerateQuestions")
            .WithSummary("Generate interview questions")
            .WithDescription("Generate AI-powered interview questions based on the stored CV and job description. Questions are grouped by topic and calibrated to the candidate's proficiency level.");
    }

    /// <summary>
    /// Generate interview questions.
    /// </summary>
    private static async Task<Results<Ok<QuestionsResponse>, ProblemHttpResult>> GenerateQuestions(
        IQuestionService questionService,
        ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await questionService.GenerateQuestionsAsync(cancellationToken);
            return TypedResults.Ok(response);
        }
        catch (InvalidOperationException ex) when (ex.Message == "CV_NOT_FOUND")
        {
            return TypedResults.Problem(
                title: "CV Required",
                detail: "No CV has been uploaded. Please upload a CV first using POST /api/cvs.",
                statusCode: StatusCodes.Status400BadRequest,
                type: "https://api.cvquestiongen.com/errors/cv-required"
            );
        }
        catch (InvalidOperationException ex) when (ex.Message == "JOB_NOT_FOUND")
        {
            return TypedResults.Problem(
                title: "Job Description Required",
                detail: "No job description has been uploaded. Please upload a job description first using POST /api/jobs.",
                statusCode: StatusCodes.Status400BadRequest,
                type: "https://api.cvquestiongen.com/errors/job-required"
            );
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "AI service error during question generation");
            return TypedResults.Problem(
                title: "AI Service Unavailable",
                detail: "The AI service is temporarily unavailable. Please retry later.",
                statusCode: StatusCodes.Status503ServiceUnavailable,
                type: "https://api.cvquestiongen.com/errors/ai-unavailable"
            );
        }
    }
}
