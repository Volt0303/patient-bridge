namespace PatientBridge.Output;

/// <summary>
/// Persists already-formatted output text. Kept separate from <see cref="IRecordFormatter"/>
/// so the destination (a file today, could be a network share or API later) can change
/// without touching the format, and vice versa.
/// </summary>
public interface IOutputWriter
{
    void Write(string content);
}
