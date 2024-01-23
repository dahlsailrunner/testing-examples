using Bogus;
using System.Text.Json;

namespace SimpleApiWithData.Tests.Utilities;

public class BaseTest(WebApplicationFactory<Program> factory, ITestOutputHelper output) 
    : IClassFixture<WebApplicationFactory<Program>>
{
    protected ITestOutputHelper Output { get; } = output;
    protected HttpClient Client { get; } = factory.CreateClient();
    protected Faker BogusFaker { get; } = new();

    protected readonly JsonSerializerOptions JsonPretty = new()
    {
        WriteIndented = true
    };

    protected void OutputJson(object content)
    {
        Output.WriteLine(JsonSerializer.Serialize(content, JsonPretty));
    }
}
