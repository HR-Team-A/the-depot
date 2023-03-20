﻿using ConsoleApp;
using System;
using System.Media;
using System.Windows;

namespace the_depot
{
    public class Menu
    {
        public static List<Option> options = new List<Option>();
        public static List<Option> optionsReserveren = new List<Option>();
        public static List<Option> optionsMinutes = new List<Option>();
        public static string reservationTime;

        static void Main(string[] args)
        {
            // Create options that you want your menu to have
            DateTime dt = DateTime.Parse("11:00:00 AM");

            optionsReserveren = new List<Option>();
            for (int i = 0; i < 7; i++)
            {
                if (i < 1)
                {
                optionsReserveren.Add(new Option(dt.ToString("H:mm"), () => WriteTemporaryMessage("Ticket code input")));
                    DateTime dtMinutes = dt;
                    for (int j = 0; j < 2; j++)
                    {
                        dtMinutes = dtMinutes.AddMinutes(20);
                        optionsReserveren.Add(new Option("  " + dtMinutes.ToString("H:mm"), () => WriteTemporaryMessage("Ticket code input")));
                    }
                }
                if (i >= 1)
                {
                    optionsReserveren.Add(new Option(dt.ToString("H:mm"), () => ChooseMenu(optionsMinutes, dt.ToString("HH"))));
                }
                dt = dt.AddHours(1);
            };

            optionsReserveren.Add(new Option("Rondleiding annuleren", () => WriteTemporaryMessage("Rondleiding is geannuleerd")));

            optionsMinutes = new List<Option>
            {
                new Option(reservationTime + ":00", () => WriteTemporaryMessage(reservationTime + ":00 is geselecteerd")),
                new Option(reservationTime + ":20", () => WriteTemporaryMessage(reservationTime + ":20 is geselecteerd")),
                new Option(reservationTime + ":40", () => WriteTemporaryMessage(reservationTime + ":40 is geselecteerd")),
                new Option("Back", () => ChooseMenu(optionsReserveren))
            };
            ChooseMenu(optionsReserveren);
        }
        // Default action of all the options. You can create more methods
        static void WriteTemporaryMessage(string message)
        {
            Console.Clear();
            Console.WriteLine(message);
            Thread.Sleep(10000);
            ChooseMenu(options);
        }

        static void ChooseMenu(List<Option> options, string reserveTime = "")
        {
            // Set the reservation hour
            reservationTime = reserveTime;

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