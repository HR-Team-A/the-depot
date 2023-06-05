using System.Runtime.InteropServices;
using ConsoleApp;
using TheDepot.Models;
using TheDepot.Repositories;
using TheDepot.Services;

namespace TheDepot
{
    public class Menu
    {
        static void Main(string[] args)
        {
            // Load files
            var options = TourService.MakeToursMenuList();
            ChooseMenu(options);
        }

        public static void WriteTemporaryMessage(string message, string returnMessage) {
            Console.Clear();
            Console.WriteLine(message);
            Console.WriteLine(returnMessage);
            Console.ReadKey();
        }

        // Default action of all the options. You can create more methods
        public static void WriteTemporaryMessageAndReturnToMenu(string message)
        {
            WriteTemporaryMessage(message, "Druk op een knop om terug te gaan naar het hoofdmenu.");
            var options = TourService.MakeToursMenuList();
            ChooseMenu(options);
        }

        // scan the code and show the role
        public static string WriteMessageAndScanCode(string message, bool tourStarted = false, int tour_Id = 0, bool admin = false)
        {
            Console.Clear();
            Console.WriteLine(message);
            var code = Console.ReadLine() ?? string.Empty;
            return code;
        }
        public static void ChooseMenu(List<Option> options)
        {
            // Set the reservation hour

            // Set the default index of the selected item to be the first
            int index = 0;

            // Write the menu out
            WriteMenu(options, options[index]);

            // Store key info in here
            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();

                // Handle each key input (down arrow will write the menu again with a different selected item)
                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (index + 1 < options.Count)
                    {
                        index++;
                        WriteMenu(options, options[index]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (index - 1 >= 0)
                    {
                        index--;
                        WriteMenu(options, options[index]);
                    }
                }
                // Handle different action for the option
                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    options[index].Selected.Invoke();
                    index = 0;
                }
            }
            while (keyinfo.Key != ConsoleKey.X);

            Console.ReadKey();
        }

        static void WriteMenu(List<Option> options, Option selectedOption)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("________                        __   \n" +
                "\\______ \\   ____ ______   _____/  |_ \n" +
                " |    |  \\_/ __ \\\\____ \\ /  _ \\   __\\\n" +
                " |    `   \\  ___/|  |_> >  <_> )  |  \n" +
                "/_______  /\\___  >   __/ \\____/|__|  \n" +
                "        \\/     \\/|__|                \n" +
                " \n" +
                "Welkom bij Depot Boijmans Van Beuningen \n" +
                "Het eerste publiek toegankelijk kunstdepot ter wereld. \n" +
                "Gebruik de pijltjestoetsen om door het menu te navigeren en een reservering te kunnen plaatsen. \n" +
                " ");
            foreach (Option option in options)
            {
                if (option == selectedOption)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("> ");
                }
                else
                {
                    Console.Write(" ");
                }

                Console.WriteLine(option.Name);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
