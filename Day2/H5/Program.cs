using System;

static class GradeCalculator
{
    public static double GetAverage(double s1, double s2, double s3)
    {
        return (s1 + s2 + s3) / 3;
    }

    public static double GetAverage(params double[] marks)
    {
        if (marks == null || marks.Length == 0) return 0;
        double total = 0;
        foreach (double m in marks) total += m;
        return total / marks.Length;
    }

    public static string GetGrade(double average, string defaultGrade = "N/A")
    {
        if (average == 0) return defaultGrade;
        if (average >= 90) return "A";
        if (average >= 75) return "B";
        if (average >= 60) return "C";
        if (average >= 45) return "D";
        return "F";
    }

    public static bool TryEvaluate(double average, out string grade, out string status)
    {
        grade = GetGrade(average);
        status = average >= 45 ? "Pass" : "Fail";
        return average >= 45;
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== H5: Student Grade System ===\n");

        double avg1 = GradeCalculator.GetAverage(78, 85, 92);
        Console.WriteLine($"3 subjects (78,85,92): avg={avg1:F2}  grade={GradeCalculator.GetGrade(avg1)}");

        double avg2 = GradeCalculator.GetAverage(55, 60, 70, 80, 90);
        Console.WriteLine($"5 subjects params    : avg={avg2:F2}  grade={GradeCalculator.GetGrade(avg2)}");

        Console.WriteLine($"Zero average         : {GradeCalculator.GetGrade(0)}");

        Console.WriteLine();
        double[] tests = { 92.0, 74.0, 55.0, 40.0 };
        foreach (double avg in tests)
        {
            GradeCalculator.TryEvaluate(avg, out string grade, out string status);
            Console.WriteLine($"  avg={avg}  grade={grade}  status={status}");
        }

        Console.WriteLine();
        double m1 = ReadMark("Mark 1: ");
        double m2 = ReadMark("Mark 2: ");
        double m3 = ReadMark("Mark 3: ");

        double userAvg = GradeCalculator.GetAverage(m1, m2, m3);
        GradeCalculator.TryEvaluate(userAvg, out string g, out string s);
        Console.WriteLine($"\nAverage: {userAvg:F2}  Grade: {g}  Result: {s}");
    }

    static double ReadMark(string prompt)
    {
        double value;
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (double.TryParse(input, out value) && value >= 0 && value <= 100)
                return value;
            Console.WriteLine("  Enter a number between 0 and 100.");
        }
    }
}