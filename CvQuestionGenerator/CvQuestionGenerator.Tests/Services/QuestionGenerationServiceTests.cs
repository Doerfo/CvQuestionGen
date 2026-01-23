using CvQuestionGenerator.API.Models;
using CvQuestionGenerator.API.Models.CV;
using CvQuestionGenerator.API.Models.JobDescription;
using CvQuestionGenerator.API.Models.Questions;
using CvQuestionGenerator.API.Repositories;
using CvQuestionGenerator.API.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CvQuestionGenerator.Tests.Services;

public class QuestionGenerationServiceTests
{
    private readonly ICVRepository _mockCVRepository;
    private readonly IJobDescRepository _mockJobDescRepository;
    private readonly IAIService _mockAIService;
    private readonly ILogger<QuestionGenerationService> _mockLogger;
    private readonly QuestionGenerationService _sut;

    public QuestionGenerationServiceTests()
    {
        _mockCVRepository = Substitute.For<ICVRepository>();
        _mockJobDescRepository = Substitute.For<IJobDescRepository>();
        _mockAIService = Substitute.For<IAIService>();
        _mockLogger = Substitute.For<ILogger<QuestionGenerationService>>();
        _sut = new QuestionGenerationService(_mockCVRepository, _mockJobDescRepository, _mockAIService, _mockLogger);
    }

    [Fact]
    public async Task GenerateQuestionsAsync_WithValidData_ReturnsQuestionSet()
    {
        // Arrange
        var cvData = new CVData
        {
            RawText = "Test CV",
            ExtractedData = new CVExtractedData
            {
                Skills = [new Skill { Name = "C#", ProficiencyLevel = ProficiencyLevel.Advanced }]
            }
        };

        var jobDescData = new JobDescriptionData
        {
            RawText = "Test JD",
            ExtractedData = new JobDescExtractedData
            {
                RequiredSkills = [new RequiredSkill { Name = "C#", RequiredLevel = ProficiencyLevel.Intermediate }]
            }
        };

        var expectedQuestionSet = new QuestionSet
        {
            TopicGroups = [
                new TopicGroup
                {
                    Topic = "Backend - C#",
                    DifficultyLevel = ProficiencyLevel.Intermediate,
                    Questions = [
                        new Question
                        {
                            QuestionText = "What is dependency injection?",
                            AnswerGuidelines = "Candidate should explain DI concept",
                            KeyTerms = [new KeyTerm { Term = "DI", Explanation = "Design pattern for loose coupling" }],
                            SuggestedFollowUps = ["How does DI help with testing?"]
                        }
                    ]
                }
            ]
        };

        _mockCVRepository.Exists().Returns(true);
        _mockJobDescRepository.Exists().Returns(true);
        _mockCVRepository.Get().Returns(cvData);
        _mockJobDescRepository.Get().Returns(jobDescData);
        _mockAIService.GenerateQuestionsAsync(cvData.ExtractedData, jobDescData.ExtractedData, Arg.Any<CancellationToken>())
            .Returns(expectedQuestionSet);

        // Act
        var result = await _sut.GenerateQuestionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.TopicGroups);
        Assert.Equal("Backend - C#", result.TopicGroups[0].Topic);
    }

    [Fact]
    public async Task GenerateQuestionsAsync_WithoutCV_ThrowsInvalidOperationException()
    {
        // Arrange
        _mockCVRepository.Exists().Returns(false);
        _mockJobDescRepository.Exists().Returns(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.GenerateQuestionsAsync());
        Assert.Contains("CV must be uploaded", exception.Message);
    }

    [Fact]
    public async Task GenerateQuestionsAsync_WithoutJobDescription_ThrowsInvalidOperationException()
    {
        // Arrange
        _mockCVRepository.Exists().Returns(true);
        _mockJobDescRepository.Exists().Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.GenerateQuestionsAsync());
        Assert.Contains("Job description must be uploaded", exception.Message);
    }

    [Fact]
    public async Task GenerateQuestionsAsync_CallsAIServiceWithCorrectData()
    {
        // Arrange
        var cvExtracted = new CVExtractedData
        {
            Skills = [new Skill { Name = "Angular", ProficiencyLevel = ProficiencyLevel.Intermediate }]
        };
        var jobDescExtracted = new JobDescExtractedData
        {
            RequiredSkills = [new RequiredSkill { Name = "Angular", RequiredLevel = ProficiencyLevel.Intermediate }]
        };

        var cvData = new CVData { RawText = "CV", ExtractedData = cvExtracted };
        var jobDescData = new JobDescriptionData { RawText = "JD", ExtractedData = jobDescExtracted };

        _mockCVRepository.Exists().Returns(true);
        _mockJobDescRepository.Exists().Returns(true);
        _mockCVRepository.Get().Returns(cvData);
        _mockJobDescRepository.Get().Returns(jobDescData);
        _mockAIService.GenerateQuestionsAsync(Arg.Any<CVExtractedData>(), Arg.Any<JobDescExtractedData>(), Arg.Any<CancellationToken>())
            .Returns(new QuestionSet());

        // Act
        await _sut.GenerateQuestionsAsync();

        // Assert
        await _mockAIService.Received(1).GenerateQuestionsAsync(
            Arg.Is<CVExtractedData>(cv => cv.Skills.Count == 1 && cv.Skills[0].Name == "Angular"),
            Arg.Is<JobDescExtractedData>(jd => jd.RequiredSkills.Count == 1 && jd.RequiredSkills[0].Name == "Angular"),
            Arg.Any<CancellationToken>());
    }
}
