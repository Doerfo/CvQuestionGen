using CvQuestionGenerator.API.DTOs;
using CvQuestionGenerator.API.Models.CV;
using CvQuestionGenerator.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CvQuestionGenerator.API.Controllers;

/// <summary>
/// Controller for CV management operations.
/// </summary>
[ApiController]
[Route("api/cvs")]
[Authorize]
public class CVController : ControllerBase
{
    private readonly ICVService _cvService;
    private readonly ILogger<CVController> _logger;

    public CVController(ICVService cvService, ILogger<CVController> logger)
    {
        _cvService = cvService;
        _logger = logger;
    }

    /// <summary>
    /// Submit a CV for processing. Replaces any existing CV.
    /// </summary>
    /// <param name="request">The CV submission request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>204 No Content on success.</returns>
    /// <response code="204">CV submitted successfully.</response>
    /// <response code="400">Invalid request - CV text is empty or null.</response>
    /// <response code="401">Unauthorized - API key is missing or invalid.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SubmitCV([FromBody] SubmitCVRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse { Error = "CV text is required and cannot be empty" });
        }

        try
        {
            await _cvService.SubmitCVAsync(request.CvText, cancellationToken);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid CV submission request");
            return BadRequest(new ErrorResponse { Error = ex.Message });
        }
    }

    /// <summary>
    /// Get the currently stored CV with extracted data.
    /// </summary>
    /// <returns>The CV data with raw text and extracted information.</returns>
    /// <response code="200">CV retrieved successfully.</response>
    /// <response code="401">Unauthorized - API key is missing or invalid.</response>
    /// <response code="404">No CV has been uploaded.</response>
    [HttpGet]
    [ProducesResponseType(typeof(CVData), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public IActionResult GetCV()
    {
        var cvData = _cvService.GetCV();

        if (cvData is null)
        {
            return NotFound(new ErrorResponse { Error = "No CV has been uploaded" });
        }

        return Ok(cvData);
    }
}
