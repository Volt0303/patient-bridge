using System.Windows.Forms;
using PatientBridge;
using PatientBridge.Config;
using PatientBridge.Input;
using PatientBridge.Logging;
using PatientBridge.Notifications;
using PatientBridge.Output;
using PatientBridge.Parsing;

try
{
    // Resolve config from the exe's own folder, not the current working directory.
    var baseDir = AppContext.BaseDirectory;
    var config = AppConfig.Load(Path.Combine(baseDir, "config.json"));

    // Logs must go somewhere a standard user can write. Never under Program Files.
    // A relative LogDirectory is rooted under C:\ProgramData\PatientBridge.
    var logDir = Path.IsPathRooted(config.LogDirectory)
        ? config.LogDirectory
        : Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "PatientBridge",
            config.LogDirectory);

    // ---- Composition root: this is the ONE place concrete implementations are chosen ----
    var logger = new AppLogger(logDir);

    ICardParser parser = new FixedWidthCardParser(config);
    IRecordFormatter formatter = new PatBdtFormatter();
    IOutputWriter writer = new FileOutputWriter(config.OutputDirectory, config.OutputFileName);

    // Notification channels. Logging is always on. To also show tray balloons, add a
    // TrayNotifier here, e.g.:  new CompositeNotifier(new LogNotifier(logger), new TrayNotifier(trayIcon))
    INotifier notifier = new CompositeNotifier(new LogNotifier(logger));

    var handler = new CardScanHandler(parser, formatter, writer, notifier);
    // ------------------------------------------------------------------------------------

    logger.LogInfo("PatientBridge started.");

    using var reader = new KeyboardCardReader();
    reader.CardScanned += handler.Handle;
    reader.Start();

    var exePath = Environment.ProcessPath ?? Application.ExecutablePath;
    AppConfig.RegisterStartup("PatientBridge", exePath);

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
