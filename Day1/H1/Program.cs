using System;

interface IBillingCalculator
{
    double CalculateBill(double unitsConsumed, double ratePerUnit, double fixedCharges);
}

class ResidentialBilling : IBillingCalculator
{
    public double CalculateBill(double unitsConsumed, double ratePerUnit, double fixedCharges)
    {
        return (unitsConsumed * ratePerUnit) + fixedCharges;
    }
}

class CommercialBilling : IBillingCalculator
{
    private const double SurchargeRate = 0.15;

    public double CalculateBill(double unitsConsumed, double ratePerUnit, double fixedCharges)
    {
        double baseBill = (unitsConsumed * ratePerUnit) + fixedCharges;
        return baseBill + (baseBill * SurchargeRate);
    }
}

class BillingFactory
{
    public static IBillingCalculator GetCalculator(string customerType)
    {
        switch (customerType.Trim().ToLower())
        {
            case "residential": return new ResidentialBilling();
            case "commercial": return new CommercialBilling();
            default: return null;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Electricity Billing Calculator ===\n");

        string customerType = "";
        IBillingCalculator calculator = null;
        while (calculator == null)
        {
            Console.Write("Enter Customer Type (Residential / Commercial): ");
            customerType = Console.ReadLine();
            calculator = BillingFactory.GetCalculator(customerType);
            if (calculator == null)
                Console.WriteLine("Please enter Residential or Commercial.");
        }

        double units = ReadPositiveDouble("Enter Units Consumed (kWh): ");
        double rate = ReadPositiveDouble("Enter Rate per Unit: ");
        double fixedCharges = ReadNonNegativeDouble("Enter Fixed Charges: ");

        double totalBill = calculator.CalculateBill(units, rate, fixedCharges);

        Console.WriteLine();
        Console.WriteLine($"Customer Type  : {customerType.Trim()}");
        Console.WriteLine($"Units Consumed : {units} kWh");
        Console.WriteLine($"Rate per Unit  : {rate}");
        Console.WriteLine($"Fixed Charges  : {fixedCharges}");
        if (customerType.Trim().ToLower() == "commercial")
            Console.WriteLine("Surcharge 15% applied.");
        Console.WriteLine($"Total Bill     : {Math.Round(totalBill, 2)}");
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

    static double ReadNonNegativeDouble(string prompt)
    {
        double value;
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (!double.TryParse(input, out value) || value < 0)
            {
                Console.WriteLine("Enter a valid non-negative number.");
                continue;
            }
            break;
        }
        return value;
    }
}