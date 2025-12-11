namespace CvQuestionGenerator.API.Models;

/// <summary>
/// Represents a technical term with a plain English explanation.
/// </summary>
public sealed record KeyTerm
{
    /// <summary>The technical term.</summary>
    public required string Term { get; init; }
    
    /// <summary>Plain English explanation of the term.</summary>
    public required string Explanation { get; init; }
}
