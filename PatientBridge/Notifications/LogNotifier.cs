using PatientBridge.Logging;

namespace PatientBridge.Notifications;

/// <summary>
/// Writes each scan outcome to the log files. This is the always-on notifier;
/// user-facing channels can be added alongside it via <see cref="CompositeNotifier"/>.
/// </summary>
public class LogNotifier : INotifier
{
    private readonly AppLogger _logger;

    public LogNotifier(AppLogger logger)
    {
        _logger = logger;
    }

    public void Notify(ScanOutcome outcome)
    {
        if (outcome.Success)
            _logger.LogInfo($"Success: PatientId={outcome.Data.PatientId}");
        else
            _logger.LogError(outcome.RawData, string.Join("; ", outcome.Errors));
    }
}
