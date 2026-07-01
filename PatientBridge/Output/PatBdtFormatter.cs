using PatientBridge.Parsing;

namespace PatientBridge.Output;

/// <summary>
/// Builds the pat.bdt text. ALL pat.bdt format knowledge lives here — the TAG numbers,
/// the line order, and the header/footer. Future changes to the pat.bdt format should
/// only require editing this class. TAG and data are concatenated directly (spec §5).
/// A field left blank yields a TAG-only line (spec: blank data on failure).
/// </summary>
public class PatBdtFormatter : IRecordFormatter
{
    // Fixed header / footer lines (no data).
    private const string Header1 = "01380000020";
    private const string Header2 = "01380000022";
    private const string Header3 = "01380006100";
    private const string Footer1 = "01380000023";
    private const string Footer2 = "01380000021";

    // Data-line TAGs (7 chars each), written immediately before their value.
    private const string TagPatientId = "0133000";
    private const string TagFirstName = "0183101";
    private const string TagLastName  = "0133102";
    private const string TagBirthDate = "0173103";
    private const string TagGender    = "0103110";

    public string Format(CardData data)
    {
        var lines = new[]
        {
            Header1,
            Header2,
            Header3,
            $"{TagPatientId}{data.PatientId}",
            $"{TagFirstName}{data.FirstName}",
            $"{TagLastName}{data.LastName}",
            $"{TagBirthDate}{data.BirthDate}",
            $"{TagGender}{data.Gender}",
            Footer1,
            Footer2
        };

        // Trailing newline so every line (including the last) is terminated,
        // matching the original File.WriteAllLines output.
        return string.Join(Environment.NewLine, lines) + Environment.NewLine;
    }
}
