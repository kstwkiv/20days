using BikeRental;
using System;
using System.Collections.Generic;

public class Program
{
    public static SortedDictionary<int, Bike> bikeDetails =
        new SortedDictionary<int, Bike>();

    public static void Main(string[] args)
    {
        BikeUtility utility = new BikeUtility();

        while (true)
        {
            Console.WriteLine("1. Add Bike Details");
            Console.WriteLine("2. Group Bikes By Brand");
            Console.WriteLine("3. Exit");

            Console.Write("Enter your choice: ");
            int choice = int.Parse(Console.ReadLine());

            if (choice == 1)
            {
                Console.Write("Enter the model: ");
                string model = Console.ReadLine();

                Console.Write("Enter the brand: ");
                string brand = Console.ReadLine();

                Console.Write("Enter the price per day: ");
                int pricePerDay = int.Parse(Console.ReadLine());

                utility.AddBikeDetails(
                    model,
                    brand,
                    pricePerDay
                );

                Console.WriteLine(
                    "Bike details added successfully"
                );
            }
            else if (choice == 2)
            {
                SortedDictionary<string, List<Bike>> grouped =
                    utility.GroupBikesByBrand();

                foreach (KeyValuePair<string, List<Bike>> item in grouped)
                {
                    foreach (Bike bike in item.Value)
                    {
                        Console.WriteLine(
                            item.Key + " " + bike.Model
                        );
                    }
                }
            }
            else if (choice == 3)
            {
                break;
            }
        }
    }
}