using System.Text.Json;
using System.Linq;

namespace CodingProblems;

public record Student(string Name, int Score);

public class Question4_StringFormat
{
    public static string FilterAndSerialize(string[] items, int minScore)
    {
        var students = new System.Collections.Generic.List<Student>();
        
        foreach (string item in items)
        {
            string[] parts = item.Split(':');
            if (parts.Length == 2)
            {
                string name = parts[0];
                if (int.TryParse(parts[1], out int score))
                {
                    students.Add(new Student(name, score));
                }
            }
        }
        
        var filtered = students
            .Where(s => s.Score >= minScore)
            .OrderByDescending(s => s.Score)
            .ThenBy(s => s.Name)
            .ToList();
        
        return JsonSerializer.Serialize(filtered);
    }
}
