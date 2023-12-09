using CarvedRock.Data.Entities;

namespace CarvedRock.Catalog.DbIntegrationTests;

[Collection("API Integration Test")]
public class ProductsControllerTests(ITestOutputHelper output, CustomApiFactory<Program> factory, SharedFixture shared)
    : BaseTest(output, factory, shared)
{
    [Fact]
    public async Task Anonymous_Request_Gets_Unauthorized()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/v1/products");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task Authenticated_Request_Gets_ProductRecord(string id)
    {
        var product = await ClientWithBearer.GetForResult<Product>(
            $"/v1/products/{id}", HttpStatusCode.OK);

        Assert.NotNull(product);
        Assert.Equal(id, product.Id.ToString());
    }

    [Fact]
    public async Task Authenticated_Request_Gets_All_Products()
    {
        var allProducts = await ClientWithBearer.GetForResult<List<Product>>(
            $"/v1/products", HttpStatusCode.OK, Output);

        Assert.NotEmpty(allProducts);
        Assert.Equal(SharedFixture.ProductCount, allProducts.Count);
    }
}
