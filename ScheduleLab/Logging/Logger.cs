using System.Text;
using ScheduleLab.Services;

namespace ScheduleLab.Logging;

public enum LogEventType
{
    Filtering,
    Transformation,
    Saving
}


public sealed class Logger
{
    private static readonly Lazy<Logger> _instance = new(() => new Logger());
    public static Logger Instance => _instance.Value;

    private readonly object _sync = new();
    public string FilePath { get; }

   
    private Logger()
    {
        FilePath = Path.Combine(AppPaths.OutputFolder, "log.txt");
    }

    public void Log(LogEventType type, string message)
    {
        string line = $"{DateTime.Now:dd.MM.yyyy HH:mm:ss} {TypeToText(type)}. {message}";
        lock (_sync)
        {
            File.AppendAllText(FilePath, line + Environment.NewLine, Encoding.UTF8);
        }
    }

    private static string TypeToText(LogEventType type) => type switch
    {
        LogEventType.Filtering => "Фільтрація",
        LogEventType.Transformation => "Трансформація",
        LogEventType.Saving => "Збереження",
        _ => type.ToString()
    };
}
