using System;

enum LogLevel { Trace, Debug, Info, Warning, Error, Fatal, Unknown }

static class LogParser
{
    public static bool ParseLogLine(
        in string logLine,
        out DateTime timestamp,
        out LogLevel level,
        ref int linesProcessed)
    {
        timestamp = DateTime.MinValue;
        level = LogLevel.Unknown;

        if (string.IsNullOrWhiteSpace(logLine))
            return false;

        string[] parts = logLine.Split(' ', 3);
        if (parts.Length < 3) return false;

        if (!DateTime.TryParse($"{parts[0]} {parts[1]}", out timestamp))
            return false;

        string rest = parts[2];
        int colon = rest.IndexOf(':');
        string levelStr = colon >= 0 ? rest[..colon].Trim() : rest.Trim();

        if (!Enum.TryParse<LogLevel>(levelStr, ignoreCase: true, out level))
            level = LogLevel.Unknown;

        linesProcessed++;
        return true;
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== M3: Log File Parser ===\n");

        int count = 0;

        string[] lines =
        {
            "2023-10-27 14:30:00 ERROR: Disk full",
            "2023-10-27 14:31:05 WARNING: Memory high",
            "2023-10-27 14:32:10 INFO: Service started",
            "INVALID LINE"
        };

        foreach (string line in lines)
        {
            bool ok = LogParser.ParseLogLine(in line, out DateTime ts, out LogLevel lvl, ref count);

            if (ok)
                Console.WriteLine($"  [{lvl}] {ts:yyyy-MM-dd HH:mm:ss}  |  {line}");
            else
                Console.WriteLine($"  Failed to parse: {line}");
        }

        Console.WriteLine($"\nTotal parsed: {count}");
    }
}