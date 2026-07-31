using System;

static class FinancialCalculator
{
    public static double CalculateCompoundInterest(
        double principal,
        double rate,
        double time = 1,
        int compoundingFrequency = 1)
    {
        if (principal <= 0) throw new ArgumentException("Principal must be positive.");
        if (rate < 0) throw new ArgumentException("Rate cannot be negative.");
        if (time <= 0) throw new ArgumentException("Time must be positive.");
        if (compoundingFrequency <= 0) throw new ArgumentException("Compounding frequency must be positive.");

        double futureValue = principal * Math.Pow(1 + rate / compoundingFrequency,
                                                   compoundingFrequency * time);
        return Math.Round(futureValue, 2);
    }

    public static double CalculateCompoundInterest(double principal, double monthlyRate, int months)
    {
        if (principal <= 0) throw new ArgumentException("Principal must be positive.");
        if (monthlyRate < 0) throw new ArgumentException("Rate cannot be negative.");
        if (months <= 0) throw new ArgumentException("Months must be positive.");

        double futureValue = principal * Math.Pow(1 + monthlyRate, months);
        return Math.Round(futureValue, 2);
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== M1: Financial Calculator ===\n");

        double result1 = FinancialCalculator.CalculateCompoundInterest(10000, 0.05, 10);
        Console.WriteLine($"Annually (default): ${result1:N2}");

        double result2 = FinancialCalculator.CalculateCompoundInterest(
            principal: 10000, rate: 0.05, time: 10, compoundingFrequency: 12);
        Console.WriteLine($"Monthly: ${result2:N2}");

        double result3 = FinancialCalculator.CalculateCompoundInterest(10000, 0.05, 10, compoundingFrequency: 4);
        Console.WriteLine($"Quarterly: ${result3:N2}");

        double result4 = FinancialCalculator.CalculateCompoundInterest(10000, 0.004167, 120);
        Console.WriteLine($"Monthly rate overload: ${result4:N2}");
    }
}