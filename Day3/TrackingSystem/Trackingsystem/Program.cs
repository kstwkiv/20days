using System;
using System.Collections.Generic;
using System.Linq;

class Passenger
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ContactNumber { get; set; }
}

class Train
{
    public int TrainNumber { get; set; }
    public string Name { get; set; }
    public string Source { get; set; }
    public string Destination { get; set; }
}

class Ticket
{
    public int TicketId { get; set; }
    public Passenger Passenger { get; set; }
    public Train Train { get; set; }
    public string SeatNumber { get; set; }
    public string Class { get; set; } // Sleeper, AC, General
    public double Amount { get; set; }
    public DateTime BookingDate { get; set; }
    public string PaymentStatus { get; set; }
}

class TicketingSystem
{
    private List<Ticket> tickets = new List<Ticket>();

    public void BookTicket(Ticket t)
    {
        tickets.Add(t);
    }

    public double TotalAmountCollected()
    {
        return tickets.Where(t => t.PaymentStatus == "Paid").Sum(t => t.Amount);
    }

    public List<Ticket> GetTicketsByPassenger(int passengerId)
    {
        return tickets.Where(t => t.Passenger.Id == passengerId).ToList();
    }

    public List<Passenger> GetPassengersByTrain(int trainNumber)
    {
        return tickets.Where(t => t.Train.TrainNumber == trainNumber)
                      .Select(t => t.Passenger)
                      .ToList();
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Q4: Train Ticketing System ===\n");

        var p1 = new Passenger { Id = 1, Name = "Avinash", ContactNumber = "9876543210" };
        var p2 = new Passenger { Id = 2, Name = "Priya", ContactNumber = "9123456780" };
        var p3 = new Passenger { Id = 3, Name = "Rahul", ContactNumber = "9988776655" };

        var t1 = new Train { TrainNumber = 12345, Name = "Chennai Express", Source = "Chennai", Destination = "Mumbai" };
        var t2 = new Train { TrainNumber = 67890, Name = "Rajdhani", Source = "Delhi", Destination = "Bangalore" };

        var system = new TicketingSystem();

        system.BookTicket(new Ticket { TicketId = 1, Passenger = p1, Train = t1, SeatNumber = "A1", Class = "AC", Amount = 1200, BookingDate = DateTime.Today, PaymentStatus = "Paid" });
        system.BookTicket(new Ticket { TicketId = 2, Passenger = p2, Train = t1, SeatNumber = "B3", Class = "Sleeper", Amount = 450, BookingDate = DateTime.Today, PaymentStatus = "Paid" });
        system.BookTicket(new Ticket { TicketId = 3, Passenger = p1, Train = t2, SeatNumber = "C2", Class = "AC", Amount = 1800, BookingDate = DateTime.Today, PaymentStatus = "Paid" });
        system.BookTicket(new Ticket { TicketId = 4, Passenger = p3, Train = t2, SeatNumber = "D5", Class = "General", Amount = 300, BookingDate = DateTime.Today, PaymentStatus = "Paid" });
        system.BookTicket(new Ticket { TicketId = 5, Passenger = p2, Train = t2, SeatNumber = "E1", Class = "Sleeper", Amount = 600, BookingDate = DateTime.Today, PaymentStatus = "Paid" });

        Console.WriteLine($"Total Amount Collected: Rs.{system.TotalAmountCollected()}\n");

        Console.WriteLine($"Tickets for Passenger '{p1.Name}':");
        foreach (var t in system.GetTicketsByPassenger(p1.Id))
            Console.WriteLine($"  Ticket#{t.TicketId} | Train: {t.Train.Name} | Seat: {t.SeatNumber} | Class: {t.Class} | Rs.{t.Amount}");

        Console.WriteLine($"\nPassengers on Train '{t1.Name}':");
        foreach (var p in system.GetPassengersByTrain(t1.TrainNumber))
            Console.WriteLine($"  {p.Name} | {p.ContactNumber}");
    }
}