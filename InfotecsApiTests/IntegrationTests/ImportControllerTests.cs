using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InfotecsApi.Data;
using InfotecsApiTests.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace InfotecsApiTests.IntegrationTests;

public class ImportControllerTests: IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebAppFactory _factory;
    
    private const string ImportUrl = "/api/import";

    public ImportControllerTests(TestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        
        _factory.ResetDatabase();

    }

    [Fact]
    public async Task ImportCsv_WithValidData_ShouldReturn200AndSaveToDatabase()
    {
        // Arrange
        var csvBytes = TestDataCreator.CreateValidCsvBytes();
        var content = TestDataCreator.CreateMultipartContent(csvBytes, "ok_test.csv");

        // Act
        var response = await _client.PostAsync(ImportUrl, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify data in database
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var values = db.Values.Where(v => v.FileName == "ok_test").ToList();
        values.Should().HaveCount(5);

        var result = db.Results.FirstOrDefault(r => r.FileName == "ok_test");
        result.Should().NotBeNull();
        result!.AvgValue.Should().BeApproximately(174.42, 0.01);
    }

    [Fact]
    public async Task ImportCsv_WithTooManyRows_ShouldReturn400()
    {
        // Arrange
        var csvBytes = TestDataCreator.CreateInvalidCsvBytes_TooManyRows();
        var content = TestDataCreator.CreateMultipartContent(csvBytes, "many_lines.csv");

        // Act
        var response = await _client.PostAsync(ImportUrl, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error!.Error.Should().Contain("number of lines");
    }

    [Fact]
    public async Task ImportCsv_WithNegativeValue_ShouldReturn400()
    {
        // Arrange
        var csvBytes = TestDataCreator.CreateInvalidCsvBytes_NegativeValue();
        var content = TestDataCreator.CreateMultipartContent(csvBytes, "negative_value.csv");

        // Act
        var response = await _client.PostAsync(ImportUrl, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error!.Error.Should().Contain("Value");
    }
    
    [Fact]
    public async Task ImportCsv_WithNegativeExecutionTime_ShouldReturn400()
    {
        // Arrange
        var csvBytes = TestDataCreator.CreateInvalidCsvBytes_NegativeExecutionTime();
        var content = TestDataCreator.CreateMultipartContent(csvBytes, "negative_ETime.csv");

        // Act
        var response = await _client.PostAsync(ImportUrl, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error!.Error.Should().Contain("Execution time");
    }

    [Fact]
    public async Task ImportCsv_WithInvalidDateForm_ShouldReturn400()
    {
        // Arrange
        var csvBytes = TestDataCreator.CreateInvalidCsvBytes_InvalidDateFormat();
        var content = TestDataCreator.CreateMultipartContent(csvBytes, "invalid_date.csv");

        // Act
        var response = await _client.PostAsync(ImportUrl, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task ImportCsv_WithInvalidDateOutOfBoundForward_ShouldReturn400()
    {
        // Arrange
        var csvBytes = TestDataCreator.CreateInvalidCsvBytes_OutOfBoundForwardDate();
        var content = TestDataCreator.CreateMultipartContent(csvBytes, "invalid_date.csv");

        // Act
        var response = await _client.PostAsync(ImportUrl, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task ImportCsv_WithInvalidDateOutOfBoundBackward_ShouldReturn400()
    {
        // Arrange
        var csvBytes = TestDataCreator.CreateInvalidCsvBytes_OutOfBoundBackwardDate();
        var content = TestDataCreator.CreateMultipartContent(csvBytes, "invalid_date.csv");

        // Act
        var response = await _client.PostAsync(ImportUrl, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    

    [Fact]
    public async Task ImportCsv_SameFileTwice_ShouldReplaceData()
    {
        // Arrange
        var csvBytes = TestDataCreator.CreateValidCsvBytes();
        var content1 = TestDataCreator.CreateMultipartContent(csvBytes, "replace.csv");
        var content2 = TestDataCreator.CreateMultipartContent(csvBytes, "replace.csv");

        // Act
        await _client.PostAsync(ImportUrl, content1);
        var response = await _client.PostAsync(ImportUrl, content2);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var values = db.Values.Where(v => v.FileName == "replace").ToList();
        values.Should().HaveCount(5); 

        var results = db.Results.Where(r => r.FileName == "replace").ToList();
        results.Should().HaveCount(1);
    }
    
    [Fact]
    public async Task ImportCsv_WithEmptyFile_ShouldReturn400()
    {
        // Arrange
        var content = TestDataCreator.CreateMultipartContent([], "empty.csv");

        // Act
        var response = await _client.PostAsync(ImportUrl, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ImportCsv_WithOnlyHeader_ShouldReturn400()
    {
        // Arrange
        var csvBytes = TestDataCreator.CreateInvalidCsvBytes_OnlyHeader();
        var content = TestDataCreator.CreateMultipartContent(csvBytes, "header_only.csv");

        // Act
        var response = await _client.PostAsync(ImportUrl, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task ImportCsv_WithMissingColumn_ShouldReturn400()
    {
        var csvBytes = TestDataCreator.CreateInvalidCsvBytes_MissingColumn();
        var content = TestDataCreator.CreateMultipartContent(csvBytes, "missing_column.csv");

        // Act
        var response = await _client.PostAsync(ImportUrl, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    private class ErrorResponse
    {
        public string Error { get; set; } = string.Empty;
    }
}