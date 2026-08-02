using System;

static class Logger
{
    public static string FormatLogMessage(string template, params object[] args)
    {
        if (string.IsNullOrEmpty(template)) return "";
        if (args == null || args.Length == 0) return template;

        string Replace(string t, object[] values)
        {
            string result = t;
            for (int i = 0; i < values.Length; i++)
                result = result.Replace("{" + i + "}", Format(values[i]));
            return result;
        }

        return Replace(template, args);
    }

    private static string Format(object value)
    {
        if (value == null) return "(null)";
        if (value is DateTime dt) return dt.ToString("yyyy-MM-dd HH:mm:ss");
        if (value is string s)
        {
            if (int.TryParse(s, out int n)) return n.ToString();
            if (double.TryParse(s, out double d)) return d.ToString();
            return s;
        }
        return value.ToString() ?? "";
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== H3: Log Message Formatter ===\n");

        Console.WriteLine(Logger.FormatLogMessage(
            "User {0} logged in from {1} at {2}",
            "JohnDoe", "192.168.1.1", DateTime.Now));

        Console.WriteLine(Logger.FormatLogMessage(
            "Order #{0} placed by {1} for {2} items",
            42, "Alice", 3));

        Console.WriteLine(Logger.FormatLogMessage(
            "Retry attempt {0} of {1}", "3", "5"));

        Console.WriteLine(Logger.FormatLogMessage("Server started."));
    }
}