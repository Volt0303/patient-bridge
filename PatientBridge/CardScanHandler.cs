using PatientBridge.Notifications;
using PatientBridge.Output;
using PatientBridge.Parsing;

namespace PatientBridge;

/// <summary>
/// Orchestrates one scan end to end: parse → format → write → notify. Depends only on
/// the interfaces, so any layer can be swapped from the composition root without
/// changing this logic. This is the single place the pipeline is defined.
/// </summary>
public class CardScanHandler
{
    private readonly ICardParser _parser;
    private readonly IRecordFormatter _formatter;
    private readonly IOutputWriter _writer;
    private readonly INotifier _notifier;

    public CardScanHandler(
        ICardParser parser,
        IRecordFormatter formatter,
        IOutputWriter writer,
        INotifier notifier)
    {
        _parser = parser;
        _formatter = formatter;
        _writer = writer;
        _notifier = notifier;
    }

    public void Handle(string rawText)
    {
        var result = _parser.Parse(rawText);

        try
        {
            // Always write the file, even with blank fields (spec: blank data, TAG only).
            var content = _formatter.Format(result.Data);
            _writer.Write(content);
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Output: {ex.Message}");
        }

        _notifier.Notify(new ScanOutcome
        {
            RawData = rawText,
            Data = result.Data,
            Errors = result.Errors
        });
    }
}
