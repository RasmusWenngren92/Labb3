using System.Security.Principal;
using System.Xml;
using Labb3_Anropa_databasen.Data;
using Labb3_Anropa_databasen.Models;
using Spectre.Console;

namespace Labb3_Anropa_databasen;

public class Menus
{
    public Menus()
    {
    }

    public static void DisplayMainMenu()
    {
        //AnsiConsole for displaying choices presented to the user, 
        //preventing any errors by only displaying available choices
        Console.Clear();
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Please select an option from the list.")
                .PageSize(10)
                .MoreChoicesText("[grey](Use arrows to move up and down, then press [enter]) [/]")
                .AddChoices("Display Staff", "Display Students", "Display Courses", "Display Grades", "Add Student", "Add Staff","Exit"));

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
                Console.WriteLine("\n\tBye bye :) ");
                Thread.Sleep(2000);
                return;
        }
    }

    public static void DisplayStaff()
    {

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What do you want to show?")
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
                .Title("What do you want to show?")
                .AddChoices("Show all Students","Show all Students by Course", "Main Menu"));

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
                .Title("How would you like to sort Students?")
                .AddChoices("First Name", "Last Name"));
        
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select sorting order")
                .AddChoices("Ascending", "Descending"));
        
        DataService.GetAllStudents(name, selection);
    }

    public static void StudentsByCourse()
    {
        using (var context = new SchoolDbContext())
        {
            
            var indentedCourses = context.Courses
                        .ToDictionary(course => $"  {course.CourseName}", course => course);
                    
                    var courseName = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("What course would you like to sort by?")
                            .AddChoices(indentedCourses.Keys)
                        );
                        
                        var selectedCourse = indentedCourses.Values
                            .FirstOrDefault(c => c.CourseName.Equals(courseName.Trim(), StringComparison.OrdinalIgnoreCase));

                        if (selectedCourse == null)
                        {
                            Console.WriteLine("No course selected.");
                            return;
                        }
                    
                    var name = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("How would you like to sort Students?")
                            .AddChoices("First Name", "Last Name")
                        );
                    
                    var selection = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Select sorting order")
                            .AddChoices("Ascending", "Descending")
                        );
                    
                        DataService.GetStudentsByCourse(selectedCourse.CourseId, name, selection);
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
                .Title("What do you want to show?")
                .AddChoices("New Grades", "All Grades", "Main Menu"));

        switch (choice)
        {
            case "New Grades":
                Console.WriteLine("Fetching new grades...");
                Thread.Sleep(2000);
                DataService.GetNewGrades();
                break;

            case "All Grades":
                Console.WriteLine("Fetching all grades...");
                Thread.Sleep(2000);
                DataService.GetAllGrades();  
                break;

            case "Main Menu":
                Console.WriteLine("Returning to main menu...");
                DisplayMainMenu();
                break;
        }
        
    }
}