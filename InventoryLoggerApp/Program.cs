using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Define the InventoryItem record
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// Define the marker interface for logging
public interface IInventoryEntity
{
    int Id { get; }
}

// Create a generic InventoryLogger class
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new List<T>();
    private string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return _log;
    }

    public void SaveToFile()
    {
        try
        {
            using (var writer = new StreamWriter(_filePath))
            {
                string json = JsonSerializer.Serialize(_log);
                writer.WriteLine(json);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error saving data to file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            using (var reader = new StreamReader(_filePath))
            {
                string json = reader.ReadToEnd();
                _log = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error loading data from file: {ex.Message}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing JSON data: {ex.Message}");
        }
    }
}

// Create the InventoryApp class
public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Smartphone", 20, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Tablet", 15, DateTime.Now));
    }

    public void SaveData()
    {
        _logger.SaveToFile();
    }

    public void LoadData()
    {
        _logger.LoadFromFile();
    }

    public void PrintAllItems()
    {
        foreach (var item in _logger.GetAll())
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded}");
        }
    }
}

// Main application
class Program
{
    static void Main(string[] args)
    {
        string filePath = "inventory.json"; // Adjust the path as needed
        var inventoryApp = new InventoryApp(filePath);

        // Seed sample data
        inventoryApp.SeedSampleData();

        // Save data
        inventoryApp.SaveData();

        // Clear memory and simulate a new session
        inventoryApp = new InventoryApp(filePath);

        // Load data
        inventoryApp.LoadData();

        // Print all items
        Console.WriteLine("Inventory Items:");
        inventoryApp.PrintAllItems();
    }
}
