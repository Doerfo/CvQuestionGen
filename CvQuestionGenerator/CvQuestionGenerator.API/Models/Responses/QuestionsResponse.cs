namespace CvQuestionGenerator.API.Models.Responses;

/// <summary>
/// Response containing generated interview questions.
/// </summary>
public sealed record QuestionsResponse
{
    /// <summary>ID of the CV used for generation.</summary>
    public required Guid CvId { get; init; }
    
    /// <summary>ID of the job description used for generation.</summary>
    public required Guid JobId { get; init; }
    
    /// <summary>Generated question blocks grouped by topic.</summary>
    public required IReadOnlyList<QuestionBlock> QuestionBlocks { get; init; }
    
    /// <summary>When the questions were generated.</summary>
    public required DateTimeOffset GeneratedAt { get; init; }
}
