using System.Data;
using System.Data.Common;
using Labb3_Anropa_databasen.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using Rule = Spectre.Console.Rule;

namespace Labb3_Anropa_databasen;

public class Ui : DbContext
{
    public const string TextLogo = @"
 _____      _                 _  ____________ 
/  ___|    | |               | | |  _  \ ___ \
\ `--.  ___| |__   ___   ___ | | | | | | |_/ /
 `--. \/ __| '_ \ / _ \ / _ \| | | | | | ___ \
/\__/ / (__| | | | (_) | (_) | | | |/ /| |_/ /
\____/ \___|_| |_|\___/ \___/|_| |___/ \____/ ";


    //Method for auto generating tables
    internal static Table CreateTable<T>(IEnumerable<T>? items, string? title = null)
    {
        if (items == null || !items.Any())
        {
            AnsiConsole.MarkupLine("[red]No data available to display in the table.[/]");
            return new Table(); // Return an empty table or skip rendering.
        }
        
        Console.ForegroundColor = ConsoleColor.DarkRed;
        AnsiConsole.Write(new Rule($"\n\t{title}: ").Centered());
        Console.ResetColor();

        var table = new Table()
            .Centered()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.DarkRed);

        // Get the keys from the first dictionary 
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
        //Method for displaying footer using AirConsole and system Color
        Console.ForegroundColor = ConsoleColor.DarkRed;
        AnsiConsole.Write(new Rule("\n\tPress any key to return or Q to exit").Centered());
        Console.ResetColor();
        //Letting the user exit the program without returning to Main Menu
        var userInput = Console.ReadKey(true).KeyChar.ToString().ToLower();
        if (userInput == "q")
        {
            Console.Clear();
            DisplayCenterdText(TextLogo);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            DisplayCenterdText("Bye bye!");
            Console.ResetColor();
            Thread.Sleep(2000);
            Environment.Exit(0);//Exits the program
        }
        Menus.DisplayMainMenu();
    }
    
    public static void Animation(string title)
    {
        AnsiConsole.Status()
            .AutoRefresh(true) // Automatically refreshes the status during the process
            .Spinner(Spinner.Known.Dots) // Use a dot spinner to indicate activity
            .SpinnerStyle(Style.Parse("yellow bold")) // Set the spinner style to bold yellow
            .Start($"[blue]{title}...[/]", _ =>
            {
                Thread.Sleep(2000); // Simulate a 2-second delay for checking the receiver
            });
    }
    public static void DisplayCenterdText(string text)
    {
        
        var textLines = $"{text}".Split(Environment.NewLine);
        foreach (var line in textLines)
        {
            int padding = (Console.WindowWidth - line.Length) / 2;
            Console.WriteLine(line.PadLeft(line.Length + padding));
        }
    }

    public static List<Dictionary<string, string?>> DataFormater(DbDataReader reader, string? dateColumnName = null, string dateFormat = "yyyy-MM-dd")
    {
        //Initializing a list to hold formatted data
        var data = new List<Dictionary<string, string?>>();
        while (reader.Read())
        {
            //Creates a dictionary to represent a single row of data
            var row = new Dictionary<string, string?>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                if (!string.IsNullOrEmpty(dateColumnName) && columnName.Equals(dateColumnName)) // Date column formatting
                {
                    row[columnName] = reader.IsDBNull(i) ? null : reader.GetDateTime(i).ToString(dateFormat);
                }
                else // General column formatting
                {
                    row[columnName] = reader[i] == DBNull.Value ? string.Empty : reader[i]?.ToString();
                }
            }
            data.Add(row);
        }

        return data;
    }
    public static List<Dictionary<string, string>> ExtractProperties<T>(IEnumerable<T> items, List<string> propertiesToDisplay)
    {
        // Initialize an empty list to hold the formatted data (rows)
        var data = new List<Dictionary<string, string>>();

        foreach (var item in items)
        {
            // Create a dictionary to hold the data for the current item
            var row = new Dictionary<string, string>();

            foreach (var propertyName in propertiesToDisplay)
            {
                var property = item?.GetType().GetProperty(propertyName);
                if (property == null) continue;
                var propertyValue = property.GetValue(item);

                // Format DateTime values into "yyyy-MM-dd"
                if (propertyValue is DateTime dateValue)
                {
                    row[propertyName] = dateValue.ToString("yyyy-MM-dd");
                }
                else
                {
                    // For other properties, convert the value to string, or use empty string if null
                    row[propertyName] = propertyValue?.ToString() ?? string.Empty;
                }
            }

            data.Add(row);
        }

        return data;
    }

    public static void CourseEnrollment(SqlConnection conn, int studentId, SqlTransaction transaction)
    {
        // Create a dictionary to store course names and their corresponding CourseIDs from the database 
        var courses = new Dictionary<string, int>();
        try
        {
            // The SqlCommand is executed within a transaction to maintain consistency.
            using (var fetchCoursesCommand =
                   new SqlCommand("SELECT CourseID, CourseName FROM Courses", conn, transaction))
            using (var reader = fetchCoursesCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    var courseName = reader["CourseName"].ToString();
                    var courseId = (int)reader["CourseID"];
                    if (courseName != null) courses.Add(courseName, courseId);
                }
            }

            if (courses.Count > 0)
            {
                // Let the user select one or more courses using an interactive multi-selection prompt.
                var selectedCourses = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<string>()
                        .Title("Select courses to enroll:")
                        .AddChoices(courses.Keys) // Populate the prompt with the course names from the dictionary
                );
                if (selectedCourses.Count == 0)
                {
                    Console.WriteLine("No courses selected. Returning to the main menu.");
                    return; // Exit method if no course is selected
                }

                foreach (var courseName in selectedCourses)
                {
                    var courseId = courses[courseName];
                    // Prepare the SQL query to insert the enrollment record into the Enrollments table
                    var enrollQuery =
                        "INSERT INTO Enrollments (StudentID_FK, CourseID_FK) VALUES (@StudentID_FK, @CourseID_FK)";
                    using (var enrollCommand = new SqlCommand(enrollQuery, conn, transaction))
                    {
                        // Add parameters for StudentID and CourseID to the SQL command.
                        enrollCommand.Parameters.Add(new SqlParameter("@StudentID_FK", SqlDbType.Int)
                            { Value = studentId });
                        enrollCommand.Parameters.Add(new SqlParameter("@CourseID_FK", SqlDbType.Int)
                            { Value = courseId });

                        enrollCommand.ExecuteNonQuery();
                        Console.WriteLine(
                            $"Successfully enrolled student with ID {studentId} into course {courseName}.");
                    }
                }
            }
            else
            {
                Console.WriteLine("No courses available to enroll.");
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
