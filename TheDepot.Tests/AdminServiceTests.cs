using TheDepot.Services;

namespace TheDepot.Tests;

public class AdminServiceTests
{
    [Fact]
    public void GetRecommendations()
    {
        //arrange -- maken van benodigde classes etc.
        List<string> expectation = new List<string> {
            "De rondleiding om 14:00 had 0/13 bezoekers. Wij adviseren minder bezoekers toe te laten, of deze rondleiding te laten vervallen."

        };
        //execute -- aanroepen functie die je wil testen
        var result = AdminService.GetRecommendations();

        //validate
        Assert.Equal(expectation, result);
        Assert.NotEmpty(result);

    }
}
