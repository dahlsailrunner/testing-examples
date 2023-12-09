# CarvedRock.Catalog

This is a sample API that is used to show some different testing technques.

## Running the Solution

**PREREQUISITE:** [Seq](https://datalust.co/seq) is used as a destination to write log entries in addition to the Console -- they are much easier to read and explore via Seq.

The easiest way to run Seq locally is to run it as a Docker container:

```bash
docker pull datalust/seq
docker run -d --name seq -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest
```

After this you should be able to see a user interface at [http://localhost:5341](http://localhost:5341)

Running the application will open a web browser to the Swagger user interface. Test away! 

This solution was generated from an ASP.NET Core WebAPI template that comes with a bundle of features:

- Semantic Versioning for the API via the URL Path
- OpenAPI (Swagger) document generation and user interface
- Error Logging to Seq and Console
- Liveness health check at `/health`
- Global error handling and shielding using `ProblemDetails`
- Request logging for HTTP status and elapsed time of requests and logged-in user
- Unit tests for business logic

## Things to Try

- Open the solution and explore
- Run the solution - Swagger UI page should be opened
    - Verify that just trying a method gives you a `401` response (the API requires authentication)
    - Click `Authorize` and sign in via a demo instance of the Duende IdentityServer (credentials are on the login page)
    - Now try the main `WeatherForecast` method - it should work
    - Use a value of `11111` for the `postalCode` input and it will throw an exception and return a `500` response
    - Use a value of `22222` for the `postalCode` input and it will throw an exception and return a `400` response with an additional message
- Check log entries
    - Should be available at [http://localhost:5341](http://localhost:5341)
- Run unit tests
    - A few unit tests are included with mock data
    - Integration tests should also work
- Check out the liveness health check at `/health`

## Database

To run the database locally, simply do this:

```bash
docker run --name cr-postgres -p 5432:5432 -e POSTGRES_PASSWORD=testingIsFUN -d postgres
```

When you run locally in Development, the database will be cleaned up and the data 
is re-created each time the application starts. 

### Migrations

Before generating any NEW migrations, make sure the local database (or whatever database
you are targeting) is up and running and has the latest migrations applied.  The easiest
way to do this is just run the solution.

To generate new migrations, run this command from the `CarvedRock.Data` directory (replace
`<migration-name>` with a meaningful valid class name, like `AddedCategory`):

```bash
dotnet ef migrations add <migration-name> 
```

This will create a new migration file in the `CarvedRock.Data/Migrations` directory that you
can review and edit if necessary.

## Contributing

For the repo and make a PR!  Make sure to update the tests! :)
