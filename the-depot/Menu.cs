using ConsoleApp;
using System;

namespace the_depot
{
    public class Menu
    {
        public static List<Option> options;
        public static List<Option> options_reserveren;
        static void Main(string[] args)
        {
            // Create options that you want your menu to have
            options = new List<Option>
                {
                    new Option("Rondleiding reserveren", () => ChooseMenu(options_reserveren)),
                    new Option("Rondleiding annuleren", () =>  WriteTemporaryMessage("Rondleiding is geannuleerd")),
                    new Option("Exit", () => Environment.Exit(0)),
                };

            options_reserveren = new List<Option>
            {
                new Option("11:00 -- 12:00", () => WriteTemporaryMessage("tijd 1")),
                new Option("12:00 -- 13:00", () => WriteTemporaryMessage("tijd 2")),
                new Option("13:00 -- 14:00", () => WriteTemporaryMessage("tijd 3")),
                new Option("14:00 -- 15:00", () => WriteTemporaryMessage("tijd 4")),
                new Option("15:00 -- 16:00", () => WriteTemporaryMessage("tijd 5")),
                new Option("16:00 -- 17:00", () => WriteTemporaryMessage("tijd 6")),
                new Option("17:00", () => WriteTemporaryMessage("tijd 7")),
            };

            ChooseMenu(options);
        }
        // Default action of all the options. You can create more methods
        static void WriteTemporaryMessage(string message)
        {
            Console.Clear();
            Console.WriteLine(message);
            Thread.Sleep(3000);
            WriteMenu(options_reserveren, options_reserveren.First());
        }

        static void ChooseMenu(List<Option> options)
        {
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

            foreach (Option option in options)
            {
                if (option == selectedOption)
                {
                    Console.Write("> ");
                }
                else
                {
                    Console.Write(" ");
                }

                Console.WriteLine(option.Name);
            }
        }
    }
    public class Option
    {
        public string Name { get; }
        public Action Selected { get; }

        public Option(string name, Action selected)
        {
            Name = name;
            Selected = selected;
        }
    }
}