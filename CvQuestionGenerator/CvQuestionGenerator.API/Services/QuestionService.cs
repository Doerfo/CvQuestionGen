using System.Text.Json;
using CvQuestionGenerator.API.Models;
using CvQuestionGenerator.API.Models.Responses;
using CvQuestionGenerator.API.Storage;
using Microsoft.Extensions.AI;

namespace CvQuestionGenerator.API.Services;

/// <summary>
/// Service for generating interview questions using AI.
/// </summary>
public sealed class QuestionService(
    IDataStore dataStore,
    IChatClient chatClient,
    ILogger<QuestionService> logger) : IQuestionService
{
    /// <inheritdoc/>
    public async Task<QuestionsResponse> GenerateQuestionsAsync(CancellationToken cancellationToken = default)
    {
        var cv = dataStore.GetCv();
        var job = dataStore.GetJob();

        if (cv is null)
        {
            throw new InvalidOperationException("CV_NOT_FOUND");
        }

        if (job is null)
        {
            throw new InvalidOperationException("JOB_NOT_FOUND");
        }

        logger.LogInformation("Generating questions for CV {CvId} and Job {JobId}", cv.Id, job.Id);

        // Build context for AI
        var cvSummary = BuildCvSummary(cv);
        var jobSummary = BuildJobSummary(job);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, AppConstants.Prompts.QuestionGeneration),
            new(ChatRole.User, $"""
                Generate interview questions based on the following:
                
                ## CANDIDATE CV SUMMARY
                {cvSummary}
                
                ## JOB REQUIREMENTS
                {jobSummary}
                
                Please generate question blocks for the most relevant skill matches.
                """)
        };

        try
        {
            var response = await chatClient.GetResponseAsync(
                messages,
                new ChatOptions { ResponseFormat = ChatResponseFormat.Json },
                cancellationToken);

            var content = response.Text ?? throw new InvalidOperationException("AI returned empty response");
            logger.LogDebug("Question generation response: {Response}", content);

            var questionBlocks = ParseQuestionBlocks(content);
            logger.LogInformation("Generated {BlockCount} question blocks", questionBlocks.Count);

            return new QuestionsResponse
            {
                CvId = cv.Id,
                JobId = job.Id,
                QuestionBlocks = questionBlocks,
                GeneratedAt = DateTimeOffset.UtcNow
            };
        }
        catch (Exception ex) when (ex is not OperationCanceledException && ex is not InvalidOperationException)
        {
            logger.LogError(ex, "Failed to generate questions");
            throw new InvalidOperationException("AI service failed to generate questions", ex);
        }
    }

    private static string BuildCvSummary(Cv cv)
    {
        var lines = new List<string>();

        if (cv.PersonalInfo.Name is not null)
            lines.Add($"Name: {cv.PersonalInfo.Name}");
        
        if (cv.PersonalInfo.Title is not null)
            lines.Add($"Title: {cv.PersonalInfo.Title}");

        lines.Add("");
        lines.Add("Skills:");
        foreach (var skill in cv.Skills)
        {
            var yearsInfo = skill.YearsOfExperience.HasValue ? $" ({skill.YearsOfExperience} years)" : "";
            lines.Add($"- {skill.Name}: {skill.Proficiency} [{skill.Category}]{yearsInfo}");
        }

        lines.Add("");
        lines.Add("Experience:");
        foreach (var exp in cv.Experience)
        {
            lines.Add($"- {exp.JobTitle} at {exp.Company}" + (exp.Duration is not null ? $" ({exp.Duration})" : ""));
        }

        return string.Join("\n", lines);
    }

    private static string BuildJobSummary(JobDescription job)
    {
        var lines = new List<string>();

        lines.Add("Required Skills:");
        foreach (var skill in job.RequiredSkills)
        {
            var yearsInfo = skill.YearsOfExperience.HasValue ? $" ({skill.YearsOfExperience} years required)" : "";
            lines.Add($"- {skill.Name}: {skill.Proficiency} [{skill.Category}]{yearsInfo}");
        }

        if (job.Competencies.Count > 0)
        {
            lines.Add("");
            lines.Add("Key Competencies:");
            foreach (var competency in job.Competencies)
            {
                lines.Add($"- {competency}");
            }
        }

        if (job.ExperienceRequirements.Count > 0)
        {
            lines.Add("");
            lines.Add("Experience Requirements:");
            foreach (var req in job.ExperienceRequirements)
            {
                var yearsInfo = req.YearsRequired.HasValue ? $" ({req.YearsRequired}+ years)" : "";
                lines.Add($"- {req.Area}: {req.MinimumLevel}{yearsInfo}");
            }
        }

        return string.Join("\n", lines);
    }

    private static List<QuestionBlock> ParseQuestionBlocks(string json)
    {
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var blocksElement = root.GetProperty("questionBlocks");

        var blocks = new List<QuestionBlock>();
        foreach (var blockElement in blocksElement.EnumerateArray())
        {
            var topicLabel = blockElement.GetProperty("topicLabel").GetString()!;
            var difficultyStr = blockElement.GetProperty("difficulty").GetString()!;
            
            if (!Enum.TryParse<DifficultyLevel>(difficultyStr, ignoreCase: true, out var difficulty))
                difficulty = DifficultyLevel.Standard;

            var questions = ParseQuestions(blockElement.GetProperty("questions"));

            blocks.Add(new QuestionBlock
            {
                TopicLabel = topicLabel,
                Difficulty = difficulty,
                Questions = questions
            });
        }

        return blocks;
    }

    private static List<InterviewQuestion> ParseQuestions(JsonElement element)
    {
        var questions = new List<InterviewQuestion>();
        foreach (var qElement in element.EnumerateArray())
        {
            var text = qElement.GetProperty("text").GetString()!;
            var guidelines = qElement.GetProperty("answerGuidelines").GetString()!;
            var keyTerms = ParseKeyTerms(qElement.GetProperty("keyTerms"));
            var followUps = ParseStringArray(qElement.GetProperty("followUpQuestions"));

            questions.Add(new InterviewQuestion
            {
                Text = text,
                AnswerGuidelines = guidelines,
                KeyTerms = keyTerms,
                FollowUpQuestions = followUps
            });
        }
        return questions;
    }

    private static List<KeyTerm> ParseKeyTerms(JsonElement element)
    {
        var terms = new List<KeyTerm>();
        foreach (var termElement in element.EnumerateArray())
        {
            terms.Add(new KeyTerm
            {
                Term = termElement.GetProperty("term").GetString()!,
                Explanation = termElement.GetProperty("explanation").GetString()!
            });
        }
        return terms;
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
