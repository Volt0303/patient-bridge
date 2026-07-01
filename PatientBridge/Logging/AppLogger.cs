namespace PatientBridge.Logging;

/// <summary>
/// Appends timestamped entries to daily log files (info_YYYYMMDD.log / error_YYYYMMDD.log)
/// under the given directory. Used by the notification layer; not referenced by the
/// parser or output code.
/// </summary>
public class AppLogger
{
    private readonly string _logDirectory;

    public AppLogger(string logDirectory)
    {
        _logDirectory = logDirectory;
        Directory.CreateDirectory(logDirectory);
    }

    public void LogError(string rawData, string reason)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var date = DateTime.Now.ToString("yyyyMMdd");
        var logFile = Path.Combine(_logDirectory, $"error_{date}.log");

        var entry = $"[{timestamp}] {reason}{Environment.NewLine}RAW: {rawData}{Environment.NewLine}";
        File.AppendAllText(logFile, entry);
    }

    public void LogInfo(string message)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var date = DateTime.Now.ToString("yyyyMMdd");
        var logFile = Path.Combine(_logDirectory, $"info_{date}.log");

        var entry = $"[{timestamp}] {message}{Environment.NewLine}";
        File.AppendAllText(logFile, entry);
    }
}
