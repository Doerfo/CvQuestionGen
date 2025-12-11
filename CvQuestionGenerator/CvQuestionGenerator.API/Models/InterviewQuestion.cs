namespace CvQuestionGenerator.API.Models;

/// <summary>
/// Represents a single interview question with supporting information.
/// </summary>
public sealed record InterviewQuestion
{
    /// <summary>The interview question text.</summary>
    public required string Text { get; init; }
    
    /// <summary>Non-technical explanation of what to look for in answers.</summary>
    public required string AnswerGuidelines { get; init; }
    
    /// <summary>Technical terms with plain English explanations.</summary>
    public required IReadOnlyList<KeyTerm> KeyTerms { get; init; }
    
    /// <summary>Suggested follow-up questions.</summary>
    public required IReadOnlyList<string> FollowUpQuestions { get; init; }
}
