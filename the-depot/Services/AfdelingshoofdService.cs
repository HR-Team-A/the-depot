namespace the_depot.Services
{
    public static class AfdelingshoofdService
    {
        public static List<string> GetRecommendations()
        {
            List<string> recommendations = new List<string>();

            var reservations = ReservationService.LoadReservations();
            var tours = TourService.LoadTours();

            foreach (var tour in tours)
            {
                int reservationCount = reservations.Count(x => x.Tour_Id == tour.Id);
                int percent = (int)Math.Round((double)(100 / tour.MaxAttendees) * reservationCount);

                if(percent < 20)
                {
                    recommendations.Add($"De rondleiding om {tour.Time.ToString("HH:mm")} had {reservationCount} bezoekers. Je kan deze beter kleiner maken of laten vervallen.");
                }
                else if( percent > 80)
                {
                    recommendations.Add($"De rondleiding om {tour.Time.ToString("HH:mm")} had {reservationCount} bezoekers. Je kan deze beter groter maken of een rondleiding toevoegen.");
                }
            }

            return recommendations;
        }
    }
}