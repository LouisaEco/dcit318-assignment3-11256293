using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareManagement
{
    // ====== Generic Repository ======
    public class Repository<T>
    {
        private readonly List<T> items = new();

        public void Add(T item) => items.Add(item);
        public List<T> GetAll() => new(items);
        public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);
        public bool Remove(Func<T, bool> predicate)
        {
            var toRemove = items.FirstOrDefault(predicate);
            if (toRemove == null) return false;
            items.Remove(toRemove);
            return true;
        }
    }

    // ====== Patient Class ======
    public class Patient
    {
        public int Id;
        public string Name;
        public int Age;
        public string Gender;

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString() =>
            $"Patient {{ Id={Id}, Name={Name}, Age={Age}, Gender={Gender} }}";
    }

    // ====== Prescription Class ======
    public class Prescription
    {
        public int Id;
        public int PatientId;
        public string MedicationName;
        public DateTime DateIssued;

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString() =>
            $"Prescription {{ Id={Id}, PatientId={PatientId}, Medication={MedicationName}, Date={DateIssued:yyyy-MM-dd} }}";
    }

    // ====== Health System App ======
    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new();

        public void AddPatientInteractive()
        {
            Console.Write("Enter Patient ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Enter Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Age: ");
            int age = int.Parse(Console.ReadLine());

            Console.Write("Enter Gender (M/F): ");
            string gender = Console.ReadLine();

            _patientRepo.Add(new Patient(id, name, age, gender));
            Console.WriteLine(" Patient added successfully.");
        }

        public void AddPrescriptionInteractive()
        {
            Console.Write("Enter Prescription ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Enter Patient ID: ");
            int patientId = int.Parse(Console.ReadLine());

            var patient = _patientRepo.GetById(p => p.Id == patientId);
            if (patient == null)
            {
                Console.WriteLine(" Patient not found.");
                return;
            }

            Console.Write("Enter Medication Name: ");
            string medName = Console.ReadLine();

            Console.Write("Enter Date Issued (yyyy-mm-dd): ");
            DateTime dateIssued = DateTime.Parse(Console.ReadLine());

            _prescriptionRepo.Add(new Prescription(id, patientId, medName, dateIssued));
            Console.WriteLine("✅ Prescription added successfully.");

            BuildPrescriptionMap();
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap = _prescriptionRepo
                .GetAll()
                .GroupBy(p => p.PatientId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.TryGetValue(patientId, out var list) ? new List<Prescription>(list) : new List<Prescription>();
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("\n--- All Patients ---");
            foreach (var p in _patientRepo.GetAll())
                Console.WriteLine(p);
        }

        public void PrintPrescriptionsForPatient()
        {
            Console.Write("Enter Patient ID to view prescriptions: ");
            int id = int.Parse(Console.ReadLine());

            var prescriptions = GetPrescriptionsByPatientId(id);
            if (prescriptions.Count == 0)
            {
                Console.WriteLine("No prescriptions found.");
                return;
            }

            Console.WriteLine($"\n--- Prescriptions for Patient ID {id} ---");
            foreach (var pr in prescriptions)
                Console.WriteLine(pr);
        }
    }

    // ====== Main Program ======
    class Program
    {
        static void Main()
        {
            var app = new HealthSystemApp();

            while (true)
            {
                Console.WriteLine("\n===== Healthcare System =====");
                Console.WriteLine("1. Add Patient");
                Console.WriteLine("2. Add Prescription");
                Console.WriteLine("3. View All Patients");
                Console.WriteLine("4. View Prescriptions by Patient");
                Console.WriteLine("5. Exit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": app.AddPatientInteractive(); break;
                    case "2": app.AddPrescriptionInteractive(); break;
                    case "3": app.PrintAllPatients(); break;
                    case "4": app.PrintPrescriptionsForPatient(); break;
                    case "5": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }
    }
}
