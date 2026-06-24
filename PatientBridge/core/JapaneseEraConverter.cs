namespace PatientBridge.Core;

public static class JapaneseEraConverter
{
    public static string ToWesternDate(string eraDate)
    {
        if (string.IsNullOrWhiteSpace(eraDate) || eraDate.Length < 7)
            throw new ArgumentException($"Invalid era date format: {eraDate}");

        char era = char.ToUpper(eraDate[0]);
        if (!int.TryParse(eraDate.Substring(1, 2), out int year))
            throw new ArgumentException($"Invalid year in era date: {eraDate}");
        if (!int.TryParse(eraDate.Substring(3, 2), out int month))
            throw new ArgumentException($"Invalid month in era date: {eraDate}");
        if (!int.TryParse(eraDate.Substring(5, 2), out int day))
            throw new ArgumentException($"Invalid day in era date: {eraDate}");

        int westernYear = era switch
        {
            'T' => 1911 + year,
            'S' => 1925 + year,
            'H' => 1988 + year,
            'R' => 2018 + year,
            _ => throw new ArgumentException($"Unknown era: {era}")
        };

        return $"{day:D2}{month:D2}{westernYear}";
    }
}
