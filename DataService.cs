using Labb3_Anropa_databasen.Data;
using Labb3_Anropa_databasen.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Labb3_Anropa_databasen;

public class DataService : DbContext
{
    private const string ConnectionString = @"Server=(localdb)\MSSQLLocalDB;Database=SchoolDB;Trusted_Connection=True;";
    private const string Lines = "---------------------------";


    public static void GetAllStaff()
    {
        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            string sqlQuery = @"SELECT FirstName, LastName, Role,HireDate, Subject FROM Employees";
            using (SqlCommand command = new SqlCommand(sqlQuery, conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("Staff");

                    Console.WriteLine(Lines);

                    while (reader.Read())
                    {

                        Console.WriteLine(
                            $"First Name: {reader["FirstName"],-10} | Last Name: {reader["LastName"],-15} | " +
                            $"Role: {reader["Role"],-10} | Subject: {reader["Subject"],-18} | Hire Date {reader["HireDate"]}");
                    }

                    
                    Console.WriteLine("\n\tPress Enter to return to Main Menu");
                    Console.ReadLine();
                    Menus.DisplayMainMenu();
                }
            }
        }
    }

    public static void GetTeachers()
    {
        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            string sqlQuery =
                @"SELECT FirstName, LastName, Role,HireDate, Subject FROM Employees WHERE Role = 'Teacher'";
            using (SqlCommand command = new SqlCommand(sqlQuery, conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("Teachers");

                    Console.WriteLine(Lines);

                    while (reader.Read())
                    {

                        Console.WriteLine(
                            $"First Name: {reader["FirstName"],-10} | Last Name: {reader["LastName"],-15} | " +
                            $"Role: {reader["Role"],-10} | Subject: {reader["Subject"],-18} | Hire Date {reader["HireDate"]}");
                    }

                    Console.WriteLine("\n\tPress Enter to return to Main Menu");
                    Console.ReadLine();
                    Menus.DisplayMainMenu();
                }
            }
        }
    }

    public static void GetAllStudents(string name, string selection)
    {
        string sortBy = name.Equals("FirstName", StringComparison.OrdinalIgnoreCase) ? "firstname" : "lastname";
        bool ascending = selection.Equals("Ascending", StringComparison.OrdinalIgnoreCase);

        using (var context = new SchoolDbContext())
        {
            var students = ascending
                ? context.Students.OrderBy(s => sortBy == "firstname" ? s.FirstName : s.LastName)
                : context.Students.OrderByDescending(s => sortBy == "firstname" ? s.FirstName : s.LastName);

            Console.WriteLine("Students:");
            Console.WriteLine(Lines);

            foreach (var student in students)
            {
                Console.WriteLine(
                    $"First Name: {student.FirstName,-10} | Last Name: {student.LastName,-15} | Enrollment Date: {student.EnrollmentDate}");
            }

            Console.WriteLine("\n\tPress Enter to return to Main Menu");
            Console.ReadLine();
            Menus.DisplayMainMenu();
        }
    }

    public static void GetStudentsByCourse(int courseId, string name, string selection)
    {
        string sortBy = name.Equals("FirstName", StringComparison.OrdinalIgnoreCase) ? "Firstname" : "Lastname";
        bool ascending = selection.Equals("Ascending", StringComparison.OrdinalIgnoreCase);

        using (var context = new SchoolDbContext())
        {

        }

        Console.WriteLine("\n\tPress Enter to return to Main Menu");
        Console.ReadLine();
        Menus.DisplayMainMenu();
    }

    public static void GetAllCourses()
    {
        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            string sqlQuery = @"
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
            using (SqlCommand command = new SqlCommand(sqlQuery, conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    Console.WriteLine("Courses");
                    Console.WriteLine(Lines);


                    while (reader.Read())
                    {

                        Console.WriteLine(
                            $"Course Name: {reader["CourseName"],-25} | Average Grade: {reader["AverageGrade"],-10} | " +
                            $"Min Grade: {reader["MinGrade"],-5} | Max Grade: {reader["MaxGrade"]}");

                    }

                    Console.WriteLine("\n\tPress Enter to return to Main Menu");
                    Console.ReadLine();
                    Menus.DisplayMainMenu();
                }
            }
        }
    }

    public static void AddStudent()
    {
        // string FirstName, string LastName, DateTime EnrollmentDate, string Gender, DateTime BirthDate
    }

    public static void AddStaff()
    {
        // string FirstName, string LastName, string Role, DateTime HireDate, string Subject
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

            using (SqlCommand command = new SqlCommand(sqlQuery, conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    Console.WriteLine("Grades set the last month");
                    Console.WriteLine(Lines);
                    Console.WriteLine("{0,-30} | {1,-20} | {2,-10} | {3,-10}",
                        "Student", "Course", "Grade", "Date");

                    while (reader.Read())
                    {

                        Console.WriteLine("{0,-30} | {1,-20} | {2,-10} | {3:yyyy-MM-dd}",
                            reader["StudentFullName"],
                            reader["CourseName"],
                            reader["NumericGrade"],
                            reader["GradeSetDate"]);
                    }

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
}