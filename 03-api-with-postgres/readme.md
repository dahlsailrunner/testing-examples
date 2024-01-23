# Simple API with Postgres

> **NOTE:** This is basically the same as the `02-simple-api-sqlite` 
> project, but with a Postgres database instead of SQLite.

Key (new) features:

- A `Products` controller has been added with `GET many`, `GET by id`, and `POST new`
methods
- The data behind the products controller comes from a PostgreSQL database and uses EF Core
- In the `Development` environment, some hard-coded data is always used as a baseline
when the app starts up (the same 6 products are always inserted and anything else is
removed)
- [FluentValidation](https://docs.fluentvalidation.net/en/latest/) is used to validate new `Product` objects that are posted for creation
  - `Name` and `Category` properties are required
  - `Name` cannot be more than 50 characters
  - `Category` cannot be more than 20 characters
  - `Name` must be unique (in the database)
- `ProblemDetails` objects are returned for both `NotFound` results (in
the `GET by id` method) and for `BadRequest` results (when validation errors occur in the `POST` method)

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

## Tests

Building upon the already-in-place `CustomApiFactory` that was introduced
in `01-simple-api`, this solution will expand upon that and add a `DatabaseFixture`
and shared context for test classes - both constructs available in `XUnit`.

All new tests are in the `ProductsControllerTests` class (root level file in the test
project).

The `DatabaseFixture` is used to create a new version of the `LocalContext`
using an in-memory flavor of SQLite.

It also creates some products using the [Bogus library](https://github.com/bchavez/Bogus)
when the database is started.

In the updated `CustomApiFactory`, the SQLite `DbContext` that was originally
added to the DI engine in `Program.cs` for the API project is removed, and the
one that was created within the `DatabaseFixture` class is added.
