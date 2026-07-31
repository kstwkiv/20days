using System;

interface IInvestmentCalculator
{
    double CalculateReturn(double principal, double annualRatePercent, int years);
    string InvestmentType { get; }
}

class SimpleInterestCalculator : IInvestmentCalculator
{
    public string InvestmentType => "Simple Interest";

    public double CalculateReturn(double principal, double annualRatePercent, int years)
    {
        double interest = (principal * annualRatePercent * years) / 100;
        return principal + interest;
    }
}

class CompoundInterestCalculator : IInvestmentCalculator
{
    public string InvestmentType => "Compound Interest";

    public double CalculateReturn(double principal, double annualRatePercent, int years)
    {
        return principal * Math.Pow(1 + (annualRatePercent / 100), years);
    }
}

class RecurringDepositCalculator : IInvestmentCalculator
{
    public string InvestmentType => "Recurring Deposit";

    public double CalculateReturn(double monthlyDeposit, double annualRatePercent, int years)
    {
        double monthlyRate = annualRatePercent / (12 * 100);
        int months = years * 12;
        return monthlyDeposit * ((Math.Pow(1 + monthlyRate, months) - 1) / monthlyRate) * (1 + monthlyRate);
    }
}

class InvestmentFactory
{
    public static IInvestmentCalculator GetCalculator(string type)
    {
        switch (type.Trim().ToLower())
        {
            case "simple": return new SimpleInterestCalculator();
            case "compound": return new CompoundInterestCalculator();
            case "recurring": return new RecurringDepositCalculator();
            default: return null;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Investment Calculator ===\n");

        IInvestmentCalculator calculator = null;
        string investmentType = "";
        while (calculator == null)
        {
            Console.Write("Enter Investment Type (Simple / Compound / Recurring): ");
            investmentType = Console.ReadLine();
            calculator = InvestmentFactory.GetCalculator(investmentType);
            if (calculator == null)
                Console.WriteLine("Please enter Simple, Compound, or Recurring.");
        }

        double principal = ReadPositiveDouble(
            calculator.InvestmentType == "Recurring Deposit"
                ? "Enter Monthly Deposit Amount: "
                : "Enter Principal Amount: ");

        double rate = ReadRateDouble("Enter Annual Interest Rate (%): ");
        int years = ReadPositiveInt("Enter Duration (years): ");

        double projectedValue = calculator.CalculateReturn(principal, rate, years);
        double totalInvested = calculator.InvestmentType == "Recurring Deposit"
                                ? principal * years * 12
                                : principal;

        Console.WriteLine();
        Console.WriteLine($"Type            : {calculator.InvestmentType}");
        Console.WriteLine($"Principal       : {principal}");
        Console.WriteLine($"Rate            : {rate}%");
        Console.WriteLine($"Duration        : {years} year(s)");
        Console.WriteLine($"Total Invested  : {Math.Round(totalInvested, 2)}");
        Console.WriteLine($"Projected Value : {Math.Round(projectedValue, 2)}");
        Console.WriteLine($"Total Return    : {Math.Round(projectedValue - totalInvested, 2)}");
    }

    static double ReadPositiveDouble(string prompt)
    {
        double value;
        while (true)
        {
            Console.Write(prompt);
            if (!double.TryParse(Console.ReadLine(), out value) || value <= 0)
            { Console.WriteLine("Enter a valid positive number."); continue; }
            break;
        }
        return value;
    }

    static double ReadRateDouble(string prompt)
    {
        double value;
        while (true)
        {
            Console.Write(prompt);
            if (!double.TryParse(Console.ReadLine(), out value) || value <= 0 || value > 100)
            { Console.WriteLine("Enter a rate between 0.01 and 100."); continue; }
            break;
        }
        return value;
    }

    static int ReadPositiveInt(string prompt)
    {
        int value;
        while (true)
        {
            Console.Write(prompt);
            if (!int.TryParse(Console.ReadLine(), out value) || value <= 0)
            { Console.WriteLine("Enter a valid positive number."); continue; }
            break;
        }
        return value;
    }
}