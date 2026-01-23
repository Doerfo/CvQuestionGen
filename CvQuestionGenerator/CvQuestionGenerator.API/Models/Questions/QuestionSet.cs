using System.Text.Json.Serialization;

namespace CvQuestionGenerator.API.Models.Questions;

/// <summary>
/// A set of generated interview questions organized by topic.
/// </summary>
public class QuestionSet
{
    /// <summary>
    /// Timestamp when the questions were generated.
    /// </summary>
    [JsonPropertyName("generatedAt")]
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Questions grouped by topic.
    /// </summary>
    [JsonPropertyName("topicGroups")]
    public List<TopicGroup> TopicGroups { get; set; } = [];
}
