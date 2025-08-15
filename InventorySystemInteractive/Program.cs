using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventorySystemInteractive
{
    // a) Immutable record
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // b) Marker interface
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // c) Generic InventoryLogger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private readonly List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public void Add(T item) => _log.Add(item);

        public List<T> GetAll() => new List<T>(_log);

        public void SaveToFile()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_log, options);
                using var writer = new StreamWriter(_filePath);
                writer.Write(json);
                Console.WriteLine($"Data successfully saved to {_filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Save Error] {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine("[Load Warning] File not found. Starting with empty log.");
                    return;
                }

                using var reader = new StreamReader(_filePath);
                var json = reader.ReadToEnd();
                var items = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                _log.Clear();
                _log.AddRange(items);
                Console.WriteLine($"Data successfully loaded from {_filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Load Error] {ex.Message}");
            }
        }
    }

    // f) InventoryApp
    public class InventoryApp
    {
        private readonly InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Notebook", 50, DateTime.Now));
            _logger.Add(new InventoryItem(2, "Pen", 200, DateTime.Now));
            _logger.Add(new InventoryItem(3, "Stapler", 15, DateTime.Now));
            _logger.Add(new InventoryItem(4, "Marker", 80, DateTime.Now));
            _logger.Add(new InventoryItem(5, "Envelope Pack", 30, DateTime.Now));

            Console.WriteLine("Sample data seeded.");
        }

        public void AddItemInteractive()
        {
            try
            {
                Console.Write("Enter Item ID: ");
                int id = int.Parse(Console.ReadLine()!);

                Console.Write("Enter Item Name: ");
                string name = Console.ReadLine()!;

                Console.Write("Enter Quantity: ");
                int qty = int.Parse(Console.ReadLine()!);

                var item = new InventoryItem(id, name, qty, DateTime.Now);
                _logger.Add(item);

                Console.WriteLine("Item added successfully.");
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input format. Please enter numbers for ID and Quantity.");
            }
        }

        public void SaveData() => _logger.SaveToFile();

        public void LoadData() => _logger.LoadFromFile();

        public void PrintAllItems()
        {
            var items = _logger.GetAll();
            if (items.Count == 0)
            {
                Console.WriteLine("No items in inventory.");
                return;
            }

            Console.WriteLine("Inventory Items:");
            foreach (var item in items)
                Console.WriteLine($"ID={item.Id}, Name={item.Name}, Quantity={item.Quantity}, Added={item.DateAdded:g}");
        }
    }

    class Program
    {
        static void Main()
        {
            string file = "inventory.json";
            var app = new InventoryApp(file);

            while (true)
            {
                Console.WriteLine("\n--- Inventory Management ---");
                Console.WriteLine("1. Seed Sample Data");
                Console.WriteLine("2. Add New Item");
                Console.WriteLine("3. Save Inventory to File");
                Console.WriteLine("4. Load Inventory from File");
                Console.WriteLine("5. Print All Items");
                Console.WriteLine("6. Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine()!;
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        app.SeedSampleData();
                        break;
                    case "2":
                        app.AddItemInteractive();
                        break;
                    case "3":
                        app.SaveData();
                        break;
                    case "4":
                        app.LoadData();
                        break;
                    case "5":
                        app.PrintAllItems();
                        break;
                    case "6":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }
    }
}
