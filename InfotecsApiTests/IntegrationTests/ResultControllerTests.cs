using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InfotecsApi.Data;
using InfotecsApi.Models;
using InfotecsApi.Models.DTOs;
using InfotecsApiTests.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace InfotecsApiTests.IntegrationTests;

public class ResultControllerTests: IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebAppFactory _factory;

    public ResultControllerTests(TestWebAppFactory factory)
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

        db.Results.AddRange(
            new ResultModel
            {
                FileName = "high_values",
                MinDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                DeltaSeconds = 1200,
                AvgExecutionTime = 6.3,
                AvgValue = 178.9,
                MedianValue = 175.0,
                MinValue = 150.0,
                MaxValue = 210.0
            },
            new ResultModel
            {
                FileName = "low_values",
                MinDate = new DateTime(2024, 2, 10, 0, 0, 0, DateTimeKind.Utc),
                DeltaSeconds = 1800,
                AvgExecutionTime = 2.0,
                AvgValue = 15.8,
                MedianValue = 15.0,
                MinValue = 11.0,
                MaxValue = 20.0
            }
        );

        db.SaveChanges();
    }

    [Fact]
    public async Task GetResults_WithoutFilters_ShouldReturnAllResults()
    {
        // Act
        var response = await _client.GetAsync("/api/results");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var results = await response.Content.ReadFromJsonAsync<List<ResultResponseDto>>();
        results.Should().NotBeNull();
        results!.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetResults_FilterByFileName_ShouldReturnMatchingResult()
    {
        // Act
        var response = await _client.GetAsync("/api/results?fileName=high_values");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var results = await response.Content.ReadFromJsonAsync<List<ResultResponseDto>>();
        results.Should().HaveCount(1);
        results![0].FileName.Should().Be("high_values");
    }

    [Fact]
    public async Task GetResults_FilterByAvgValue_ShouldReturnMatchingResults()
    {
        // Act
        var response = await _client.GetAsync("/api/results?avgValueFrom=100&avgValueTo=200");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var results = await response.Content.ReadFromJsonAsync<List<ResultResponseDto>>();
        results.Should().HaveCount(1);
        results![0].FileName.Should().Be("high_values");
    }
    
    [Fact]
    public async Task GetResults_FilterByAvgExecutionTime_ShouldReturnMatchingResults()
    {
        // Act
        var response = await _client.GetAsync("/api/results?AvgExecutionTimeFrom=1&AvgExecutionTimeTo=3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var results = await response.Content.ReadFromJsonAsync<List<ResultResponseDto>>();
        results.Should().HaveCount(1);
        results![0].FileName.Should().Be("low_values");
    }

    [Fact]
    public async Task GetResults_FilterByDateRange_ShouldReturnMatchingResults()
    {
        // Act
        var response = await _client.GetAsync(
            "/api/results?minDateFrom=2024-02-01T00:00:00Z&minDateTo=2024-02-28T23:59:59Z");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var results = await response.Content.ReadFromJsonAsync<List<ResultResponseDto>>();
        results.Should().HaveCount(1);
        results![0].FileName.Should().Be("low_values");
    }

    [Fact]
    public async Task GetResults_WithInvalidDateFormat_ShouldReturn400()
    {
        // Act
        var response = await _client.GetAsync("/api/results?minDateFrom=invalid");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}