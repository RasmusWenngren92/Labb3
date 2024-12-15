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
        Console.ForegroundColor = ConsoleColor.DarkRed;
        AnsiConsole.Write(new Rule("\n\tPress any key to return ").Centered());
        Console.ResetColor();
        Console.ReadLine();
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

    public static List<Dictionary<string, string?>> DataFormater(DbDataReader reader, string? dateColumnName = null)
    {
        var data = new List<Dictionary<string, string?>>();
        while (reader.Read())
        {
            var row = new Dictionary<string, string?>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                if (!string.IsNullOrEmpty(dateColumnName) && columnName.Equals(dateColumnName))
                {
                    row[columnName] = reader.IsDBNull(i) ? null : ((DateTime)reader.GetValue(i)).ToString("yyyy-MM-dd");
                }
                else
                {
                    row[columnName] = reader.IsDBNull(i) ? string.Empty : reader.GetValue(i).ToString();
                }
            }
            data.Add(row);
        }

        return data;
    }
    public static List<Dictionary<string, string>> ExtractProperties<T>(IEnumerable<T> items, List<string> propertiesToDisplay)
    {
        var data = new List<Dictionary<string, string>>();

        foreach (var item in items)
        {
            var row = new Dictionary<string, string>();

            foreach (var propertyName in propertiesToDisplay)
            {
                var property = item?.GetType().GetProperty(propertyName);
                if (property != null)
                {
                    var propertyValue = property.GetValue(item);

                    // Format DateTime values
                    if (propertyValue is DateTime dateValue)
                    {
                        row[propertyName] = dateValue.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        row[propertyName] = propertyValue?.ToString() ?? string.Empty;
                    }
                }
            }

            data.Add(row);
        }

        return data;
    }

    public static void CourseEnrollment(SqlConnection conn, int studentId, SqlTransaction transaction)
    {
        var courses = new Dictionary<string, int>();
        using (var fetchCoursesCommand = new SqlCommand("SELECT CourseID, CourseName FROM Courses", conn, transaction))
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
            var selectedCourses = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("Select courses to enroll:")
                    .AddChoices(courses.Keys)
            );

            foreach (var courseName in selectedCourses)
            {
                int courseId = courses[courseName];
                var enrollQuery = "INSERT INTO Enrollments (StudentID_FK, CourseID_FK) VALUES (@StudentID_FK, @CourseID_FK)";
                using (var enrollCommand = new SqlCommand(enrollQuery, conn, transaction))
                {
                    enrollCommand.Parameters.Add(new SqlParameter("@StudentID_FK", SqlDbType.Int) { Value = studentId });
                    enrollCommand.Parameters.Add(new SqlParameter("@CourseID_FK", SqlDbType.Int) { Value = courseId });

                    enrollCommand.ExecuteNonQuery();
                    Console.WriteLine($"Successfully enrolled student with ID {studentId} into course {courseName}.");
                }
            }
        }
        else
        {
            Console.WriteLine("No courses available to enroll.");
        }
    }

}
