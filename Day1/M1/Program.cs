using System;
class Program
{
    static void Main()
    {
        double price;
        int quantity;
        double discount;

        Console.WriteLine("Enter item price: ");

        if(!double.TryParse(Console.ReadLine(),out price))
        {
            Console.WriteLine("Invalid Price eneterd!!");
            return;
        }

        Console.WriteLine("Enter the quantity:");
        if(!int.TryParse(Console.ReadLine(),out quantity))
        {
            Console.WriteLine("Invalid quantity entered!!");
            return;

        }

        Console.WriteLine("Enter discount percentage: ");
        if(!double.TryParse(Console.ReadLine(),out discount))
        {
            Console.WriteLine("Invalid dicount percentage enetered!!");
            return;
             
        }

        if(price<0 || quantity <=0 || discount < 0)
        {
            Console.WriteLine("Invalid input values!!");
            return;
        }

        double subtotal = price * quantity;
        double discountAmount = subtotal * (discount / 100);
        double finalAmount = subtotal - discountAmount;

        Console.WriteLine("\n-------BILL-------");
        Console.WriteLine("Subtotal: "+Math.Round(subtotal,2));
        Console.WriteLine("Discount: " + Math.Round(discountAmount, 2));
        Console.WriteLine("Final Amount: " + Math.Round(finalAmount, 2));
            
    }
}