using System.Windows.Forms;
using PatientBridge.Config;
using PatientBridge.Core;

try
{
    // Resolve config from the exe's own folder, not the current working directory.
    // When launched from a Start Menu shortcut the working directory is not the install folder.
    var baseDir = AppContext.BaseDirectory;
    var configPath = Path.Combine(baseDir, "config.json");
    var config = AppConfig.Load(configPath);

    // Logs must go somewhere a standard user can write. Never under Program Files.
    // A relative LogDirectory is rooted under C:\ProgramData\PatientBridge.
    var logDir = Path.IsPathRooted(config.LogDirectory)
        ? config.LogDirectory
        : Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "PatientBridge",
            config.LogDirectory);

    var logger = new AppLogger(logDir);
    var parser = new CardDataParser(config);
    var generator = new PatBdtGenerator(config);

    logger.LogInfo("PatientBridge started.");

    void OnCardScanned(string rawData)
    {
        try
        {
            // Always write the file. Per spec, fields that cannot be read are left
            // blank (TAG only) rather than skipping the file entirely.
            var result = parser.Parse(rawData);
            generator.Generate(result.Data);

            if (result.HasErrors)
                logger.LogError(rawData, string.Join("; ", result.Errors));
            else
                logger.LogInfo($"Success: PatientId={result.Data.PatientId}");
        }
        catch (Exception ex)
        {
            // Unexpected failure (e.g. cannot write the output file).
            logger.LogError(rawData, ex.Message);
        }
    }

    var exePath = Environment.ProcessPath ?? Application.ExecutablePath;
    AppConfig.RegisterStartup("PatientBridge", exePath);

    using var hook = new KeyboardHook(OnCardScanned);

    var trayIcon = new NotifyIcon
    {
        Icon = SystemIcons.Application,
        Text = "PatientBridge",
        Visible = true
    };

    var exitItem = new ToolStripMenuItem("Exit");
    exitItem.Click += (_, _) =>
    {
        AppConfig.UnregisterStartup("PatientBridge");
        trayIcon.Visible = false;
        Application.Exit();
    };

    trayIcon.ContextMenuStrip = new ContextMenuStrip();
    trayIcon.ContextMenuStrip.Items.Add(exitItem);

    Application.Run();
}
catch (Exception ex)
{
    MessageBox.Show(
        $"PatientBridge failed to start:\n\n{ex.Message}",
        "PatientBridge",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error);
}
