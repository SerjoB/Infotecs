using InfotecsApi.ErrorHandler;
using InfotecsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InfotecsApi.Data.Repositories;

public class CsvDataRepository: ICsvDataRepository
{
    private readonly AppDbContext _context;

    public CsvDataRepository(AppDbContext context)
    {
        _context = context;
    }

    private async Task DeleteExistingDataAsync(string fileName, CancellationToken ct)
    {
        await _context.Values
            .Where(x => x.FileName == fileName)
            .ExecuteDeleteAsync(ct);

        await _context.Results
            .Where(x => x.FileName == fileName)
            .ExecuteDeleteAsync(ct);
    }

    public async Task ImportDataAsync(
        string fileName, 
        IEnumerable<ValueModel> values, 
        ResultModel result, 
        CancellationToken ct)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            await DeleteExistingDataAsync(fileName, ct);
            
            _context.Values.AddRange(values);
            _context.Results.Add(result);

            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync(ct);
            throw new DataImportException("Failed to save CSV data to database", ex);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            throw new DataImportException("Unexpected error during data import", ex);
        }
    }
}