using CvQuestionGenerator.API.Models.JobDescription;
using CvQuestionGenerator.API.Repositories;
using CvQuestionGenerator.API.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CvQuestionGenerator.Tests.Services;

public class JobDescServiceTests
{
    private readonly IJobDescRepository _mockRepository;
    private readonly IAIService _mockAIService;
    private readonly ILogger<JobDescService> _mockLogger;
    private readonly JobDescService _sut;

    public JobDescServiceTests()
    {
        _mockRepository = Substitute.For<IJobDescRepository>();
        _mockAIService = Substitute.For<IAIService>();
        _mockLogger = Substitute.For<ILogger<JobDescService>>();
        _sut = new JobDescService(_mockRepository, _mockAIService, _mockLogger);
    }

    [Fact]
    public async Task SubmitJobDescriptionAsync_WithValidInput_StoresJobDescription()
    {
        // Arrange
        var jobDescText = "Senior C# Developer\n5+ years experience\nAngular knowledge required";
        var extractedData = new JobDescExtractedData
        {
            RequiredSkills = [
                new RequiredSkill { Name = "C#", RequiredLevel = API.Models.ProficiencyLevel.Expert },
                new RequiredSkill { Name = "Angular", RequiredLevel = API.Models.ProficiencyLevel.Intermediate }
            ],
            ExperienceLevel = "5+ years",
            KeyCompetencies = ["Problem solving", "Team collaboration"]
        };

        _mockAIService.ExtractJobDescriptionDataAsync(jobDescText, Arg.Any<CancellationToken>())
            .Returns(extractedData);

        // Act
        await _sut.SubmitJobDescriptionAsync(jobDescText);

        // Assert
        _mockRepository.Received(1).Set(Arg.Is<JobDescriptionData>(jd => 
            jd.RawText == jobDescText && 
            jd.ExtractedData == extractedData));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SubmitJobDescriptionAsync_WithInvalidInput_ThrowsArgumentException(string? invalidText)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _sut.SubmitJobDescriptionAsync(invalidText!));
    }

    [Fact]
    public void GetJobDescription_WhenExists_ReturnsJobDescription()
    {
        // Arrange
        var storedJobDesc = new JobDescriptionData
        {
            RawText = "Test Job Description",
            ExtractedData = new JobDescExtractedData(),
            CreatedAt = DateTime.UtcNow
        };
        _mockRepository.Get().Returns(storedJobDesc);

        // Act
        var result = _sut.GetJobDescription();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(storedJobDesc.RawText, result.RawText);
    }

    [Fact]
    public void GetJobDescription_WhenNotExists_ReturnsNull()
    {
        // Arrange
        _mockRepository.Get().Returns((JobDescriptionData?)null);

        // Act
        var result = _sut.GetJobDescription();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SubmitJobDescriptionAsync_ReplacesExistingJobDescription()
    {
        // Arrange
        var oldJobDesc = new JobDescriptionData { RawText = "Old JD", ExtractedData = new JobDescExtractedData() };
        var newJobDescText = "New Job Description";
        var newExtractedData = new JobDescExtractedData();

        _mockRepository.Get().Returns(oldJobDesc);
        _mockAIService.ExtractJobDescriptionDataAsync(newJobDescText, Arg.Any<CancellationToken>())
            .Returns(newExtractedData);

        // Act
        await _sut.SubmitJobDescriptionAsync(newJobDescText);

        // Assert
        _mockRepository.Received(1).Set(Arg.Is<JobDescriptionData>(jd => jd.RawText == newJobDescText));
    }
}
