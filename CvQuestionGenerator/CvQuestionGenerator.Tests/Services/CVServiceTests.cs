using CvQuestionGenerator.API.Models.CV;
using CvQuestionGenerator.API.Repositories;
using CvQuestionGenerator.API.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CvQuestionGenerator.Tests.Services;

public class CVServiceTests
{
    private readonly ICVRepository _mockRepository;
    private readonly IAIService _mockAIService;
    private readonly ILogger<CVService> _mockLogger;
    private readonly CVService _sut;

    public CVServiceTests()
    {
        _mockRepository = Substitute.For<ICVRepository>();
        _mockAIService = Substitute.For<IAIService>();
        _mockLogger = Substitute.For<ILogger<CVService>>();
        _sut = new CVService(_mockRepository, _mockAIService, _mockLogger);
    }

    [Fact]
    public async Task SubmitCVAsync_WithValidInput_StoresCV()
    {
        // Arrange
        var cvText = "John Doe\njohn@example.com\n5 years C# experience";
        var extractedData = new CVExtractedData
        {
            PersonalInfo = new PersonalInfo { Name = "John Doe", Email = "john@example.com" },
            Skills = [new Skill { Name = "C#", ProficiencyLevel = API.Models.ProficiencyLevel.Expert, ExperienceContext = "5 years" }]
        };

        _mockAIService.ExtractCVDataAsync(cvText, Arg.Any<CancellationToken>())
            .Returns(extractedData);

        // Act
        await _sut.SubmitCVAsync(cvText);

        // Assert
        _mockRepository.Received(1).Set(Arg.Is<CVData>(cv => 
            cv.RawText == cvText && 
            cv.ExtractedData == extractedData));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SubmitCVAsync_WithInvalidInput_ThrowsArgumentException(string? invalidCvText)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _sut.SubmitCVAsync(invalidCvText!));
    }

    [Fact]
    public void GetCV_WhenCVExists_ReturnsCV()
    {
        // Arrange
        var storedCV = new CVData
        {
            RawText = "Test CV",
            ExtractedData = new CVExtractedData(),
            CreatedAt = DateTime.UtcNow
        };
        _mockRepository.Get().Returns(storedCV);

        // Act
        var result = _sut.GetCV();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(storedCV.RawText, result.RawText);
    }

    [Fact]
    public void GetCV_WhenNoCVExists_ReturnsNull()
    {
        // Arrange
        _mockRepository.Get().Returns((CVData?)null);

        // Act
        var result = _sut.GetCV();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SubmitCVAsync_ReplacesExistingCV()
    {
        // Arrange
        var oldCV = new CVData { RawText = "Old CV", ExtractedData = new CVExtractedData() };
        var newCvText = "New CV";
        var newExtractedData = new CVExtractedData();

        _mockRepository.Get().Returns(oldCV);
        _mockAIService.ExtractCVDataAsync(newCvText, Arg.Any<CancellationToken>())
            .Returns(newExtractedData);

        // Act
        await _sut.SubmitCVAsync(newCvText);

        // Assert
        _mockRepository.Received(1).Set(Arg.Is<CVData>(cv => cv.RawText == newCvText));
    }
}
