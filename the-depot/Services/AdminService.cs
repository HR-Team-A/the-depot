using System;
using TheDepot.Repositories;

namespace TheDepot.Services
{
    public static class AdminService
    {
        public static List<string> GetRecommendations()
        {
            List<string> recommendations = new List<string>();
            var reservations = ReservationRepository.All();
            var currentTime = DateTime.Now.TimeOfDay;
            var startedTours = TourRepository.FindByStarted(true);

            foreach (var tour in startedTours)
            {
                int reservationCount = reservations.Count(x => x.Tour_Id == tour.Id);
                int percent = (int)Math.Round((double)(100 / tour.MaxAttendees) * reservationCount);

                if(percent <= 20)
                {
                    recommendations.Add($"De rondleiding om {tour.Time.ToString("HH:mm")} had {reservationCount}/{tour.MaxAttendees} bezoekers. Wij adviseren minder rondleidingen rond dit tijdstip.");
                }
                else
                {
                    recommendations.Add($"De rondleiding om {tour.Time.ToString("HH:mm")} had {reservationCount}/{tour.MaxAttendees} bezoekers. Wij adviseren meer rondleidingen rond dit tijdstip.");
                }
            }
            return recommendations;
        }

        public static void MakeAdminList()
        {
            Console.Clear();
            List<string> recommendations = GetRecommendations();
            if (!recommendations.Any())
            {
                Menu.WriteTemporaryMessageAndReturnToMenu("Op dit moment zijn er geen aanpassingen nodig.");
            }

            // Make string of list, split by new line.
            string recommendationStr = string.Join("\n", recommendations);
            Menu.WriteTemporaryMessageAndReturnToMenu(recommendationStr);
        }

        public static void ScanAdminCode(string message)
        {
            var code = Menu.WriteMessageAndScanCode(message);
            var dayKey = DayKeyRepository.GetByKey(code);
            if (dayKey == null)
            {
                Menu.WriteTemporaryMessageAndReturnToMenu("Deze code is niet gevonden");
            }
            if (dayKey!.Role != Constants.Roles.DepartmentHead)
            {
                Menu.WriteTemporaryMessageAndReturnToMenu("Deze code heeft verkeerde rechten");
            }
            MakeAdminList();
        }
    }
}