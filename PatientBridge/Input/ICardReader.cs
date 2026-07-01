namespace PatientBridge.Input;

/// <summary>
/// A source of raw card text. Implementations capture input from a specific
/// device (USB-HID keyboard, serial reader, etc.) and raise <see cref="CardScanned"/>
/// with the raw string. The rest of the app depends only on this interface, so the
/// hardware can be swapped without touching parsing, output, or notification code.
/// </summary>
public interface ICardReader : IDisposable
{
    /// <summary>Raised once per completed scan, carrying the raw card text.</summary>
    event Action<string>? CardScanned;

    /// <summary>Begin listening for scans.</summary>
    void Start();
}
