namespace PatientBridge.Parsing;

/// <summary>
/// The parsed patient fields, independent of any input or output format.
/// </summary>
public class CardData
{
    public string PatientId { get; set; } = "";
    public string LastName { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string BirthDate { get; set; } = "";
    public string Gender { get; set; } = "";
}
