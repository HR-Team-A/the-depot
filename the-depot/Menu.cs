using ConsoleApp;
using the_depot.Models;
using the_depot.Services;

namespace the_depot
{
    public class Menu
    {
        public static List<Option> options = new List<Option>();
        public static List<Option> optionsReservation = new List<Option>();
        public static List<Option> optionsMinutes = new List<Option>();

        static void Main(string[] args)
        {
            // Create options that you want your menu to have
            DateTime dt = DateTime.Parse("11:00:00 AM");

            optionsReservation = new List<Option>();
            for (int i = 0; i < 7; i++)
            {
                if (i < 6)
                {
                    optionsReservation.Add(new Option(dt.ToString("H:mm"), () => WriteTemporaryMessage("Ticket code input"), DateTime.MinValue));
                    DateTime dtMinutes = dt;
                    for (int j = 0; j < 2; j++)
                    {
                        dtMinutes = dtMinutes.AddMinutes(20);
                        optionsReservation.Add(new Option("  " + dtMinutes.ToString("H:mm"), () => WriteTemporaryMessage("Ticket code input"), DateTime.MinValue));
                    }
                }
                if (i >= 6)
                {
                    optionsReservation.Add(new Option(dt.ToString("H:mm"), () => WriteTemporaryMessage("Ticket code input"), DateTime.MinValue));
                }
                dt = dt.AddHours(1);
            };

            optionsReservation.Add(new Option("Rondleiding annuleren", () => WriteTemporaryMessage("Rondleiding is geannuleerd"), DateTime.MinValue));

            ChooseMenu(optionsReservation);
        }
        // Default action of all the options. You can create more methods
        static void WriteTemporaryMessage(string message)
        {
            Console.Clear();
            Console.WriteLine(message);
            Thread.Sleep(5000);
            ChooseMenu(optionsReservation);
        }

        // scan the code and show the role
        private static void WriteMessageAndCodeScan(string message)
        {
            Console.Clear();
            Console.WriteLine(message);
            Console.WriteLine("Scan code:");
            var code = Console.ReadLine() ?? string.Empty;
            //temp messages 
            switch (CodeValidationService.GetRole(code))
            {
                case (Constants.Roles.Visitor):
                    WriteTemporaryMessage("Reservering is succesvol gemaakt");
                    break;
                case (Constants.Roles.Guide):
                    WriteTemporaryMessage("Todo: rondleiding starten");
                    break;
                case (Constants.Roles.DepartmentHead):
                    WriteTemporaryMessage("Reservering is succesvol gemaakt");
                    break;
                default:
                    WriteTemporaryMessage("Code is niet geldig");
                    break;
            }
        }

        //cancel the reservation 
        static void CancelReservation(string message)
        {
            Console.Clear();
            Console.WriteLine("Scan code:");
            var code = Console.ReadLine() ?? string.Empty;
            if (CodeValidationService.GetRole(code) == Constants.Roles.Visitor)
                WriteTemporaryMessage(message);
            else
                WriteTemporaryMessage("Code is niet geldig");
        }

        static void ChooseMenu(List<Option> options)
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
        public DateTime Time { get; }

        public Option(string name, Action selected, DateTime time)
        {
            Time = time;
            Name = name;
            Selected = selected;
        }
    }
}