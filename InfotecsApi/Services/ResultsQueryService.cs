using InfotecsApi.Data;
using InfotecsApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InfotecsApi.Services;

public class ResultsQueryService
{
    private readonly AppDbContext _db;

    public ResultsQueryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ResultResponseDto>> GetFilteredResultsAsync(
        ResultFilterDto filter, 
        CancellationToken ct)
    {
        var query = _db.Results.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(filter.FileName))
        {
            query = query.Where(r => r.FileName == filter.FileName);
        }

        if (filter.MinDateFrom.HasValue)
        {
            query = query.Where(r => r.MinDate >= filter.MinDateFrom.Value);
        }

        if (filter.MinDateTo.HasValue)
        {
            query = query.Where(r => r.MinDate <= filter.MinDateTo.Value);
        }

        if (filter.AvgValueFrom.HasValue)
        {
            query = query.Where(r => r.AvgValue >= filter.AvgValueFrom.Value);
        }

        if (filter.AvgValueTo.HasValue)
        {
            query = query.Where(r => r.AvgValue <= filter.AvgValueTo.Value);
        }

        if (filter.AvgExecutionTimeFrom.HasValue)
        {
            query = query.Where(r => r.AvgExecutionTime >= filter.AvgExecutionTimeFrom.Value);
        }

        if (filter.AvgExecutionTimeTo.HasValue)
        {
            query = query.Where(r => r.AvgExecutionTime <= filter.AvgExecutionTimeTo.Value);
        }

        var results = await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ResultResponseDto
            {
                Id = r.Id,
                FileName = r.FileName,
                MinDate = r.MinDate,
                DeltaSeconds = r.DeltaSeconds,
                AvgExecutionTime = r.AvgExecutionTime,
                AvgValue = r.AvgValue,
                MedianValue = r.MedianValue,
                MinValue = r.MinValue,
                MaxValue = r.MaxValue,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync(ct);

        return results;
    }
}