using System.Text.Json.Serialization;

namespace CvQuestionGenerator.API.Models.Questions;

/// <summary>
/// A group of questions organized by topic with a difficulty level.
/// </summary>
public class TopicGroup
{
    /// <summary>
    /// Topic name (e.g., "Frontend - Angular").
    /// </summary>
    [JsonPropertyName("topic")]
    public required string Topic { get; set; }

    /// <summary>
    /// Difficulty level for this topic group.
    /// </summary>
    [JsonPropertyName("difficultyLevel")]
    public ProficiencyLevel DifficultyLevel { get; set; }

    /// <summary>
    /// Questions for this topic.
    /// </summary>
    [JsonPropertyName("questions")]
    public List<Question> Questions { get; set; } = [];
}
