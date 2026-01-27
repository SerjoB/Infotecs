using FluentAssertions;
using InfotecsApi.Models.DTOs;
using InfotecsApi.Validators;

namespace InfotecsApiTests.UnitTests;

public class CsvValidatorTests
{
    private readonly CsvValidator _validator;

    public CsvValidatorTests()
    {
        _validator = new CsvValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldNotThrow()
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
        var act = () => _validator.Validate(rows);

        // Assert
        act.Should().NotThrow();
    }

    #region Valid Boundary Tests 
    
    [Fact]
    public void Validate_WithExactly1Row_ShouldNotThrow()
    {
        // Arrange
        var rows = new List<CsvRowDto>
        {
            new() 
            { 
                Date = DateTime.UtcNow, 
                ExecutionTime = 1, 
                Value = 50 
            }
        };

        // Act
        var act = () => _validator.Validate(rows);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_WithExactly10000Rows_ShouldNotThrow()
    {
        // Arrange
        var rows = Enumerable.Range(0, 10_000)
            .Select(i => new CsvRowDto
            {
                Date = DateTime.UtcNow.AddMinutes(-i),
                ExecutionTime = 5,
                Value = 100
            })
            .ToList();

        // Act
        var act = () => _validator.Validate(rows);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_WithDateExactlyAt2000_ShouldNotThrow()
    {
        // Arrange
        var rows = new List<CsvRowDto>
        {
            new() 
            { 
                Date = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), 
                ExecutionTime = 1, 
                Value = 50 
            }
        };

        // Act
        var act = () => _validator.Validate(rows);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_WithZeroExecutionTime_ShouldNotThrow()
    {
        // Arrange
        var rows = new List<CsvRowDto>
        {
            new() 
            { 
                Date = DateTime.UtcNow, 
                ExecutionTime = 0, 
                Value = 50 
            }
        };

        // Act
        var act = () => _validator.Validate(rows);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_WithZeroValue_ShouldNotThrow()
    {
        // Arrange
        var rows = new List<CsvRowDto>
        {
            new() 
            { 
                Date = DateTime.UtcNow, 
                ExecutionTime = 5, 
                Value = 0 
            }
        };

        // Act
        var act = () => _validator.Validate(rows);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Invalid Boundary Tests 
    
    [Theory]
    [InlineData(0)]
    [InlineData(10_001)]
    [InlineData(20_000)]
    public void Validate_WithInvalidRowCount_ShouldThrowValidationException(int count)
    {
        // Arrange
        var rows = Enumerable.Range(0, count)
            .Select(i => new CsvRowDto
            {
                Date = DateTime.UtcNow.AddMinutes(-i),
                ExecutionTime = 5,
                Value = 100
            })
            .ToList();

        // Act
        var act = () => _validator.Validate(rows);

        // Assert
        act.Should().Throw<CsvValidator.ValidationException>()
            .WithMessage("*number of lines*");
    }


    [Theory]
    [InlineData("1999-12-31")]
    [InlineData("1900-01-01")]
    [InlineData("2200-01-01")]
    [InlineData("3000-01-01")]
    public void Validate_WithVariousInvalidDates_ShouldThrowValidationException(string dateString)
    {
        // Arrange
        var rows = new List<CsvRowDto>
        {
            new() 
            { 
                Date = DateTime.Parse(dateString).ToUniversalTime(), 
                ExecutionTime = 5, 
                Value = 100 
            }
        };

        // Act
        var act = () => _validator.Validate(rows);

        // Assert
        act.Should().Throw<CsvValidator.ValidationException>()
            .WithMessage("*Invalid date*");
    }

    #endregion

    #region ExecutionTime Validation Tests

    [Fact]
    public void Validate_WithNegativeExecutionTime_ShouldThrowValidationException()
    {
        // Arrange
        var rows = new List<CsvRowDto>
        {
            new() 
            { 
                Date = DateTime.UtcNow, 
                ExecutionTime = -1, 
                Value = 100 
            }
        };

        // Act
        var act = () => _validator.Validate(rows);

        // Assert
        act.Should().Throw<CsvValidator.ValidationException>()
            .WithMessage("*Execution time*0*");
    }

    [Fact]
    public void Validate_WithNegativeValue_ShouldThrowValidationException()
    {
        // Arrange
        var rows = new List<CsvRowDto>
        {
            new() 
            { 
                Date = DateTime.UtcNow, 
                ExecutionTime = 5, 
                Value = -1 
            }
        };

        // Act
        var act = () => _validator.Validate(rows);

        // Assert
        act.Should().Throw<CsvValidator.ValidationException>()
            .WithMessage("*Value*0*");
    }
    #endregion

    [Fact]
    public void Validate_WithVeryLargeValues_ShouldNotThrow()
    {
        // Arrange
        var rows = new List<CsvRowDto>
        {
            new() 
            { 
                Date = DateTime.UtcNow, 
                ExecutionTime = double.MaxValue, 
                Value = double.MaxValue 
            }
        };

        // Act
        var act = () => _validator.Validate(rows);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_WithDecimalValues_ShouldNotThrow()
    {
        // Arrange
        var rows = new List<CsvRowDto>
        {
            new() 
            { 
                Date = DateTime.UtcNow, 
                ExecutionTime = 5.123456789, 
                Value = 100.987654321 
            }
        };

        // Act
        var act = () => _validator.Validate(rows);

        // Assert
        act.Should().NotThrow();
    }
}