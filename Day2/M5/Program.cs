using System;

static class MathOperations
{
    public static int Add(int a, int b) => a + b;

    public static int Add(params int[] numbers)
    {
        int sum = 0;
        foreach (int n in numbers) sum += n;
        return sum;
    }

    public static int Multiply(int a, int b) => a * b;

    public static int Multiply(params int[] numbers)
    {
        int product = 1;
        foreach (int n in numbers) product *= n;
        return product;
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== M5: Math Operations ===\n");

        Console.WriteLine($"Add(5, 10)          = {MathOperations.Add(5, 10)}");
        Console.WriteLine($"Add(1,2,3,4,5)      = {MathOperations.Add(1, 2, 3, 4, 5)}");
        Console.WriteLine($"Add(10, 20, 30)     = {MathOperations.Add(10, 20, 30)}");

        Console.WriteLine();

        Console.WriteLine($"Multiply(2, 3)      = {MathOperations.Multiply(2, 3)}");
        Console.WriteLine($"Multiply(2,3,4,5)   = {MathOperations.Multiply(2, 3, 4, 5)}");

        Console.WriteLine();

        int[] scores = { 10, 20, 30, 40 };
        Console.WriteLine($"Add(array)          = {MathOperations.Add(scores)}");
    }
}