using InfotecsApi.Models;

namespace InfotecsApi.Data.Repositories;

public interface ICsvDataRepository
{
    Task ImportDataAsync(string fileName, 
        IEnumerable<ValueModel> values, 
        ResultModel result, 
        CancellationToken ct);
}