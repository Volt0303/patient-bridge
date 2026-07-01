namespace PatientBridge.Notifications;

/// <summary>
/// Fans a single outcome out to several notifiers. Lets the composition root combine,
/// say, logging and a tray balloon without any layer knowing about the others.
/// One failing notifier does not stop the rest.
/// </summary>
public class CompositeNotifier : INotifier
{
    private readonly IReadOnlyList<INotifier> _notifiers;

    public CompositeNotifier(params INotifier[] notifiers)
    {
        _notifiers = notifiers;
    }

    public void Notify(ScanOutcome outcome)
    {
        foreach (var notifier in _notifiers)
        {
            try
            {
                notifier.Notify(outcome);
            }
            catch
            {
                // A notification channel must never break scan processing.
            }
        }
    }
}
