using System;

interface IShippingCalculator
{
    double CalculateCost(double weightKg, double distanceKm);
}

class StandardShipping : IShippingCalculator
{
    public double CalculateCost(double weightKg, double distanceKm)
    {
        return weightKg * distanceKm * 0.05;
    }
}

class ExpressShipping : IShippingCalculator
{
    public double CalculateCost(double weightKg, double distanceKm)
    {
        return (weightKg * distanceKm * 0.10) + 50.0;
    }
}

class FragileShipping : IShippingCalculator
{
    public double CalculateCost(double weightKg, double distanceKm)
    {
        return (weightKg * distanceKm * 0.075) + 100.0;
    }
}

class ShippingFactory
{
    public static IShippingCalculator GetCalculator(string packageType)
    {
        switch (packageType.Trim().ToLower())
        {
            case "standard": return new StandardShipping();
            case "express": return new ExpressShipping();
            case "fragile": return new FragileShipping();
            default: return null;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Shipping Cost Calculator ===\n");

        IShippingCalculator calculator = null;
        string packageType = "";
        while (calculator == null)
        {
            Console.Write("Enter Package Type (Standard / Express / Fragile): ");
            packageType = Console.ReadLine();
            calculator = ShippingFactory.GetCalculator(packageType);
            if (calculator == null)
                Console.WriteLine("Please enter Standard, Express, or Fragile.");
        }

        double weight = ReadPositiveDouble("Enter Package Weight (kg): ");
        double distance = ReadPositiveDouble("Enter Shipping Distance (km): ");

        double cost = calculator.CalculateCost(weight, distance);

        Console.WriteLine();
        Console.WriteLine($"Package Type  : {packageType.Trim()}");
        Console.WriteLine($"Weight        : {weight} kg");
        Console.WriteLine($"Distance      : {distance} km");
        Console.WriteLine($"Shipping Cost : {Math.Round(cost, 2)}");
    }

    static double ReadPositiveDouble(string prompt)
    {
        double value;
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (!double.TryParse(input, out value) || value <= 0)
            {
                Console.WriteLine("Enter a valid positive number.");
                continue;
            }
            break;
        }
        return value;
    }
}