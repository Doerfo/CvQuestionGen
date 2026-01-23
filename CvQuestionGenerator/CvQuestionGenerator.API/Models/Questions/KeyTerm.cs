using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CvQuestionGenerator.API.Models.Questions;

/// <summary>
/// A technical term with plain English explanation.
/// </summary>
public class KeyTerm
{
    /// <summary>
    /// The technical term.
    /// </summary>
    [Required]
    [JsonPropertyName("term")]
    public required string Term { get; set; }

    /// <summary>
    /// Plain English explanation of the term.
    /// </summary>
    [Required]
    [JsonPropertyName("explanation")]
    public required string Explanation { get; set; }
}
