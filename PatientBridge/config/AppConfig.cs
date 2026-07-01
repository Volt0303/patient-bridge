using System.Text.Json;
using Microsoft.Win32;

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
    public int BirthDateStart { get; set; } = 34;
    public int BirthDateLength { get; set; } = 7;

    public static AppConfig Load(string path = "config.json")
    {
        if (!File.Exists(path))
            return new AppConfig();

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
    }

    public static void RegisterStartup(string appName, string exePath)
    {
        using var key = Registry.CurrentUser.OpenSubKey(
            @"Software\Microsoft\Windows\CurrentVersion\Run", true);
        key?.SetValue(appName, $"\"{exePath}\"");
    }

    public static void UnregisterStartup(string appName)
    {
        using var key = Registry.CurrentUser.OpenSubKey(
            @"Software\Microsoft\Windows\CurrentVersion\Run", true);
        key?.DeleteValue(appName, false);
    }
}
