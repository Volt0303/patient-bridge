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

public class CardDataParser
{
    private readonly AppConfig _config;

    public CardDataParser(AppConfig config)
    {
        _config = config;
    }

    public CardData Parse(string rawData)
    {
        if (rawData.Length < _config.GenderStart + _config.GenderLength)
            throw new ArgumentException($"Raw data too short: {rawData}");

        var patientId = rawData.Substring(_config.PatientIdStart, _config.PatientIdLength).Trim();
        var fullName = rawData.Substring(_config.NameStart, _config.NameLength).Trim();
        var birthDateRaw = rawData.Substring(_config.BirthDateStart, _config.BirthDateLength).Trim();
        var genderRaw = rawData.Substring(_config.GenderStart, _config.GenderLength).Trim();

        var nameParts = fullName.Split(' ', 2);
        var lastName = nameParts[0];
        var firstName = nameParts.Length > 1 ? nameParts[1] : "";

        var birthDate = JapaneseEraConverter.ToWesternDate(birthDateRaw);

        var gender = genderRaw.ToUpper() switch
        {
            "M" => "1",
            "F" => "2",
            _ => throw new ArgumentException($"Unknown gender: {genderRaw}")
        };

        return new CardData
        {
            PatientId = patientId,
            LastName = lastName,
            FirstName = firstName,
            BirthDate = birthDate,
            Gender = gender
        };
    }
}
