using FluentAssertions;
using InfotecsApi.Models.DTOs;
using InfotecsApi.Services;

namespace InfotecsApiTests.UnitTests;

public class ResultCalculatorServiceTests
{
    private readonly ResultCalculatorService _calculator;

    public ResultCalculatorServiceTests()
    {
        _calculator = new ResultCalculatorService();
    }

    #region Basic Calculation Tests

    [Fact]
    public void Calculate_WithSingleRow_ShouldReturnCorrectResult()
    {
        // Arrange
        var rows = new List<CsvRowDto>
        {
            new() 
            { 
                Date = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc), 
                ExecutionTime = 5.5, 
                Value = 100.5 
            }
        };

        // Act
        var result = _calculator.Calculate("test_file", rows);

        // Assert
        result.FileName.Should().Be("test_file");
        result.MinDate.Should().Be(new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc));
        result.DeltaSeconds.Should().Be(0);
        result.AvgExecutionTime.Should().Be(5.5);
        result.AvgValue.Should().Be(100.5);
        result.MedianValue.Should().Be(100.5);
        result.MinValue.Should().Be(100.5);
        result.MaxValue.Should().Be(100.5);
    }

    [Fact]
    public void Calculate_WithMultipleRows_ShouldCalculateCorrectAverages()
    {
        // Arrange
        var rows = new List<CsvRowDto>
        {
            new() { Date = DateTime.UtcNow, ExecutionTime = 2, Value = 10 },
            new() { Date = DateTime.UtcNow, ExecutionTime = 4, Value = 20 },
            new() { Date = DateTime.UtcNow, ExecutionTime = 6, Value = 30 }
        };

        // Act
        var result = _calculator.Calculate("test", rows);

        // Assert
        result.AvgExecutionTime.Should().Be(4);
        result.AvgValue.Should().Be(20);
    }

    #endregion

    #region Median Calculation Tests

    [Theory]
    [InlineData(new double[] { 1 }, 1)]
    [InlineData(new double[] { 1, 2, 3 }, 2)]
    [InlineData(new double[] { 1, 2, 3, 4 }, 2.5)]
    [InlineData(new double[] { 3, 2, 1, 5, 4 }, 3)]
    [InlineData(new double[] { 60, 30, 20, 40, 10, 50 }, 35)]
    public void Calculate_VariousMedianScenarios_ShouldReturnCorrectMedian(double[] values, double expectedMedian)
    {
        // Arrange
        var rows = values.Select(v => new CsvRowDto
        {
            Date = DateTime.UtcNow,
            ExecutionTime = 1,
            Value = v
        }).ToList();

        // Act
        var result = _calculator.Calculate("test", rows);

        // Assert
        result.MedianValue.Should().Be(expectedMedian);
    }

    #endregion

    #region Min/Max Value Tests

    [Fact]
    public void Calculate_ShouldReturnCorrectMinAndMaxValues()
    {
        // Arrange
        var rows = new List<CsvRowDto>
        {
            new() { Date = DateTime.UtcNow, ExecutionTime = 1, Value = 50 },
            new() { Date = DateTime.UtcNow, ExecutionTime = 2, Value = 10 }, // Min
            new() { Date = DateTime.UtcNow, ExecutionTime = 3, Value = 100 }, // Max
            new() { Date = DateTime.UtcNow, ExecutionTime = 4, Value = 30 }
        };

        // Act
        var result = _calculator.Calculate("test", rows);

        // Assert
        result.MinValue.Should().Be(10);
        result.MaxValue.Should().Be(100);
    }

    [Fact]
    public void Calculate_WithAllSameValues_MinMaxShouldBeEqual()
    {
        // Arrange
        var rows = new List<CsvRowDto>
        {
            new() { Date = DateTime.UtcNow, ExecutionTime = 1, Value = 50 },
            new() { Date = DateTime.UtcNow, ExecutionTime = 2, Value = 50 },
            new() { Date = DateTime.UtcNow, ExecutionTime = 3, Value = 50 }
        };

        // Act
        var result = _calculator.Calculate("test", rows);

        // Assert
        result.MinValue.Should().Be(50);
        result.MaxValue.Should().Be(50);
        result.MedianValue.Should().Be(50);
        result.AvgValue.Should().Be(50);
    }

    #endregion

    #region Date and Delta Calculation Tests

    [Fact]
    public void Calculate_ShouldReturnMinDate()
    {
        // Arrange
        var date1 = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var date2 = new DateTime(2024, 1, 15, 11, 0, 0, DateTimeKind.Utc);
        var date3 = new DateTime(2024, 1, 15, 9, 0, 0, DateTimeKind.Utc);

        var rows = new List<CsvRowDto>
        {
            new() { Date = date1, ExecutionTime = 1, Value = 10 },
            new() { Date = date2, ExecutionTime = 2, Value = 20 },
            new() { Date = date3, ExecutionTime = 3, Value = 30 }
        };

        // Act
        var result = _calculator.Calculate("test", rows);

        // Assert
        result.MinDate.Should().Be(date3);
    }
    
    [Theory]
    [InlineData(0, 0)]
    [InlineData(60, 60)]
    [InlineData(3600, 3600)]
    [InlineData(86400, 86400)]
    public void Calculate_VariousDeltaScenarios_ShouldCalculateCorrectly(int secondsDifference, double expectedDelta)
    {
        // Arrange
        var minDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var maxDate = minDate.AddSeconds(secondsDifference);

        var rows = new List<CsvRowDto>
        {
            new() { Date = minDate, ExecutionTime = 1, Value = 10 },
            new() { Date = maxDate, ExecutionTime = 2, Value = 20 }
        };

        // Act
        var result = _calculator.Calculate("test", rows);

        // Assert
        result.DeltaSeconds.Should().Be(expectedDelta);
    }

    #endregion

    #region FileName Tests

    [Theory]
    [InlineData("file")]
    [InlineData("test")]
    [InlineData("damn_you_unit_tests")]
    public void Calculate_ShouldSetCorrectFileName(string fileName)
    {
        // Arrange
        var rows = new List<CsvRowDto>
        {
            new() { Date = DateTime.UtcNow, ExecutionTime = 1, Value = 10 }
        };

        // Act
        var result = _calculator.Calculate(fileName, rows);

        // Assert
        result.FileName.Should().Be(fileName);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Calculate_WithVeryLargeDataset_ShouldCalculateCorrectly()
    {
        // Arrange - 1000 rows
        var rows = Enumerable.Range(1, 1000)
            .Select(i => new CsvRowDto
            {
                Date = DateTime.UtcNow.AddMinutes(i),
                ExecutionTime = i,
                Value = i * 10
            })
            .ToList();

        // Act
        var result = _calculator.Calculate("large_test", rows);

        // Assert
        result.AvgExecutionTime.Should().Be(500.5); // Average of 1 to 1000
        result.AvgValue.Should().Be(5005); // Average of 10 to 10_000
        result.MinValue.Should().Be(10);
        result.MaxValue.Should().Be(10_000);
        result.MedianValue.Should().Be(5005); // (5000 + 5010) / 2
    }

    [Fact]
    public void Calculate_WithDecimalValues_ShouldHandlePrecision()
    {
        // Arrange
        var rows = new List<CsvRowDto>
        {
            new() { Date = DateTime.UtcNow, ExecutionTime = 1.111, Value = 10.555 },
            new() { Date = DateTime.UtcNow, ExecutionTime = 2.222, Value = 20.666 },
            new() { Date = DateTime.UtcNow, ExecutionTime = 3.333, Value = 30.777 }
        };

        // Act
        var result = _calculator.Calculate("test", rows);

        // Assert
        result.AvgExecutionTime.Should().BeApproximately(2.222, 0.001);
        result.AvgValue.Should().BeApproximately(20.666, 0.001);
    }

    [Fact]
    public void Calculate_WithZeroValues_ShouldCalculateCorrectly()
    {
        // Arrange
        var rows = new List<CsvRowDto>
        {
            new() { Date = DateTime.UtcNow, ExecutionTime = 0, Value = 0 },
            new() { Date = DateTime.UtcNow, ExecutionTime = 0, Value = 0 }
        };

        // Act
        var result = _calculator.Calculate("test", rows);

        // Assert
        result.AvgExecutionTime.Should().Be(0);
        result.AvgValue.Should().Be(0);
        result.MedianValue.Should().Be(0);
        result.MinValue.Should().Be(0);
        result.MaxValue.Should().Be(0);
    }

    #endregion
}