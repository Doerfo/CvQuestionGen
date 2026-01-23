using System.Text.Json.Serialization;

namespace CvQuestionGenerator.API.Models.CV;

/// <summary>
/// Structured data extracted from a CV by AI.
/// </summary>
public class CVExtractedData
{
    /// <summary>
    /// Personal information (name, email, phone).
    /// </summary>
    [JsonPropertyName("personalInfo")]
    public PersonalInfo PersonalInfo { get; set; } = new();

    /// <summary>
    /// Skills with proficiency levels.
    /// </summary>
    [JsonPropertyName("skills")]
    public List<Skill> Skills { get; set; } = [];

    /// <summary>
    /// Work experience history.
    /// </summary>
    [JsonPropertyName("workExperience")]
    public List<WorkExperience> WorkExperience { get; set; } = [];

    /// <summary>
    /// Educational background.
    /// </summary>
    [JsonPropertyName("education")]
    public List<Education> Education { get; set; } = [];
}
