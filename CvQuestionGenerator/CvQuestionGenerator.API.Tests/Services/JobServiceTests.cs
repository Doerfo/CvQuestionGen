using CvQuestionGenerator.API.Models;
using CvQuestionGenerator.API.Models.Responses;
using CvQuestionGenerator.API.Services;
using CvQuestionGenerator.API.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace CvQuestionGenerator.API.Tests.Services;

public class JobServiceTests
{
    private readonly Mock<IDataStore> _dataStoreMock = new();
    private readonly Mock<IAiExtractionService> _aiExtractionServiceMock = new();
    private readonly Mock<ILogger<JobService>> _loggerMock = new();
    private readonly JobService _sut;

    public JobServiceTests()
    {
        _sut = new JobService(_dataStoreMock.Object, _aiExtractionServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task UploadJobAsync_CallsAiExtractionAndStoresJob()
    {
        // Arrange
        var jobText = "Test job description";
        var extractionResult = CreateTestExtractionResult();
        _aiExtractionServiceMock
            .Setup(x => x.ExtractJobDataAsync(jobText, It.IsAny<CancellationToken>()))
            .ReturnsAsync(extractionResult);

        // Act
        var result = await _sut.UploadJobAsync(jobText);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Job description successfully processed", result.Message);
        Assert.Equal(extractionResult.RequiredSkills, result.Extraction.RequiredSkills);
        Assert.Equal(extractionResult.Competencies, result.Extraction.Competencies);
        
        _dataStoreMock.Verify(x => x.StoreJob(It.Is<JobDescription>(job => 
            job.OriginalText == jobText &&
            job.RequiredSkills == extractionResult.RequiredSkills &&
            job.Competencies == extractionResult.Competencies
        )), Times.Once);
    }

    [Fact]
    public async Task UploadJobAsync_ReturnsNewGuid()
    {
        // Arrange
        var jobText = "Test job description";
        var extractionResult = CreateTestExtractionResult();
        _aiExtractionServiceMock
            .Setup(x => x.ExtractJobDataAsync(jobText, It.IsAny<CancellationToken>()))
            .ReturnsAsync(extractionResult);

        // Act
        var result = await _sut.UploadJobAsync(jobText);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public void GetJob_WhenJobExists_ReturnsJob()
    {
        // Arrange
        var job = CreateTestJob();
        _dataStoreMock.Setup(x => x.GetJob()).Returns(job);

        // Act
        var result = _sut.GetJob();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(job.Id, result.Id);
    }

    [Fact]
    public void GetJob_WhenNoJobExists_ReturnsNull()
    {
        // Arrange
        _dataStoreMock.Setup(x => x.GetJob()).Returns((JobDescription?)null);

        // Act
        var result = _sut.GetJob();

        // Assert
        Assert.Null(result);
    }

    private static JobExtractionResult CreateTestExtractionResult() => new()
    {
        RequiredSkills = 
        [
            new Skill { Name = "Java", Category = SkillCategory.Backend, Proficiency = ProficiencyLevel.Advanced }
        ],
        Competencies = ["Leadership", "Problem-solving"],
        ExperienceRequirements = 
        [
            new ExperienceRequirement { Area = "Java Development", MinimumLevel = ProficiencyLevel.Advanced, YearsRequired = 5 }
        ]
    };

    private static JobDescription CreateTestJob() => new()
    {
        Id = Guid.NewGuid(),
        OriginalText = "Test job description",
        RequiredSkills = [],
        Competencies = [],
        ExperienceRequirements = [],
        ExtractedAt = DateTimeOffset.UtcNow
    };
}
