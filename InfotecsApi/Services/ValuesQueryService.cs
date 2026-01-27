using InfotecsApi.Data;
using InfotecsApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InfotecsApi.Services;

public class ValuesQueryService
{
    private readonly AppDbContext _db;

    public ValuesQueryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ValueResponseDto>> GetLastValuesAsync(
        string fileName, 
        CancellationToken ct)
    {
        var values = await _db.Values
            .Where(v => v.FileName == fileName)
            .OrderByDescending(v => v.Date)
            .Take(10)
            .Select(v => new ValueResponseDto
            {
                Id = v.Id,
                FileName = v.FileName,
                Date = v.Date,
                ExecutionTime = v.ExecutionTime,
                Value = v.Value
            })
            .ToListAsync(ct);

        return values;
    }
}