using System;
using System.Collections.Generic;
using System.IO;

namespace GradingSystem
{
    // a) Student
    public class Student
    {
        public int Id;
        public string FullName;
        public int Score;

        public Student(int id, string fullName, int score)
        {
            Id = id; FullName = fullName; Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100) return "A";
            if (Score >= 70) return "B";
            if (Score >= 60) return "C";
            if (Score >= 50) return "D";
            return "F";
        }

        public override string ToString() => $"{FullName} (ID: {Id}): Score = {Score}, Grade = {GetGrade()}";
    }

    // b) InvalidScoreFormatException
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    // c) MissingFieldException
    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    // d) Processor
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();

            using var reader = new StreamReader(inputFilePath);
            string? line;
            int lineNo = 0;

            while ((line = reader.ReadLine()) != null)
            {
                lineNo++;
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                if (parts.Length < 3)
                    throw new MissingFieldException($"Line {lineNo}: expected 3 fields (Id, FullName, Score) but got {parts.Length}.");

                if (!int.TryParse(parts[0].Trim(), out int id))
                    throw new InvalidScoreFormatException($"Line {lineNo}: invalid Id format '{parts[0]}'.");

                string fullName = parts[1].Trim();

                if (!int.TryParse(parts[2].Trim(), out int score))
                    throw new InvalidScoreFormatException($"Line {lineNo}: invalid Score format '{parts[2]}'.");

                students.Add(new Student(id, fullName, score));
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using var writer = new StreamWriter(outputFilePath);
            foreach (var s in students)
            {
                writer.WriteLine(s.ToString());
            }
        }
    }

    class Program
    {
        static void Main()
        {
            var students = new List<Student>();
            var processor = new StudentResultProcessor();

            while (true)
            {
                Console.WriteLine("\n--- Student Grading System ---");
                Console.WriteLine("1. Add Student");
                Console.WriteLine("2. View Students");
                Console.WriteLine("3. Load Students From File");
                Console.WriteLine("4. Generate Report to File");
                Console.WriteLine("5. Exit");
                Console.Write("Choose an option: ");
                var choice = Console.ReadLine();

                if (choice == "1")
                {
                    try
                    {
                        Console.Write("Enter Student ID: ");
                        int id = int.Parse(Console.ReadLine() ?? "0");

                        Console.Write("Enter Full Name: ");
                        string name = Console.ReadLine() ?? "";

                        Console.Write("Enter Score: ");
                        if (!int.TryParse(Console.ReadLine(), out int score))
                            throw new InvalidScoreFormatException("Score must be an integer.");

                        students.Add(new Student(id, name, score));
                        Console.WriteLine("Student added successfully!");
                    }
                    catch (InvalidScoreFormatException ex)
                    {
                        Console.WriteLine($"[Error] {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Unexpected Error] {ex.Message}");
                    }
                }
                else if (choice == "2")
                {
                    if (students.Count == 0) Console.WriteLine("No students available.");
                    else
                    {
                        Console.WriteLine("\nCurrent Students:");
                        foreach (var s in students) Console.WriteLine(s);
                    }
                }
                else if (choice == "3")
                {
                    try
                    {
                        Console.Write("Enter input file path: ");
                        string path = Console.ReadLine() ?? "";
                        var fileStudents = processor.ReadStudentsFromFile(path);
                        students.AddRange(fileStudents);
                        Console.WriteLine($"Loaded {fileStudents.Count} students from file.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Error] {ex.Message}");
                    }
                }
                else if (choice == "4")
                {
                    try
                    {
                        Console.Write("Enter output file path: ");
                        string outPath = Console.ReadLine() ?? "";
                        processor.WriteReportToFile(students, outPath);
                        Console.WriteLine($"Report generated successfully at {outPath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Error] {ex.Message}");
                    }
                }
                else if (choice == "5")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid option, try again.");
                }
            }

            Console.WriteLine("\nExiting Grading System.");
        }
    }
}
