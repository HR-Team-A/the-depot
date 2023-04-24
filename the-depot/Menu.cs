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
            //load files
            DayKeyService.LoadDayKeys();

            // Create options that you want your menu to have
            optionsReservation = new List<Option>();

            // Loop through all available tours and add them as an option.
            foreach (var tour in TourService.LoadTours().FindAll(tour => tour.Started))
            {
                AddOption(tour.Time, false);
            }

            optionsReservation.Add(new Option("Rondleiding annuleren", () => CancelReservation("Rondleiding is geannuleerd"), DateTime.MinValue));

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
            
            DayKeyService.LoadDayKeys();

            var dayKey = DayKeyService.GetDayKey(code);
            
            // Code does not exist
            if (dayKey == null)
            {
                WriteTemporaryMessage("Code bestaat niet");
                return;
            }
            
            switch (dayKey.Role)
            {
                case (Constants.Roles.Visitor):
                    DayKeyService.SetDayKeyUsed(code, out string error);
                    if(string.IsNullOrEmpty(error))
                        WriteTemporaryMessage("Reservering is succesvol gemaakt");
                    else
                    {
                        WriteTemporaryMessage(error);
                    }
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
            {
                DayKeyService.CancelReservation(code);
                WriteTemporaryMessage(message);
            }
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

        static void AddOption(DateTime tourTime, bool isTabbed)
        {
            if (isTabbed)
            {
                optionsReservation.Add(new Option("  " + tourTime.ToString("H:mm"), () => WriteMessageAndCodeScan($"{tourTime.ToString("H:mm")} is geselecteerd"), DateTime.MinValue));
            }
            else
            {
                optionsReservation.Add(new Option(tourTime.ToString("H:mm"), () => WriteMessageAndCodeScan($"{tourTime.ToString("H:mm")} is geselecteerd"), DateTime.MinValue));
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