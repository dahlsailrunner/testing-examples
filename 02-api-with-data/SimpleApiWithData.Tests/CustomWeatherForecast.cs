using SimpleApiWithData.Tests.Utilities;

namespace SimpleApiWithData.Tests;

[Collection("IntegrationTests")]
public class CustomWeatherForecast(CustomApiFactory<Program> factory, ITestOutputHelper output)
    : BaseTest(factory, output), IClassFixture<CustomApiFactory<Program>>
{
    private readonly List<string> _possibleSummaries =
        ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    [Fact]
    public async Task HappyPathReturnsGoodData()
    {
        var forecastResult = await Client.GetJsonResultAsync<ForecastAndEnv>(
            "/weatherForecast?postalCode=12345", HttpStatusCode.OK);

        Assert.Equal("test", forecastResult.Environment);
        Assert.Equal(5, forecastResult.Forecast.Length);
        foreach (var forecast in forecastResult.Forecast)
        {
            Assert.Equal(forecast.TemperatureF, 32 + (int)(forecast.TemperatureC / 0.5556));
            Assert.Contains(forecast.Summary, _possibleSummaries);
        }
    }

    [Fact]
    public async Task MissingPostalCodeReturnsBadRequest()
    {
        var problemDetails = await Client.GetJsonResultAsync<ProblemDetails>("/weatherForecast",
            HttpStatusCode.BadRequest, Output);

        Assert.Equal("Postal Code is required.", problemDetails.Detail);
        Assert.Equal(400, problemDetails.Status);
    }

    [Theory]
    [InlineData("1234", "Postal Code should be 5 digits long.")]
    [InlineData("123456", "Postal Code should be 5 digits long.")]
    [InlineData("abcde", "Postal Code should be all digits.")]
    [InlineData("1234a", "Postal Code should be all digits.")]
    [InlineData("ab", "Postal Code should be 5 digits long.")]
    public async Task BadPostalCodeReturnsBadRequest(string inputValue, string expectedMessage)
    {
        var problemDetails = await Client.GetJsonResultAsync<ProblemDetails>(
            $"/weatherForecast?postalCode={inputValue}", HttpStatusCode.BadRequest);

        OutputJson(problemDetails);

        Assert.Equal(expectedMessage, problemDetails.Detail);
        Assert.Equal(400, problemDetails.Status);
    }

    [Fact]
    public async Task ErrorParameterShouldReturnServerError()
    {
        var problemDetails = await Client.GetJsonResultAsync<ProblemDetails>(
            "/weatherForecast?postalCode=error", HttpStatusCode.InternalServerError);

        Assert.StartsWith("An error occurred ", problemDetails.Title);
        Assert.Equal(500, problemDetails.Status);
    }
}
