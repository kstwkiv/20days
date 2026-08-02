using System;
using System.Collections.Generic;

class AppConfiguration
{
    public string SourceName { get; set; } = "";
    public Dictionary<string, string> Settings { get; } = new();

    public void Print()
    {
        Console.WriteLine($"  Source: {SourceName}");
        foreach (var kv in Settings)
            Console.WriteLine($"  {kv.Key} = {kv.Value}");
    }
}

interface IConfigurationSource
{
    string SourceName { get; }
    bool TryLoad(out AppConfiguration config);
}

class EnvSource : IConfigurationSource
{
    public string SourceName => "EnvironmentVariables";

    public bool TryLoad(out AppConfiguration config)
    {
        config = null!;
        Console.WriteLine($"  Trying {SourceName}...");

        string val = Environment.GetEnvironmentVariable("APP_DB") ?? "";
        if (string.IsNullOrEmpty(val))
        {
            Console.WriteLine($"  {SourceName}: no data. Skipping.\n");
            return false;
        }

        config = new AppConfiguration { SourceName = SourceName };
        config.Settings["db"] = val;
        return true;
    }
}

class FileSource : IConfigurationSource
{
    private readonly string _path;
    public string SourceName => "JsonFile";

    public FileSource(string path) => _path = path;

    public bool TryLoad(out AppConfiguration config)
    {
        config = null!;
        Console.WriteLine($"  Trying {SourceName} ({_path})...");

        if (!System.IO.File.Exists(_path))
        {
            Console.WriteLine($"  {SourceName}: file not found. Skipping.\n");
            return false;
        }

        config = new AppConfiguration { SourceName = SourceName };
        config.Settings["file"] = _path;
        return true;
    }
}

class DefaultSource : IConfigurationSource
{
    public string SourceName => "Defaults";

    public bool TryLoad(out AppConfiguration config)
    {
        Console.WriteLine($"  Trying {SourceName}...");
        config = new AppConfiguration { SourceName = SourceName };
        config.Settings["db_host"] = "localhost";
        config.Settings["db_port"] = "5432";
        config.Settings["timeout"] = "30";
        Console.WriteLine($"  {SourceName}: loaded.\n");
        return true;
    }
}

static class ConfigurationLoader
{
    public static bool Load(out AppConfiguration config, params IConfigurationSource[] sources)
    {
        config = null!;
        foreach (IConfigurationSource source in sources)
            if (source.TryLoad(out config)) return true;
        return false;
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== H1: Configuration Loader ===\n");

        bool ok = ConfigurationLoader.Load(
            out AppConfiguration config,
            new EnvSource(),
            new FileSource("app.json"),
            new DefaultSource()
        );

        if (ok)
        {
            Console.WriteLine("Loaded:");
            config.Print();
        }
        else
        {
            Console.WriteLine("No configuration loaded.");
        }
    }
}