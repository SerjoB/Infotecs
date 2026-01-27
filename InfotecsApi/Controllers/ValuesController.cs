using InfotecsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace InfotecsApi.Controllers;

[ApiController]
[Route("api/values")]
public class ValuesController : ControllerBase
{
    private readonly ValuesQueryService _service;
    private readonly ILogger<ValuesController> _logger;

    public ValuesController(ValuesQueryService service,  ILogger<ValuesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("{fileName}/last")]
    public async Task<IActionResult> GetLastValues(
        string fileName,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return BadRequest(new { error = "File name can't be empty" });
        }

        try
        {
            var values = await _service.GetLastValuesAsync(fileName, ct);

            if (values.Count != 0) 
                return Ok(values);
            
            _logger.LogWarning("File not found: {FileName}", fileName);
            return NotFound(new { error = $"File '{fileName}' not found" });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving values for file: {FileName}", fileName);
            return StatusCode(500, new { error = "Inner server error" });
        }
    }
}