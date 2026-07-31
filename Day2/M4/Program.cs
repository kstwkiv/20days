using System;

static class GeometryLibrary
{
    public static double CalculateArea(double radius, int decimals = 2)
    {
        if (radius <= 0) throw new ArgumentException("Radius must be positive.");
        return Math.Round(Math.PI * radius * radius, decimals);
    }

    public static double CalculateArea(double length, double width)
    {
        if (length <= 0 || width <= 0) throw new ArgumentException("Dimensions must be positive.");
        return length * width;
    }

    public static double CalculateArea(double baseLength, double height, bool isTriangle)
    {
        if (baseLength <= 0 || height <= 0) throw new ArgumentException("Dimensions must be positive.");
        return 0.5 * baseLength * height;
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== M4: Geometry Library ===\n");

        Console.WriteLine($"Circle    r=5 (default 2dp) : {GeometryLibrary.CalculateArea(5)}");
        Console.WriteLine($"Rectangle 4x6               : {GeometryLibrary.CalculateArea(4, 6)}");
        Console.WriteLine($"Triangle  base=3 h=7        : {GeometryLibrary.CalculateArea(3, 7, true)}");
        Console.WriteLine($"Circle    r=5 (4dp, named)  : {GeometryLibrary.CalculateArea(radius: 5, decimals: 4)}");
        Console.WriteLine($"Circle    r=10 (6dp, named) : {GeometryLibrary.CalculateArea(radius: 10, decimals: 6)}");
        Console.WriteLine($"Rectangle 12x5              : {GeometryLibrary.CalculateArea(12, 5)}");
        Console.WriteLine($"Triangle  base=8 h=6        : {GeometryLibrary.CalculateArea(8, 6, true)}");
    }
}