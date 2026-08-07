using System;
using System.Collections.Generic;

namespace RestaurantMenu
{
    class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }

    class CourseCategory
    {
        public string Name { get; set; } // e.g. Starters, Main Course, Desserts
        public List<MenuItem> Items { get; set; } = new List<MenuItem>();
    }

    class Menu
    {
        public bool IsSpecialMenu { get; set; }
        public List<CourseCategory> Categories { get; set; } = new List<CourseCategory>();

        // List the total number of menu items
        public int GetTotalItems()
        {
            int count = 0;
            foreach (var category in Categories)
            {
                count += category.Items.Count;
            }
            return count;
        }

        // List all the menu items for a particular course category
        public List<MenuItem> GetItemsByCategory(string categoryName)
        {
            foreach (var category in Categories)
            {
                if (category.Name == categoryName)
                {
                    return category.Items;
                }
            }
            return new List<MenuItem>();
        }
    }

    class Restaurant
    {
        public List<Menu> Menus { get; set; } = new List<Menu>();

        // List all the special discount menu
        public List<Menu> GetSpecialDiscountMenus()
        {
            List<Menu> specialMenus = new List<Menu>();
            foreach (var menu in Menus)
            {
                if (menu.IsSpecialMenu)
                {
                    specialMenus.Add(menu);
                }
            }
            return specialMenus;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Menu regularMenu = new Menu { IsSpecialMenu = false };

      
            CourseCategory starters = new CourseCategory { Name = "Starters" };
            starters.Items.Add(new MenuItem { Id = 1, Name = "Soup", Price = 50 });
            starters.Items.Add(new MenuItem { Id = 2, Name = "Spring Roll", Price = 80 });
            regularMenu.Categories.Add(starters);

            CourseCategory mainCourse = new CourseCategory { Name = "Main Course" };
            mainCourse.Items.Add(new MenuItem { Id = 3, Name = "Paneer Tikka", Price = 150 });
            regularMenu.Categories.Add(mainCourse);

            CourseCategory desserts = new CourseCategory { Name = "Desserts" };
            desserts.Items.Add(new MenuItem { Id = 4, Name = "Ice Cream", Price = 40 });
            regularMenu.Categories.Add(desserts);

            Menu specialMenu = new Menu { IsSpecialMenu = true };
           

            Restaurant rest = new Restaurant();
            rest.Menus.Add(regularMenu);
            rest.Menus.Add(specialMenu);

            Console.WriteLine($"Total items in regular menu: {regularMenu.GetTotalItems()}");

            Console.WriteLine("\nItems in 'Starters' category:");
            foreach (var item in regularMenu.GetItemsByCategory("Starters"))
            {
                Console.WriteLine($"- {item.Name} (Rs. {item.Price})");
            }

            Console.WriteLine($"\nTotal special discount menus available: {rest.GetSpecialDiscountMenus().Count}");
        }
    }
}