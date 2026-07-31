using System;
class Program
{
    static void Main()
    {
        double length,width,height;

            Console.WriteLine("Enter the Length: ");
            if(!double.TryParse(Console.ReadLine(),out length ) || length<0){
                Console.WriteLine("Invalid Input");
                return;

            }
        

       
            Console.WriteLine("Enter the Height: ");
            if (!double.TryParse(Console.ReadLine(), out height) || height < 0){
                Console.WriteLine("Invalid Input");
                return;

            }
        

        
            Console.WriteLine("Enter the Width: ");
            if (!double.TryParse(Console.ReadLine(), out width) || width < 0){
                Console.WriteLine("Invalid Input");
                return;

            }
        

        double volume = length * width * height;

        Console.WriteLine("\nVolume: " +volume);
    }
}