# Integration Testing Examples

This repository takes a step-by-step approach for
exploring techinques to use in integration tests for
ASP.NET Core API projects using XUnit
and `WebApplicationFactory` (part of
Microsoft.AspNetCore.Mvc.Testing).

## Using XUnit with WebApplicationFactory

The `01-simple-api` folder contains a solution that gets started
with a slightly-modified "weather forecast" minimal API and gets into
some testing - including a `CustomApiFactory` class that inherits
from `WebApplicationFactory`.

A [blog post with explanations](https://knowyourtoolset.com/2024/01/integration-testing/) is available.

## Using a Fixture and TestContainers for Data

The `02-simple-api-with-sqlite` and
`03-simple-api-with-postgres` folders add a controller
for "products" that has get many, get single, and create
methods that use a real database - SQLite and Postgres
respectively.

Both solutions show some always-refreshed data used
in the "Development" environment: hard-coded in the
SQLite flavor and then using Bogus in the Postgres version.

The important content is in the Test projects for each
solution - and both include a  `DatabaseFixture` class
that establishes the database and some test data using
Bogus that is a little different than the "Development"
data mentioned above.

- The SQLite version uses an in-memory version of SQLite
- The Postgres version uses [TestContainers](https://testcontainers.com/)
with a Postgres to run a temporary Docker container
for the postgres database that will only exist for
the duration of the test and is then cleaned up.

The tests refer to the `OriginalProducts` that were
created during startup.
