using System;
class Program
{
    static void Main()
    {
        decimal weight;
        int height;

        Console.WriteLine("Enter your weight in kgs: ");
        if(!decimal.TryParse(Console.ReadLine(),out weight))
        {
            Console.WriteLine("Invalid input for weight.");
            return;
        }

        Console.WriteLine("Enter the height in meters: ");
        if(!int.TryParse(Console.ReadLine(),out height))
        {
            Console.WriteLine("Invalid input for height!!");
            return;
        }

        if(height<0 || weight < 0)
        {
            Console.WriteLine("Height and weight cannot be negative.");
            return;
        }

        double bmi = (double)weight / (height * height);
        Console.WriteLine("Your BMI is : " + bmi);
        if (bmi < 18.5)
        {
            Console.WriteLine("You are UnderWeight");
        }
        else if(bmi>=18.5 && bmi < 24.9)
        {
            Console.WriteLine("You are Normal weight");
        }
        else
        {
            Console.WriteLine("You are Obese");

        }

    }
}