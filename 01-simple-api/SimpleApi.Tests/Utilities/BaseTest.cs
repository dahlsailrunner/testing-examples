using Xunit.Abstractions;

namespace SimpleApi.Tests.Utilities;

public class BaseTest : IClassFixture<WebApplicationFactory<Program>>
{
    protected HttpClient Client { get; }
    protected ITestOutputHelper Output { get; }

    protected BaseTest(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        Output = output;
        Client = factory.CreateClient();
    }
}
