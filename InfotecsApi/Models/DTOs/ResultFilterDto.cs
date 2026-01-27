using System.ComponentModel.DataAnnotations;

namespace InfotecsApi.Models.DTOs;

public class ResultFilterDto
{
    /// <summary>
    /// Filter by file name (without extension)
    /// </summary>
    public string? FileName { get; set; }
    
    /// <summary>
    /// Filter by minimum date (start). Example: 2024-01-15T10:00:00Z
    /// </summary>
    public DateTime? MinDateFrom { get; set; }
    
    /// <summary>
    /// Filter by minimum date (end). Example: 2024-12-31T23:59:59Z
    /// </summary>
    public DateTime? MinDateTo { get; set; }
    
    /// <summary>
    /// Filter by average value (minimum)
    /// </summary>
    [Range(0, double.MaxValue)]
    public double? AvgValueFrom { get; set; }
    
    /// <summary>
    /// Filter by average value (maximum)
    /// </summary>
    [Range(0, double.MaxValue)]
    public double? AvgValueTo { get; set; }
    
    /// <summary>
    /// Filter by average execution time in seconds (minimum)
    /// </summary>
    [Range(0, double.MaxValue)]
    public double? AvgExecutionTimeFrom { get; set; }
    
    /// <summary>
    /// Filter by average execution time in seconds (maximum)
    /// </summary>
    [Range(0, double.MaxValue)]
    public double? AvgExecutionTimeTo { get; set; }

}