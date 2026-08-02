using System;

static class MathRecursion
{
    public static long Factorial(int n)
    {
        if (n < 0) throw new ArgumentException("Number must be non-negative.");
        if (n == 0 || n == 1) return 1;
        return n * Factorial(n - 1);
    }

    public static int Fibonacci(int n)
    {
        if (n < 0) throw new ArgumentException("Number must be non-negative.");
        if (n == 0) return 0;
        if (n == 1) return 1;
        return Fibonacci(n - 1) + Fibonacci(n - 2);
    }

    public static int SumOfDigits(int n, ref int callCount)
    {
        callCount++;
        if (n < 10) return n;
        return (n % 10) + SumOfDigits(n / 10, ref callCount);
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== H4: Recursion - Factorial, Fibonacci, Sum of Digits ===\n");

        Console.WriteLine("Factorials:");
        for (int i = 0; i <= 10; i++)
            Console.WriteLine($"  {i}! = {MathRecursion.Factorial(i)}");

        Console.WriteLine("\nFibonacci:");
        for (int i = 0; i <= 10; i++)
            Console.WriteLine($"  F({i}) = {MathRecursion.Fibonacci(i)}");

        Console.WriteLine("\nSum of Digits:");
        int[] nums = { 123, 4567, 99, 7 };
        foreach (int num in nums)
        {
            int calls = 0;
            int sum = MathRecursion.SumOfDigits(num, ref calls);
            Console.WriteLine($"  SumOfDigits({num}) = {sum}  (calls: {calls})");
        }

        Console.WriteLine();
        Console.Write("Enter a number (0-20) for factorial: ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out int val) && val >= 0 && val <= 20)
            Console.WriteLine($"  {val}! = {MathRecursion.Factorial(val)}");
        else
            Console.WriteLine("  Invalid input.");
    }
}