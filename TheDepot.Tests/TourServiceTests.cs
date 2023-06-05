using System;
using Moq;
using the_depot;
using the_depot.Interfaces;
using TheDepot.Services;
using Xunit.Abstractions;

namespace TheDepot.Tests
{
	public class TourServiceTests
    {
        [Fact]
        public void SomeTest()
		{

            Mock<IConsole> mock = new Mock<IConsole>();
            mock.SetupSequence(x => x.ReadKey()).Returns("Test 1").Returns("Test 2");

            OutputStatic.Output = mock.Object;


            TourService.StartTourOrMakeReservation(999999);



            mock.Verify(m => m.WriteLine(It.IsIn<string>("", "")),Times.Exactly(2));





            



        }
	}
}

