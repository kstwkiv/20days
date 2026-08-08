using System;
using System.Collections.Generic;
using System.Linq;

// ==========================================
// LIBRARY ITEM
// ==========================================

public abstract class LibraryItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsAvailable { get; private set; }

    protected LibraryItem(int id, string title)
    {
        Id = id;
        Title = title;
        IsAvailable = true;
    }

    internal void BorrowItem()
    {
        if (!IsAvailable)
            throw new InvalidOperationException(
                "Book is already borrowed."
            );

        IsAvailable = false;
    }

    internal void ReturnItem()
    {
        IsAvailable = true;
    }
}


// ==========================================
// BOOK
// ==========================================

public partial class Book : LibraryItem
{
    public string Author { get; set; }

    public Book(int id, string title, string author)
        : base(id, title)
    {
        Author = author;
    }
}


// ==========================================
// BOOK - GENERATED CODE
// ==========================================

public partial class Book
{
    // Imagine this was generated automatically
    public string ISBN { get; set; }

    public string GetBookInfo()
    {
        return $"{Title} by {Author}";
    }
}


// ==========================================
// MAGAZINE
// ==========================================

public class Magazine : LibraryItem
{
    public int IssueNumber { get; set; }

    public Magazine(int id, string title, int issueNumber)
        : base(id, title)
    {
        IssueNumber = issueNumber;
    }
}


// ==========================================
// JOURNAL
// ==========================================

public class Journal : LibraryItem
{
    public string ResearchArea { get; set; }

    public Journal(int id, string title, string researchArea)
        : base(id, title)
    {
        ResearchArea = researchArea;
    }
}


// ==========================================
// GENERIC REPOSITORY
// ==========================================

public class Repository<T> where T : LibraryItem
{
    private List<T> items = new List<T>();

    public void Add(T item)
    {
        items.Add(item);
    }

    public T GetById(int id)
    {
        return items.FirstOrDefault(item => item.Id == id);
    }

    public List<T> GetAll()
    {
        return items;
    }

    public void Remove(int id)
    {
        T item = GetById(id);

        if (item != null)
        {
            items.Remove(item);
        }
    }
}


// ==========================================
// LIBRARY
// ==========================================

public class Library
{
    private List<LibraryItem> items =
        new List<LibraryItem>();


    public void AddItem(LibraryItem item)
    {
        items.Add(item);
    }


    // ======================================
    // INDEXER
    // ======================================

    public LibraryItem this[string title]
    {
        get
        {
            return items.FirstOrDefault(
                item => item.Title.Equals(
                    title,
                    StringComparison.OrdinalIgnoreCase
                )
            );
        }
    }


    // ======================================
    // BORROW
    // ======================================

    public void Borrow(string title)
    {
        LibraryItem item = this[title];

        if (item == null)
        {
            Console.WriteLine("Item not found.");
            return;
        }

        if (!item.IsAvailable)
        {
            Console.WriteLine("Item is already borrowed.");
            return;
        }

        item.BorrowItem();

        Console.WriteLine(
            $"{item.Title} borrowed successfully."
        );
    }


    // ======================================
    // RETURN
    // ======================================

    public void Return(string title)
    {
        LibraryItem item = this[title];

        if (item == null)
        {
            Console.WriteLine("Item not found.");
            return;
        }

        item.ReturnItem();

        Console.WriteLine(
            $"{item.Title} returned successfully."
        );
    }


    // ======================================
    // SEARCH
    // ======================================

    public List<LibraryItem> Search(string keyword)
    {
        return items
            .Where(item =>
                item.Title.Contains(
                    keyword,
                    StringComparison.OrdinalIgnoreCase
                ))
            .ToList();
    }
}


// ==========================================
// EXTENSION METHODS
// ==========================================

public static class LibraryExtensions
{
    public static List<Book> GetAvailableBooks(
        this List<Book> books)
    {
        return books
            .Where(book => book.IsAvailable)
            .ToList();
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
        // GENERIC REPOSITORY
        // ==================================

        Repository<Book> bookRepository =
            new Repository<Book>();

        Repository<Magazine> magazineRepository =
            new Repository<Magazine>();

        Repository<Journal> journalRepository =
            new Repository<Journal>();


        // ==================================
        // CREATE BOOKS
        // ==================================

        Book book1 = new Book(
            1,
            "Clean Code",
            "Robert C. Martin"
        );

        Book book2 = new Book(
            2,
            "The Pragmatic Programmer",
            "Andrew Hunt"
        );


        // ==================================
        // ADD TO REPOSITORY
        // ==================================

        bookRepository.Add(book1);
        bookRepository.Add(book2);


        // ==================================
        // MAGAZINE
        // ==================================

        Magazine magazine =
            new Magazine(
                3,
                "Tech Monthly",
                25
            );

        magazineRepository.Add(magazine);


        // ==================================
        // JOURNAL
        // ==================================

        Journal journal =
            new Journal(
                4,
                "AI Research",
                "Artificial Intelligence"
            );

        journalRepository.Add(journal);


        // ==================================
        // LIBRARY
        // ==================================

        Library library = new Library();

        library.AddItem(book1);
        library.AddItem(book2);
        library.AddItem(magazine);
        library.AddItem(journal);


        // ==================================
        // INDEXER
        // ==================================

        LibraryItem item = library["Clean Code"];

        if (item != null)
        {
            Console.WriteLine(
                "Found: " + item.Title
            );
        }


        // ==================================
        // BORROW
        // ==================================

        library.Borrow("Clean Code");


        // ==================================
        // SEARCH
        // ==================================

        Console.WriteLine("\nSearch Results:");

        List<LibraryItem> results =
            library.Search("AI");

        foreach (LibraryItem result in results)
        {
            Console.WriteLine(result.Title);
        }


        // ==================================
        // RETURN
        // ==================================

        library.Return("Clean Code");


        // ==================================
        // EXTENSION METHOD
        // ==================================

        List<Book> books =
            bookRepository.GetAll();

        List<Book> availableBooks =
            books.GetAvailableBooks();

        Console.WriteLine("\nAvailable Books:");

        foreach (Book book in availableBooks)
        {
            Console.WriteLine(book.Title);
        }
    }
}