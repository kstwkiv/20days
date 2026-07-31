using System;
class Program
{
    static void Main()
    {
        double obalance, tdeposits, twithdrawals;
        Console.WriteLine("Enter the opening balanace:");
        if (!double.TryParse(Console.ReadLine(), out obalance) || obalance < 0)
        {
            Console.WriteLine("Invalid Input");
            return;
        }

        Console.WriteLine("Enter the total deposits:");
        if (!double.TryParse(Console.ReadLine(), out tdeposits) || tdeposits < 0)
        {
            Console.WriteLine("Invalid Input");
            return;
        }

        Console.WriteLine("Enter the total withdrawals:");
        if (!double.TryParse(Console.ReadLine(), out twithdrawals) || twithdrawals < 0)
        {
            Console.WriteLine("Invalid Input");
            return;
        }

        double finalBalance = obalance + tdeposits - twithdrawals;
        Console.WriteLine("Final Balance: " + finalBalance);

    }
}