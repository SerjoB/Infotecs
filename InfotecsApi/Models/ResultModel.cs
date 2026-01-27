namespace InfotecsApi.Models;

public class ResultModel
{
    public long Id { get; set; }
    public string FileName { get; set; } = null!;
    public DateTime MinDate { get; set; }
    public double DeltaSeconds { get; set; }
    public double AvgExecutionTime { get; set; }
    public double AvgValue { get; set; }
    public double MedianValue { get; set; }
    public double MinValue { get; set; }
    public double MaxValue { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}