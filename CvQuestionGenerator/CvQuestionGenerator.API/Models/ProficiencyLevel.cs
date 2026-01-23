namespace CvQuestionGenerator.API.Models;

/// <summary>
/// Represents the proficiency level for a skill.
/// </summary>
public enum ProficiencyLevel
{
    /// <summary>Mentioned/touched/familiar with the skill.</summary>
    Beginner,
    
    /// <summary>1-3 years or project experience.</summary>
    Intermediate,
    
    /// <summary>3-5 years or extensive experience.</summary>
    Advanced,
    
    /// <summary>5+ years or demonstrated mastery.</summary>
    Expert
}
