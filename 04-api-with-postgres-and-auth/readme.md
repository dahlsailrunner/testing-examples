# Simple API with Postgres and Authentication

> **NOTE:** This is basically the same as the `03-simple-api-postgres` 
> project, but requires authenticated users to make API calls.

Key (new) features:

- Authentication using JWT tokens
- The `POST` method requires an `admin` role

We need to have good, clean, readable tests that cover the above.

## Running the API

If you want to run the API, you need to have a Postgres instance 
running locally.  The easiest way to do that is with Docker:

```bash
docker pull postgres
docker run --name some-postgres -p 5432:5432 -e POSTGRES_PASSWORD=testingiscool -d postgres
```

This will leave you with a running Postgres instance that can be connected
to with the connection string in `appsettings.json`.

If you have a different Postgres instance, just update the connection string.

And if you want to use a different database, like SQL Server, you need
to update the NuGet packages and then update `Program.cs` to use SQL
when creating the DbContext for the DI engine.  You also need to 
remove the `Data/Migrations` folder and create new migrations:

```bash
dotnet ef migrations add Initial - o Data/Migrations
```

## Creating a Coverage Report

It's pretty easy to see the amount of code that's been covered
by the integration tests without having to commit / push code
and run pipelines.

> **Prerequisite:** You need to have the report generator
> CLI tool installed to make this work:
> `dotnet tool install -g dotnet-reportgenerator-globaltool`

Once you have the above prerequisite taken care of (it's a
global tool - so once for your machine in general is enough),
then just run the `local-coverage.bat` script in the root
directory of the solution.  It's just three commands in case
you want to run them by hand:

```bash
dotnet test --settings .\Tests\tests.runsettings

reportgenerator -reports:".\Tests\**\TestResults\**\coverage.cobertura.xml" -targetdir:"coverage" -reporttypes:Html

.\coverage\index.htm
```

The first line runs the tests (the `tests.runsettings`) includes
some configuration that will capture coverage information.

The `reportgenerator` command looks at the generated `coverage.cobertura.xml` files and merges them into an HTML report, which
should be launched into a web browser with the final command.

### Coverage Excludes "Migrations"

In addition to generating coverage information, the `tests.runsettings` 
file also excludes the `Migrations` folder from the coverage report.

For more details about configuring `runsettings`, 
see the [coverlet documentation](https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/VSTestIntegration.md#advanced-options-supported-via-runsettings) 