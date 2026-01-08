using CvQuestionGenerator.API.Models;
using CvQuestionGenerator.API.Storage;

namespace CvQuestionGenerator.API.Tests.Storage;

public class InMemoryDataStoreTests
{
    private readonly InMemoryDataStore _sut = new();

    [Fact]
    public void GetCv_WhenNoCvStored_ReturnsNull()
    {
        // Act
        var result = _sut.GetCv();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void StoreCv_ThenGetCv_ReturnsSameCv()
    {
        // Arrange
        var cv = CreateTestCv();

        // Act
        _sut.StoreCv(cv);
        var result = _sut.GetCv();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cv.Id, result.Id);
        Assert.Equal(cv.OriginalText, result.OriginalText);
    }

    [Fact]
    public void StoreCv_WhenCvAlreadyExists_OverwritesPreviousCv()
    {
        // Arrange
        var cv1 = CreateTestCv();
        var cv2 = CreateTestCv();

        // Act
        _sut.StoreCv(cv1);
        _sut.StoreCv(cv2);
        var result = _sut.GetCv();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cv2.Id, result.Id);
    }

    [Fact]
    public void GetJob_WhenNoJobStored_ReturnsNull()
    {
        // Act
        var result = _sut.GetJob();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void StoreJob_ThenGetJob_ReturnsSameJob()
    {
        // Arrange
        var job = CreateTestJob();

        // Act
        _sut.StoreJob(job);
        var result = _sut.GetJob();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(job.Id, result.Id);
        Assert.Equal(job.OriginalText, result.OriginalText);
    }

    [Fact]
    public void StoreJob_WhenJobAlreadyExists_OverwritesPreviousJob()
    {
        // Arrange
        var job1 = CreateTestJob();
        var job2 = CreateTestJob();

        // Act
        _sut.StoreJob(job1);
        _sut.StoreJob(job2);
        var result = _sut.GetJob();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(job2.Id, result.Id);
    }

    [Fact]
    public void Clear_RemovesBothCvAndJob()
    {
        // Arrange
        _sut.StoreCv(CreateTestCv());
        _sut.StoreJob(CreateTestJob());

        // Act
        _sut.Clear();

        // Assert
        Assert.Null(_sut.GetCv());
        Assert.Null(_sut.GetJob());
    }

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
