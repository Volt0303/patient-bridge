using System.Windows.Forms;

namespace PatientBridge.Notifications;

/// <summary>
/// Shows a Windows tray balloon for scan outcomes. Provided ready-to-use but NOT wired
/// in by default, because popping a balloon on every scan may not be desired. When the
/// client asks for user notifications, enable it in the composition root (Program.cs)
/// with a single line — no other code needs to change.
/// </summary>
public class TrayNotifier : INotifier
{
    private readonly NotifyIcon _trayIcon;
    private readonly bool _notifyOnSuccess;

    public TrayNotifier(NotifyIcon trayIcon, bool notifyOnSuccess = false)
    {
        _trayIcon = trayIcon;
        _notifyOnSuccess = notifyOnSuccess;
    }

    public void Notify(ScanOutcome outcome)
    {
        if (outcome.Success)
        {
            if (_notifyOnSuccess)
                _trayIcon.ShowBalloonTip(3000, "PatientBridge",
                    $"Read patient {outcome.Data.PatientId}", ToolTipIcon.Info);
        }
        else
        {
            _trayIcon.ShowBalloonTip(5000, "PatientBridge",
                "Card could not be read correctly. Some fields are blank.", ToolTipIcon.Warning);
        }
    }
}
