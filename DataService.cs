using Labb3_Anropa_databasen.Data;
using Labb3_Anropa_databasen.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
                        
                        Console.WriteLine($"First Name: {reader["FirstName"],-10} | Last Name: {reader["LastName"], -15} | " +
                                          $"Role: {reader["Role"], -10} | Subject: {reader["Subject"], -18} | Hire Date {reader["HireDate"]}");
                    }
                    Thread.Sleep(2000);
                    Console.WriteLine("Returning to Main Menu");
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
            string sqlQuery = @"SELECT FirstName, LastName, Role,HireDate, Subject FROM Employees WHERE Role = 'Teacher'";
            using (SqlCommand command = new SqlCommand(sqlQuery, conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("Teachers");

                    Console.WriteLine(Lines);

                    while (reader.Read())
                    {
                        
                        Console.WriteLine($"First Name: {reader["FirstName"],-10} | Last Name: {reader["LastName"], -15} | " +
                                          $"Role: {reader["Role"], -10} | Subject: {reader["Subject"], -18} | Hire Date {reader["HireDate"]}");
                    }
                    Thread.Sleep(2000);
                    Console.WriteLine("Returning to Main Menu");
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
            Thread.Sleep(2000);
            Console.WriteLine("Returning to Main Menu");
            Menus.DisplayMainMenu();
        }
    }
    
    public static void GetStudentsByCourse(string course, string name, string selection)
    {
        string sortBy = name.Equals("FirstName", StringComparison.OrdinalIgnoreCase) ? "firstname" : "lastname";
        bool ascending = selection.Equals("Ascending", StringComparison.OrdinalIgnoreCase);

        using (var context = new SchoolDbContext())
        {
            
        }
        Thread.Sleep(2000);
        Console.WriteLine("Returning to Main Menu");
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
                        
                        Console.WriteLine($"Course Name: {reader["CourseName"],-25} | Average Grade: {reader["AverageGrade"], -10} | " +
                                          $"Min Grade: {reader["MinGrade"], -5} | Max Grade: {reader["MaxGrade"]}");

                    }
                    Thread.Sleep(2000);
                    Console.WriteLine("Returning to Main Menu");
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

    public static void GetAllGrades()
    {
        
    }
    
}