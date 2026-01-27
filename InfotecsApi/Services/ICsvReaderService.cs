using InfotecsApi.Models.DTOs;

namespace InfotecsApi.Services;

public interface ICsvReaderService
{
    Task<List<CsvRowDto>> ReadAsync(Stream stream, CancellationToken ct);
}