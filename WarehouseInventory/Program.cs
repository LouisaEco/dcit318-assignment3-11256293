using System;
using System.Collections.Generic;

namespace InventorySystemInteractive
{
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id; Name = name; Quantity = quantity; Brand = brand; WarrantyMonths = warrantyMonths;
        }

        public override string ToString() =>
            $"ElectronicItem {{ Id={Id}, Name={Name}, Qty={Quantity}, Brand={Brand}, Warranty={WarrantyMonths}m }}";
    }

    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id; Name = name; Quantity = quantity; ExpiryDate = expiryDate;
        }

        public override string ToString() =>
            $"GroceryItem {{ Id={Id}, Name={Name}, Qty={Quantity}, Expiry={ExpiryDate:d} }}";
    }

    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
        }

        public List<T> GetAllItems() => new(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");
            var item = GetItemById(id);
            item.Quantity = newQuantity;
        }
    }

    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(1, "Phone", 10, "TechBrand", 24));
            _electronics.AddItem(new ElectronicItem(2, "Laptop", 5, "CompPro", 12));
            _electronics.AddItem(new ElectronicItem(3, "Headset", 20, "SoundX", 6));

            _groceries.AddItem(new GroceryItem(101, "Rice 5kg", 30, DateTime.Today.AddMonths(12)));
            _groceries.AddItem(new GroceryItem(102, "Milk", 50, DateTime.Today.AddDays(20)));
            _groceries.AddItem(new GroceryItem(103, "Eggs (tray)", 40, DateTime.Today.AddDays(10)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
                Console.WriteLine(" - " + item);
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Stock increased: ID {id} -> {item.Quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Removed item ID {id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] {ex.Message}");
            }
        }

        public InventoryRepository<ElectronicItem> Electronics => _electronics;
        public InventoryRepository<GroceryItem> Groceries => _groceries;
    }

    class Program
    {
        static void Main()
        {
            var manager = new WareHouseManager();
            manager.SeedData();

            while (true)
            {
                Console.WriteLine("\n--- Warehouse Inventory Menu ---");
                Console.WriteLine("1. View Grocery Items");
                Console.WriteLine("2. View Electronic Items");
                Console.WriteLine("3. Add Grocery Item");
                Console.WriteLine("4. Add Electronic Item");
                Console.WriteLine("5. Increase Stock");
                Console.WriteLine("6. Remove Item");
                Console.WriteLine("7. Exit");
                Console.Write("Choose option: ");
                var choice = Console.ReadLine();

                try
                {
                    if (choice == "1") manager.PrintAllItems(manager.Groceries);
                    else if (choice == "2") manager.PrintAllItems(manager.Electronics);
                    else if (choice == "3")
                    {
                        Console.Write("Enter ID: "); int id = int.Parse(Console.ReadLine());
                        Console.Write("Enter Name: "); string name = Console.ReadLine();
                        Console.Write("Enter Quantity: "); int qty = int.Parse(Console.ReadLine());
                        Console.Write("Enter Expiry Date (yyyy-mm-dd): "); DateTime exp = DateTime.Parse(Console.ReadLine());
                        manager.Groceries.AddItem(new GroceryItem(id, name, qty, exp));
                    }
                    else if (choice == "4")
                    {
                        Console.Write("Enter ID: "); int id = int.Parse(Console.ReadLine());
                        Console.Write("Enter Name: "); string name = Console.ReadLine();
                        Console.Write("Enter Quantity: "); int qty = int.Parse(Console.ReadLine());
                        Console.Write("Enter Brand: "); string brand = Console.ReadLine();
                        Console.Write("Enter Warranty Months: "); int wm = int.Parse(Console.ReadLine());
                        manager.Electronics.AddItem(new ElectronicItem(id, name, qty, brand, wm));
                    }
                    else if (choice == "5")
                    {
                        Console.Write("Enter Type (G/E): ");
                        string type = Console.ReadLine().ToUpper();
                        Console.Write("Enter ID: "); int id = int.Parse(Console.ReadLine());
                        Console.Write("Enter Increase Amount: "); int qty = int.Parse(Console.ReadLine());

                        if (type == "G") manager.IncreaseStock(manager.Groceries, id, qty);
                        else manager.IncreaseStock(manager.Electronics, id, qty);
                    }
                    else if (choice == "6")
                    {
                        Console.Write("Enter Type (G/E): ");
                        string type = Console.ReadLine().ToUpper();
                        Console.Write("Enter ID: "); int id = int.Parse(Console.ReadLine());

                        if (type == "G") manager.RemoveItemById(manager.Groceries, id);
                        else manager.RemoveItemById(manager.Electronics, id);
                    }
                    else if (choice == "7")
                        break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] {ex.Message}");
                }
            }
        }
    }
}
