namespace PatientBridge.Output;

/// <summary>
/// Writes the output text to a file, overwriting each time so the file always holds
/// exactly one patient (spec §5). The target directory is created if missing.
/// </summary>
public class FileOutputWriter : IOutputWriter
{
    private readonly string _directory;
    private readonly string _fileName;

    public FileOutputWriter(string directory, string fileName)
    {
        _directory = directory;
        _fileName = fileName;
    }

    public void Write(string content)
    {
        Directory.CreateDirectory(_directory);
        var path = Path.Combine(_directory, _fileName);
        File.WriteAllText(path, content);
    }
}
