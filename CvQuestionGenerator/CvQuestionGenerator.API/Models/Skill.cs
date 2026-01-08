using System.Text.Json.Serialization;

namespace CvQuestionGenerator.API.Models;

/// <summary>
/// Proficiency level for a skill, from Beginner to Expert.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProficiencyLevel
{
    /// <summary>0-2 years, "familiar", "touched"</summary>
    Beginner = 1,
    
    /// <summary>2-4 years, "worked with", "used"</summary>
    Intermediate = 2,
    
    /// <summary>4-6 years, "proficient", "strong"</summary>
    Advanced = 3,
    
    /// <summary>6+ years, "expert", "lead"</summary>
    Expert = 4
}

/// <summary>
/// Category of a technical or professional skill.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SkillCategory
{
    Frontend,
    Backend,
    Database,
    DevOps,
    Cloud,
    Mobile,
    MachineLearning,
    Testing,
    SoftSkills,
    Other
}

/// <summary>
/// Represents a technical or professional skill with proficiency level.
/// </summary>
public sealed record Skill
{
    /// <summary>Skill name (e.g., "Java", "Angular").</summary>
    public required string Name { get; init; }
    
    /// <summary>Category of the skill.</summary>
    public required SkillCategory Category { get; init; }
    
    /// <summary>Proficiency level in this skill.</summary>
    public required ProficiencyLevel Proficiency { get; init; }
    
    /// <summary>Years of experience with this skill, if known.</summary>
    public int? YearsOfExperience { get; init; }
    
    /// <summary>Original text snippet for reference.</summary>
    public string? Context { get; init; }
}
