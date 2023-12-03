namespace CarvedRock.Catalog.IntegrationTests;

[Collection("API Integration Test")]
public class WeatherForecastControllerTests(ITestOutputHelper output, CustomApiFactory<Program> factory, SharedFixture shared) 
    : BaseTest(output, factory, shared)
{
    [Fact]
    public async Task Anonymous_Request_Gets_Unauthorized()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/v1/weatherforecast");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("45326")]
    public async Task Authenticated_Request_Gets_Weather_Forecast(string postalCode)
    {
        var weather = await ClientWithBearer.GetForResult<WeatherForecast[]>(
            $"/v1/weatherforecast?postalCode={postalCode}");

        weather.Should().NotBeNullOrEmpty();
        weather.Should().HaveCount(5);
    }

    [Theory]
    [InlineData("11111", HttpStatusCode.InternalServerError)]
    [InlineData("22222", HttpStatusCode.BadRequest)]
    public async Task Authenticated_Request_Returns_ProblemDetails(string badInput, HttpStatusCode expectedResult)
    {
        var problem = await ClientWithBearer.GetForProblem(
                       $"/v1/weatherforecast?postalCode={badInput}", expectedResult);

        problem.Should().NotBeNull();
        problem.Status.Should().Be((int) expectedResult);
    }
}