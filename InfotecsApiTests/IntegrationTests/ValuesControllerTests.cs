using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InfotecsApi.Data;
using InfotecsApi.Models;
using InfotecsApi.Models.DTOs;
using InfotecsApiTests.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace InfotecsApiTests.IntegrationTests;

public class ValuesControllerTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebAppFactory _factory;

    public ValuesControllerTests(TestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        
        _factory.ResetDatabase();
        
        SeedTestData();
    }

    private void SeedTestData()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var values = new List<ValueModel>();
        for (int i = 0; i < 15; i++)
        {
            values.Add(new ValueModel
            {
                FileName = "test_file",
                Date = new DateTime(2024, 1, 15, 10, i, 0, DateTimeKind.Utc),
                ExecutionTime = 5.0 + i * 0.1,
                Value = 100.0 + i * 10
            });
        }

        db.Values.AddRange(values);
        db.SaveChanges();
    }

    [Fact]
    public async Task GetLastValues_WithExistingFile_ShouldReturn10Values()
    {
        // Act
        var response = await _client.GetAsync("/api/values/test_file/last");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var values = await response.Content.ReadFromJsonAsync<List<ValueResponseDto>>();
        values.Should().HaveCount(10);
        values.Select(v => v.Date).Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task GetLastValues_WithNonExistentFile_ShouldReturn404()
    {
        // Act
        var response = await _client.GetAsync("/api/values/nonexistent/last");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetLastValues_WithEmptyFileName_ShouldReturn400()
    {
        // Act
        var response = await _client.GetAsync("/api/values/ /last");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}