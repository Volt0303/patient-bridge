using PatientBridge.Config;

namespace PatientBridge.Core;

public class PatBdtGenerator
{
    private readonly AppConfig _config;

    public PatBdtGenerator(AppConfig config)
    {
        _config = config;
    }

    public void Generate(CardData data)
    {
        Directory.CreateDirectory(_config.OutputDirectory);

        var lines = new[]
        {
            "01380000020",
            "01380000022",
            "01380006100",
            $"0133000{data.PatientId}",
            $"0183101{data.FirstName}",
            $"0133102{data.LastName}",
            $"0173103{data.BirthDate}",
            $"0103110{data.Gender}",
            "01380000023",
            "01380000021"
        };

        var outputPath = Path.Combine(_config.OutputDirectory, _config.OutputFileName);
        File.WriteAllLines(outputPath, lines);
    }
}
