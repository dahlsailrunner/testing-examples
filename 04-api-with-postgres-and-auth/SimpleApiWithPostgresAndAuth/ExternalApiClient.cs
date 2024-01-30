using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace SimpleApiWithPostgresAndAuth;

public record InternalClaim(string Type, string Value);

public interface IExternalApiClient
{
    Task<List<InternalClaim>> GetSampleResult(HttpContext ctx);
}

public class ExternalApiClient : IExternalApiClient
{
    private HttpClient Client { get; }

    public ExternalApiClient(HttpClient client, IConfiguration config)
    {
        client.BaseAddress = new Uri(config.GetValue<string>("ExternalApiBaseUrl")!);
        Client = client;
    }

    public async Task<List<InternalClaim>> GetSampleResult(HttpContext ctx)
    {
        var token = await ctx.GetTokenAsync("access_token");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var claims = await Client.GetFromJsonAsync<List<InternalClaim>>("api/test");
        return claims!;
    }
}
