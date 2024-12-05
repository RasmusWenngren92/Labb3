using Spectre.Console;

namespace Labb3_Anropa_databasen;

public class Menus
{
    public static void DisplayMainMenu()
    {
        //AnsiConsole for displaying choices presented to the user, 
        //preventing any errors by only displaying available choices
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Please select an option from the list.")
                .PageSize(10)
                .MoreChoicesText("[grey](Use arrows to move up and down, then press [enter]) [/]")
                .AddChoices("Display Staff", "Display Students", "Display Courses", "Add Student", "Add Staff"));

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
                return;
            
        }
        
    }

    public static void DisplayCourses()
    {
        //Call method for displaying ALL Courses, average Grade,
        //and lowest and highest grade
    }

    public static void AddStudent()
    {
        //Call Method for creating a new student
    }

    public static void AddStaff()
    {
        //Call Method for creating a new student
    }

    public static void ShowAllStudents()
    {
        var chocie = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("How would you like to sort Students?")
                .AddChoices("First Name", "Last Name"));
        
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select sorting order")
                .AddChoices("Ascending", "Descending"));
        
        //Call method to display students in desired order {choice} {selection}
    }

    public static void StudentsByCourse()
    {
        var choice = AnsiConsole.Prompt(
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
        
        //Call method to display students in desired order of {class} {name} {selection}
    }

    public static void DisplayAllStaff()
    {
        //Call method for displaying all staff
    }

    public static void DisplayTeachers()
    {
        //Call method for displaying Teachers
    }
}