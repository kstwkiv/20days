using System;
using System.Collections.Generic;

// ==========================================
// ABSTRACT EMPLOYEE
// ==========================================

public abstract class Employee
{
    private int id;
    private string name;
    private decimal salary;

    public int Id
    {
        get { return id; }
        set
        {
            if (value <= 0)
                throw new ArgumentException("ID must be greater than 0.");

            id = value;
        }
    }

    public string Name
    {
        get { return name; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty.");

            name = value;
        }
    }

    public decimal Salary
    {
        get { return salary; }
        set
        {
            if (value < 0)
                throw new ArgumentException("Salary cannot be negative.");

            salary = value;
        }
    }

    // Abstract methods
    public abstract decimal CalculateSalary();

    public abstract decimal CalculateBonus();
}


// ==========================================
// PERMANENT EMPLOYEE
// ==========================================

public class PermanentEmployee : Employee
{
    public override decimal CalculateSalary()
    {
        return Salary;
    }

    public override decimal CalculateBonus()
    {
        return Salary * 0.10m;
    }
}


// ==========================================
// CONTRACT EMPLOYEE
// ==========================================

public class ContractEmployee : Employee
{
    public int WorkingDays { get; set; }

    public decimal DailyRate { get; set; }

    public override decimal CalculateSalary()
    {
        return WorkingDays * DailyRate;
    }

    public override decimal CalculateBonus()
    {
        return CalculateSalary() * 0.05m;
    }
}


// ==========================================
// INTERN
// ==========================================

public class Intern : Employee
{
    public override decimal CalculateSalary()
    {
        return Salary;
    }

    public override decimal CalculateBonus()
    {
        return 0;
    }
}


// ==========================================
// PAYROLL ENGINE
// ==========================================

public class PayrollEngine
{
    public void GenerateReport(List<Employee> employees)
    {
        Console.WriteLine("========== PAYROLL REPORT ==========");

        foreach (Employee employee in employees)
        {
            // Anonymous type
            var report = new
            {
                EmployeeId = employee.Id,
                EmployeeName = employee.Name,
                Salary = employee.CalculateSalary(),
                Bonus = employee.CalculateBonus(),
                TotalPay =
                    employee.CalculateSalary()
                    + employee.CalculateBonus()
            };

            Console.WriteLine(
                $"ID: {report.EmployeeId}"
            );

            Console.WriteLine(
                $"Name: {report.EmployeeName}"
            );

            Console.WriteLine(
                $"Salary: ₹{report.Salary}"
            );

            Console.WriteLine(
                $"Bonus: ₹{report.Bonus}"
            );

            Console.WriteLine(
                $"Total Pay: ₹{report.TotalPay}"
            );

            Console.WriteLine("-----------------------------------");
        }
    }
}


// ==========================================
// PROGRAM
// ==========================================

public class Program
{
    public static void Main()
    {
        // ==================================
        // OBJECT INITIALIZERS
        // ==================================

        var emp1 = new PermanentEmployee
        {
            Id = 1,
            Name = "Pankaj",
            Salary = 60000
        };

        var emp2 = new ContractEmployee
        {
            Id = 2,
            Name = "Rahul",
            WorkingDays = 20,
            DailyRate = 2000
        };

        var emp3 = new Intern
        {
            Id = 3,
            Name = "Priya",
            Salary = 15000
        };


        // ==================================
        // STORE EMPLOYEES
        // ==================================

        List<Employee> employees =
            new List<Employee>
            {
                emp1,
                emp2,
                emp3
            };


        // ==================================
        // GENERATE PAYROLL
        // ==================================

        PayrollEngine payroll =
            new PayrollEngine();

        payroll.GenerateReport(employees);
    }
}
