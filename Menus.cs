using Spectre.Console;

namespace Labb3_Anropa_databasen;

public class Menus
{



// - [ ]  Lägga till nya elever (kan lösas med [ADO.NET](http://ADO.NET) och SQL, annars Entity framework)
//         Användaren får möjlighet att mata in uppgifter om en ny elev och den datan sparas då ner i databasen.

// - [ ]  Lägga till ny personal (ska lösas genom Entity framework)
// Användaren får möjlighet att mata in uppgifter om en ny anställd och den data sparas då ner i databasen.
    public static void DisplayMainMenu()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Please select an option from the list.")
                .PageSize(10)
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
        // - [ ]  Hämta personal (kan lösas med [ADO.NET](http://ADO.NET) och SQL, annars Entity framework)
// Användaren får välja om denna vill se alla anställda, eller bara inom en av kategorierna så som ex lärare.

    }
    public static void DisplayStudents()
    {
        // - [ ]  Hämta alla elever (ska lösas med Entity framework)
// Användaren får välja om de vill se eleverna sorterade på för- eller efternamn och om det ska vara stigande eller fallande sortering.

// - [ ]  Hämta alla elever i en viss klass (ska lösas med Entity framework)
// Användaren ska först få se en lista med alla klasser som finns, sedan får användaren välja en av klasserna och då skrivs alla elever i den klassen ut.

//     🏆 Extra utmaning (Frivillig): låt användaren även få välja sortering på eleverna som i punkten ovan.
// - [ ]  Hämta alla betyg som satts den senaste månaden (kan lösas med [ADO.NET](http://ADO.NET) och SQL, annars Entity framework)
//         Här får användaren direkt en lista med alla betyg som satts senaste månaden där elevens namn, kursens namn och betyget framgår.

    }

    public static void DisplayCourses()
    {
        // - [ ]  Hämta en lista med alla kurser och det snittbetyg som eleverna fått på den kursen samt det högsta och lägsta betyget som någon fått i kursen (kan lösas med [ADO.NET](http://ADO.NET) och SQL, annars Entity framework)
//         Här får användaren direkt upp en lista med alla kurser i databasen, snittbetyget samt det högsta och lägsta betyget för varje kurs.
//             💡 Tips: Det kan vara svårt att göra detta med betyg i form av bokstäver så du kan välja att lagra betygen som siffror istället.

    }

    public static void AddStudent()
    {
        
    }

    public static void AddStaff()
    {
        
    }
}