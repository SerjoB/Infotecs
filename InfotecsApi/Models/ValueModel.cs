namespace InfotecsApi.Models;

public class ValueModel
{
    public long Id { get; set; }
    public string FileName { get; set; } = null!;
    public DateTime Date { get; set; }
    public double ExecutionTime { get; set; }
    public double Value { get; set; }
}