using InfotecsApi.Models;
using InfotecsApi.Models.DTOs;

namespace InfotecsApi.Services;

public class ResultCalculatorService: IResultCalculatorService
{
    public ResultModel Calculate(string fileName, IReadOnlyList<CsvRowDto> rows)
    {
        var minDate = rows.Min(x => x.Date);
        var maxDate = rows.Max(x => x.Date);

        var deltaSeconds = (maxDate - minDate).TotalSeconds;

        var values = rows.Select(x => x.Value).OrderBy(x => x).ToArray();

        var median = values.Length % 2 == 0
            ? (values[values.Length / 2 - 1] + values[values.Length / 2]) / 2
            : values[values.Length / 2];

        return new ResultModel
        {
            FileName = fileName,
            MinDate = minDate,
            DeltaSeconds = deltaSeconds,
            AvgExecutionTime = rows.Average(x => x.ExecutionTime),
            AvgValue = rows.Average(x => x.Value),
            MedianValue = median,
            MaxValue = values[^1],
            MinValue = values[0]
        };
    }
}