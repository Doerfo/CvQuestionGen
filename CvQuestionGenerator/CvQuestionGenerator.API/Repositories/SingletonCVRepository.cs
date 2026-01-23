using CvQuestionGenerator.API.Models.CV;

namespace CvQuestionGenerator.API.Repositories;

/// <summary>
/// Singleton repository for storing a single CV in memory.
/// </summary>
public class SingletonCVRepository : ICVRepository
{
    private CVData? _currentCV;
    private readonly object _lock = new();

    /// <inheritdoc/>
    public void Set(CVData cvData)
    {
        lock (_lock)
        {
            _currentCV = cvData;
        }
    }

    /// <inheritdoc/>
    public CVData? Get()
    {
        lock (_lock)
        {
            return _currentCV;
        }
    }

    /// <inheritdoc/>
    public bool Exists()
    {
        lock (_lock)
        {
            return _currentCV is not null;
        }
    }
}
