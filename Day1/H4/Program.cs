using System;

class PatientData
{
    public string Name { get; set; }
    public int Age { get; set; }
    public double WeightKg { get; set; }
    public double HeightM { get; set; }
    public double Temperature { get; set; }
}

class Validator
{
    public static bool TryValidateAge(string input, out int age)
    {
        age = 0;
        if (!int.TryParse(input, out age)) return false;
        return age > 0 && age <= 130;
    }

    public static bool TryValidateWeight(string input, out double weight)
    {
        weight = 0;
        if (!double.TryParse(input, out weight)) return false;
        return weight > 0 && weight <= 500;
    }

    public static bool TryValidateHeight(string input, out double height)
    {
        height = 0;
        if (!double.TryParse(input, out height)) return false;
        return height > 0 && height <= 3.0;
    }

    public static bool TryValidateTemperature(string input, out double temp)
    {
        temp = 0;
        if (!double.TryParse(input, out temp)) return false;
        return temp >= 30.0 && temp <= 45.0;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Hospital Patient Registration ===\n");

        PatientData patient = new PatientData();

        while (string.IsNullOrWhiteSpace(patient.Name))
        {
            Console.Write("Enter Patient Name: ");
            patient.Name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(patient.Name))
                Console.WriteLine("Name cannot be empty.");
        }

        while (true)
        {
            Console.Write("Enter Age (1-130): ");
            if (Validator.TryValidateAge(Console.ReadLine(), out int age)) { patient.Age = age; break; }
            Console.WriteLine("Invalid age.");
        }

        while (true)
        {
            Console.Write("Enter Weight in kg: ");
            if (Validator.TryValidateWeight(Console.ReadLine(), out double w)) { patient.WeightKg = w; break; }
            Console.WriteLine("Invalid weight.");
        }

        while (true)
        {
            Console.Write("Enter Height in meters: ");
            if (Validator.TryValidateHeight(Console.ReadLine(), out double h)) { patient.HeightM = h; break; }
            Console.WriteLine("Invalid height.");
        }

        while (true)
        {
            Console.Write("Enter Temperature in C (30-45): ");
            if (Validator.TryValidateTemperature(Console.ReadLine(), out double t)) { patient.Temperature = t; break; }
            Console.WriteLine("Invalid temperature.");
        }

        double bmi = patient.WeightKg / (patient.HeightM * patient.HeightM);
        string bmiCategory = bmi < 18.5 ? "Underweight" : bmi < 25 ? "Normal" : bmi < 30 ? "Overweight" : "Obese";
        string tempStatus = patient.Temperature >= 37.5 ? "Fever" : "Normal";

        Console.WriteLine();
        Console.WriteLine($"Name        : {patient.Name}");
        Console.WriteLine($"Age         : {patient.Age}");
        Console.WriteLine($"Weight      : {patient.WeightKg} kg");
        Console.WriteLine($"Height      : {patient.HeightM} m");
        Console.WriteLine($"Temperature : {patient.Temperature}C ({tempStatus})");
        Console.WriteLine($"BMI         : {Math.Round(bmi, 2)} ({bmiCategory})");
    }
}