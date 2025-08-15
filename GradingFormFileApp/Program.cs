using System;
using System.Collections.Generic;
using System.IO;

// Define the Student class
public class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70 && Score < 80) return "B";
        if (Score >= 60 && Score < 70) return "C";
        if (Score >= 50 && Score < 60) return "D";
        return "F";
    }
}

// Define the InvalidScoreFormatException class
public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

// Define the MissingFieldException class
public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

// Create the StudentResultProcessor class
public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        List<Student> students = new List<Student>();

        using (var reader = new StreamReader(inputFilePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var fields = line.Split(',');
                if (fields.Length != 3)
                {
                    throw new MissingFieldException("A student record is missing required fields.");
                }

                int id;
                if (!int.TryParse(fields[0], out id))
                {
                    throw new InvalidScoreFormatException($"Invalid ID format: {fields[0]}");
                }

                string fullName = fields[1].Trim();
                int score;

                if (!int.TryParse(fields[2], out score))
                {
                    throw new InvalidScoreFormatException($"Invalid score format: {fields[2]}");
                }

                students.Add(new Student(id, fullName, score));
            }
        }

        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (var writer = new StreamWriter(outputFilePath))
        {
            foreach (var student in students)
            {
                writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
            }
        }
    }
}

// Main application
class Program
{
    static void Main(string[] args)
    {
        var processor = new StudentResultProcessor();
        string inputFilePath = "students.txt"; // Adjust the path as needed
        string outputFilePath = "student_report.txt"; // Adjust the path as needed

        try
        {
            var students = processor.ReadStudentsFromFile(inputFilePath);
            processor.WriteReportToFile(students, outputFilePath);
            Console.WriteLine("Student report generated successfully.");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }
}
