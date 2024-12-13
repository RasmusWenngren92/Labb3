using System.Security.Principal;
using System.Xml;
using Labb3_Anropa_databasen.Data;
using Labb3_Anropa_databasen.Models;
using Spectre.Console;

namespace Labb3_Anropa_databasen;

public static class Menus
{
    public static void DisplayMainMenu()
    {
        Console.Clear();
        Ui.DisplayCenterdText(Ui.TextLogo);
        Console.WriteLine();
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\nPlease select an option from the list.")
                .PageSize(10)
                .MoreChoicesText("[grey](Use arrows to move up and down, then press [enter]) [/]")
                .AddChoices("Display Staff", "Display Students", "Display Courses", "Display Grades", "Add Student",
                    "Add Staff", "Exit"));

        switch (choice)
        {
            case "Display Staff":
                DisplayStaff();
                break;
            case "Display Students":
                DisplayStudents();
                break;
            case "Display Courses":
                DisplayCourses();
                break;
            case "Display Grades":
                DisplayGrades();
                break;
            case "Add Student":
                AddStudent();
                break;
            case "Add Staff":
                AddStaff();
                break;
            case "Exit":
                Console.Clear();
                Ui.DisplayCenterdText(Ui.TextLogo);
                Console.WriteLine();
                Ui.DisplayCenterdText("Bye bye!");
                Thread.Sleep(2000);
                return;
        }
    }

    public static void DisplayStaff()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\nWhat do you want to show?")
                .AddChoices("All Staff", "Only Teachers", "Main Menu"));
        switch (choice)
        {
            case "All Staff":
                DisplayAllStaff();
                break;
            case "Only Teachers":
                DisplayTeachers();
                break;
            case "Main Menu":
                DisplayMainMenu();
                break;
        }
    }

    public static void DisplayStudents()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\nWhat do you want to show?")
                .AddChoices("Show all Students", "Show all Students by Course", "Main Menu"));

        switch (choice)
        {
            case "Show all Students":
                ShowAllStudents();
                break;
            case "Show all Students by Course":
                StudentsByCourse();
                break;
            case "Main Menu":
                DisplayMainMenu();
                return;
        }
    }

    public static void DisplayCourses()
    {
        DataService.GetAllCourses();
    }

    public static void AddStudent()
    {
        DataService.AddStudent();
    }

    public static void AddStaff()
    {
        DataService.AddStaff();
    }

    public static void ShowAllStudents()
    {
        var name = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\nHow would you like to sort Students?")
                .AddChoices("First Name", "Last Name"));

        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\nSelect sorting order")
                .AddChoices("Ascending", "Descending"));

        DataService.GetAllStudents(name, selection);
    }

    public static void StudentsByCourse()
    {
        Console.Clear();
        Ui.DisplayCenterdText(Ui.TextLogo);
        using (var context = new SchoolDbContext())
        {
            var indentedCourses = context.Courses
                .ToDictionary(course => $"  {course.CourseName}", course => course);
            
            var courseName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\nWhat course would you like to sort by?")
                    .AddChoices(indentedCourses.Keys)
            );
            
            var selectedCourse = indentedCourses.Values
                .FirstOrDefault(c => c.CourseName.Equals(courseName.Trim()));
            
            
            var name = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\nHow would you like to sort Students?")
                    .AddChoices("First Name", "Last Name")
            );
            
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\nSelect sorting order")
                    .AddChoices("Ascending", "Descending")
            );
            
            if (selectedCourse != null) DataService.GetStudentsByCourse(selectedCourse.CourseId, name, selection);
        }
        
    }

    public static void DisplayAllStaff()
    {
        DataService.GetAllStaff();
    }

    public static void DisplayTeachers()
    {
        DataService.GetTeachers();
    }

    public static void DisplayGrades()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\nWhat do you want to show?")
                .AddChoices("New Grades", "All Grades", "Main Menu"));

        switch (choice)
        {
            case "New Grades":
                Ui.Animation("Fetching new grades");
                // Thread.Sleep(2000);
                DataService.GetNewGrades();
                break;

            case "All Grades":
                Ui.Animation("Fetching all grades");
                // Thread.Sleep(2000);
                DataService.GetAllGrades();
                break;

            case "Main Menu":
                Console.WriteLine("\nReturning to main menu...");
                DisplayMainMenu();
                break;
        }
    }
}