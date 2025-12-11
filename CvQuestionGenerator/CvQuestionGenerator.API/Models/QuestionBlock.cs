using System.Text.Json.Serialization;

namespace CvQuestionGenerator.API.Models;

/// <summary>
/// Difficulty level for interview questions based on candidate proficiency vs job requirements.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DifficultyLevel
{
    /// <summary>For skill gaps - tests basic understanding.</summary>
    Foundational,
    
    /// <summary>Candidate meets requirements - tests practical application.</summary>
    Standard,
    
    /// <summary>Candidate exceeds requirements - tests depth and edge cases.</summary>
    Advanced
}

/// <summary>
/// Represents a group of related interview questions for a skill topic.
/// </summary>
public sealed record QuestionBlock
{
    /// <summary>Topic label (e.g., "Frontend - Angular").</summary>
    public required string TopicLabel { get; init; }
    
    /// <summary>Difficulty level for this block's questions.</summary>
    public required DifficultyLevel Difficulty { get; init; }
    
    /// <summary>List of interview questions for this topic (exactly 5).</summary>
    public required IReadOnlyList<InterviewQuestion> Questions { get; init; }
}
