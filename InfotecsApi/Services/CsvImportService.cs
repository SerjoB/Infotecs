using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using InfotecsApi.Data;
using InfotecsApi.Data.Repositories;
using InfotecsApi.Models;
using InfotecsApi.Models.DTOs;
using InfotecsApi.Validators;
using Microsoft.EntityFrameworkCore;


namespace InfotecsApi.Services;

public class CsvImportService
{
    private readonly CsvValidator _validator;
    private readonly IResultCalculatorService _resultCalculator;
    private readonly ICsvReaderService _csvReader;
    private readonly ICsvDataRepository _repository;

    public CsvImportService(
        CsvValidator validator,
        IResultCalculatorService resultCalculator,
        ICsvReaderService csvReader,
        ICsvDataRepository repository)
    {
        _validator = validator;
        _resultCalculator = resultCalculator;
        _csvReader = csvReader;
        _repository = repository;
    }

    public async Task ImportAsync(IFormFile file, CancellationToken ct)
    {
        if (file.Length == 0)
            throw new CsvValidator.ValidationException("File is empty");

        await using var stream = file.OpenReadStream();
        List<CsvRowDto> rows;
    
        try
        {
            rows = await _csvReader.ReadAsync(stream, ct);
        }
        catch (CsvHelperException)
        {
            throw new CsvValidator.ValidationException("File contains invalid data. Check the format");
        }
        catch (FormatException)
        {
            throw new CsvValidator.ValidationException("File contains data of an invalid format");
        }

        _validator.Validate(rows);
        
        var filename = Path.GetFileNameWithoutExtension(file.FileName);
        
        var entities = rows.Select(r => new ValueModel
        {
            FileName = filename,
            Date = r.Date,
            ExecutionTime = r.ExecutionTime,
            Value = r.Value
        }).ToList();
        
        var result = _resultCalculator.Calculate(filename, rows);
        
        await _repository.ImportDataAsync(filename, entities, result, ct);
    }
}