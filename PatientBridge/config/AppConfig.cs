using System.Text.Json;

namespace PatientBridge.Config;

public class AppConfig
{
    public string OutputDirectory { get; set; } = "C:\\bdt";
    public string OutputFileName { get; set; } = "pat.bdt";
    public string LogDirectory { get; set; } = "logs";
    public int PatientIdStart { get; set; } = 0;
    public int PatientIdLength { get; set; } = 7;
    public int NameStart { get; set; } = 12;
    public int NameLength { get; set; } = 20;
    public int BirthDateStart { get; set; } = 35;
    public int BirthDateLength { get; set; } = 7;
    public int GenderStart { get; set; } = 42;
    public int GenderLength { get; set; } = 1;

    public static AppConfig Load(string path = "config.json")
    {
        if (!File.Exists(path))
            return new AppConfig();

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
    }
}
