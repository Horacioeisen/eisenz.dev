using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;
using System.Runtime.ExceptionServices;
namespace EMPLOYEEPAYROLLSYSTEM
{
    public abstract class Employee
    {
        public string Name { get; set; }
        public string EmployeeID { get; set; }
        public string Address { get; set; }
        public string Position { get; set; }

        public Employee(string name, string employeeID, string address, string position)
        {
            this.Name = name;
            this.Address = address;
            this.Position = position;
            this.EmployeeID = employeeID;
        }       
        
        public abstract decimal CalcPay();

        public virtual decimal CalcTaxes(decimal grosspay)
        {
            return grosspay * 0.10m; // 10% tax
        }
        public double CalcNetPay()
        {
            var grosspay = CalcPay();
            var taxes = CalcTaxes(grosspay);
            return (double)(grosspay - taxes);
        }
        public abstract string ToCSVFormat();
        public abstract string EmployeeType { get; }
    }
    class FullTimeEmployee : Employee
    {
        public decimal MonthlyPay { get; set; }

        public FullTimeEmployee(string name, string employeeid, string address, string position, decimal monthlypay) : base (name, employeeid, address, position) 
        {
            MonthlyPay = monthlypay;
        }
        public override decimal CalcPay()
        {
            return MonthlyPay;
        }
        public override string ToCSVFormat()
        {
            return $"FullTime,{Name},{EmployeeID},{Address},{Position},{MonthlyPay}";
        }
        public override string EmployeeType => "FullTime";
    }
    class PartTimeEmployee : Employee
    {
        public decimal HourlyRate {  get; set; }
        public int HoursWorked {  get; set; }

        public PartTimeEmployee(string name, string employeeid, string address, string position, decimal hourlyrate, int hoursworked) : base (name, employeeid, address, position) 
        {
            HourlyRate = hourlyrate;
            HoursWorked = hoursworked;
        }
        public override decimal CalcPay()
        {
            return HourlyRate * HoursWorked;
        }
        public override string ToCSVFormat()
        {
            return $"PartTime,{Name},{EmployeeID},{Address},{Position},{HourlyRate},{HoursWorked}";
        }
        public override string EmployeeType => "PartTime";

    }
    class Payroll
    {
        private List<Employee> employees = new List<Employee>(); 
        private const string FilePath = "employees.csv";       
         
        public Payroll()
        {
            LoadEmployeesFromFile();
        }    
        public void AddEmployee()
        {
            Console.Write("Employee Name: ");
            string name = GetNonEmptyInput();

            if (employees.Exists(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("An employee with this NAME already exists. Please try again.");
                return;
            }

            Console.Write("Employee ID: ");
            string employeeID = GetNonEmptyInput();
            
            if (employees.Exists(e => e.EmployeeID.Equals(employeeID, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("An employee with this ID already exists. Please use a unique ID.");
                return;
            }

            Console.Write("Employee Address: ");
            string address = GetNonEmptyInput();
            Console.Write("Employee Position: ");
            string position = GetNonEmptyInput();

            Console.WriteLine("Select Employee Type"); 
            Console.WriteLine("1. FULL-TIME");
            Console.WriteLine("2. PART-TIME");
            int employeeType;
             while (!int.TryParse(Console.ReadLine(), out employeeType) || (employeeType != 1 && employeeType !=2))
            {
                Console.WriteLine("Invalid selection. Please enter 1 for Full-Time or 2 for Part-Time.");
            }

            Employee newEmployee = null;

            switch (employeeType)
            {
                case 1:
                    Console.WriteLine("Enter Monthly Salary: ");
                    decimal monthlysalary = decimal.Parse(Console.ReadLine());
                    newEmployee = new FullTimeEmployee(name, employeeID, address, position, monthlysalary);
                    break;
                case 2:
                    Console.WriteLine("Enter Hourly Rate: ");
                    decimal hourlyrate = decimal.Parse(Console.ReadLine());
                    Console.WriteLine("Enter Hours Worked: ");
                    int hoursworked = int.Parse(Console.ReadLine());
                    newEmployee = new PartTimeEmployee(name, employeeID, address, position, hourlyrate, hoursworked);
                    break;
                default:
                    Console.WriteLine("Invalid employee type. Please try again.");
                    break;
            }
            Console.Clear();
            if (newEmployee != null)
            {
                employees.Add(newEmployee); // to add employee
                SaveEmployeesToFile();
                Console.WriteLine("Employee added successfully!");
            }                                                     
        }
        public void DeleteEmployee()
        {
            Console.Write("Enter Employee ID to delete: ");
            string employeeID = GetNonEmptyInput();
            Console.Clear();
        
            Employee employeeToDelete = employees.Find(e => e.EmployeeID == employeeID);

                if(employeeToDelete != null)
                {
                    employees.Remove(employeeToDelete); //to delete employee
                    Console.WriteLine("Employee deleted successfully!");
                    SaveEmployeesToFile();
                    return;
                }
            
            Console.WriteLine($"Employee with ID {employeeID} is not found");
        }
        public void UpdateEmployee()
        {
            Console.Write("Enter Employee ID to update: ");
            string employeeID = Console.ReadLine();
            Console.Clear();

            Employee employeeToUpdate = employees.Find(e => e.EmployeeID == employeeID);
            if (employeeToUpdate != null)
            {
                Console.WriteLine($"Updating details for {employeeToUpdate.Name} ({employeeToUpdate.EmployeeType})");
                Console.Write("New Name (Leave blank to keep current): ");
                string newName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    employeeToUpdate.Name = newName;
                }

                Console.Write("New Address (Leave blank to keep current): ");
                string newAddress = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newAddress))
                {
                    employeeToUpdate.Address = newAddress;
                }

                Console.Write("New Position (Leave blank to keep current): ");
                string newPosition = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newPosition))
                {
                    employeeToUpdate.Position = newPosition;
                }

                if (employeeToUpdate is FullTimeEmployee ft)
                {
                    Console.Write("New Monthly Salary (Leave blank to keep current): ");
                    string newSalary = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newSalary) && decimal.TryParse(newSalary, out decimal updatedSalary))
                    {
                        ft.MonthlyPay = updatedSalary;
                    }
                }
                else if (employeeToUpdate is PartTimeEmployee pt)
                {
                    Console.Write("New Hourly Rate (Leave blank to keep current): ");
                    string newHourlyRate = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newHourlyRate) && decimal.TryParse(newHourlyRate, out decimal updatedRate))
                    {
                        pt.HourlyRate = updatedRate;
                    }

                    Console.Write("New Hours Worked (Leave blank to keep current): ");
                    string newHoursWorked = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newHoursWorked) && int.TryParse(newHoursWorked, out int updatedHours))
                    {
                        pt.HoursWorked = updatedHours;
                    }
                }

                SaveEmployeesToFile();
                Console.WriteLine("Employee updated successfully!");
            }
            else
            {
                Console.WriteLine($"Employee with ID {employeeID} not found.");
            }
        }
        public void DisplayAllEmployees()
        {   
            
            Console.WriteLine("==================================================== EMPLOYEE LIST ====================================================");

            if (employees.Count == 0)
            {
                Console.WriteLine("No employees found in the system.");
                return;
            }

            Console.WriteLine("{0,-5} | {1,-20} | {2, -10} | {3, -20} | {4,-26} | {5, -12} | {6, -2}", "No", "Name", "ID", "Address", "Position", "EmployeeType", "Salary");
            Console.WriteLine(new string('-', 119)); 
            for (int i = 0; i < employees.Count; i++)
            {
                var employee = employees[i];
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.WriteLine("{0,-5} | {1,-20} | {2, -10} | {3, -20} | {4,-26} | {5, -12} | \u20B1{6, -1}", i + 1, employee.Name, employee.EmployeeID, employee.Address, employee.Position, employee.EmployeeType, employee.CalcPay());
                Console.WriteLine(new string('-', 119)); 
            }
        }
        public void GeneratePayStubForEmployee()
        {
            Console.Write("Enter the Employee ID for the specific employee pay stub: ");
            string specificEmployeeID = Console.ReadLine();
            Console.Clear();
            
            Employee employeeStub = employees.Find(e => e.EmployeeID == specificEmployeeID);

            if (employeeStub != null)
            {
                decimal grosspay = employeeStub.CalcPay();
                decimal taxes = employeeStub.CalcTaxes(grosspay);
                decimal netpay = grosspay - taxes;
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("\t\t  RIS TECH COMPANY\n");
                Console.ResetColor();
                Console.WriteLine($"PAY SLIP\t\tDate: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                Console.WriteLine(new string('=', 50));
                Console.WriteLine($"Employee Name: {employeeStub.Name}");
                Console.WriteLine($"Employee ID: {employeeStub.EmployeeID}");
                Console.WriteLine($"Employee Type: {employeeStub.EmployeeType}");      
                Console.WriteLine($"Position: {employeeStub.Position}");
                Console.WriteLine(new string('-', 50));
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.WriteLine($"Gross pay: \t\t\t\t\u20B1{grosspay:F2}");
                Console.WriteLine($"Tax deduction: \t\t\t\t-\u20B1{taxes:F2}");
                Console.WriteLine($"Net pay: \t\t\t\t\u20B1{netpay:F2}");
                Console.WriteLine(new string('=', 50));
                Console.WriteLine("");
                return;
            } else
            {
                Console.WriteLine($"Employee with ID {specificEmployeeID} is not found.");
            }
        }
        public void SearchEmployee()
        {
            Console.Write("Enter the Employee's (firstname/lastname or ID) to search: ");
            string searchName = Console.ReadLine()?.Trim();
            bool found = false;

            foreach (var employee in employees)
            {
                if (employee != null)
                {
                    string[] empNameParts = employee.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    string firstName = empNameParts.Length > 0 ? empNameParts[0] : string.Empty;
                    string lastName = empNameParts.Length > 1 ? empNameParts[1] : string.Empty;


                    if (employee.EmployeeID.Equals(searchName, StringComparison.OrdinalIgnoreCase) ||
                        firstName.Equals(searchName, StringComparison.OrdinalIgnoreCase) ||
                        lastName.Equals(searchName, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("\n");
                        Console.WriteLine("{0,-25} | {1,-15} | {2, -24} | {3, -26} | {4, -12} | {5, -2}", "Name", "ID", "Adress", "Position", "EmployeeType", "Salaray");
                        Console.WriteLine(new string('-', 125));
                        Console.OutputEncoding = System.Text.Encoding.UTF8;
                        Console.WriteLine("{0,-25} | {1,-15} | {2, -24} | {3, -26} | {4, -12} | \u20B1{5, -2}", employee.Name, employee.EmployeeID, employee.Address, employee.Position, employee.EmployeeType, employee.CalcPay());
                        found = true;
                    }
                }
            }
            if (!found)
            {
                Console.WriteLine($"No employee found with the name \"{searchName}\".");
            }
        }
        private void SaveEmployeesToFile() //writing a file
        {            
            try
            {
                using (StreamWriter writer = new StreamWriter(FilePath, false))
                {
                    foreach (var employee in employees)
                    {
                        if (employee is FullTimeEmployee)
                        {
                            writer.WriteLine(employee.ToCSVFormat());
                        } else if (employee is PartTimeEmployee)
                        {
                            writer.WriteLine(employee.ToCSVFormat());
                        }
                    }
                }
                Console.WriteLine("Employees saved successfully.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Failed to write the file: {ex.Message}"); 
            }
        }
        private void LoadEmployeesFromFile()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    using (StreamReader reader = new StreamReader(FilePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] data = line.Split(',');
                            if (data[0] == "FullTime" && data.Length == 6)
                            {
                                employees.Add(new FullTimeEmployee(
                                    data[1],
                                    data[2],
                                    data[3],
                                    data[4],
                                    decimal.Parse(data[5])
                                ));
                            }
                            else if (data[0] == "PartTime" && data.Length == 7)
                            {
                                employees.Add(new PartTimeEmployee(
                                    data[1],
                                    data[2],
                                    data[3],
                                    data[4],
                                    decimal.Parse(data[5]),
                                    int.Parse(data[6])
                                ));
                            }
                        }
                    }
                    Console.WriteLine("Employees loaded successfully.");
                }
                else
                {
                    Console.WriteLine("No employee data file found. Starting fresh.");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Failed to read the file: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error during file loading: {ex.Message}");
            }
        }
        private string GetNonEmptyInput()
        {
            string input = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Input cannot be empty. Please try again.");
                input = Console.ReadLine();
            }
            return input;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\t\t\t\t\t\t\t\t\t  RRR   III  SSS    TTTTT  EEEEE  CCC  H   H");
            Console.WriteLine("\t\t\t\t\t\t\t\t\t  R  R   I   S        T    E      C    H   H");
            Console.WriteLine("\t\t\t\t\t\t\t\t\t  RRR    I    SSS     T    EEEE   C    HHHHH");
            Console.WriteLine("\t\t\t\t\t\t\t\t\t  R R    I       S    T    E      C    H   H");
            Console.WriteLine("\t\t\t\t\t\t\t\t\t  R  R  III  SSS      T    EEEEE  CCC  H   H");
            Console.ResetColor();
            Thread.Sleep(1000);
            Payroll payroll = new Payroll();
            
            while (true)
            {
                try
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t   >EMPLOYEE PAYROLL SYSTEM<");
                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t   [1] Add Employee");
                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t   [2] Delete Employee");
                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t   [3] Update Employee");
                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t   [4] Generate Pay Stub");
                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t   [5] Display All Employees");
                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t   [6] Search Employee");
                    Console.WriteLine("\t\t\t\t\t\t\t\t\t\t   [7] Exit");
                    Console.Write("\t\t\t\t\t\t\t\t\t\t   Choose an option: ");

                    if (!int.TryParse(Console.ReadLine(), out int choice))  
                    {
                        Console.Clear();
                        Console.WriteLine("Invalid input! Please enter a number between 1 and 7.");
                        continue;
                    }

                    switch (choice)
                    {
                        case 1:
                            Console.Clear();
                            payroll.AddEmployee();
                            break;
                        case 2:
                            payroll.DeleteEmployee();
                            break;
                        case 3:
                            payroll.UpdateEmployee();
                            break;
                        case 4:
                            payroll.GeneratePayStubForEmployee();
                            break;
                        case 5:
                            Console.Clear();
                            payroll.DisplayAllEmployees();
                            break;
                        case 6:
                            Console.Clear();
                            payroll.SearchEmployee();
                            break;
                        case 7:
                            Console.Clear();
                            Console.WriteLine("Exiting program...Goodbye!");
                            return;
                        default:
                            Console.Clear();
                            Console.WriteLine("Invalid choice! Please choose a number between 1 and 7.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    Console.WriteLine("Please try again.");
                }

            }
        }                                                     
    }
}
