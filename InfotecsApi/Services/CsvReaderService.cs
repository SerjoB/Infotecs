using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using InfotecsApi.Models.DTOs;

namespace InfotecsApi.Services;

public class CsvReaderService: ICsvReaderService
{
    public async Task<List<CsvRowDto>> ReadAsync(Stream stream, CancellationToken ct)
    {
        using var reader = new StreamReader(stream);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture) // check what it does
        {
            Delimiter = ";",
            HasHeaderRecord = true
        };

        using var csv = new CsvReader(reader, config);
        
        // Configure DateTime parsing to treat all dates as UTC
        // This ensures consistent timezone handling regardless of server location
        csv.Context.TypeConverterOptionsCache
            .GetOptions<DateTime>().DateTimeStyle = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal; 
        
        return await csv.GetRecordsAsync<CsvRowDto>(ct).ToListAsync(ct);
    }
}