using SimpleApiWithData.Data.Entities;
using SimpleApiWithData.Tests.Utilities;

namespace SimpleApiWithData.Tests;

[Collection("IntegrationTests")]
public class ProductControllerTests(CustomApiFactory<Program> factory, ITestOutputHelper output, 
    DatabaseFixture dbFixture) : BaseTest(factory, output), IClassFixture<CustomApiFactory<Program>>
{
    [Fact(DisplayName = "Get all products")]
    public async Task GetProductsReturnsProducts()
    {
        var retrievedProducts = await Client.GetJsonResultAsync<List<Product>>("/v1/products", 
            HttpStatusCode.OK);

        Assert.NotNull(retrievedProducts);

        // NOTE: because the tests are not explicitly ordered and there are tests that add products,
        // we may have more products than we started with
        Assert.True(dbFixture.OriginalProducts.Count <= retrievedProducts.Count);

        OutputJson(retrievedProducts);

        var randomProduct = BogusFaker.PickRandom(dbFixture.OriginalProducts);
        Assert.Contains(retrievedProducts, c => c.Id == randomProduct.Id);

        var product = retrievedProducts.First(c => c.Id == randomProduct.Id);
        Assert.Equal(randomProduct.Name, product.Name);
    }

    [Fact(DisplayName = "Get Product by ID happy path")]
    public async Task GetProductByIdReturnsProduct()
    {
        var productToFind = BogusFaker.PickRandom(dbFixture.OriginalProducts);

        var foundProduct = await Client.GetJsonResultAsync<Product>($"/v1/products/{productToFind.Id}", 
            HttpStatusCode.OK);

        Assert.NotNull(foundProduct);
        Assert.Equal(foundProduct.Id, productToFind.Id);
        Assert.Equal(foundProduct.Name, productToFind.Name);
        Assert.Equal(foundProduct.Price, productToFind.Price);
        Assert.Equal(foundProduct.Description, productToFind.Description);
        Assert.Equal(foundProduct.Category, productToFind.Category);
    }

    [Fact(DisplayName = "Get product by ID not found")]
    public async Task GetProductByIdReturnsNotFound()
    {
        var maxCompanyId = dbFixture.OriginalProducts.Max(c => c.Id);
        var shouldNotBeFoundId = maxCompanyId + 10;

        var response = await Client.GetJsonResultAsync<ProblemDetails>($"/v1/products/{shouldNotBeFoundId}", 
            HttpStatusCode.NotFound);
        Assert.Equal("Resource not found.", response.Title);
    }

    [Fact]
    public async Task CreateProductHappyPath()
    {
        var newProduct = dbFixture.ProductFaker.Generate();

        var createdProduct = await Client.PostJsonForResultAsync<Product>($"/v1/products", newProduct,
            HttpStatusCode.OK, Output);

        Assert.NotNull(createdProduct);
        Assert.NotEqual(0, createdProduct.Id);
        Assert.Equal(newProduct.Name, createdProduct.Name);
        Assert.Equal(newProduct.Price, createdProduct.Price);
        Assert.Equal(newProduct.Description, createdProduct.Description);
        Assert.Equal(newProduct.Category, createdProduct.Category);
        Assert.Equal(newProduct.ImgUrl, createdProduct.ImgUrl);

        // verify we can retrieve the new product
        var retrievedProduct = await Client.GetJsonResultAsync<Product>(
            $"/v1/products/{createdProduct.Id}", HttpStatusCode.OK);

        Assert.NotNull(retrievedProduct);
        Assert.Equal(createdProduct.Id, retrievedProduct.Id);
        Assert.Equal(createdProduct.Name, retrievedProduct.Name);
        Assert.Equal(createdProduct.Price, retrievedProduct.Price);
        Assert.Equal(createdProduct.Description, retrievedProduct.Description);
        Assert.Equal(createdProduct.Category, retrievedProduct.Category);
        Assert.Equal(createdProduct.ImgUrl, retrievedProduct.ImgUrl);
    }

    [Fact(DisplayName = "Cannot create product with duplicate name")]
    public async Task CreateProductDuplicate()
    {
        var existingCompany = BogusFaker.PickRandom(dbFixture.OriginalProducts);

        var newProduct = dbFixture.ProductFaker.Generate();
        newProduct.Name = existingCompany.Name;

        var problem = await Client.PostJsonForResultAsync<ProblemDetails>($"/v1/products", newProduct,
            HttpStatusCode.BadRequest);

        OutputJson(problem);
        Assert.Equal("Validation error(s) occurred.", problem.Title);
        Assert.Equal("A product with the same name already exists.", problem.Extensions["Name"]!.ToString());
        //Assert.Equal("Category is required.", problem.Extensions["Category"]!.ToString());
    }

    [Theory(DisplayName = "Create Product Name validation errors")]
    [InlineData(null, "Name is required.")]
    [InlineData("", "Name is required.")]
    [InlineData("__too_long__", "Name must not exceed 50 characters.")]
    public async Task CreateProductNameValidations(string? productName, string expectedMessage)
    {
        var newProduct = dbFixture.ProductFaker.Generate();
        newProduct.Name = productName != "__too_long__" 
            ? productName ?? "" 
            : BogusFaker.Lorem.Letter(51);

        var problem = await Client.PostJsonForResultAsync<ProblemDetails>($"/v1/products", newProduct,
            HttpStatusCode.BadRequest);

        Assert.Equal("Validation error(s) occurred.", problem.Title);
        Assert.Equal(expectedMessage, problem.Extensions["Name"]!.ToString());
    }

    [Theory(DisplayName = "Create Product Category validation errors")]
    [InlineData(null, "Category is required.")]
    [InlineData("", "Category is required.")]
    [InlineData("__too_long__", "Category must not exceed 20 characters.")]
    public async Task CreateProductCategoryValidations(string? categoryName, string expectedMessage)
    {
        var newProduct = dbFixture.ProductFaker.Generate();
        newProduct.Category = categoryName != "__too_long__"
            ? categoryName ?? ""
            : BogusFaker.Lorem.Letter(21);

        var problem = await Client.PostJsonForResultAsync<ProblemDetails>($"/v1/products", newProduct,
            HttpStatusCode.BadRequest);

        OutputJson(problem);

        Assert.Equal("Validation error(s) occurred.", problem.Title);
        Assert.Equal(expectedMessage, problem.Extensions["Category"]!.ToString());
    }

    [Fact(DisplayName = "Create Product multiple validation errors")]
    public async Task CreateProductMultipleValidations()
    {
        var newProduct = dbFixture.ProductFaker.Generate();
        newProduct.Name = "";
        newProduct.Category = "";

        var problem = await Client.PostJsonForResultAsync<ProblemDetails>($"/v1/products", newProduct,
            HttpStatusCode.BadRequest);

        Assert.Equal("Validation error(s) occurred.", problem.Title);
        Assert.Equal("Name is required.", problem.Extensions["Name"]!.ToString());
        Assert.Equal("Category is required.", problem.Extensions["Category"]!.ToString());
    }
}
