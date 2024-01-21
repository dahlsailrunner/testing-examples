namespace SimpleApi.Tests;

public class WeatherForecast(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly List<string> _possibleSummaries =
        ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    [Fact]
    public async Task HappyPathReturnsGoodData()
    {
        var client = factory.CreateClient();

        var forecastResult = await client.GetFromJsonAsync<ForecastAndEnv>(
            "/weatherForecast?postalCode=12345");

        Assert.Equal("Development", forecastResult?.Environment);
        Assert.Equal(5, forecastResult?.Forecast.Length);
        foreach (var forecast in forecastResult?.Forecast!)
        {
            Assert.Equal(forecast.TemperatureF, 32 + (int)(forecast.TemperatureC / 0.5556));
            Assert.Contains(forecast.Summary, _possibleSummaries);
        }
    }

    [Fact]
    public async Task MissingPostalCodeReturnsBadRequest()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/weatherForecast");

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        Assert.NotNull(problemDetails);
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
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/weatherForecast?postalCode={inputValue}");

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(problemDetails);
        Assert.Equal(expectedMessage, problemDetails.Detail);
        Assert.Equal(400, problemDetails.Status);
    }

    [Fact]
    public async Task ErrorParameterShouldReturnServerError()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/weatherForecast?postalCode=error");

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.NotNull(problemDetails);
        Assert.StartsWith("An error occurred ", problemDetails.Title);
        Assert.Equal(500, problemDetails.Status);
    }
}