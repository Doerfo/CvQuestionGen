using CvQuestionGenerator.API.DTOs;
using CvQuestionGenerator.API.Models.JobDescription;
using CvQuestionGenerator.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CvQuestionGenerator.API.Controllers;

/// <summary>
/// Controller for job description management operations.
/// </summary>
[ApiController]
[Route("api/jobs")]
[Authorize]
public class JobDescController : ControllerBase
{
    private readonly IJobDescService _jobDescService;
    private readonly ILogger<JobDescController> _logger;

    public JobDescController(IJobDescService jobDescService, ILogger<JobDescController> logger)
    {
        _jobDescService = jobDescService;
        _logger = logger;
    }

    /// <summary>
    /// Submit a job description for processing. Replaces any existing job description.
    /// </summary>
    /// <param name="request">The job description submission request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>204 No Content on success.</returns>
    /// <response code="204">Job description submitted successfully.</response>
    /// <response code="400">Invalid request - job description text is empty or null.</response>
    /// <response code="401">Unauthorized - API key is missing or invalid.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SubmitJobDescription([FromBody] SubmitJobDescriptionRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse { Error = "Job description text is required and cannot be empty" });
        }

        try
        {
            await _jobDescService.SubmitJobDescriptionAsync(request.JobDescriptionText, cancellationToken);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid job description submission request");
            return BadRequest(new ErrorResponse { Error = ex.Message });
        }
    }

    /// <summary>
    /// Get the currently stored job description with extracted data.
    /// </summary>
    /// <returns>The job description data with raw text and extracted information.</returns>
    /// <response code="200">Job description retrieved successfully.</response>
    /// <response code="401">Unauthorized - API key is missing or invalid.</response>
    /// <response code="404">No job description has been uploaded.</response>
    [HttpGet]
    [ProducesResponseType(typeof(JobDescriptionData), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public IActionResult GetJobDescription()
    {
        var jobDescData = _jobDescService.GetJobDescription();

        if (jobDescData is null)
        {
            return NotFound(new ErrorResponse { Error = "No job description has been uploaded" });
        }

        return Ok(jobDescData);
    }
}
