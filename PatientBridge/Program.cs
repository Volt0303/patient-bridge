using PatientBridge.Config;
using PatientBridge.Core;

var config = AppConfig.Load();
var logger = new AppLogger(config.LogDirectory);
var parser = new CardDataParser(config);
var generator = new PatBdtGenerator(config);

logger.LogInfo("PatientBridge started.");
Console.WriteLine("PatientBridge running. Paste card data and press Enter:");

while (true)
{
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        continue;

    try
    {
        var cardData = parser.Parse(input);
        generator.Generate(cardData);
        logger.LogInfo($"Success: PatientId={cardData.PatientId}");
        Console.WriteLine($"OK: {cardData.PatientId} {cardData.LastName} {cardData.FirstName}");
    }
    catch (Exception ex)
    {
        logger.LogError(input, ex.Message);
        Console.WriteLine($"ERROR: {ex.Message}");
    }
}
