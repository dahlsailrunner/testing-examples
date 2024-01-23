# Simple API

> **NOTE:** A [blog post explaining this code and the testing structure](https://knowyourtoolset.com/2024/01/integration-testing/) is available.

Key features:

- `GET` method has a `postalCode` query string parameter
  - It's required and needs to be 5 digits long
  - If it passes validation a good response is sent
  - If it fails validation a `ProblemDetails` reponse is sent with a `BadRequest (400)` response
- Passing "error" as the `postalCode` will trigger a server error that should return `ProblemDetails` with a `500 InteralServerErrror` status

## Tests

The whole point of this simple project is to
better understand automated integration tests.

The project uses XUnit and the `Microsoft.AspNetCore.Mvc.Testing` package.

The tests are in the `SimpleApi.IntegrationTests` project.

The foundational building blocks of more complex
tests start here:

- Using `WebApplicationFactory` and custom versions thereof
- Using simple `HttpClient` extensions to keep actual test code a little leaner
- Use theories and inline data to test different input values 
- Use `ITestOutputHelper` for troubleshooting purposes
- A base class for tests can also simplify code

### WeatherForecast.cs

This is the simplest possible tests that takes
advantage of global using statemtents and the
built-in `WebApplicationFactory` class but little else.

- Each test method has a `factory.CreateClient();` line to create an HTTP client
- For responses that aren't `200 OK` we need to get the full response and evaluate the status and handle the JSON
- The **only** additional files needed for these tests is `Utilities/GlobalUsings.cs`

Note that this file is 76 lines long - just to
do these simple tests.

Note the use of the `Theory` attribute and the `InlineData` 
attribute to test various invalid `postalCode` values 
and the responses we expect for some of them.

### BetterWeatherForecast.cs

This is a slightly-improved version of the tests.

It takes advantage of the `Utilities/HttpClientExtensions.cs` methods
and the `ITestOutputHelper`, along with a base class for the tests.

The `BaseTest` class has a `CreateClient()` method 
that creates an `HttpClient` that each of the tests 
can use (simplifying code a bit).

The `HttpClientExtensions` class has a generic `GetJsonResultAsync()` 
method that takes a URI and will return the deserialized
JSON response as the generic type, and will also
ensure the return value is not null and will check the
HTTP status code against whatever is expected (the method
parameter).

Additionally, if the result cannot be deserialized
into the generic type and the `ITestOutputHelper`
is passed in, the JSON response that did return will 
be written to the test output.

All of these new features and plumbing are in the
`Utilities` folder - and are present to simplify
the tests themselves and make them more readable.

Note that the new `BetterWeatherForecast.cs` file is only 
72 lines long - **and it has another entire skipped
test to show the use of `ITestOutputHelper`**.

When you're writing tests for more complex APIs
these subtle improvements can make a big difference
in the readability and maintainability of the tests.

### CustomWeatherForecast.cs

This final tests file uses a custom `WebApplicationFactory`
called `CustomApiFactory` (also in the `Utilities` folder).

The benefit of a custom `WebApplicationFactory` is 
that we can tweak the configuration and services
of the API before it starts up.

In this example we're just changing the environment
(the `ASPNETCORE_ENVIRONMENT` environment variable set in `Properties/launchSettings.json`)
from `Development` to `test`.

In the future (and in more complex APIs) we can
use this to tweak our data and other such things.
 