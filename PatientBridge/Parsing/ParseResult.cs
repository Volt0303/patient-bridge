namespace PatientBridge.Parsing;

/// <summary>
/// The outcome of parsing one scan: the extracted <see cref="Data"/> plus a list of
/// per-field errors. Fields that could not be read are left blank in <see cref="Data"/>
/// and their reason recorded in <see cref="Errors"/> (spec: blank data, TAG only).
/// </summary>
public class ParseResult
{
    public CardData Data { get; } = new();
    public List<string> Errors { get; } = new();
    public bool HasErrors => Errors.Count > 0;
}
