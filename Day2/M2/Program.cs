Console.WriteLine("Hello, World!");
using System;
using System.Collections.Generic;

static class LibraryOrderProcessor
{
    public static bool TryParseISBN(string rawIsbn, out string cleanIsbn)
    {
        cleanIsbn = string.Empty;

        if (string.IsNullOrWhiteSpace(rawIsbn))
            return false;

        string stripped = rawIsbn.Replace("-", "").Replace(" ", "").Trim();

        if (stripped.Length != 13)
            return false;

        foreach (char c in stripped)
            if (!char.IsDigit(c)) return false;

        cleanIsbn = stripped;
        return true;
    }

    public static bool TryProcessOrder(out List<string> validISBNs, params string[] rawISBNs)
    {
        validISBNs = new List<string>();

        if (rawISBNs == null || rawISBNs.Length == 0)
            return false;

        foreach (string raw in rawISBNs)
        {
            if (TryParseISBN(raw.Trim(), out string clean))
            {
                validISBNs.Add(clean);
                Console.WriteLine($"  Valid   : {raw.Trim()} -> {clean}");
            }
            else
            {
                Console.WriteLine($"  Invalid : {raw.Trim()}");
            }
        }

        return validISBNs.Count > 0;
    }

    public static bool TryProcessOrder(string csv, out List<string> validISBNs)
    {
        return TryProcessOrder(out validISBNs, csv.Split(','));
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== M2: Library ISBN Order Processor ===\n");

        string input = "978-3-16-148410-0, 1234567890123, invalid-isbn, 978-1-4028-9462-6";
        Console.WriteLine($"Input: {input}\n");

        bool success = LibraryOrderProcessor.TryProcessOrder(input, out List<string> validISBNs);

        Console.WriteLine($"\nResult: {success}");
        foreach (string isbn in validISBNs)
            Console.WriteLine($"  {isbn}");

        Console.WriteLine();

        LibraryOrderProcessor.TryProcessOrder(
            out List<string> v2,
            "978-3-16-148410-0", "BADISBN", "9781402894626");
        Console.WriteLine("params overload: " + string.Join(", ", v2));
    }
}