using System;
using System.Collections.Generic;

// Define the generic Repository class
public class Repository<T>
{
    private List<T> items = new List<T>();

    public void Add(T item)
    {
        items.Add(item);
    }

    public List<T> GetAll()
    {
        return items;
    }

    public T GetById(Func<T, bool> predicate)
    {
        return items.Find(predicate) ?? throw new InvalidOperationException("Item not found.");
    }

    public bool Remove(Func<T, bool> predicate)
    {
        var itemToRemove = items.Find(predicate);
        if (itemToRemove != null)
        {
            items.Remove(itemToRemove);
            return true;
        }
        return false;
    }
}

// Define the Patient class
public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }
}

// Define the Prescription class
public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }
}

// Define the HealthSystemApp class
public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new Repository<Patient>();
    private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

    public void SeedData()
    {
        // Add patients
        _patientRepo.Add(new Patient(1, "Alice", 30, "Female"));
        _patientRepo.Add(new Patient(2, "Bob", 45, "Male"));
        _patientRepo.Add(new Patient(3, "Charlie", 28, "Male"));

        // Add prescriptions
        _prescriptionRepo.Add(new Prescription(1, 1, "Medication A", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(2, 1, "Medication B", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(3, 2, "Medication C", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(4, 3, "Medication D", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(5, 2, "Medication E", DateTime.Now));
    }

    public void BuildPrescriptionMap()
    {
        foreach (var prescription in _prescriptionRepo.GetAll())
        {
            if (!_prescriptionMap.ContainsKey(prescription.PatientId))
            {
                _prescriptionMap[prescription.PatientId] = new List<Prescription>();
            }
            _prescriptionMap[prescription.PatientId].Add(prescription);
        }
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("Patients:");
        foreach (var patient in _patientRepo.GetAll())
        {
            Console.WriteLine($"ID: {patient.Id}, Name: {patient.Name}, Age: {patient.Age}, Gender: {patient.Gender}");
        }
    }

    public void PrintPrescriptionsForPatient(int id)
    {
        Console.WriteLine($"\nPrescriptions for Patient ID {id}:");
        if (_prescriptionMap.TryGetValue(id, out var prescriptions))
        {
            foreach (var prescription in prescriptions)
            {
                Console.WriteLine($"Prescription ID: {prescription.Id}, Medication: {prescription.MedicationName}, Date Issued: {prescription.DateIssued}");
            }
        }
        else
        {
            Console.WriteLine("No prescriptions found for this patient.");
        }
    }
}

// Main application
class Program
{
    static void Main(string[] args)
    {
        HealthSystemApp healthSystemApp = new HealthSystemApp();

        healthSystemApp.SeedData();
        healthSystemApp.BuildPrescriptionMap();
        healthSystemApp.PrintAllPatients();

        // Assuming we want to display prescriptions for patient with ID 1
        healthSystemApp.PrintPrescriptionsForPatient(1);
    }
}