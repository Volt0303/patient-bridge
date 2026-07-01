namespace PatientBridge.Notifications;

/// <summary>
/// Reports the outcome of a scan to the user or a log. This layer is fully independent
/// of parsing and output: adding a new channel (tray balloon, message box, sound, etc.)
/// means adding one <see cref="INotifier"/> implementation and wiring it in the
/// composition root — no changes to business logic.
/// </summary>
public interface INotifier
{
    void Notify(ScanOutcome outcome);
}
