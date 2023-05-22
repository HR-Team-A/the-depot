using System.Runtime.InteropServices;
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
            // Load files
            DayKeyService.LoadDayKeys();
            LoadReservationOptions();
            ChooseMenu(optionsReservation);
        }
        // Default action of all the options. You can create more methods
        static void WriteTemporaryMessage(string message)
        {
            Console.Clear();
            Console.WriteLine(message);
            Console.WriteLine("Druk op een knop om door te gaan...");
            Console.ReadKey();
            LoadReservationOptions();
            ChooseMenu(optionsReservation);
        }

        // scan the code and show the role
        private static void WriteMessageAndCodeScan(string message, bool tourStarted = false, int tour_Id = 0, bool admin = false)
        {
            Console.Clear();
            if (!admin)
            {
                Console.WriteLine(message);
            }
            Console.WriteLine("Scan code:");
            var code = Console.ReadLine() ?? string.Empty;
            if (tourStarted && code == "stop")
            {
                LoadReservationOptions();
                ChooseMenu(optionsReservation);
            }
            DayKeyService.LoadDayKeys();

            var dayKey = DayKeyService.GetDayKey(code);

            // Code does not exist
            if (dayKey == null)
            {
                if (!tourStarted)
                {
                    WriteTemporaryMessage("Code bestaat niet");
                    return;
                }
                else
                {
                    WriteMessageAndCodeScan("Uw code is niet gevonden", true, tour_Id);
                }
            }
            else
            {
                if (!admin)
                {
                    switch (dayKey.Role)
                    {
                        case (Constants.Roles.Visitor):
                            if (tourStarted)
                            {
                                var error = ReservationService.SetReservationAttended(dayKey.Id, tour_Id);
                                if (string.IsNullOrEmpty(error))
                                {
                                    // We check the OS, only windows supports the useage of Console.Beep();
                                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                    {
                                        Console.Beep();
                                    }
                                    else
                                    {
                                        Console.WriteLine("Successful beep sound!");
                                    }
                                    WriteMessageAndCodeScan($"{TourService.GetTourStartingInformation(tour_Id)}  \nU ben successvol aangemeld, laat de volgende bezoeker hun code scannen", true, tour_Id);
                                }
                                WriteMessageAndCodeScan(error, true, tour_Id);
                            }
                            else
                            {
                                var info = ReservationService.AddReservation(dayKey.Id, tour_Id);
                                if (string.IsNullOrEmpty(info))
                                    WriteTemporaryMessage("Reservering is succesvol gemaakt");
                                else
                                {
                                    WriteTemporaryMessage(info);
                                }
                            }
                            break;
                        case (Constants.Roles.Guide):
                            TourService.StartTour(tour_Id);
                            WriteMessageAndCodeScan($"{TourService.GetTourStartingInformation(tour_Id)} \nRondleiding gestart, laat de bezoekers hun code scannen:", true, tour_Id);
                            break;
                        case (Constants.Roles.DepartmentHead):
                            if (!tourStarted)
                                WriteTemporaryMessage("Code is voor afdelingshoofd, reserveren is niet mogelijk.");
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
        }

        //cancel the reservation 
        static void CancelReservation(string message)
        {
            Console.Clear();
            Console.WriteLine("Scan code:");
            var code = Console.ReadLine() ?? string.Empty;
            if (DayKeyService.GetDayKey(code)?.Role == Constants.Roles.Visitor)
            {
                var id = DayKeyService.GetDayKey(code)?.Id;
                if (id != null)
                {
                    ReservationService.CancelReservation((int)id, out string error);
                    if (!string.IsNullOrEmpty(error))
                    {
                        WriteTemporaryMessage(error);
                    }
                }

                WriteTemporaryMessage(message);
            }
            else
                WriteTemporaryMessage("Code is niet geldig");
        }

        static void ShowAdminData()
        {
            Console.Clear();
            List<string> recommendations = AdminService.GetRecommendations();
            if (!recommendations.Any())
            {
                WriteTemporaryMessage("Op dit moment zijn er geen aanpassingen nodig.");
            }
            
            // Make string of list, split by new line.
            string recommendationStr = string.Join("\n", recommendations);
            WriteTemporaryMessage(recommendationStr);
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
                Console.WriteLine("Vul een datum in (dd/mm/yyyy):");
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
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Gebruik de pijltjestoetsen om door het menu bewegen.");
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

        static void AddOption(Tour tour)
        {
            var tourTime = tour.Time;
            int tourId = tour.Id;
            int maxAttendees = tour.MaxAttendees;
            int attendees = TourService.GetAttendeesCount(tour.Id);
            int availableSpots = maxAttendees - attendees;
            
            string text = availableSpots > 0 ? $"{tourTime.ToString("H:mm")} - Vrije plekken: {availableSpots}" : $"{tourTime.ToString("H:mm")} - Geen vrij plekken";
            
            optionsReservation.Add(new Option(text, () => WriteMessageAndCodeScan($"{tourTime.ToString("H:mm")} is geselecteerd", false, tourId), DateTime.MinValue));
        }

        static void LoadReservationOptions()
        {
            // Create options that you want your menu to have
            optionsReservation = new List<Option>();
            string dateTimeNowStr = DateTime.Now.ToShortTimeString();
            // Loop through all available tours and add them as an option.
            foreach (var tour in TourService.LoadTours().FindAll(tour => !tour.Started))
            {
                string tourTime = tour.Time.ToShortTimeString();
                if (DateTime.Parse(dateTimeNowStr) < DateTime.Parse(tourTime))
                {
                    AddOption(tour);
                }
            }
            
            optionsReservation.Add(new Option("Rondleiding annuleren", () => CancelReservation("Rondleiding is geannuleerd"), DateTime.MinValue));
            optionsReservation.Add(new Option("Afdelingshoofd menu", () => WriteMessageAndCodeScan("", false, 0, true), DateTime.MinValue));
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
