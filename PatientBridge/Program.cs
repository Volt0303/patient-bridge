using System.Windows.Forms;
using PatientBridge.Config;
using PatientBridge.Core;

var config = AppConfig.Load();
var logger = new AppLogger(config.LogDirectory);
var parser = new CardDataParser(config);
var generator = new PatBdtGenerator(config);

logger.LogInfo("PatientBridge started.");

void OnCardScanned(string rawData)
{
    try
    {
        var cardData = parser.Parse(rawData);
        generator.Generate(cardData);
        logger.LogInfo($"Success: PatientId={cardData.PatientId}");
    }
    catch (Exception ex)
    {
        logger.LogError(rawData, ex.Message);
    }
}

using var hook = new KeyboardHook(OnCardScanned);

var trayIcon = new NotifyIcon
{
    Icon = SystemIcons.Application,
    Text = "PatientBridge",
    Visible = true
};

var exitItem = new ToolStripMenuItem("Exit");
exitItem.Click += (_, _) => Application.Exit();

trayIcon.ContextMenuStrip = new ContextMenuStrip();
trayIcon.ContextMenuStrip.Items.Add(exitItem);

Application.Run();

trayIcon.Visible = false;
