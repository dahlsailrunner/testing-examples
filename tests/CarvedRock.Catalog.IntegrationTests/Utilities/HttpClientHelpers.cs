﻿using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace CarvedRock.Catalog.IntegrationTests.Utilities;

public static class HttpClientHelpers
{
    public static async Task<T> GetForResult<T>(this HttpClient client, string url, ITestOutputHelper? output = null)
    {
        var response = await client.GetAsync(url);
        if (output != null) await WriteJsonMessage(response, output);

        response.Should().BeSuccessful();
        var targetObject = await response.Content.ReadFromJsonAsync<T>();

        Assert.NotNull(targetObject);
        return targetObject!;
    }

    public static async Task<ProblemDetails> GetForProblem(this HttpClient client, string url, 
        HttpStatusCode expectedStatus, ITestOutputHelper? output = null)
    {
        var response = await client.GetAsync(url);
        if (output != null) await WriteJsonMessage(response, output);

        response.StatusCode.Should().Be(expectedStatus);
        var targetObject = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(targetObject);
        return targetObject!;
    }

    public static async Task WriteJsonMessage(HttpResponseMessage response, ITestOutputHelper output)
    {
        var stringResponse = await response.Content.ReadAsStringAsync();
        var jsonObject = JsonDocument.Parse(stringResponse);
        var json = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true });
        output.WriteLine(json);
    }
}
