# Simple API with Data

> **NOTE:** See the `01-simple-api` folder for notes about what was already
> here before we added the `Products` controller with EF Core data.

Key (new) features:

- A `Products` controller has been added with `GET many`, `GET by id`, and `POST new`
methods
- The data behind the products controller comes from a SQLite database and uses EF Core
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
