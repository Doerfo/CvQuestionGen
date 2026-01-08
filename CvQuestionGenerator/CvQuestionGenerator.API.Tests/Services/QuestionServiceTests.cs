using CvQuestionGenerator.API.Models;
using CvQuestionGenerator.API.Services;
using CvQuestionGenerator.API.Storage;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Moq;

namespace CvQuestionGenerator.API.Tests.Services;

public class QuestionServiceTests
{
    private readonly Mock<IDataStore> _dataStoreMock = new();
    private readonly Mock<IChatClient> _chatClientMock = new();
    private readonly Mock<ILogger<QuestionService>> _loggerMock = new();
    private readonly QuestionService _sut;

    public QuestionServiceTests()
    {
        _sut = new QuestionService(_dataStoreMock.Object, _chatClientMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GenerateQuestionsAsync_WhenNoCv_ThrowsInvalidOperationException()
    {
        // Arrange
        _dataStoreMock.Setup(x => x.GetCv()).Returns((Cv?)null);
        _dataStoreMock.Setup(x => x.GetJob()).Returns(CreateTestJob());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.GenerateQuestionsAsync());
        Assert.Equal("CV_NOT_FOUND", exception.Message);
    }

    [Fact]
    public async Task GenerateQuestionsAsync_WhenNoJob_ThrowsInvalidOperationException()
    {
        // Arrange
        _dataStoreMock.Setup(x => x.GetCv()).Returns(CreateTestCv());
        _dataStoreMock.Setup(x => x.GetJob()).Returns((JobDescription?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.GenerateQuestionsAsync());
        Assert.Equal("JOB_NOT_FOUND", exception.Message);
    }

    [Fact]
    public async Task GenerateQuestionsAsync_WhenCvAndJobExist_CallsChatClient()
    {
        // Arrange
        var cv = CreateTestCv();
        var job = CreateTestJob();
        _dataStoreMock.Setup(x => x.GetCv()).Returns(cv);
        _dataStoreMock.Setup(x => x.GetJob()).Returns(job);

        var responseJson = """
            {
              "questionBlocks": [
                {
                  "topicLabel": "Backend - C#",
                  "difficulty": "Standard",
                  "questions": [
                    {
                      "text": "Describe your experience with C#",
                      "answerGuidelines": "Look for practical examples",
                      "keyTerms": [
                        { "term": "C#", "explanation": "A programming language" }
                      ],
                      "followUpQuestions": ["What version?"]
                    }
                  ]
                }
              ]
            }
            """;

        var chatResponse = new ChatResponse([new ChatMessage(ChatRole.Assistant, responseJson)]);
        _chatClientMock
            .Setup(x => x.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(chatResponse);

        // Act
        var result = await _sut.GenerateQuestionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cv.Id, result.CvId);
        Assert.Equal(job.Id, result.JobId);
        Assert.Single(result.QuestionBlocks);
        Assert.Equal("Backend - C#", result.QuestionBlocks[0].TopicLabel);
    }

    private static Cv CreateTestCv() => new()
    {
        Id = Guid.NewGuid(),
        OriginalText = "Test CV text",
        PersonalInfo = new PersonalInfo { Name = "Test User" },
        Skills = [new Skill { Name = "C#", Category = SkillCategory.Backend, Proficiency = ProficiencyLevel.Advanced }],
        Experience = [new WorkExperience { JobTitle = "Developer", Company = "Test Corp" }],
        Education = [new Education { Degree = "BSc", Institution = "Test University" }],
        ExtractedAt = DateTimeOffset.UtcNow
    };

    private static JobDescription CreateTestJob() => new()
    {
        Id = Guid.NewGuid(),
        OriginalText = "Test job description",
        RequiredSkills = [new Skill { Name = "C#", Category = SkillCategory.Backend, Proficiency = ProficiencyLevel.Intermediate }],
        Competencies = ["Problem-solving"],
        ExperienceRequirements = [],
        ExtractedAt = DateTimeOffset.UtcNow
    };
}
