using InfotecsApi.ErrorHandler;
using InfotecsApi.Services;
using InfotecsApi.Validators;
using Microsoft.AspNetCore.Mvc;

namespace InfotecsApi.Controllers;

[ApiController]
[Route("api/import")]
public class ImportController : ControllerBase
{
    private readonly CsvImportService _service;
    private readonly ILogger<ImportController> _logger;

    public ImportController(CsvImportService service, ILogger<ImportController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(IFormFile? file, CancellationToken ct)
    {
        if (file == null)
            return BadRequest("File is missing");
        try
        {
            await _service.ImportAsync(file, ct);
            return Ok(new { message = "File successfully processed" });
        }
        catch (CsvValidator.ValidationException ex)
        {
            _logger.LogWarning("Validation failed for file {FileName}: {Message}", 
                file.FileName, ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (DataImportException ex)
        {
            _logger.LogError(ex, "Data import failed for file: {FileName}", file.FileName);
            return StatusCode(500, new { error = "Failed to save data. Please try again." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during import of file: {FileName}", file.FileName);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}