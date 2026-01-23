using CvQuestionGenerator.API.DTOs;
using CvQuestionGenerator.API.Models.Questions;
using CvQuestionGenerator.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CvQuestionGenerator.API.Controllers;

/// <summary>
/// Controller for interview question generation.
/// </summary>
[ApiController]
[Route("api/questions")]
[Authorize]
public class QuestionController : ControllerBase
{
    private readonly IQuestionGenerationService _questionService;
    private readonly ILogger<QuestionController> _logger;

    public QuestionController(IQuestionGenerationService questionService, ILogger<QuestionController> logger)
    {
        _questionService = questionService;
        _logger = logger;
    }

    /// <summary>
    /// Generate interview questions based on the currently stored CV and job description.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Generated interview questions grouped by topic.</returns>
    /// <response code="200">Questions generated successfully.</response>
    /// <response code="400">CV or job description not uploaded.</response>
    /// <response code="401">Unauthorized - API key is missing or invalid.</response>
    [HttpGet]
    [ProducesResponseType(typeof(QuestionSet), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GenerateQuestions(CancellationToken cancellationToken)
    {
        try
        {
            var questionSet = await _questionService.GenerateQuestionsAsync(cancellationToken);
            return Ok(questionSet);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot generate questions - prerequisites not met");
            return BadRequest(new ErrorResponse { Error = ex.Message });
        }
    }
}
