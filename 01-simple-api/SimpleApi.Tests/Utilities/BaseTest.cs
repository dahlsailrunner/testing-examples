namespace SimpleApi.Tests.Utilities;

public class BaseTest : IClassFixture<WebApplicationFactory<Program>>
{
    protected HttpClient Client { get; }

    protected BaseTest(WebApplicationFactory<Program> factory)
    {
        Client = factory.CreateClient();
    }
}
