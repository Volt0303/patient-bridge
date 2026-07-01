using PatientBridge.Config;

namespace PatientBridge.Core;

public class CardData
{
    public string PatientId { get; set; } = "";
    public string LastName { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string BirthDate { get; set; } = "";
    public string Gender { get; set; } = "";
}

public class ParseResult
{
    public CardData Data { get; } = new();
    public List<string> Errors { get; } = new();
    public bool HasErrors => Errors.Count > 0;
}

public class CardDataParser
{
    private readonly AppConfig _config;

    public CardDataParser(AppConfig config)
    {
        _config = config;
    }

    // Parses each field independently. A field that cannot be read is left blank
    // (spec: "データが正常に読み込まれない場合は、データをブランクにしておくこと（TAG のみ）")
    // and its error is recorded, but the other fields are still returned.
    public ParseResult Parse(string rawData)
    {
        var result = new ParseResult();

        try
        {
            result.Data.PatientId = Slice(rawData, _config.PatientIdStart, _config.PatientIdLength);
        }
        catch (Exception ex)
        {
            result.Errors.Add($"PatientId: {ex.Message}");
        }

        try
        {
            var fullName = Slice(rawData, _config.NameStart, _config.NameLength);
            // Surname and given name are separated by a space (spec §4).
            var parts = fullName.Split(' ', 2);
            result.Data.LastName = parts[0];
            result.Data.FirstName = parts.Length > 1 ? parts[1] : "";
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Name: {ex.Message}");
        }

        try
        {
            var birthDateRaw = Slice(rawData, _config.BirthDateStart, _config.BirthDateLength);
            result.Data.BirthDate = JapaneseEraConverter.ToWesternDate(birthDateRaw);
        }
        catch (Exception ex)
        {
            result.Errors.Add($"BirthDate: {ex.Message}");
        }

        // Per spec: gender is not stored on the card; output is always female (2).
        result.Data.Gender = "2";

        return result;
    }

    // Returns the trimmed slice, or throws if the raw data is too short for the field.
    private static string Slice(string s, int start, int length)
    {
        if (start < 0 || start >= s.Length)
            throw new ArgumentException($"data too short (need index {start}, length was {s.Length})");

        var end = Math.Min(start + length, s.Length);
        return s.Substring(start, end - start).Trim();
    }
}
