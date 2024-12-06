using Microsoft.Data.SqlClient;

namespace Labb3_Anropa_databasen;

public class DataService
{
    private const string ConnectionString = @"Server=(localdb)\MSSQLLocalDB;Database=SchoolDB;Trusted_Connection=True;";
    private const string lines = "---------------------------";
    

    public static void GetAllStaff()
    {
        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            string sqlQuery = @"SELECT * FROM Staff";
            using (SqlCommand command = new SqlCommand(sqlQuery, conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("Staff");
                    Console.WriteLine(lines);

                    while (reader.Read())
                    {
                        Console.WriteLine($"Role: {reader["Role"],-25} | First Name: {reader["First Name"], -10} | " +
                                          $"Last Name: {reader["Last Name"], -5} | Subject: {reader["Subject"]}");
                    }

                }
            }
        }
    }

    public static void GetTeachers()
    {
        
    }

    public static void GetAllStudents(string name, string selection)
    {
        
    }

    public static void GetStudentsByCourse(string course, string name, string selection)
    {
        
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
                    Console.WriteLine(lines);

                   
                    while (reader.Read())
                    {
                        
                        Console.WriteLine($"Course Name: {reader["CourseName"],-25} | Average Grade: {reader["AverageGrade"], -10} | " +
                                          $"Min Grade: {reader["MinGrade"], -5} | Max Grade: {reader["MaxGrade"]}");

                    }
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
}