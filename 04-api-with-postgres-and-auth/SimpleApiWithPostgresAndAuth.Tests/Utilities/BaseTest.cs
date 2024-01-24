using System.Text.Json;
using Bogus;

namespace SimpleApiWithPostgresAndAuth.Tests.Utilities;

public class BaseTest 
    : IClassFixture<WebApplicationFactory<Program>>
{
    protected ITestOutputHelper Output { get; } 
    protected HttpClient Client { get; }
    protected HttpClient AdminClient { get; }
    protected Faker BogusFaker { get; } = new();

    public BaseTest(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        Output = output;
        Client = factory.CreateClient();
        AdminClient = factory.CreateClient();
        AdminClient.DefaultRequestHeaders.Add("X-Test-role", "admin");
    }

    protected readonly JsonSerializerOptions JsonPretty = new()
    {
        WriteIndented = true
    };

    protected void OutputJson(object content)
    {
        Output.WriteLine(JsonSerializer.Serialize(content, JsonPretty));
    }
}
