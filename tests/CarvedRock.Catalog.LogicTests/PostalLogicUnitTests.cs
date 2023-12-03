using FluentAssertions;
using CarvedRock.Catalog.Logic;
using Xunit;

namespace CarvedRock.Catalog.LogicTests;

public class PostalLogicUnitTests
{
    // NOTE: if the logic object below has dependencies for its constructor you would use the NSubstitute library and set them  up inside
    // a constructor for this class.
    private readonly PostalCodeLogic _logic = new();

    [Fact]
    public void FiveOnesShouldThrowException()
    {
        var act = () => _logic.GetCityForPostalCode("11111");
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void FiveTwosShouldThrowApplicationException()
    {
        var act = () => _logic.GetCityForPostalCode("22222");
        act.Should().Throw<ApplicationException>();
    }

    [Fact]
    public void NormalPostalCodeReturnsCityName()
    {
        var city = _logic.GetCityForPostalCode("12345");
        var citiesInLogic = new List<string>
            { "New York", "Chicago", "Minneapolis", "Seattle", "Huntington Beach", "Dallas" };
        city.Should().BeOneOf(citiesInLogic);
    }
}
