using CvQuestionGenerator.API.Models.JobDescription;

namespace CvQuestionGenerator.API.Repositories;

/// <summary>
/// Singleton repository for storing a single job description in memory.
/// </summary>
public class SingletonJobDescRepository : IJobDescRepository
{
    private JobDescriptionData? _currentJobDesc;
    private readonly object _lock = new();

    /// <inheritdoc/>
    public void Set(JobDescriptionData jobDescData)
    {
        lock (_lock)
        {
            _currentJobDesc = jobDescData;
        }
    }

    /// <inheritdoc/>
    public JobDescriptionData? Get()
    {
        lock (_lock)
        {
            return _currentJobDesc;
        }
    }

    /// <inheritdoc/>
    public bool Exists()
    {
        lock (_lock)
        {
            return _currentJobDesc is not null;
        }
    }
}
