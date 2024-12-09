using System.Security.Principal;
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
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Please select an option from the list.")
                .PageSize(10)
                .MoreChoicesText("[grey](Use arrows to move up and down, then press [enter]) [/]")
                .AddChoices("Display Staff", "Display Students", "Display Courses", "Display Grades", "Add Student", "Add Staff"));

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
        var course = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What course would you like to sort by?")
                .AddChoices("{INSERT CHOICES DISPLAYING Courses}"));
        
        var name = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("How would you like to sort Students?")
                .AddChoices("First Name", "Last Name"));
        
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select sorting order")
                .AddChoices("Ascending", "Descending"));
        
        DataService.GetStudentsByCourse(course, name, selection);
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
        DataService.GetAllGrades();
    }
}