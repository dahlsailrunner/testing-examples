using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CarvedRock.Catalog.IntegrationTests.Utilities;

public class BaseTest : IClassFixture<CustomApiFactory<Program>>, IClassFixture<SharedFixture>
{
    protected BaseTest(ITestOutputHelper output, WebApplicationFactory<Program> factory, SharedFixture shared) 
    {
        Output = output;

        ClientWithBearer = factory.CreateClient();
        ClientWithBearer.SetBearerToken(shared.AccessToken);
    }

    public HttpClient ClientWithBearer { get; set; }

    public ITestOutputHelper Output { get; set; }
}
