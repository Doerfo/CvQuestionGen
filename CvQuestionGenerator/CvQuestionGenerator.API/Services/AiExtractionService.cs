using System.Text.Json;
using CvQuestionGenerator.API.Models;
using CvQuestionGenerator.API.Models.Responses;
using Microsoft.Extensions.AI;

namespace CvQuestionGenerator.API.Services;

/// <summary>
/// Service for AI-powered extraction of structured data from text using Azure OpenAI.
/// </summary>
public sealed class AiExtractionService(IChatClient chatClient, ILogger<AiExtractionService> logger) : IAiExtractionService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <inheritdoc/>
    public async Task<CvExtractionResult> ExtractCvDataAsync(string cvText, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting CV extraction for text of length {Length}", cvText.Length);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, AppConstants.Prompts.CvExtraction),
            new(ChatRole.User, $"Please extract structured data from this CV:\n\n{cvText}")
        };

        try
        {
            var response = await chatClient.GetResponseAsync(
                messages,
                new ChatOptions { ResponseFormat = ChatResponseFormat.Json },
                cancellationToken);

            var content = response.Text ?? throw new InvalidOperationException("AI returned empty response");
            logger.LogDebug("CV extraction response: {Response}", content);

            var result = ParseCvExtractionResult(content);
            logger.LogInformation("CV extraction completed. Found {SkillCount} skills", result.Skills.Count);
            
            return result;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to extract CV data");
            throw new InvalidOperationException("AI service failed to process CV", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<JobExtractionResult> ExtractJobDataAsync(string jobText, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting job description extraction for text of length {Length}", jobText.Length);

        var systemPrompt = AppConstants.Prompts.JobExtraction;
        if (string.IsNullOrEmpty(systemPrompt))
        {
            throw new InvalidOperationException("Job extraction prompt not configured");
        }

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, $"Please extract structured data from this job description:\n\n{jobText}")
        };

        try
        {
            var response = await chatClient.GetResponseAsync(
                messages,
                new ChatOptions { ResponseFormat = ChatResponseFormat.Json },
                cancellationToken);

            var content = response.Text ?? throw new InvalidOperationException("AI returned empty response");
            logger.LogDebug("Job extraction response: {Response}", content);

            var result = ParseJobExtractionResult(content);
            logger.LogInformation("Job extraction completed. Found {SkillCount} required skills", result.RequiredSkills.Count);
            
            return result;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to extract job data");
            throw new InvalidOperationException("AI service failed to process job description", ex);
        }
    }

    private static CvExtractionResult ParseCvExtractionResult(string json)
    {
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var personalInfo = ParsePersonalInfo(root.GetProperty("personalInfo"));
        var skills = ParseSkills(root.GetProperty("skills"));
        var experience = ParseExperience(root.GetProperty("experience"));
        var education = ParseEducation(root.GetProperty("education"));
        var warnings = root.TryGetProperty("warnings", out var warningsElement) 
            ? ParseStringArray(warningsElement) 
            : null;

        return new CvExtractionResult
        {
            PersonalInfo = personalInfo,
            Skills = skills,
            Experience = experience,
            Education = education,
            Warnings = warnings?.Count > 0 ? warnings : null
        };
    }

    private static JobExtractionResult ParseJobExtractionResult(string json)
    {
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var requiredSkills = ParseSkills(root.GetProperty("requiredSkills"));
        var competencies = ParseStringArray(root.GetProperty("competencies"));
        var experienceRequirements = ParseExperienceRequirements(root.GetProperty("experienceRequirements"));
        var warnings = root.TryGetProperty("warnings", out var warningsElement) 
            ? ParseStringArray(warningsElement) 
            : null;

        return new JobExtractionResult
        {
            RequiredSkills = requiredSkills,
            Competencies = competencies,
            ExperienceRequirements = experienceRequirements,
            Warnings = warnings?.Count > 0 ? warnings : null
        };
    }

    private static PersonalInfo ParsePersonalInfo(JsonElement element)
    {
        return new PersonalInfo
        {
            Name = element.TryGetProperty("name", out var name) && name.ValueKind != JsonValueKind.Null 
                ? name.GetString() : null,
            Title = element.TryGetProperty("title", out var title) && title.ValueKind != JsonValueKind.Null 
                ? title.GetString() : null,
            Summary = element.TryGetProperty("summary", out var summary) && summary.ValueKind != JsonValueKind.Null 
                ? summary.GetString() : null
        };
    }

    private static List<Skill> ParseSkills(JsonElement element)
    {
        var skills = new List<Skill>();
        foreach (var skillElement in element.EnumerateArray())
        {
            var name = skillElement.GetProperty("name").GetString()!;
            var categoryStr = skillElement.GetProperty("category").GetString()!;
            var proficiencyStr = skillElement.GetProperty("proficiency").GetString()!;
            
            if (!Enum.TryParse<SkillCategory>(categoryStr, ignoreCase: true, out var category))
                category = SkillCategory.Other;
            
            if (!Enum.TryParse<ProficiencyLevel>(proficiencyStr, ignoreCase: true, out var proficiency))
                proficiency = ProficiencyLevel.Intermediate;

            skills.Add(new Skill
            {
                Name = name,
                Category = category,
                Proficiency = proficiency,
                YearsOfExperience = skillElement.TryGetProperty("yearsOfExperience", out var years) && years.ValueKind == JsonValueKind.Number 
                    ? years.GetInt32() : null,
                Context = skillElement.TryGetProperty("context", out var context) && context.ValueKind != JsonValueKind.Null 
                    ? context.GetString() : null
            });
        }
        return skills;
    }

    private static List<WorkExperience> ParseExperience(JsonElement element)
    {
        var experiences = new List<WorkExperience>();
        foreach (var expElement in element.EnumerateArray())
        {
            experiences.Add(new WorkExperience
            {
                JobTitle = expElement.GetProperty("jobTitle").GetString()!,
                Company = expElement.GetProperty("company").GetString()!,
                Duration = expElement.TryGetProperty("duration", out var duration) && duration.ValueKind != JsonValueKind.Null 
                    ? duration.GetString() : null,
                Responsibilities = expElement.TryGetProperty("responsibilities", out var resp) 
                    ? ParseStringArray(resp) : []
            });
        }
        return experiences;
    }

    private static List<Education> ParseEducation(JsonElement element)
    {
        var educations = new List<Education>();
        foreach (var eduElement in element.EnumerateArray())
        {
            educations.Add(new Education
            {
                Degree = eduElement.GetProperty("degree").GetString()!,
                Institution = eduElement.GetProperty("institution").GetString()!,
                Year = eduElement.TryGetProperty("year", out var year) && year.ValueKind != JsonValueKind.Null 
                    ? year.GetString() : null
            });
        }
        return educations;
    }

    private static List<ExperienceRequirement> ParseExperienceRequirements(JsonElement element)
    {
        var requirements = new List<ExperienceRequirement>();
        foreach (var reqElement in element.EnumerateArray())
        {
            var levelStr = reqElement.GetProperty("minimumLevel").GetString()!;
            if (!Enum.TryParse<ProficiencyLevel>(levelStr, ignoreCase: true, out var level))
                level = ProficiencyLevel.Intermediate;

            requirements.Add(new ExperienceRequirement
            {
                Area = reqElement.GetProperty("area").GetString()!,
                MinimumLevel = level,
                YearsRequired = reqElement.TryGetProperty("yearsRequired", out var years) && years.ValueKind == JsonValueKind.Number 
                    ? years.GetInt32() : null
            });
        }
        return requirements;
    }

    private static List<string> ParseStringArray(JsonElement element)
    {
        var items = new List<string>();
        foreach (var item in element.EnumerateArray())
        {
            var value = item.GetString();
            if (!string.IsNullOrEmpty(value))
                items.Add(value);
        }
        return items;
    }
}
