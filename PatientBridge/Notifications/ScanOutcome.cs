using PatientBridge.Parsing;

namespace PatientBridge.Notifications;

/// <summary>
/// The result of processing one scan, passed to every <see cref="INotifier"/>.
/// Carries enough context for any notifier (log, tray balloon, message box) to
/// decide what to show, without reaching back into the parser or output layers.
/// </summary>
public class ScanOutcome
{
    public required string RawData { get; init; }
    public required CardData Data { get; init; }
    public required IReadOnlyList<string> Errors { get; init; }

    public bool Success => Errors.Count == 0;
}
