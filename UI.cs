using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Spectre.Console;

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


    internal static Table CreateTable<T>(IEnumerable<T> items, string? title = null)
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
                Thread.Sleep(2000); // Simulate a 3-second delay for checking the receiver
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
    
}
