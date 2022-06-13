using Spectre.Console;
using VoltRpc.Logging;

namespace VoltRpc.Demo.Host;

public class SpectreLogger : ILogger
{
    public SpectreLogger(LogVerbosity logVerbosity)
    {
        LogVerbosity = logVerbosity;
    }
    
    public LogVerbosity LogVerbosity { get; set; }
    
    public void Debug(string message)
    {
        if(LogVerbosity >= LogVerbosity.Debug)
            AnsiConsole.MarkupLine($"[[[silver]DEBUG[/]]]: {message}");
        
    }

    public void Info(string message)
    {
        if(LogVerbosity >= LogVerbosity.Info)
            AnsiConsole.MarkupLine($"[[[white]INFO[/]]]: {message}");
    }

    public void Warn(string message)
    {
        if(LogVerbosity >= LogVerbosity.Warn)
            AnsiConsole.MarkupLine($"[[[yellow]WARN[/]]]: {message}");
    }

    public void Error(string message)
    {
        if(LogVerbosity >= LogVerbosity.Error)
            AnsiConsole.MarkupLine($"[red][[ERROR]] {message}[/]");
    }
}