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
        public static List<Option> adminOptions = new List<Option>();

        static void Main(string[] args)
        {
            //load files
            DayKeyService.LoadDayKeys();

            // Create options that you want your menu to have
            DateTime tourTime = DateTime.Parse("11:00:00 AM");

            optionsReservation = new List<Option>();
            for (int i = 0; i < 7; i++)
            {
                if (i < 6)
                {
                    AddOption(tourTime, false);
                    for (int j = 0; j < 2; j++)
                    {
                        tourTime = tourTime.AddMinutes(20);
                        AddOption(tourTime, true);
                    }
                }
                if (i >= 6)
                {
                    AddOption(tourTime, false);
                }
                tourTime = tourTime.AddMinutes(20);
            };

            optionsReservation.Add(new Option("Rondleiding annuleren", () => CancelReservation("Rondleiding is geannuleerd"), DateTime.MinValue));
            optionsReservation.Add(new Option("Admin scherm", () => WriteMessageAndCodeScan("", true), DateTime.MinValue));

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
        private static void WriteMessageAndCodeScan(string message, bool admin)
        {
            Console.Clear();
            if (!admin)
            {
                Console.WriteLine(message);
            }
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

            if (!admin)
            {
                switch (dayKey.Role)
                {
                    case (Constants.Roles.Visitor):
                        DayKeyService.SetDayKeyUsed(code, out string error);
                        if (string.IsNullOrEmpty(error))
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
            else
            {
                switch (dayKey.Role)
                {
                    case (Constants.Roles.DepartmentHead):
                        ShowAdminData();
                        break;
                    default:
                        WriteTemporaryMessage("Code is niet geldig");
                        break;
                }
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

        static void ShowAdminData()
        {
            Console.Clear();
            adminOptions = new List<Option>();
            adminOptions.Add(new Option("Dag overzicht", () => ChooseDate(), DateTime.MinValue));
            adminOptions.Add(new Option("Week overzicht", () => ChooseDate(), DateTime.MinValue));
            adminOptions.Add(new Option("Maand overzicht", () => ChooseDate(), DateTime.MinValue));

            ChooseMenu(adminOptions);
        }

        static void ChooseDate()
        {
            Console.Clear();
            Console.WriteLine("Vul een datum in (dd/mm/yyyy):");

            DateTime date;
            string[] formats = { "dd/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "d/MM/yyyy",
                "dd/MM/yy", "dd/M/yy", "d/M/yy", "d/MM/yy", "yyyy/MM/dd", "yyyy/M/dd", "yyyy/MM/d", "yyyy/M/d"};

            while (!DateTime.TryParseExact(Console.ReadLine(), formats,
                 System.Globalization.CultureInfo.InvariantCulture,
                 System.Globalization.DateTimeStyles.None,
                 out date))
            {
                Console.Clear();
                Console.WriteLine("Vul een datum in:");
                Console.WriteLine("Verkeerde input. Probeer opnieuw.");
            }
            Console.Clear();
            Console.WriteLine("Datum geselecteerd: " + date);
            Console.WriteLine("TODO: Add data");
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
                optionsReservation.Add(new Option("  " + tourTime.ToString("H:mm"), () => WriteMessageAndCodeScan($"{tourTime.ToString("H:mm")} is geselecteerd", false), DateTime.MinValue));
            }
            else
            {
                optionsReservation.Add(new Option(tourTime.ToString("H:mm"), () => WriteMessageAndCodeScan($"{tourTime.ToString("H:mm")} is geselecteerd", false), DateTime.MinValue));
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
