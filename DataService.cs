using System.Text.RegularExpressions;
using Labb3_Anropa_databasen.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace Labb3_Anropa_databasen;

public partial class DataService : DbContext
{
    private const string ConnectionString = @"Server=(localdb)\MSSQLLocalDB;Database=SchoolDB;Trusted_Connection=True;";
    private static readonly string Lines = new ('-', 60);


    public static void GetAllStaff()
    {
        using (var conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            var sqlQuery = @"SELECT FirstName, LastName, Role, HireDate, Subject FROM Employees";
            using (var command = new SqlCommand(sqlQuery, conn))
            {
                using (var reader = command.ExecuteReader())
                {
                    var data = new List<Dictionary<string, string>>();
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, string>();

                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var columnName = reader.GetName(i);
                            if (columnName.Equals("HireDate"))
                                row[columnName] = reader.IsDBNull(i)
                                    ? "N/A"
                                    : ((DateTime)reader.GetValue(i)).ToString("yyyy-MM-dd");
                            else
                                row[columnName] = reader.GetValue(i).ToString() ?? string.Empty;
                        }

                        data.Add(row);
                    }

                    var table = CreateTable(data, "All Staff");
                    AnsiConsole.Write(table);
                    Footer();
                }
            }
        }
    }

    public static void GetTeachers()
    {
        using (var conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            var sqlQuery =
                @"SELECT FirstName, LastName, Role, HireDate, Subject FROM Employees WHERE Role = 'Teacher'";
            using (var command = new SqlCommand(sqlQuery, conn))
            {
                using (var reader = command.ExecuteReader())
                {
                    var data = new List<Dictionary<string, string>>();
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, string>();

                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var columnName = reader.GetName(i);
                            if (columnName.Equals("HireDate"))
                                row[columnName] = reader.IsDBNull(i)
                                    ? "N/A"
                                    : ((DateTime)reader.GetValue(i)).ToString("yyyy-MM-dd");
                            else
                                row[columnName] = reader.GetValue(i).ToString() ?? string.Empty;
                        }

                        data.Add(row);
                    }

                    var table = CreateTable(data, "All Teachers");
                    AnsiConsole.Write(table);
                    Footer();
                }
            }
        }
    }

    public static void GetAllStudents(string name, string selection)
    {
        var sortBy = name.Equals("FirstName", StringComparison.OrdinalIgnoreCase) ? "firstname" : "lastname";
        var ascending = selection.Equals("Ascending", StringComparison.OrdinalIgnoreCase);

        using (var context = new SchoolDbContext())
        {
            var data = new List<Dictionary<string, string>>();
            var students = ascending
                ? context.Students.OrderBy(s => sortBy == "firstname" ? s.FirstName : s.LastName)
                : context.Students.OrderByDescending(s => sortBy == "firstname" ? s.FirstName : s.LastName);

            foreach (var student in students)
            {
                var row = new Dictionary<string, string>();
                var properties = student.GetType().GetProperties();

                foreach (var property in properties)
                {
                    var propertyName = property.Name;
                    var propertyValue = property.GetValue(student);
                    if (propertyValue is DateTime dateValue)
                        row[propertyName] = dateValue.ToString("yyyy-MM-dd");
                    else
                        row[propertyName] = propertyValue?.ToString() ?? string.Empty;
                }

                data.Add(row);
            }

            var table = CreateTable(data, "All Students");
            AnsiConsole.Write(table);
            Footer();
        }
    }

    public static void GetStudentsByCourse(int courseId, string name, string selection)
    {
        using (var context = new SchoolDbContext())
        {
            var course = context.Courses.FirstOrDefault(c => c.CourseId == courseId);
            var query = context.Enrollments
                .Where(e => e.CourseIdFk == courseId)
                .Join(context.Students,
                    enrollment => enrollment.StudentIdFk,
                    student => student.StudentId,
                    (enrollment, student) => new
                    {
                        student.StudentId,
                        student.FirstName,
                        student.LastName,
                        student.EnrollmentDate,
                        course!.CourseName
                    });

            if (name.Equals("First Name", StringComparison.OrdinalIgnoreCase))
                query = selection.Equals("Ascending", StringComparison.OrdinalIgnoreCase)
                    ? query.OrderBy(x => x.FirstName)
                    : query.OrderByDescending(x => x.FirstName);
            else
                query = selection.Equals("Ascending", StringComparison.OrdinalIgnoreCase)
                    ? query.OrderBy(x => x.LastName)
                    : query.OrderByDescending(x => x.LastName);

            var students = query.ToList();
            if (students.Count == 0) Console.WriteLine("No students found in this course.");
            var data = students.Select(student => new Dictionary<string, string>
            {
                { "CourseName", course!.CourseName },
                { "FirstName", student.FirstName },
                { "LastName", student.LastName },
                {
                    "EnrollmentDate",
                    student.EnrollmentDate.HasValue ? student.EnrollmentDate.Value.ToString("yyyy-MM-dd") : "N/A"
                }
            }).ToList();

            var table = CreateTable(data, "All Students by Course");
            AnsiConsole.Write(table);
            Footer();
        }
    }

    public static void GetAllCourses()
    {
        using (var conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            var sqlQuery = @"
            SELECT 
                Courses.CourseName, 
                 CEILING(AVG(CAST(Grades.NumericGrade AS FLOAT)) * 10) / 10.0 AS AverageGrade, 
                MIN(Grades.NumericGrade) AS MinGrade, 
                MAX(Grades.NumericGrade) AS MaxGrade
            FROM 
                Grades
            JOIN 
                Enrollments ON Grades.EnrollmentID_FK = Enrollments.EnrollmentID
            JOIN 
                Courses ON Enrollments.CourseID_FK = Courses.CourseID
            GROUP BY 
                Courses.CourseName;";
            using (var command = new SqlCommand(sqlQuery, conn))
            {
                using (var reader = command.ExecuteReader())
                {
                    var data = new List<Dictionary<string, string>>();

                    while (reader.Read())
                    {
                        var row = new Dictionary<string, string>();
                        for (var i = 0; i < reader.FieldCount; i++)
                            row[reader.GetName(i)] = reader.GetValue(i).ToString() ?? string.Empty;
                        data.Add(row);
                    }

                    var table = CreateTable(data, "All Courses");
                    AnsiConsole.Write(table);
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    AnsiConsole.Write(new Rule("\n\tPress any key to return ").Centered());
                    Console.ResetColor();
                    Console.ReadLine();
                    Menus.DisplayMainMenu();
                }
            }
        }
    }

    public static void AddStudent()
    {
        var firstName = CheckInput("Enter Student First Name: ");

        var lastName = CheckInput("Enter Student Last Name: ");

        Console.WriteLine("Enter Gender(Optional): ");
        var gender = Console.ReadLine();

        Console.WriteLine("Enter Birth Date (yyyy-mm-dd): ");
        DateTime birthDate;
        while (!DateTime.TryParse(Console.ReadLine(), out birthDate))
            Console.WriteLine("Invalid date format. Please enter again (yyyy-mm-dd)");

        var sqlQuery =
            @"INSERT INTO Students (FirstName, LastName, Gender, BirthDate, EnrollmentDate) VALUES (@FirstName, @LastName, @Gender, @BirthDate, @EnrollmentDate)";
        using (var conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            using (var command = new SqlCommand(sqlQuery, conn))
            {
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@Gender", gender);
                command.Parameters.AddWithValue("@BirthDate", birthDate);
                command.Parameters.AddWithValue("@EnrollmentDate", DateTime.Now);

                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0) Console.WriteLine("Student Added");

                Console.WriteLine("\n\tPress Enter to return to Main Menu");
                Console.ReadLine();
                Menus.DisplayMainMenu();
            }
        }
    }

    public static void AddStaff()
    {
        var firstName = CheckInput("Enter Staff First Name: ");

        var lastName = CheckInput("Enter Staff Last Name: ");

        var role = CheckInput("Enter Staff Role: ");

        Console.WriteLine("Enter Hire Date (yyyy-mm-dd): " +
                          "(If you want to use today's date press enter.)");
        DateTime hireDate;
        var hireDateInput = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(hireDateInput))
        {
            hireDate = DateTime.Now;
            Console.WriteLine("Today's date used.");
        }
        else
        {
            while (!DateTime.TryParse(Console.ReadLine(), out hireDate))
                Console.WriteLine("Invalid date format. Please enter again (yyyy-mm-dd)");
        }

        var subject = CheckInput("Enter Staff Subject : (Optional)");
        subject = string.IsNullOrWhiteSpace(subject) ? "" : subject;

        var sqlQuery =
            @"INSERT INTO Employees (FirstName, LastName, Role, HireDate, Subject) VALUES (@FirstName, @LastName, @Role, @HireDate, @Subject)";
        using (var conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            using (var command = new SqlCommand(sqlQuery, conn))
            {
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@Role", role);
                command.Parameters.AddWithValue("@HireDate", hireDate);
                command.Parameters.AddWithValue("@Subject", subject);


                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0) Console.WriteLine("Staff Added");

                Console.WriteLine("\n\tPress Enter to return to Main Menu");
                Console.ReadLine();
                Menus.DisplayMainMenu();
            }
        }
    }

    public static void GetNewGrades()
    {
        using (var conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            var sqlQuery = @"
            SELECT s.FirstName + ' ' + s.LastName AS StudentFullName,
            c.CourseName,
            g.NumericGrade,
            g.GradeSetDate
            FROM Grades g JOIN Enrollments e ON g.EnrollmentID_FK = e.EnrollmentID
            JOIN Students s ON e.StudentID_FK = s.StudentID
            JOIN Courses c ON e.CourseID_FK = c.CourseID
            WHERE GradeSetDate >= DATEADD(MONTH, -1, GETDATE())
            ORDER BY g.GradeSetDate DESC";

            using (var command = new SqlCommand(sqlQuery, conn))
            {
                using (var reader = command.ExecuteReader())
                {
                    Console.WriteLine("Grades set the last month");
                    Console.WriteLine(Lines);
                    Console.WriteLine("{0,-30} | {1,-20} | {2,-10} | {3,-10}",
                        "Student", "Course", "Grade", "Date");

                    while (reader.Read())
                        Console.WriteLine("{0,-30} | {1,-20} | {2,-10} | {3:yyyy-MM-dd}",
                            reader["StudentFullName"],
                            reader["CourseName"],
                            reader["NumericGrade"],
                            reader["GradeSetDate"]);

                    Console.WriteLine("\n\tPress Enter to return to Main Menu");
                    Console.ReadLine();
                    Menus.DisplayMainMenu();
                }
            }
        }
    }

    public static void GetAllGrades()
    {
        using (var conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            var sqlQuery = @"
            SELECT s.FirstName + ' ' + s.LastName AS StudentFullName,
            c.CourseName,
            g.NumericGrade,
            g.GradeSetDate
            FROM Grades g JOIN Enrollments e ON g.EnrollmentID_FK = e.EnrollmentID
            JOIN Students s ON e.StudentID_FK = s.StudentID
            JOIN Courses c ON e.CourseID_FK = c.CourseID
            ORDER BY StudentFullName DESC";

            using (var command = new SqlCommand(sqlQuery, conn))
            {
                using (var reader = command.ExecuteReader())
                {
                    Console.WriteLine("All Grades");
                    Console.WriteLine(Lines);
                    Console.WriteLine("{0,-30} | {1,-20} | {2,-10} | {3,-10}",
                        "Student", "Course", "Grade", "Date");
                    Console.WriteLine(Lines);
                    while (reader.Read())
                        Console.WriteLine("{0,-30} | {1,-20} | {2,-10} | {3:yyyy-MM-dd}",
                            reader["StudentFullName"],
                            reader["CourseName"],
                            reader["NumericGrade"],
                            reader["GradeSetDate"]);

                    Console.WriteLine(Lines);
                    Console.WriteLine("\n\tPress Enter to return to Main Menu");
                    Console.ReadLine();
                    Menus.DisplayMainMenu();
                }
            }
        }
    }

    private static string CheckInput(string prompt)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Input cannot be empty.");
                continue;
            }

            if (MyRegex().IsMatch(input)) return input;
            Console.WriteLine("Input can only contain alphanumeric characters.");
        }
    }

    [GeneratedRegex(@"^[a-öA-Ö\s]+$")]
    private static partial Regex MyRegex();

    private static Table CreateTable<T>(IEnumerable<T> items, string? title = null)
    {
        if (items == null || !items.Any())
            throw new ArgumentException("The rows cannot be null or empty.", nameof(items));

        Console.ForegroundColor = ConsoleColor.DarkRed;
        AnsiConsole.Write(new Rule($"\n\t{title}: ").Centered());
        Console.ResetColor();

        var table = new Table()
            .Centered()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.DarkRed);

        // Get the keys from the first dictionary (assuming all dictionaries have the same keys)
        var firstRow = items.First() as Dictionary<string, string>;
        if (firstRow == null) throw new InvalidOperationException("Failed to read dictionary keys.");

        // Add columns using dictionary keys
        foreach (var key in firstRow.Keys) table.AddColumn(new TableColumn($"[blue]{key}[/]").LeftAligned());

        // Add rows using dictionary values
        foreach (var item in items)
        {
            var row = (item as Dictionary<string, string>)!.Values.ToArray();
            table.AddRow(row);
        }


        return table;
    }

    public static void Footer()
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        AnsiConsole.Write(new Rule("\n\tPress any key to return ").Centered());
        Console.ResetColor();
        Console.ReadLine();
        Menus.DisplayMainMenu();
    }
   
    
}