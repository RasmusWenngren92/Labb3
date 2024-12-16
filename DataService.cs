
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
            try
            {
                //SQL query to retrieve relevant information
                var sqlQuery = @"SELECT FirstName, LastName, Role, HireDate, Subject FROM Employees ORDER BY HireDate Desc";
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
            catch (Exception e)
            {
                Console.WriteLine("Am error occured while retrieving staff: " + e.Message);
                Console.WriteLine("Error details: " + e.StackTrace);
                throw;
            }
           
        }
    }

    public static void GetTeachers()
    {
        using (var conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            try
            {
                //SQL query to retrieve relevant information
                var sqlQuery =
                    @"SELECT FirstName, LastName, Role, HireDate, Subject FROM Employees WHERE Role = 'Teacher' Order BY HireDate Desc";
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
            catch (Exception e)
            {
                Console.WriteLine("An error occured while retrieving teachers: " + e.Message);
                Console.WriteLine("Error details: " + e.StackTrace);
                throw;
            }
        }
    }

    public static void GetAllStudents(string name, string selection)
    {
        try
        {
            using (var context = new SchoolDbContext())
            {
                var query = context.Students.AsQueryable();
                //Sorting depending on input from user
                if (name.Equals("First Name", StringComparison.OrdinalIgnoreCase))
                    query = selection.Equals("Ascending", StringComparison.OrdinalIgnoreCase)
                        ? query.OrderBy(x => x.FirstName)
                        : query.OrderByDescending(x => x.FirstName);
                else
                    query = selection.Equals("Ascending", StringComparison.OrdinalIgnoreCase)
                        ? query.OrderBy(x => x.LastName)
                        : query.OrderByDescending(x => x.LastName);
                var students = query.ToList();
                var propertiesToDisplay = new List<string>
                    { "FirstName", "LastName", "EnrollmentDate", "BirthDate", "GraduationDate" };

                var data = Ui.ExtractProperties(students, propertiesToDisplay);

                //Creates and displays table with information
                var table = Ui.CreateTable(data, "All Students");
                AnsiConsole.Write(table);
                Ui.Footer();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred while retrieving students: " + e.Message);
            Console.WriteLine("Error details: " + e.StackTrace);
            throw;
        }
        
    }

    public static void GetStudentsByCourse(int courseId, string name, string selection)
    {
        try
        {
            using (var context = new SchoolDbContext())
            {
                //Retrieve course information
                var course = context.Courses.FirstOrDefault(c => c.CourseId == courseId);

                // Perform a query to join the Enrollments table with the Students table.
                var query = context.Enrollments
                    .Where(e => e.CourseIdFk == courseId)
                    .Join(context.Students,
                        enrollment => enrollment.StudentIdFk,
                        student => student.StudentId,
                        (enrollment, student) => new // Creates an anonymous object to return the necessary data
                        {
                            student.StudentId,
                            student.FirstName,
                            student.LastName,
                            student.EnrollmentDate,
                            course!.CourseName
                        });
                // Sorting the query based on the user input (First Name or Last Name, Ascending or Descending)
                if (name.Equals("First Name"))
                    query = selection.Equals("Ascending")
                        ? query.OrderBy(x => x.FirstName)
                        : query.OrderByDescending(x => x.FirstName);
                else
                    query = selection.Equals("Ascending")
                        ? query.OrderBy(x => x.LastName)
                        : query.OrderByDescending(x => x.LastName);

                // If no students are found for the given course, displays a message and allow the user to try another course
                var students = query.ToList();
                while (students.Count == 0)
                {
                    Console.WriteLine($"\n\tNo student enrolled in {course!.CourseName}.");
                    Console.WriteLine("\n\tDo you want to try another course? y/n");

                    // Get user input (y/n) to decide if they want to retry or exit.
                    var userInput = Console.ReadKey().KeyChar.ToString().ToLower();
                    if (userInput == "y")
                    {
                        // Clear the console and re-prompt the user for a new course.
                        Console.Clear();
                        Menus.StudentsByCourse();
                        return;
                    }
                    // If the user opts not to retry, exit to the main menu.
                    Menus.DisplayMainMenu();
                    return;
                }

                // Prepare the student data for display in a table format
                var data = students.Select(student => new Dictionary<string, string>
                {
                    { "Course Name", course!.CourseName },
                    { "First Name", student.FirstName },
                    { "Last Name", student.LastName },
                    {
                        "Enrollment Date",
                        student.EnrollmentDate.HasValue ? student.EnrollmentDate.Value.ToString("yyyy-MM-dd") : "N/A"
                    }
                }).ToList();

                var table = Ui.CreateTable(data, "All Students by Course");
                AnsiConsole.Write(table);
                Ui.Footer();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occured while executing: " + e.Message);
            Console.WriteLine("Error details: " + e.StackTrace);
            throw;
        }
        
    }

    public static void GetAllCourses()
    {
        using (var conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            try
            {
                // SQL query to retrieve course data along with average grade, min grade, and max grade
                // COALESCE is used to handle NULL values by replacing them with 0 for grades
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
            catch (Exception e)
            {
                Console.WriteLine("An error occured while executing: " + e.Message);
                Console.WriteLine("Error details: " + e.StackTrace);
                throw;
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
            try
            {
                using (var transaction = conn.BeginTransaction())
                {
                    // Defines the SQL query to insert the new student into the database
                    var sqlQuery =
                        @"INSERT INTO Students (FirstName, LastName, Gender, BirthDate, EnrollmentDate) 
                                    VALUES (@FirstName, @LastName, @Gender, @BirthDate, @EnrollmentDate);
                                    SELECT SCOPE_IDENTITY();";
                    var studentId = 0;
                    using (var command = new SqlCommand(sqlQuery, conn, transaction))
                    {
                        // Adds parameters to the SQL command to prevent SQL injection
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

                    // If the user chose to enroll the student in courses and the student ID is valid, proceeds to course enrollment
                    if (enrollCourse == "y" && studentId > 0) Ui.CourseEnrollment(conn, studentId, transaction);
                    transaction.Commit();
                    Ui.Footer();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured while executing: " + e.Message);
                Console.WriteLine("Error details: " + e.StackTrace);
                throw;
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
       
        // Defines the SQL query to insert the new staff into the database
        using (var conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    var sqlQuery = @"
                INSERT INTO Employees (FirstName, LastName, Role, HireDate, Subject)
                VALUES (@FirstName, @LastName, @Role, @HireDate, @Subject);
                SELECT SCOPE_IDENTITY();";
                     var teacherId = 0;
                    using (var command = new SqlCommand(sqlQuery, conn, transaction))
                    {
                        // Adds parameters to the SQL command to prevent SQL injection
                        command.Parameters.AddWithValue("@FirstName", firstName);
                        command.Parameters.AddWithValue("@LastName", lastName);
                        command.Parameters.AddWithValue("@Role", role);
                        command.Parameters.AddWithValue("@HireDate", hireDate);
                        command.Parameters.AddWithValue("@Subject", subject ?? (object)DBNull.Value);

                        var insertedTeacherId = command.ExecuteScalar();
                        if (insertedTeacherId != null)
                        {
                            teacherId = Convert.ToInt32(insertedTeacherId);
                            Console.WriteLine($"Teacher added with ID: {teacherId}");
                        }
                        else
                        {
                            throw new Exception("Failed to insert staff.");
                        }
                        
                        Console.WriteLine("Staff Added");
                    }

                    // If the user chose to add a subject this query will execute it and insert it into Courses and linq it to the teacher
                    if (!string.IsNullOrWhiteSpace(subject))
                    {
                        // Retrieve CourseID for the given subject
                        var getCourseIdQuery = "SELECT CourseID FROM Courses WHERE CourseName = @Subject";
                        var courseId = 0;

                        using (var getCourseCommand = new SqlCommand(getCourseIdQuery, conn, transaction))
                        {
                            getCourseCommand.Parameters.AddWithValue("@Subject", subject);
                            var result = getCourseCommand.ExecuteScalar();
                            if (result != null)
                            {
                                courseId = Convert.ToInt32(result);
                            }
                            else
                            {
                                // If the course does not exist, insert it into Courses
                                var addCourseQuery =
                                    "INSERT INTO Courses (CourseName) VALUES (@Subject); SELECT SCOPE_IDENTITY();";
                                using (var addCourseCommand = new SqlCommand(addCourseQuery, conn, transaction))
                                {
                                    addCourseCommand.Parameters.AddWithValue("@Subject", subject);
                                    courseId = Convert.ToInt32(addCourseCommand.ExecuteScalar());
                                    Console.WriteLine($"Course '{subject}' added with ID: {courseId}");
                                }
                            }
                        }

                        // Insert into Enrollments table to assign teacher to the course
                        var assignTeacherQuery = @"
                    INSERT INTO Enrollments (TeacherID_FK, CourseID_FK) 
                    VALUES (@TeacherID, @CourseID)";
                        using (var assignCommand = new SqlCommand(assignTeacherQuery, conn, transaction))
                        {
                            assignCommand.Parameters.AddWithValue("@TeacherID", teacherId);
                            assignCommand.Parameters.AddWithValue("@CourseID", courseId);
                            assignCommand.ExecuteNonQuery();
                            Console.WriteLine("Teacher assigned to the course successfully.");
                        }
                    }
                    transaction.Commit();
                    Ui.Footer();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Console.WriteLine("An error occured while executing: " + e.Message);
                    Console.WriteLine("Error details: " + e.StackTrace);
                }
            }
        }
    }

    public static void GetNewGrades()
    {
        using (var conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            try
            {
                //SQL query to fetch new grades 
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
                        //Formating the result using the DataFormater
                        var data = Ui.DataFormater(reader, "GradeSetDate");
                        var table = Ui.CreateTable(data, "New Grades");
                        AnsiConsole.Write(table);
                        Ui.Footer();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured while executing: " + e.Message);
                Console.WriteLine("Error details: " + e.StackTrace);
                throw;
            }
           
        }
    }
   

    public static void GetAllGrades()
    {
        using (var conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            try
            {
                //SQL query to fetch all grades
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
                        //Formating the result using the DataFormater
                        var data = Ui.DataFormater(reader, "GradeSetDate");
                        var table = Ui.CreateTable(data, "New Grades");
                        AnsiConsole.Write(table);
                        Ui.Footer();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured while executing: " + e.Message);
                Console.WriteLine("Error details: " + e.StackTrace);
                throw;
            }
            
        }
    }

    private static string CheckInput(string prompt)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Input cannot be empty.");
                continue;
            }

            if (!MyRegex().IsMatch(input))
            {
                Console.WriteLine("Input can only contain letters (a-ö, A-Ö), spaces, hyphens, and apostrophes.");
            }
            else
            {
                return input;
            }
        }
    }

    [GeneratedRegex(@"^[a-öA-Ö\s]+$")]
    private static partial Regex MyRegex();
    
}