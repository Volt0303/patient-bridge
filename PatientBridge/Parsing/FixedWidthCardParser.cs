using PatientBridge.Config;

namespace PatientBridge.Parsing;

/// <summary>
/// Parses fixed-width card text using the field offsets in <see cref="AppConfig"/>.
/// Field mappings (start/length) live in config.json so they can be changed without
/// recompiling. Each field is parsed independently: a field that cannot be read is
/// left blank and its error recorded, while the other fields still come through.
/// </summary>
public class FixedWidthCardParser : ICardParser
{
    private readonly AppConfig _config;

    public FixedWidthCardParser(AppConfig config)
    {
        _config = config;
    }

    public ParseResult Parse(string rawText)
    {
        var result = new ParseResult();

        try
        {
            result.Data.PatientId = Slice(rawText, _config.PatientIdStart, _config.PatientIdLength);
        }
        catch (Exception ex)
        {
            result.Errors.Add($"PatientId: {ex.Message}");
        }

        try
        {
            var fullName = Slice(rawText, _config.NameStart, _config.NameLength);
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
            var birthDateRaw = Slice(rawText, _config.BirthDateStart, _config.BirthDateLength);
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

    // Returns the trimmed slice, or throws if the raw text is too short for the field.
    private static string Slice(string s, int start, int length)
    {
        if (start < 0 || start >= s.Length)
            throw new ArgumentException($"data too short (need index {start}, length was {s.Length})");

        var end = Math.Min(start + length, s.Length);
        return s.Substring(start, end - start).Trim();
    }
}
