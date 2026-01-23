using System.Text.Json.Serialization;

namespace CvQuestionGenerator.API.Models.CV;

/// <summary>
/// Personal information extracted from a CV.
/// </summary>
public class PersonalInfo
{
    /// <summary>
    /// Candidate's name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Candidate's email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// Candidate's phone number.
    /// </summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
}
