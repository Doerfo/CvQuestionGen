using CvQuestionGenerator.API.Models;
using CvQuestionGenerator.API.Models.Responses;
using CvQuestionGenerator.API.Services;
using CvQuestionGenerator.API.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace CvQuestionGenerator.API.Tests.Services;

public class CvServiceTests
{
    private readonly Mock<IDataStore> _dataStoreMock = new();
    private readonly Mock<IAiExtractionService> _aiExtractionServiceMock = new();
    private readonly Mock<ILogger<CvService>> _loggerMock = new();
    private readonly CvService _sut;

    public CvServiceTests()
    {
        _sut = new CvService(_dataStoreMock.Object, _aiExtractionServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task UploadCvAsync_CallsAiExtractionAndStoresCv()
    {
        // Arrange
        var cvText = "Test CV text";
        var extractionResult = CreateTestExtractionResult();
        _aiExtractionServiceMock
            .Setup(x => x.ExtractCvDataAsync(cvText, It.IsAny<CancellationToken>()))
            .ReturnsAsync(extractionResult);

        // Act
        var result = await _sut.UploadCvAsync(cvText);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("CV successfully processed", result.Message);
        Assert.Equal(extractionResult.PersonalInfo, result.Extraction.PersonalInfo);
        Assert.Equal(extractionResult.Skills, result.Extraction.Skills);
        
        _dataStoreMock.Verify(x => x.StoreCv(It.Is<Cv>(cv => 
            cv.OriginalText == cvText &&
            cv.PersonalInfo == extractionResult.PersonalInfo &&
            cv.Skills == extractionResult.Skills
        )), Times.Once);
    }

    [Fact]
    public async Task UploadCvAsync_ReturnsNewGuid()
    {
        // Arrange
        var cvText = "Test CV text";
        var extractionResult = CreateTestExtractionResult();
        _aiExtractionServiceMock
            .Setup(x => x.ExtractCvDataAsync(cvText, It.IsAny<CancellationToken>()))
            .ReturnsAsync(extractionResult);

        // Act
        var result = await _sut.UploadCvAsync(cvText);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public void GetCv_WhenCvExists_ReturnsCv()
    {
        // Arrange
        var cv = CreateTestCv();
        _dataStoreMock.Setup(x => x.GetCv()).Returns(cv);

        // Act
        var result = _sut.GetCv();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cv.Id, result.Id);
    }

    [Fact]
    public void GetCv_WhenNoCvExists_ReturnsNull()
    {
        // Arrange
        _dataStoreMock.Setup(x => x.GetCv()).Returns((Cv?)null);

        // Act
        var result = _sut.GetCv();

        // Assert
        Assert.Null(result);
    }

    private static CvExtractionResult CreateTestExtractionResult() => new()
    {
        PersonalInfo = new PersonalInfo { Name = "Test User", Title = "Developer" },
        Skills = 
        [
            new Skill { Name = "C#", Category = SkillCategory.Backend, Proficiency = ProficiencyLevel.Advanced }
        ],
        Experience = 
        [
            new WorkExperience { JobTitle = "Developer", Company = "Test Corp" }
        ],
        Education = 
        [
            new Education { Degree = "BSc", Institution = "Test University" }
        ]
    };

    private static Cv CreateTestCv() => new()
    {
        Id = Guid.NewGuid(),
        OriginalText = "Test CV text",
        PersonalInfo = new PersonalInfo { Name = "Test User" },
        Skills = [],
        Experience = [],
        Education = [],
        ExtractedAt = DateTimeOffset.UtcNow
    };
}
