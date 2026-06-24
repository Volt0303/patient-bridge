namespace PatientBridge.Core;

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
