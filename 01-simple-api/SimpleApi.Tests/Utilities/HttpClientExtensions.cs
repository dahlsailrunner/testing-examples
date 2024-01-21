using System.Text.Json;
using Xunit.Abstractions;

namespace SimpleApi.Tests.Utilities;

public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions PrettyJsonOptions = new()
    {
        WriteIndented = true
    };

    private static readonly JsonSerializerOptions JsonWebOptions = new (JsonSerializerDefaults.Web);

    public static async Task<T> GetJsonResultAsync<T>(this HttpClient client, string uri,
        HttpStatusCode expectedHttpStatus, ITestOutputHelper? output = null)
    {
        var response = await client.GetAsync(uri);
        Assert.Equal(expectedHttpStatus, response.StatusCode);
        var stringContent = await response.Content.ReadAsStringAsync();
        try
        {
            var result = JsonSerializer.Deserialize<T>(stringContent, JsonWebOptions);
            Assert.NotNull(result);
            return result;
        }
        catch (Exception)
        {
            if (output != null) WriteJsonMessage(stringContent, output);
            throw;
        }
    }

    public static void WriteJsonMessage(string json, ITestOutputHelper output)
    {
        var jsonObject = JsonDocument.Parse(json);
        var jsonOutput = JsonSerializer.Serialize(jsonObject, PrettyJsonOptions);
        output.WriteLine("---- JSON response ----");
        output.WriteLine(jsonOutput);
        output.WriteLine("-----------------------");
    }
}
