using System;
using System.IO;
using Moq;
using the_depot;
using the_depot.Interfaces;
using TheDepot.Services;

namespace TheDepot.Tests
{
	public class ReservationServiceTests
	{
		[Fact]
        public void ScanCodeAndCancelReservation()
        {
            //arrange
            string firstMessage = "Rondleiding is geannuleerd.";
            string secondMessage = "Druk op een knop om terug te gaan naar het hoofdmenu.";
            Mock<IConsole> mock = new Mock<IConsole>();
            mock.SetupSequence(x => x.ReadLine()).Returns("2");
            mock.SetupSequence(x => x.ReadKey()).Returns("2").Returns("2");
            OutputStatic.Output = mock.Object;

            //execute
            ReservationService.ScanCodeAndCancelReservation("Rondleiding is geannuleerd.");

            //validate
            mock.Verify(m => m.WriteLine(It.IsIn<string>(firstMessage, secondMessage)), Times.Exactly(2));

        }

        ////arrange -- maken van benodigde classes etc.
        //List<string> expectation = new List<string> {
        //    "De rondleiding om 14:00 had 0/13 bezoekers. Wij adviseren minder bezoekers toe te laten, of deze rondleiding te laten vervallen."

        //};
        ////execute -- aanroepen functie die je wil testen
        //var result = AdminService.GetRecommendations();

        ////validate
        //Assert.Equal(expectation, result);
        //Assert.NotEmpty(result);
    }
}

