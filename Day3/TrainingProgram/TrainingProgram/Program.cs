using System;
using System.Collections.Generic;
using System.Linq;

class Schedule
{
    public DateTime Date { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string Venue { get; set; }
}

class TrainingProgram
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Course { get; set; }
    public string Topic { get; set; }
    public string Provider { get; set; }
    public Schedule Schedule { get; set; }
}

class TrainingManagement
{
    private List<TrainingProgram> programs = new List<TrainingProgram>();

    public void AddProgram(TrainingProgram p)
    {
        programs.Add(p);
    }

    public List<TrainingProgram> GetByProvider(string provider)
    {
        return programs.Where(p => p.Provider.Equals(provider, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public List<TrainingProgram> GetByDate(DateTime date)
    {
        return programs.Where(p => p.Schedule.Date.Date == date.Date).ToList();
    }

    public List<TrainingProgram> GetByCourse(string course)
    {
        return programs.Where(p => p.Course.Equals(course, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Q2: Training Program Management ===\n");

        var mgmt = new TrainingManagement();

        mgmt.AddProgram(new TrainingProgram
        {
            Id = 1,
            Name = "C# Basics",
            Course = "C#",
            Topic = "Syntax",
            Provider = "TechLearn",
            Schedule = new Schedule { Date = new DateTime(2026, 8, 1), StartTime = "9:00", EndTime = "12:00", Venue = "Hall A" }
        });

        mgmt.AddProgram(new TrainingProgram
        {
            Id = 2,
            Name = "OOP Deep Dive",
            Course = "C#",
            Topic = "OOP",
            Provider = "CodeAcademy",
            Schedule = new Schedule { Date = new DateTime(2026, 8, 1), StartTime = "13:00", EndTime = "17:00", Venue = "Hall B" }
        });

        mgmt.AddProgram(new TrainingProgram
        {
            Id = 3,
            Name = "SQL Fundamentals",
            Course = "SQL",
            Topic = "Queries",
            Provider = "TechLearn",
            Schedule = new Schedule { Date = new DateTime(2026, 8, 2), StartTime = "9:00", EndTime = "12:00", Venue = "Hall C" }
        });

        mgmt.AddProgram(new TrainingProgram
        {
            Id = 4,
            Name = "Advanced SQL",
            Course = "SQL",
            Topic = "Stored Procedures",
            Provider = "DataPro",
            Schedule = new Schedule { Date = new DateTime(2026, 8, 2), StartTime = "13:00", EndTime = "17:00", Venue = "Hall A" }
        });

        Console.WriteLine("Programs by provider 'TechLearn':");
        foreach (var p in mgmt.GetByProvider("TechLearn"))
            Console.WriteLine($"  {p.Name} | Course: {p.Course} | {p.Schedule.Date:dd-MMM-yyyy} {p.Schedule.StartTime}");

        Console.WriteLine("\nPrograms on 2026-08-01:");
        foreach (var p in mgmt.GetByDate(new DateTime(2026, 8, 1)))
            Console.WriteLine($"  {p.Name} | Provider: {p.Provider} | {p.Schedule.StartTime}-{p.Schedule.EndTime} @ {p.Schedule.Venue}");

        Console.WriteLine("\nPrograms for Course 'SQL':");
        foreach (var p in mgmt.GetByCourse("SQL"))
            Console.WriteLine($"  {p.Name} | Provider: {p.Provider} | {p.Schedule.Date:dd-MMM-yyyy}");
    }
}