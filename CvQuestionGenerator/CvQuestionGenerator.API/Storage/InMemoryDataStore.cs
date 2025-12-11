using CvQuestionGenerator.API.Models;

namespace CvQuestionGenerator.API.Storage;

/// <summary>
/// Thread-safe in-memory storage for CV and Job Description data.
/// Data is lost on application restart.
/// </summary>
public sealed class InMemoryDataStore : IDataStore
{
    private readonly object _lock = new();
    private Cv? _cv;
    private JobDescription? _job;

    /// <inheritdoc/>
    public void StoreCv(Cv cv)
    {
        lock (_lock)
        {
            _cv = cv;
        }
    }

    /// <inheritdoc/>
    public Cv? GetCv()
    {
        lock (_lock)
        {
            return _cv;
        }
    }

    /// <inheritdoc/>
    public void StoreJob(JobDescription job)
    {
        lock (_lock)
        {
            _job = job;
        }
    }

    /// <inheritdoc/>
    public JobDescription? GetJob()
    {
        lock (_lock)
        {
            return _job;
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        lock (_lock)
        {
            _cv = null;
            _job = null;
        }
    }
}
