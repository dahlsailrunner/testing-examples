namespace SimpleApiWithPostgresAndAuth.Tests;

[Collection("IntegrationTests")]
public class AnonymousTests(AnonymousApiFactory<Program> factory, ITestOutputHelper output)
    : BaseTest(factory, output), IClassFixture<AnonymousApiFactory<Program>>
{
    [Fact]
    public async Task AnonymousGetWeatherShouldReturnUnauthorized()
    {
        var response = await Client.GetAsync("/weatherForecast?postalCode=12345");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AnonymousGetProductsShouldReturnUnauthorized()
    {
        var response = await Client.GetAsync("/v1/products");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
