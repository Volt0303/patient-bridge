namespace PatientBridge.Parsing;

/// <summary>
/// Turns raw card text into structured <see cref="ParseResult"/>. Accepts a plain
/// string so it is completely independent of the card-reader hardware.
/// </summary>
public interface ICardParser
{
    ParseResult Parse(string rawText);
}
