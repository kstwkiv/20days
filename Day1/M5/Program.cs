using System;
class Program
{
    static void Main()
    {
        string input = Console.ReadLine();
        string[] marks = input.Split(',');
        if (marks.Length != 5)
        {
            Console.WriteLine("please enter exactly 5 marks.");
            return;
        }

        double total = 0;
        foreach(string m in marks)
        {
            double value;
            if(!double.TryParse(m,out value))
            {
                Console.WriteLine("Invalid marks entered.");
                return;
            }

            if (value < 0 || value > 100)
            {
                Console.WriteLine("Marks should be between 0 and 100.");
                return; 
            }

            total += value;
        }

        double avg = total / 5;
        double percentage = (total / 500) * 100;

        Console.WriteLine("Total: " + total);
        Console.WriteLine("Average: " + avg);
        Console.WriteLine("Percentage: " + Math.Round(percentage,2));

    }
}