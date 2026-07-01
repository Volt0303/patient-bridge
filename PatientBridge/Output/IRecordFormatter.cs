using PatientBridge.Parsing;

namespace PatientBridge.Output;

/// <summary>
/// Turns parsed <see cref="CardData"/> into the text of an output file. The concrete
/// implementation owns the entire file format, so a format change touches only it.
/// </summary>
public interface IRecordFormatter
{
    string Format(CardData data);
}
