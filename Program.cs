namespace Labb3_Anropa_databasen;

class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        Ui.DisplayCenterdText(Ui.TextLogo);
        Thread.Sleep(2000);
        Menus.DisplayMainMenu();
    }
}