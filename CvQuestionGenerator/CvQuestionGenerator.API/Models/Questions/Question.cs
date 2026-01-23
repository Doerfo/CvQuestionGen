using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CvQuestionGenerator.API.Models.Questions;

/// <summary>
/// A generated interview question with supporting information.
/// </summary>
public class Question
{
    /// <summary>
    /// The interview question text.
    /// </summary>
    [Required]
    [JsonPropertyName("questionText")]
    public required string QuestionText { get; set; }

    /// <summary>
    /// Natural language answer guidelines for recruiters.
    /// </summary>
    [Required]
    [JsonPropertyName("answerGuidelines")]
    public required string AnswerGuidelines { get; set; }

    /// <summary>
    /// Key technical terms with plain English explanations.
    /// </summary>
    [JsonPropertyName("keyTerms")]
    public List<KeyTerm> KeyTerms { get; set; } = [];

    /// <summary>
    /// Suggested follow-up questions (2-3).
    /// </summary>
    [JsonPropertyName("suggestedFollowUps")]
    public List<string> SuggestedFollowUps { get; set; } = [];
}
