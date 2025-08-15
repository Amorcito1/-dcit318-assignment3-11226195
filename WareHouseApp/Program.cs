using System;
using System.Collections.Generic;

// Define the marker interface
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

// Define the ElectronicItem class
public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }
}

// Define the GroceryItem class
public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }
}

// Custom Exceptions
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

// Create a generic InventoryRepository class
public class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new Dictionary<int, T>();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
        {
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        }
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out var item))
        {
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        }
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
        {
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        }
    }

    public List<T> GetAllItems()
    {
        return new List<T>(_items.Values);
    }

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new InvalidQuantityException("Quantity cannot be negative.");
        }
        var item = GetItemById(id);
        item.Quantity = newQuantity;
    }
}

// Create the WareHouseManager class
public class WareHouseManager
{
    private InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
    private InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

    public void SeedData()
    {
        _groceries.AddItem(new GroceryItem(1, "Milk", 50, DateTime.Now.AddMonths(2)));
        _groceries.AddItem(new GroceryItem(2, "Bread", 30, DateTime.Now.AddMonths(1)));
        _electronics.AddItem(new ElectronicItem(1, "Smartphone", 20, "BrandA", 24));
        _electronics.AddItem(new ElectronicItem(2, "Laptop", 15, "BrandB", 12));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        Console.WriteLine($"Items in inventory:");
        foreach (var item in repo.GetAllItems())
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}");
        }
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            item.Quantity += quantity;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

// Main application
class Program
{
    static void Main(string[] args)
    {
        WareHouseManager warehouseManager = new WareHouseManager();

        warehouseManager.SeedData();

        Console.WriteLine("All Grocery Items:");
        warehouseManager.PrintAllItems(warehouseManager._groceries);

        Console.WriteLine("\nAll Electronic Items:");
        warehouseManager.PrintAllItems(warehouseManager._electronics);

        // Test error handling
        try
        {
            warehouseManager._electronics.AddItem(new ElectronicItem(1, "Duplicate Smartphone", 10, "BrandC", 24)); // Duplicate
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        try
        {
            warehouseManager.RemoveItemById(warehouseManager._groceries, 999); // Non-existent
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        try
        {
            warehouseManager._groceries.UpdateQuantity(1, -5); // Invalid quantity
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}