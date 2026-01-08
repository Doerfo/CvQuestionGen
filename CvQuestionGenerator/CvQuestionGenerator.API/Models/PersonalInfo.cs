namespace CvQuestionGenerator.API.Models;

/// <summary>
/// Personal information from a CV (excluding contact details per privacy requirements).
/// </summary>
public sealed record PersonalInfo
{
    /// <summary>Candidate's name.</summary>
    public string? Name { get; init; }
    
    /// <summary>Professional title.</summary>
    public string? Title { get; init; }
    
    /// <summary>Professional summary or objective.</summary>
    public string? Summary { get; init; }
    
    // Note: Email, phone, address are NOT stored per FR-019
}
