using InfotecsApi.Models;
using InfotecsApi.Models.DTOs;

namespace InfotecsApi.Services;

public interface IResultCalculatorService
{
    ResultModel Calculate(string fileName, IReadOnlyList<CsvRowDto> rows);
}