using Microsoft.Data.SqlClient;

namespace Labb3_Anropa_databasen;

public class DataService
{
    private static string _connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=SchoolDB;Trusted_Connection=True;";

    public static void GetAllStaff()
    {
        
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
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            //Call method for displaying ALL Courses, average Grade,
            //and lowest and highest grade
            connection.Open();
            string sqlQuery = "SELECT Courses.CourseName, AVG(CAST(Grades.NumericGrade AS FLOAT)) AS AverageGrade, MIN(Grades.NumericGrade) AS MinGrade, MAX(Grades.NumericGrade) AS MaxGrade FROM Grades JOIN Enrollments ON Grades.EnrollmentID_FK = Enrollments.EnrollmentID JOIN Courses ON Enrollments.CourseID_FK  = Courses.CourseID GROUP BY Courses.CourseName;";
            
            using (SqlCommand command = new SqlCommand(sqlQuery, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("Courses");
                    Console.WriteLine("-----------------");

                    while (reader.Read())
                    {
                      
                        Console.WriteLine($"Course : {reader["CourseName"]} {reader ["NumericGrade"]}" +
                                          $"{reader["MinGrade"]} {reader["MaxGrade"]}");
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