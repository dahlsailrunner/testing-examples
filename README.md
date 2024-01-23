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
