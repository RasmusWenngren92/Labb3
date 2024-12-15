
using System.Text.RegularExpressions;
using Labb3_Anropa_databasen.Data;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace Labb3_Anropa_databasen;

public partial class DataService : DbContext
{
    //Connection string for accessing the database
    private const string ConnectionString = @"Server=(localdb)\MSSQLLocalDB;Database=SchoolDB;Trusted_Connection=True;";
    
    public static void GetAllStaff()
    {
        using (var conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            //SQL query to retrieve relevant information
            var sqlQuery = @"SELECT FirstName, LastName, Role, HireDate, Subject FROM Employees";
            using (var command = new SqlCommand(sqlQuery, conn))
            {
              
                //Executing query and processing results
                using (var reader = command.ExecuteReader())
                {
                    var data = Ui.DataFormater(reader, "HireDate");
                    var table = Ui.CreateTable(data, "All Staff");
                    AnsiConsole.Write(table);
                    Ui.Footer();
                }
            }
        }
    }

    public static void GetTeachers()
    {
        using (var conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            //SQL query to retrieve relevant information
            var sqlQuery =
                @"SELECT FirstName, LastName, Role, HireDate, Subject FROM Employees WHERE Role = 'Teacher'";
            using (var command = new SqlCommand(sqlQuery, conn))
            {
                //Executing query and processing results
                using (var reader = command.ExecuteReader())
                {
                    var data = Ui.DataFormater(reader, "HireDate");
                    var table = Ui.CreateTable(data, "All Teachers");
                    AnsiConsole.Write(table);
                    Ui.Footer();
                }
            }
        }
    }

    public static void GetAllStudents(string name, string selection)
    {
       
        using (var context = new SchoolDbContext())
        {
           
            var query = context.Students.AsQueryable();
            if (name.Equals("First Name", StringComparison.OrdinalIgnoreCase))
            {
                query = selection.Equals("Ascending", StringComparison.OrdinalIgnoreCase)
                    ? query.OrderBy(x => x.FirstName)
                    : query.OrderByDescending(x => x.FirstName);
            }
            else
            {
                query = selection.Equals("Ascending", StringComparison.OrdinalIgnoreCase)
                    ? query.OrderBy(x => x.LastName)
                    : query.OrderByDescending(x => x.LastName);
            }
            var students = query.ToList();
            var propertiesToDisplay = new List<string> { "FirstName", "LastName", "EnrollmentDate", "BirthDate", "GraduationDate" };
           
            var data = Ui.ExtractProperties(students, propertiesToDisplay);

            //Creates and displays table with information
            var table = Ui.CreateTable(data, "All Students");
            AnsiConsole.Write(table);
            Ui.Footer();
        }
    }

    public static void GetStudentsByCourse(int courseId, string name, string selection)
    {
        using (var context = new SchoolDbContext())
        {
            //Retrieve course information
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
            //Sorting based on user input (FirstName or LastName)
            if (name.Equals("First Name"))
                query = selection.Equals("Ascending")
                    ? query.OrderBy(x => x.FirstName)
                    : query.OrderByDescending(x => x.FirstName);
            else
                query = selection.Equals("Ascending")
                    ? query.OrderBy(x => x.LastName)
                    : query.OrderByDescending(x => x.LastName);

            var students = query.ToList();
            if (students.Count == 0)
            {
                Console.WriteLine($"\n\tNo student enrolled in {course!.CourseName}.");
                Console.WriteLine("\n\tDo you want to try another course? y/n");
                if (Console.ReadKey().KeyChar.ToString().ToLower() == "y")
                {
                    Console.Clear();
                    Menus.StudentsByCourse();
                }
                else
                {
                    Menus.DisplayMainMenu();
                }

                return;
            }
            
            var data = students.Select(student => new Dictionary<string, string>
            {
                { "Course Name", course!.CourseName },
                { "First Name", student.FirstName },
                { "Last Name", student.LastName },
                { "Enrollment Date",
                    student.EnrollmentDate.HasValue ? student.EnrollmentDate.Value.ToString("yyyy-MM-dd") : "N/A"
                }
            }).ToList();

            var table = Ui.CreateTable(data, "All Students by Course");
            AnsiConsole.Write(table);
            Ui.Footer();
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
                  COALESCE(CEILING(AVG(CAST(Grades.NumericGrade AS FLOAT)) * 10) / 10.0, 0) AS AverageGrade, 
            COALESCE(MIN(Grades.NumericGrade), 0) AS MinGrade, 
            COALESCE(MAX(Grades.NumericGrade), 0) AS MaxGrade
            FROM 
                Courses
            LEFT JOIN 
                Enrollments ON Courses.CourseID = Enrollments.CourseID_FK
            LEFT JOIN 
                Grades ON Enrollments.EnrollmentID = Grades.EnrollmentID_FK
            GROUP BY 
                Courses.CourseName;";
            using (var command = new SqlCommand(sqlQuery, conn))
            {
                using (var reader = command.ExecuteReader())
                {
                    var data = new List<Dictionary<string, string?>>();

                    while (reader.Read())
                    {
                        var row = new Dictionary<string, string?>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            
                            var value = reader.GetValue(i);
                            row[reader.GetName(i)] = value == DBNull.Value ? "N/A" : value.ToString();
                        }
                        data.Add(row);
                    }

                    var table = Ui.CreateTable(data, "All Courses");
                    AnsiConsole.Write(table);
                    Ui.Footer();
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
        
        var enrollCourse = CheckInput("Do you want to enroll to one or more courses? (y/n)").Trim().ToLower();
        using (var conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            using (var transaction = conn.BeginTransaction())
            {
                 var sqlQuery =
                            @"INSERT INTO Students (FirstName, LastName, Gender, BirthDate, EnrollmentDate) 
                                    VALUES (@FirstName, @LastName, @Gender, @BirthDate, @EnrollmentDate);
                                    SELECT SCOPE_IDENTITY();";
                 var studentId = 0;
                 using (var command = new SqlCommand(sqlQuery, conn, transaction))
                 {
                     command.Parameters.AddWithValue("@FirstName", firstName);
                     command.Parameters.AddWithValue("@LastName", lastName);
                     command.Parameters.AddWithValue("@Gender", gender ?? (object)DBNull.Value);
                     command.Parameters.AddWithValue("@BirthDate", birthDate);
                     command.Parameters.AddWithValue("@EnrollmentDate", DateTime.Now);


                     var insertedStudentId = command.ExecuteScalar();
                    
                    if (insertedStudentId != null)
                    {
                        studentId = Convert.ToInt32(insertedStudentId); 
                        Console.WriteLine($"Student inserted with ID: {studentId}");
                    }
                    else
                    {
                        Console.WriteLine("Failed to insert student.");
                        transaction.Rollback(); 
                        return; 
                    }
                 }

                 if (enrollCourse == "y" && studentId > 0) Ui.CourseEnrollment(conn, studentId, transaction);
                 transaction.Commit();
                 Ui.Footer();
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
            Console.Write("Today's date used.");
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
                if (rowsAffected > 0) Ui.Animation("Adding Staff");
                Console.WriteLine("Staff Added");
                
            }

            if (!string.IsNullOrWhiteSpace(subject))
            {
                var checkSubjectQuery = "SELECT COUNT(*) FROM Courses WHERE CourseName = @Subject";
                using (var checkCommand = new SqlCommand(checkSubjectQuery, conn))
                {
                    checkCommand.Parameters.AddWithValue("@Subject", subject);
                    var exist =(int)checkCommand.ExecuteScalar() > 0;

                    if (!exist)
                    {
                        var addCourseQuery = "INSERT INTO Courses (CourseName) VALUES (@Subject)";
                        using (var addCourseCommand = new SqlCommand(addCourseQuery, conn))
                        {
                            addCourseCommand.Parameters.AddWithValue("@Subject", subject);
                            addCourseCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
            Ui.Footer();
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
                    var data = Ui.DataFormater(reader, "GradeSetDate");
                    var table = Ui.CreateTable(data, "New Grades");
                    AnsiConsole.Write(table);
                    Ui.Footer();
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
                    var data = Ui.DataFormater(reader, "GradeSetDate");
                    var table = Ui.CreateTable(data, "New Grades");
                    AnsiConsole.Write(table);
                    Ui.Footer();
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
    
}