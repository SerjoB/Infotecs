using InfotecsApi.Models.DTOs;
using InfotecsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace InfotecsApi.Controllers;

[ApiController]
[Route("api/results")]
public class ResultsController : ControllerBase
{
    private readonly ResultsQueryService _service;
    private readonly ILogger<ResultsController> _logger;

    public ResultsController(ResultsQueryService service, ILogger<ResultsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetResults(
        [FromQuery] ResultFilterDto filter, 
        CancellationToken ct)
    {
        try
        {
            var results = await _service.GetFilteredResultsAsync(filter, ct);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving filtered results");
            return StatusCode(500, new { error = "Inner server error" });
        }
    }
}