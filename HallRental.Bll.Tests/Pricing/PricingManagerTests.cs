using HallRental.Bll.Common.Pricing;
using HallRental.Bll.Pricing;
using HallRental.Bll.Tests.Shared;
using NSubstitute;

namespace HallRental.Bll.Tests.Pricing;

public class PricingManagerTests
{
    [Fact]
    public async Task GetWorkingHoursPerDayAsync_SumsAllZones()
    {
        var repository = Substitute.For<IHoursPricingRepository>();
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(TestZones.Default());

        var hours = await new PricingManager(repository).GetWorkingHoursPerDayAsync(CancellationToken.None);

        Assert.Equal(17m, hours);
    }
}
