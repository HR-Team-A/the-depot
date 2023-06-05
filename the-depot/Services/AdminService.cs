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

                if(percent < 20)
                {
                    recommendations.Add($"De rondleiding om {tour.Time.ToString("HH:mm")} had {reservationCount}/{tour.MaxAttendees} bezoekers. Wij adviseren minder rondleidingen rond dit tijdstip.");
                }
                else if( percent > 80)
                {
                    recommendations.Add($"De rondleiding om {tour.Time.ToString("HH:mm")} had {reservationCount}/{tour.MaxAttendees} bezoekers. Wij adviseren meer rondleidingen rond dit tijdstip.");
                }
            }

            return recommendations;
        }

        public static void MakeAdminList()
        {
            Console.Clear();
            List<string> recommendations = AdminService.GetRecommendations();
            if (!recommendations.Any())
            {
                Menu.WriteTemporaryMessageAndReturnToMenu("Op dit moment zijn er geen aanpassingen nodig.");
            }

            // Make string of list, split by new line.
            string recommendationStr = string.Join("\n", recommendations);
            Menu.WriteTemporaryMessageAndReturnToMenu(recommendationStr);
        }
    }
}