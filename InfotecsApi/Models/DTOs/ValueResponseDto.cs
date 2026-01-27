namespace InfotecsApi.Models.DTOs;

public class ValueResponseDto
{
    public long Id { get; set; }
    public string FileName { get; set; } = null!;
    public DateTime Date { get; set; }
    public double ExecutionTime { get; set; }
    public double Value { get; set; }
}