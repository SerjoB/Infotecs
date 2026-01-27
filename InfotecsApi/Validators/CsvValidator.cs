using System.Runtime.Serialization;
using InfotecsApi.Models.DTOs;

namespace InfotecsApi.Validators;

public class CsvValidator
{
    private static readonly DateTime MinDate = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Validate(IReadOnlyCollection<CsvRowDto> rows)
    {
        if (rows.Count < 1 || rows.Count > 10_000)
            throw new ValidationException("The number of lines must be between 1 and 10,000");

        foreach (var row in rows)
        {
            if (row.Date < MinDate || row.Date > DateTime.UtcNow)
                throw new ValidationException($"Invalid date: {row.Date:o}");

            if (row.ExecutionTime < 0)
                throw new ValidationException("Execution time can't be less than 0");

            if (row.Value < 0)
                throw new ValidationException("Value can't be less than 0.");
        }
    }
    
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }
}