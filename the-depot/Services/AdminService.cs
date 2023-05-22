namespace the_depot.Services
{
    public static class AdminService
    {
        public static List<string> GetRecommendations()
        {
            List<string> recommendations = new List<string>();
            
            var reservations = ReservationService.LoadReservations();
            
            var currentTime = DateTime.Now.TimeOfDay;
            var tours = TourService.LoadTours().Where(x => x.Started);

            foreach (var tour in tours)
            {
                int reservationCount = reservations.Count(x => x.Tour_Id == tour.Id);
                int percent = (int)Math.Round((double)(100 / tour.MaxAttendees) * reservationCount);

                if(percent < 20)
                {
                    recommendations.Add($"De rondleiding om {tour.Time.ToString("HH:mm")} had {reservationCount}/{tour.MaxAttendees} bezoekers. Wij adviseren minder bezoekers toe te laten, of deze rondleiding te laten vervallen.");
                }
                else if( percent > 80)
                {
                    recommendations.Add($"De rondleiding om {tour.Time.ToString("HH:mm")} had {reservationCount}/{tour.MaxAttendees} bezoekers. Wij adviseren meer bezoekers toe te laten, of een rondleiding toe te voegen.");
                }
            }

            return recommendations;
        }
    }
}