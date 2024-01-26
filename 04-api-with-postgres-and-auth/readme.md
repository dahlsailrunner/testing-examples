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

